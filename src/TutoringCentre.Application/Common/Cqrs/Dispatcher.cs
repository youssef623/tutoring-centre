using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>The single entry point for every use case. Commands and queries take different pipelines.</summary>
public sealed class Dispatcher
{
    private const string CommandKind = "Command";
    private const string QueryKind = "Query";

    private readonly IServiceProvider _services;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IServiceProvider services, IUnitOfWork unitOfWork, ILogger<Dispatcher> logger)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(logger);
        _services = services;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>Command pipeline: validate → begin read-write → handler → save once → commit. Failures and exceptions roll back.</summary>
    public async Task<Result<TResponse>> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(command);
        var started = Stopwatch.GetTimestamp();

        var validationError = await ValidateAsync(command, ct);
        if (validationError is not null)
        {
            var invalid = Result<TResponse>.Failure(validationError);
            LogOutcome(CommandKind, typeof(TCommand).Name, invalid, started);
            return invalid;
        }

        var tenantError = CheckTenantScope(command);
        if (tenantError is not null)
        {
            var refused = Result<TResponse>.Failure(tenantError);
            LogOutcome(CommandKind, typeof(TCommand).Name, refused, started);
            return refused;
        }

        // Day 28's permission step goes HERE, directly after the tenant step and before BeginAsync, so a
        // forbidden request never opens a transaction.
        var handler = _services.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        await _unitOfWork.BeginAsync(readOnly: false, ct);
        try
        {
            var result = await handler.HandleAsync(command, ct);
            if (result.IsFailure)
            {
                await _unitOfWork.RollbackAsync(ct);
                LogOutcome(CommandKind, typeof(TCommand).Name, result, started);
                return result;
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
            LogOutcome(CommandKind, typeof(TCommand).Name, result, started);
            return result;
        }
        catch
        {
            // CancellationToken.None: if the request was cancelled the rollback must still run.
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            throw; // the global exception handler (Day 11) logs it once
        }
    }

    /// <summary>Query pipeline: validate → begin read-only → handler → commit. Never saves.</summary>
    public async Task<Result<TResponse>> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(query);
        var started = Stopwatch.GetTimestamp();

        var validationError = await ValidateAsync(query, ct);
        if (validationError is not null)
        {
            var invalid = Result<TResponse>.Failure(validationError);
            LogOutcome(QueryKind, typeof(TQuery).Name, invalid, started);
            return invalid;
        }

        var tenantError = CheckTenantScope(query);
        if (tenantError is not null)
        {
            var refused = Result<TResponse>.Failure(tenantError);
            LogOutcome(QueryKind, typeof(TQuery).Name, refused, started);
            return refused;
        }

        // Day 28's permission step goes HERE, directly after the tenant step and before BeginAsync (same
        // rule as SendAsync).
        var handler = _services.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        await _unitOfWork.BeginAsync(readOnly: true, ct);
        try
        {
            var result = await handler.HandleAsync(query, ct);
            await _unitOfWork.CommitAsync(ct); // ends the read-only transaction; there is deliberately no save here
            LogOutcome(QueryKind, typeof(TQuery).Name, result, started);
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    /// <summary>
    /// Isolation layer 1. A request that does not implement <see cref="ITenantScoped"/> is unaffected. Otherwise:
    /// an anonymous actor is refused as unauthenticated; any actor with no centre is refused as forbidden.
    /// Resolves <see cref="ICurrentActor"/> only for a tenant-scoped request, so no other pipeline needs it registered.
    /// </summary>
    private Error? CheckTenantScope<TRequest>(TRequest request)
    {
        if (request is not ITenantScoped)
        {
            return null;
        }

        var actor = _services.GetRequiredService<ICurrentActor>().Actor;
        return actor switch
        {
            AnonymousActor => Error.Unauthenticated("auth.not_authenticated", "Authentication is required."),
            _ when actor.CentreId is null => Error.Forbidden("tenant.not_selected", "A centre must be selected."),
            _ => null,
        };
    }

    private async Task<Error?> ValidateAsync<TRequest>(TRequest request, CancellationToken ct)
    {
        var validators = _services.GetServices<IValidator<TRequest>>().ToArray();
        if (validators.Length == 0)
        {
            return null;
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<ValidationFailure>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, ct);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            return null;
        }

        var fields = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);

        return Error.Validation("validation.failed", "One or more fields are invalid.", fields);
    }

    /// <summary>"Slug" → "slug"; "Address.Street" → "address.street" (each dotted segment).</summary>
    private static string ToCamelCase(string propertyName) =>
        string.Join('.', propertyName.Split('.').Select(segment =>
            segment.Length == 0 ? segment : char.ToLowerInvariant(segment[0]) + segment[1..]));

    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "One dispatch-outcome log line with this exact template is the manual's required structured-log format; a source-generated delegate would be premature for a single call site.")]
    [SuppressMessage(
        "Performance",
        "CA1873:Avoid potentially expensive logging",
        Justification = "The formatted arguments (kind, type name, outcome, error code, elapsed ms) are cheap to compute; no allocation-heavy evaluation is being guarded against.")]
    private void LogOutcome(string requestKind, string requestType, Result result, long startTimestamp)
    {
        var elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
        var outcome = result.IsSuccess ? "Success" : "Failure";
        var errorCode = result.Error?.Code ?? "none";
        var level = result.IsSuccess ? LogLevel.Information : LogLevel.Warning;

        // Structured template: named properties, never string interpolation. The request object is never logged.
        _logger.Log(
            level,
            "{RequestKind} {RequestType} completed with {Outcome} ({ErrorCode}) in {ElapsedMs} ms",
            requestKind,
            requestType,
            outcome,
            errorCode,
            elapsedMs);
    }
}
