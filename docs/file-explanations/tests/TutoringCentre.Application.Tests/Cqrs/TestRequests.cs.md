# `tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Tells the test handlers how to behave in the current test.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 160 lines and 5,948 bytes. Source hash: `fda98ecc2fee2ef879cb456b6353cfcb50db7c42306b1163539bbecb7945c66c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L9): `public enum HandlerMode`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L17): `public sealed class HandlerBehaviour`
- [Line 22](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L22): `public static class TestErrors`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L27): `public sealed record TestCommand(string Name, string Slug) : ICommand<string>;`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L29): `public sealed record TestQuery(string Name) : IQuery<string>;`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L32): `public sealed record TenantScopedTestCommand(string Name) : ICommand<string>, ITenantScoped;`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L35): `public sealed record TenantScopedTestQuery(string Name) : IQuery<string>, ITenantScoped;`
- [Line 38](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L38): `public sealed record PermissionRequiringTestCommand(string Name) : ICommand<string>, ITenantScoped, IRequirePermission`
- [Line 44](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L44): `public sealed record PermissionRequiringTestQuery(string Name) : IQuery<string>, ITenantScoped, IRequirePermission`
- [Line 49](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L49): `public sealed class TestCommandValidator : AbstractValidator<TestCommand>`
- [Line 51](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L51): `public TestCommandValidator()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L59): `public sealed class TestQueryValidator : AbstractValidator<TestQuery>`
- [Line 61](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L61): `public TestQueryValidator()`
- [Line 67](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L67): `public sealed class TenantScopedTestCommandValidator : AbstractValidator<TenantScopedTestCommand>`
- [Line 69](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L69): `public TenantScopedTestCommandValidator()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L75): `public sealed class TenantScopedTestQueryValidator : AbstractValidator<TenantScopedTestQuery>`
- [Line 77](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L77): `public TenantScopedTestQueryValidator()`
- [Line 83](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L83): `public sealed class PermissionRequiringTestCommandValidator : AbstractValidator<PermissionRequiringTestCommand>`
- [Line 85](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L85): `public PermissionRequiringTestCommandValidator()`
- [Line 91](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L91): `public sealed class PermissionRequiringTestQueryValidator : AbstractValidator<PermissionRequiringTestQuery>`
- [Line 93](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L93): `public PermissionRequiringTestQueryValidator()`
- [Line 99](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L99): `public sealed class TestCommandHandler(FakeUnitOfWork unitOfWork, HandlerBehaviour behaviour)`
- [Line 102](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L102): `public Task<Result<string>> HandleAsync(TestCommand command, CancellationToken cancellationToken)`
- [Line 114](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L114): `public sealed class TestQueryHandler(FakeUnitOfWork unitOfWork, HandlerBehaviour behaviour)`
- [Line 117](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L117): `public Task<Result<string>> HandleAsync(TestQuery query, CancellationToken cancellationToken)`
- [Line 126](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L126): `public sealed class TenantScopedTestCommandHandler(FakeUnitOfWork unitOfWork) : ICommandHandler<TenantScopedTestCommand, string>`
- [Line 128](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L128): `public Task<Result<string>> HandleAsync(TenantScopedTestCommand command, CancellationToken cancellationToken)`
- [Line 135](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L135): `public sealed class TenantScopedTestQueryHandler(FakeUnitOfWork unitOfWork) : IQueryHandler<TenantScopedTestQuery, string>`
- [Line 137](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L137): `public Task<Result<string>> HandleAsync(TenantScopedTestQuery query, CancellationToken cancellationToken)`
- [Line 144](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L144): `public sealed class PermissionRequiringTestCommandHandler(FakeUnitOfWork unitOfWork) : ICommandHandler<PermissionRequiringTestCommand, string>`
- [Line 146](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L146): `public Task<Result<string>> HandleAsync(PermissionRequiringTestCommand command, CancellationToken cancellationToken)`
- [Line 153](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L153): `public sealed class PermissionRequiringTestQueryHandler(FakeUnitOfWork unitOfWork) : IQueryHandler<PermissionRequiringTestQuery, string>`
- [Line 155](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs#L155): `public Task<Result<string>> HandleAsync(PermissionRequiringTestQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:14](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L14)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:48](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L48)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:53](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L53)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:100](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L100)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:150](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L150)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:166](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L166)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:268](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L268)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:298](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L298)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:354](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L354)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:365](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L365)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:366](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L366)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:367](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L367)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:368](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L368)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:369](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L369)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:370](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L370)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:371](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L371)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:372](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L372)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:373](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L373)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:374](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L374)
- [tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs:375](../../../../../tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs#L375)
- 1 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Cqrs;

public enum HandlerMode
{
    Succeed,
    Fail,
    Throw,
}

/// <summary>Tells the test handlers how to behave in the current test.</summary>
public sealed class HandlerBehaviour
{
    public HandlerMode Mode { get; set; }
}

public static class TestErrors
{
    public static readonly Error HandlerFailure = Error.Rule("test.handler_failed", "The handler refused.");
}

public sealed record TestCommand(string Name, string Slug) : ICommand<string>;

public sealed record TestQuery(string Name) : IQuery<string>;

/// <summary>A tenant-scoped command, for the dispatcher's layer-1 cases (C10, C12, C13).</summary>
public sealed record TenantScopedTestCommand(string Name) : ICommand<string>, ITenantScoped;

/// <summary>A tenant-scoped query, for the dispatcher's layer-1 cases (C11, C12, C13).</summary>
public sealed record TenantScopedTestQuery(string Name) : IQuery<string>, ITenantScoped;

/// <summary>A tenant-scoped, permission-requiring command, for the dispatcher's permission-step cases (C15, C16, C18).</summary>
public sealed record PermissionRequiringTestCommand(string Name) : ICommand<string>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}

/// <summary>A tenant-scoped, permission-requiring query, for the dispatcher's permission-step cases (C16, C19).</summary>
public sealed record PermissionRequiringTestQuery(string Name) : IQuery<string>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}

public sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(command => command.Name).MinimumLength(3).WithMessage("Name is too short.");
        RuleFor(command => command.Slug).MaximumLength(5).WithMessage("Slug is too long.");
    }
}

public sealed class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
    {
        RuleFor(query => query.Name).NotEmpty();
    }
}

public sealed class TenantScopedTestCommandValidator : AbstractValidator<TenantScopedTestCommand>
{
    public TenantScopedTestCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
    }
}

public sealed class TenantScopedTestQueryValidator : AbstractValidator<TenantScopedTestQuery>
{
    public TenantScopedTestQueryValidator()
    {
        RuleFor(query => query.Name).NotEmpty().WithMessage("Name is required.");
    }
}

public sealed class PermissionRequiringTestCommandValidator : AbstractValidator<PermissionRequiringTestCommand>
{
    public PermissionRequiringTestCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
    }
}

```

The excerpt stops at line 90; use the original file for the remaining 70 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
