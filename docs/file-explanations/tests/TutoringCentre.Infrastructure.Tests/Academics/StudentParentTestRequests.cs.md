# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.5/38.6: test-only requests that exercise the Student and Parent aggregates through the real AppDbContext, interceptors and unit of work — standing in for the repositories and use cases that arrive on Days 39–40.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 159 lines and 7,509 bytes. Source hash: `b5f143fd09c42b2928a6f7c8209bf54cc513f37d9e9d7900529203077f5ea22c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Academics;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L14): `internal sealed record AddParentCommand(Guid CentreId, string FullName, string Phone) : ICommand<Guid>, ITenantScoped;`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L16): `internal sealed record UpdateParentPhoneCommand(Guid ParentId, string Phone) : ICommand<Unit>, ITenantScoped;`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L18): `internal sealed record RegisterStudentCommand(`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L22): `internal sealed record AddStudentParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship) : ICommand<Unit>, ITenantScoped;`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L24): `internal sealed record RemoveStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;`
- [Line 26](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L26): `internal sealed record SetPrimaryStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L28): `internal sealed record ListStudentNamesQuery : IQuery<List<string>>, ITenantScoped;`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L30): `internal sealed record GetStudentLinksSnapshotQuery(Guid StudentId) : IQuery<StudentLinksSnapshot>, ITenantScoped;`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L32): `internal sealed record StudentLinksSnapshot(int LinksRevision, IReadOnlyList<StudentLinkSnapshot> Links);`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L34): `internal sealed record StudentLinkSnapshot(Guid ParentId, bool IsPrimary);`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L36): `internal sealed class AddParentCommandHandler(AppDbContext db) : ICommandHandler<AddParentCommand, Guid>`
- [Line 38](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L38): `public Task<Result<Guid>> HandleAsync(AddParentCommand command, CancellationToken cancellationToken)`
- [Line 57](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L57): `internal sealed class UpdateParentPhoneCommandHandler(AppDbContext db) : ICommandHandler<UpdateParentPhoneCommand, Unit>`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L59): `public async Task<Result<Unit>> HandleAsync(UpdateParentPhoneCommand command, CancellationToken cancellationToken)`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L73): `internal sealed class RegisterStudentCommandHandler(AppDbContext db) : ICommandHandler<RegisterStudentCommand, Guid>`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L75): `public Task<Result<Guid>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)`
- [Line 101](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L101): `internal sealed class AddStudentParentCommandHandler(AppDbContext db) : ICommandHandler<AddStudentParentCommand, Unit>`
- [Line 103](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L103): `public async Task<Result<Unit>> HandleAsync(AddStudentParentCommand command, CancellationToken cancellationToken)`
- [Line 111](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L111): `internal sealed class RemoveStudentParentCommandHandler(AppDbContext db) : ICommandHandler<RemoveStudentParentCommand, Unit>`
- [Line 113](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L113): `public async Task<Result<Unit>> HandleAsync(RemoveStudentParentCommand command, CancellationToken cancellationToken)`
- [Line 132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L132): `internal sealed class SetPrimaryStudentParentCommandHandler(AppDbContext db) : ICommandHandler<SetPrimaryStudentParentCommand, Unit>`
- [Line 134](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L134): `public async Task<Result<Unit>> HandleAsync(SetPrimaryStudentParentCommand command, CancellationToken cancellationToken)`
- [Line 142](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L142): `internal sealed class ListStudentNamesQueryHandler(AppDbContext db) : IQueryHandler<ListStudentNamesQuery, List<string>>`
- [Line 144](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L144): `public async Task<Result<List<string>>> HandleAsync(ListStudentNamesQuery query, CancellationToken cancellationToken)`
- [Line 151](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L151): `internal sealed class GetStudentLinksSnapshotQueryHandler(AppDbContext db) : IQueryHandler<GetStudentLinksSnapshotQuery, StudentLinksSnapshot>`
- [Line 153](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L153): `public async Task<Result<StudentLinksSnapshot>> HandleAsync(GetStudentLinksSnapshotQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L27)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:53](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L53)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L71)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L121)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:145](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L145)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs:163](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs#L163)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L22)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs:25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L25)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:198](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L198)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:199](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L199)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:200](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L200)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:201](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L201)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:202](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L202)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:203](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L203)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:204](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L204)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:205](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L205)

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
/// Task 38.5/38.6: test-only requests that exercise the Student and Parent aggregates through the real
/// AppDbContext, interceptors and unit of work — standing in for the repositories and use cases that arrive
/// on Days 39–40.
/// </summary>
internal sealed record AddParentCommand(Guid CentreId, string FullName, string Phone) : ICommand<Guid>, ITenantScoped;

internal sealed record UpdateParentPhoneCommand(Guid ParentId, string Phone) : ICommand<Unit>, ITenantScoped;

internal sealed record RegisterStudentCommand(
    Guid CentreId, string FullName, string? Phone, string? SchoolYear, Guid PrimaryParentId, ParentRelationship Relationship)
    : ICommand<Guid>, ITenantScoped;

internal sealed record AddStudentParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship) : ICommand<Unit>, ITenantScoped;

internal sealed record RemoveStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;

internal sealed record SetPrimaryStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;

internal sealed record ListStudentNamesQuery : IQuery<List<string>>, ITenantScoped;

internal sealed record GetStudentLinksSnapshotQuery(Guid StudentId) : IQuery<StudentLinksSnapshot>, ITenantScoped;

internal sealed record StudentLinksSnapshot(int LinksRevision, IReadOnlyList<StudentLinkSnapshot> Links);

internal sealed record StudentLinkSnapshot(Guid ParentId, bool IsPrimary);

internal sealed class AddParentCommandHandler(AppDbContext db) : ICommandHandler<AddParentCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(AddParentCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(phone.Error!));
        }

        var created = Parent.Create(command.CentreId, command.FullName, phone.Value);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(created.Error!));
        }

        db.Set<Parent>().Add(created.Value);
        return Task.FromResult(Result<Guid>.Success(created.Value.Id));
    }
}

internal sealed class UpdateParentPhoneCommandHandler(AppDbContext db) : ICommandHandler<UpdateParentPhoneCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateParentPhoneCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<Unit>.Failure(phone.Error!);
        }

        var parent = await db.Set<Parent>().SingleAsync(p => p.Id == command.ParentId, cancellationToken);
        var updated = parent.Update(parent.FullName, phone.Value);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class RegisterStudentCommandHandler(AppDbContext db) : ICommandHandler<RegisterStudentCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (command.Phone is not null)
        {
            var created = PhoneNumber.Create(command.Phone);
            if (created.IsFailure)
            {
                return Task.FromResult(Result<Guid>.Failure(created.Error!));
            }

            phone = created.Value;
        }

        var registered = Student.Register(
            command.CentreId, command.FullName, phone, command.SchoolYear, command.PrimaryParentId, command.Relationship);
```

The excerpt stops at line 90; use the original file for the remaining 69 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
