# `src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs`

## Purpose

The write adapter loads versioned groups and provides the lock-and-count enrollment operation. Its parameterized FOR UPDATE query names xmin explicitly, then counts active enrollments only after the row lock is held. This operation is shared by enrollment, capacity shrink, and archive to serialize the relevant decision window.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 3,823 bytes. Source hash: `9597d71dee4c1a7e256c536e541a068de3b035bcc002d7f05dc009b722dff17b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Groups;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L15): `internal sealed class GroupRepository(AppDbContext db) : IGroupRepository`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L17): `public async Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)`
- [Line 31](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L31): `public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 34](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L34): `public async Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)`
- [Line 57](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L57): `public bool IsAtVersion(Group group, uint expectedVersion)`
- [Line 63](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L63): `public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>`
- [Line 66](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L66): `public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>`
- [Line 69](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L69): `public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>`
- [Line 72](../../../../../src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs#L72): `public void Add(Group group)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:133](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L133)
- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:42](../../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L42)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the Group write-side port. It tracks entities; the unit of work saves them. No
/// centre predicate anywhere: the EF query filter and row-level security already scope every query to the
/// acting actor's centre.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GroupRepository(AppDbContext db) : IGroupRepository
{
    public async Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var group = await db.Set<Group>().SingleOrDefaultAsync(group => group.Id == id, ct);
        if (group is null)
        {
            return null;
        }

        // The caller's expected version becomes the row version EF compares at UPDATE time: a stale client is
        // refused by zero rows affected, not by a comparison here.
        db.Entry(group).Property("Version").OriginalValue = expectedVersion;
        return group;
    }

    public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>
        db.Set<Group>().SingleOrDefaultAsync(group => group.Id == id, ct);

    public async Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)
    {
        // Allow-listed raw SQL (QueryFilterBypassTests): LINQ cannot express FOR UPDATE. Interpolated, so groupId
        // is a parameter, never SQL text. xmin is named explicitly because SELECT * omits system columns and the
        // entity maps its Version to it. EF composes the tenant filter over this as a sub-select, and row-level
        // security applies to it as to any query; PostgreSQL still locks the row the sub-select returns. Tracked,
        // not AsNoTracking: the handler may change the group. The lock waits for any other holder and is released
        // by the dispatcher's commit or rollback.
        var group = await db.Set<Group>()
            .FromSql($"SELECT *, xmin FROM academics.groups WHERE id = {groupId} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (group is null)
        {
            return null;
        }

        // Counted only now, with the lock held: under READ COMMITTED this statement sees every enrollment
        // committed by the transaction that held the lock before us.
        var activeCount = await db.Set<Enrollment>()
            .CountAsync(enrollment => enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active, ct);
        return new GroupForEnrollment(group, activeCount);
    }

    public bool IsAtVersion(Group group, uint expectedVersion)
    {
        ArgumentNullException.ThrowIfNull(group);
        return db.Entry(group).Property<uint>("Version").CurrentValue == expectedVersion;
    }

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.NormalizedName == normalizedName && group.Id != excludingId, ct);

    public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.SubjectId == subjectId && group.Status == GroupStatus.Active, ct);

    public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>
        db.Set<Group>().AnyAsync(group => group.TeacherId == teacherId && group.Status == GroupStatus.Active, ct);

    public void Add(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);
        db.Set<Group>().Add(group);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
