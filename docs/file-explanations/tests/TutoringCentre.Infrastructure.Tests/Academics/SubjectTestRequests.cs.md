# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 23.6: test-only requests that exercise Subject through the real AppDbContext, interceptors and unit of work — standing in for the repository and read service that arrive on Day 25. The centre is taken as an explicit parameter, not the acting actor's own centre, only so AddSubjectCommand can also play the "constructed with another centre" scenario (test 8); every other test passes the actor's own centre.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 72 lines and 3,512 bytes. Source hash: `a86b5284503c63550035a5220ee2f0e5ad7e3207186a8dddd531363994c002b3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L15): `internal sealed record AddSubjectCommand(Guid CentreId, string Name) : ICommand<Guid>, ITenantScoped;`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L17): `internal sealed record ListSubjectNamesQuery : IQuery<List<string>>, ITenantScoped;`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L19): `internal sealed record RenameSubjectCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L23): `internal sealed record RenameSubjectThenFailCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L25): `internal sealed class AddSubjectCommandHandler(AppDbContext db) : ICommandHandler<AddSubjectCommand, Guid>`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L27): `public Task<Result<Guid>> HandleAsync(AddSubjectCommand command, CancellationToken cancellationToken)`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L40): `internal sealed class ListSubjectNamesQueryHandler(AppDbContext db) : IQueryHandler<ListSubjectNamesQuery, List<string>>`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L42): `public async Task<Result<List<string>>> HandleAsync(ListSubjectNamesQuery query, CancellationToken cancellationToken)`
- [Line 49](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L49): `internal sealed class RenameSubjectCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectCommand, Unit>`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L51): `public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L59): `internal sealed class RenameSubjectThenFailCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectThenFailCommand, Unit>`
- [Line 61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs#L61): `public async Task<Result<Unit>> HandleAsync(RenameSubjectThenFailCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:166](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L166)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs:26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L26)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs:44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs#L44)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:187](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L187)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:188](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L188)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:189](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L189)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:190](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L190)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.6: test-only requests that exercise Subject through the real AppDbContext, interceptors and unit of
/// work — standing in for the repository and read service that arrive on Day 25. The centre is taken as an
/// explicit parameter, not the acting actor's own centre, only so <c>AddSubjectCommand</c> can also play the
/// "constructed with another centre" scenario (test 8); every other test passes the actor's own centre.
/// </summary>
internal sealed record AddSubjectCommand(Guid CentreId, string Name) : ICommand<Guid>, ITenantScoped;

internal sealed record ListSubjectNamesQuery : IQuery<List<string>>, ITenantScoped;

internal sealed record RenameSubjectCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;

/// <summary>Task 31.7: renames a subject, then always fails — proves a rolled-back change leaves no audit row,
/// before a production command that fails after changing a subject exists to test instead.</summary>
internal sealed record RenameSubjectThenFailCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;

internal sealed class AddSubjectCommandHandler(AppDbContext db) : ICommandHandler<AddSubjectCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(AddSubjectCommand command, CancellationToken cancellationToken)
    {
        var created = Subject.Create(command.CentreId, command.Name);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(created.Error!));
        }

        db.Set<Subject>().Add(created.Value);
        return Task.FromResult(Result<Guid>.Success(created.Value.Id));
    }
}

internal sealed class ListSubjectNamesQueryHandler(AppDbContext db) : IQueryHandler<ListSubjectNamesQuery, List<string>>
{
    public async Task<Result<List<string>>> HandleAsync(ListSubjectNamesQuery query, CancellationToken cancellationToken)
    {
        var names = await db.Set<Subject>().Select(subject => subject.Name).ToListAsync(cancellationToken);
        return Result<List<string>>.Success(names);
    }
}

internal sealed class RenameSubjectCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await db.Set<Subject>().SingleAsync(s => s.Id == command.SubjectId, cancellationToken);
        var renamed = subject.Rename(command.NewName);
        return renamed.IsFailure ? Result<Unit>.Failure(renamed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class RenameSubjectThenFailCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectThenFailCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectThenFailCommand command, CancellationToken cancellationToken)
    {
        var subject = await db.Set<Subject>().SingleAsync(s => s.Id == command.SubjectId, cancellationToken);
        var renamed = subject.Rename(command.NewName);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        return Result<Unit>.Failure(Error.Rule("test.deliberate_failure", "Always fails, after the subject was renamed."));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
