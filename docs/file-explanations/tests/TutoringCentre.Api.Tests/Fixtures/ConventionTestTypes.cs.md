# `tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the actor set by ActorMiddleware actually reaches the Application layer (Day 16, Task 16.7).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 80 lines and 3,926 bytes. Source hash: `002bf110e7d49fcf3cf11586039c34558087ba523d397368f28eec41a9324931`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using FluentValidation;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L10): `internal sealed record ConventionCommand(string Kind) : ICommand<string>;`
- [Line 13](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L13): `internal sealed record CurrentActorQuery : IQuery<CurrentActorDto>;`
- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L15): `internal sealed record CurrentActorDto(string Kind, Guid? UserId, Guid? CentreId, StaffRole? Role);`
- [Line 17](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L17): `internal sealed record TestNameCommand(string Name) : ICommand<string>;`
- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L19): `internal sealed record TestNameBody(string Name);`
- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L21): `internal sealed record SensitiveProbe(string Email, string Password, string PhoneNumber);`
- [Line 25](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L25): `internal sealed class ConventionCommandHandler : ICommandHandler<ConventionCommand, string>`
- [Line 27](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L27): `public Task<Result<string>> HandleAsync(ConventionCommand command, CancellationToken cancellationToken)`
- [Line 51](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L51): `internal sealed class TestNameHandler : ICommandHandler<TestNameCommand, string>`
- [Line 53](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L53): `public Task<Result<string>> HandleAsync(TestNameCommand command, CancellationToken cancellationToken)`
- [Line 61](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L61): `internal sealed class TestNameValidator : AbstractValidator<TestNameCommand>`
- [Line 63](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L63): `public TestNameValidator() => RuleFor(command => command.Name).NotEmpty();`
- [Line 67](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L67): `internal sealed class CurrentActorQueryHandler(ICurrentActor currentActor) : IQueryHandler<CurrentActorQuery, CurrentActorDto>`
- [Line 69](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs#L69): `public Task<Result<CurrentActorDto>> HandleAsync(CurrentActorQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:18](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L18)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:20](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L20)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L21)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:26](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L26)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs:38](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs#L38)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L21)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:22](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L22)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:23](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L23)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs:24](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Api.Tests.Fixtures;

internal sealed record ConventionCommand(string Kind) : ICommand<string>;

/// <summary>Proves the actor set by ActorMiddleware actually reaches the Application layer (Day 16, Task 16.7).</summary>
internal sealed record CurrentActorQuery : IQuery<CurrentActorDto>;

internal sealed record CurrentActorDto(string Kind, Guid? UserId, Guid? CentreId, StaffRole? Role);

internal sealed record TestNameCommand(string Name) : ICommand<string>;

internal sealed record TestNameBody(string Name);

internal sealed record SensitiveProbe(string Email, string Password, string PhoneNumber);

/// <summary>Returns the outcome named by the request: one handler, every error kind, plus a throwing case.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ConventionCommandHandler : ICommandHandler<ConventionCommand, string>
{
    public Task<Result<string>> HandleAsync(ConventionCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var result = command.Kind switch
        {
            "ok" => Result<string>.Success("ok"),
            "validation" => Result<string>.Failure(Error.Validation(
                "test.invalid",
                "Invalid.",
                new Dictionary<string, string[]> { ["name"] = ["Name is required."] })),
            "notfound" => Result<string>.Failure(Error.NotFound("test.not_found", "Missing.")),
            "conflict" => Result<string>.Failure(Error.Conflict("test.conflict", "Conflict.")),
            "rule" => Result<string>.Failure(Error.Rule("test.rule", "Rule broken.")),
            "forbidden" => Result<string>.Failure(Error.Forbidden("test.forbidden", "Not allowed.")),
            "throws" => throw new InvalidOperationException("Secret internal detail: Host=db;Password=hunter2"),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Kind, "Unknown convention kind."),
        };

        return Task.FromResult(result);
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TestNameHandler : ICommandHandler<TestNameCommand, string>
{
    public Task<Result<string>> HandleAsync(TestNameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return Task.FromResult(Result<string>.Success($"Hello {command.Name}"));
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TestNameValidator : AbstractValidator<TestNameCommand>
{
    public TestNameValidator() => RuleFor(command => command.Name).NotEmpty();
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CurrentActorQueryHandler(ICurrentActor currentActor) : IQueryHandler<CurrentActorQuery, CurrentActorDto>
{
    public Task<Result<CurrentActorDto>> HandleAsync(CurrentActorQuery query, CancellationToken cancellationToken)
    {
        var dto = currentActor.Actor switch
        {
            StaffActor staff => new CurrentActorDto("Staff", staff.UserId, staff.CentreId, staff.Role),
            SystemActor system => new CurrentActorDto("System", null, system.CentreId, null),
            _ => new CurrentActorDto("Anonymous", null, null, null),
        };

        return Task.FromResult(Result<CurrentActorDto>.Success(dto));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
