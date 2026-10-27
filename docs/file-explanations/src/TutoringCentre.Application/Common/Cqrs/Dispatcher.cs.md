# `src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`

## Purpose

The shared application entry point validates requests, enforces selected-centre and permission markers, resolves typed handlers, and owns command versus query transaction sequencing. Command save can itself return a translated failure, so handler success is provisional. It logs request type and outcome without values. Preserve pre-transaction check order, rollback cleanup, and the prohibition on nested dispatch within one unit of work.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 280 lines and 11,532 bytes. Source hash: `f8f1da4e77a067e5c573d597a2a3a0abcdeacca5feff5619f8a6690f16f2d417`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics;`
- Line 2: `using System.Diagnostics.CodeAnalysis;`
- Line 3: `using FluentValidation;`
- Line 4: `using FluentValidation.Results;`
- Line 5: `using Microsoft.Extensions.DependencyInjection;`
- Line 6: `using Microsoft.Extensions.Logging;`
- Line 7: `using TutoringCentre.Application.Common.Ports;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L15): `public sealed class Dispatcher`
- [Line 24](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L24): `public Dispatcher(IServiceProvider services, IUnitOfWork unitOfWork, ILogger<Dispatcher> logger)`
- [Line 35](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L35): `public async Task<Result<TResponse>> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct)`
- [Line 100](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L100): `public async Task<Result<TResponse>> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct)`
- [Line 152](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L152): `private Error? CheckTenantScope<TRequest>(TRequest request)`
- [Line 174](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L174): `private Error? CheckPermission<TRequest>(TRequest request)`
- [Line 207](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L207): `private void LogPermissionDenied(string requestType, string permission, Actor actor)`
- [Line 219](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L219): `private async Task<Error?> ValidateAsync<TRequest>(TRequest request, CancellationToken ct)`
- [Line 251](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L251): `private static string ToCamelCase(string propertyName) =>`
- [Line 263](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L263): `private void LogOutcome(string requestKind, string requestType, Result result, long startTimestamp)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:88](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L88)
- [src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs:47](../../../../../../src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs#L47)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:163](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L163)
- [src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs:32](../../../../../../src/TutoringCentre.Api/Endpoints/AuditEndpoints.cs#L32)
- [src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs:36](../../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L36)
- [src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs:77](../../../../../../src/TutoringCentre.Api/Endpoints/EnrollmentEndpoints.cs#L77)
- [src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs:94](../../../../../../src/TutoringCentre.Api/Endpoints/GroupEndpoints.cs#L94)
- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:56](../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L56)
- [src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs:24](../../../../../../src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs#L24)
- [src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs:81](../../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L81)
- [src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs:62](../../../../../../src/TutoringCentre.Api/Endpoints/StaffEndpoints.cs#L62)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:106](../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L106)
- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:68](../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L68)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:78](../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L78)
- [src/TutoringCentre.Application/DependencyInjection.cs:23](../../../../../../src/TutoringCentre.Application/DependencyInjection.cs#L23)
- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:215](../../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L215)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:17](../../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:85](../../../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L85)
- [tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs:90](../../../../../../tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs#L90)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:33](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L33)
- 11 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

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

        var permissionError = CheckPermission(command);
        if (permissionError is not null)
        {
            var forbidden = Result<TResponse>.Failure(permissionError);
            LogOutcome(CommandKind, typeof(TCommand).Name, forbidden, started);
            return forbidden;
        }

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

            var saveResult = await _unitOfWork.SaveChangesAsync(ct);
            if (saveResult.IsFailure)
            {
                await _unitOfWork.RollbackAsync(ct);
                var failedSave = Result<TResponse>.Failure(saveResult.Error!);
                LogOutcome(CommandKind, typeof(TCommand).Name, failedSave, started);
                return failedSave;
            }

            await _unitOfWork.CommitAsync(ct);
            LogOutcome(CommandKind, typeof(TCommand).Name, result, started);
            return result;
        }
```

The excerpt stops at line 90; use the original file for the remaining 190 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
