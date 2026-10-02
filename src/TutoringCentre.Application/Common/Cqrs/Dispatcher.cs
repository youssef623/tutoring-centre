using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// Routes commands and queries to their handlers, each inside its own transaction.
/// Explicit generic call sites (no reflection) resolve the handler type at compile time.
/// </summary>
public sealed partial class Dispatcher
{
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

    public async Task<Result<TResponse>> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResponse>
    {
        var start = Stopwatch.GetTimestamp();

        var validationError = await ValidateAsync(command, ct).ConfigureAwait(false);
        if (validationError is not null)
        {
            LogDispatch("Command", typeof(TCommand).Name, isSuccess: false, errorCode: validationError.Code, start);
            return Result<TResponse>.Failure(validationError);
        }

        var handler = _services.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        // Month 2: tenant resolution and permission checks go here (after validation, before BeginAsync).

        await _unitOfWork.BeginAsync(readOnly: false, ct).ConfigureAwait(false);
        try
        {
            var result = await handler.HandleAsync(command, ct).ConfigureAwait(false);
            if (result.IsFailure)
            {
                await _unitOfWork.RollbackAsync(ct).ConfigureAwait(false);
                LogDispatch("Command", typeof(TCommand).Name, isSuccess: false, errorCode: result.Error!.Code, start);
                return result;
            }

            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            await _unitOfWork.CommitAsync(ct).ConfigureAwait(false);
            LogDispatch("Command", typeof(TCommand).Name, isSuccess: true, errorCode: "none", start);
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<Result<TResponse>> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResponse>
    {
        var start = Stopwatch.GetTimestamp();

        var validationError = await ValidateAsync(query, ct).ConfigureAwait(false);
        if (validationError is not null)
        {
            LogDispatch("Query", typeof(TQuery).Name, isSuccess: false, errorCode: validationError.Code, start);
            return Result<TResponse>.Failure(validationError);
        }

        var handler = _services.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        await _unitOfWork.BeginAsync(readOnly: true, ct).ConfigureAwait(false);
        try
        {
            var result = await handler.HandleAsync(query, ct).ConfigureAwait(false);
            await _unitOfWork.CommitAsync(ct).ConfigureAwait(false);
            LogDispatch(
                "Query",
                typeof(TQuery).Name,
                isSuccess: result.IsSuccess,
                errorCode: result.IsSuccess ? "none" : result.Error!.Code,
                start);
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    private async Task<Error?> ValidateAsync<TRequest>(TRequest request, CancellationToken ct)
    {
        var validators = _services.GetServices<IValidator<TRequest>>();

        var failures = new List<ValidationFailure>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(request, ct).ConfigureAwait(false);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            return null;
        }

        var fields = failures
            .GroupBy(failure => ToCamelCase(failure.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());

        return Error.Validation("validation.failed", "One or more fields are invalid.", fields);
    }

    private static string ToCamelCase(string propertyName) =>
        string.IsNullOrEmpty(propertyName)
            ? propertyName
            : char.ToLowerInvariant(propertyName[0]) + propertyName[1..];

    private void LogDispatch(string requestKind, string requestType, bool isSuccess, string errorCode, long start)
    {
        var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        var outcome = isSuccess ? "Success" : "Failure";
        var level = isSuccess ? LogLevel.Information : LogLevel.Warning;

        LogDispatchCompleted(_logger, level, requestKind, requestType, outcome, errorCode, elapsedMs);
    }

    [LoggerMessage(Message = "{RequestKind} {RequestType} completed with {Outcome} ({ErrorCode}) in {ElapsedMs} ms")]
    private static partial void LogDispatchCompleted(
        ILogger logger,
        LogLevel level,
        string requestKind,
        string requestType,
        string outcome,
        string errorCode,
        double elapsedMs);
}
