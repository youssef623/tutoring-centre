# `src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs`

## Purpose

Applies archive, subject, teacher, and teacher-ownership predicates before a capped SQL projection. Joins supply subject and teacher names; an active-enrollment correlated count supplies occupancy without a denormalized counter. Unknown or missing Own scope fails closed without querying groups.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 88 lines and 3,486 bytes. Source hash: `ac9b7620d34ddd2598738a63dd5ec51faecd625559c13d520357696a8a29226e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Groups;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L17): `internal sealed class GroupReadService(AppDbContext db) : IGroupReadService`
- [Line 21](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L21): `public async Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)`
- [Line 48](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L48): `public async Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)`
- [Line 60](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L60): `private IQueryable<Group>? Scoped(TeacherScope scope)`
- [Line 72](../../../../../src/TutoringCentre.Infrastructure/ReadServices/GroupReadService.cs#L72): `private IQueryable<GroupDto> Project(IQueryable<Group> groups) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:10](../../../../onboarding-work/extend_backend_purposes.py#L10)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:134](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L134)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Group: no-tracking projections joined to subjects and teachers for their names. Tenancy comes
/// from the EF query filter and row-level security; ownership comes from <see cref="TeacherScope"/>, applied as
/// a predicate in the same SQL so out-of-scope rows never leave the database. A <see cref="TeacherScope.None"/>
/// scope returns nothing without running a query.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GroupReadService(AppDbContext db) : IGroupReadService
{
    private const int ListCap = 500;

    public async Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        var groups = Scoped(scope);
        if (groups is null)
        {
            return [];
        }

        if (!criteria.IncludeArchived)
        {
            groups = groups.Where(group => group.Status == GroupStatus.Active);
        }

        if (criteria.SubjectId is { } subjectId)
        {
            groups = groups.Where(group => group.SubjectId == subjectId);
        }

        if (criteria.TeacherId is { } teacherId)
        {
            groups = groups.Where(group => group.TeacherId == teacherId);
        }

        return await Project(groups.OrderBy(group => group.NormalizedName).Take(ListCap)).ToListAsync(ct);
    }

    public async Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)
    {
        var groups = Scoped(scope);
        if (groups is null)
        {
            return null;
        }

        return await Project(groups.Where(group => group.Id == id)).SingleOrDefaultAsync(ct);
    }

    /// <summary>The groups the scope allows, or null when it allows none. Fails closed on any unknown kind.</summary>
    private IQueryable<Group>? Scoped(TeacherScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var groups = db.Set<Group>().AsNoTracking();
        return scope.Kind switch
        {
            TeacherScopeKind.All => groups,
            TeacherScopeKind.Own when scope.TeacherId is { } ownTeacherId => groups.Where(group => group.TeacherId == ownTeacherId),
            _ => null,
        };
    }

    private IQueryable<GroupDto> Project(IQueryable<Group> groups) =>
        from grp in groups
        join subject in db.Set<Subject>().AsNoTracking() on grp.SubjectId equals subject.Id
        join teacher in db.Set<Teacher>().AsNoTracking() on grp.TeacherId equals teacher.Id
        select new GroupDto(
            grp.Id,
            grp.Name,
            grp.SubjectId,
            subject.Name,
            grp.TeacherId,
            teacher.FullName,
            grp.Capacity,
            // Correlated count: the enrollment rows are the only source of truth; nothing stores this number.
            db.Set<Enrollment>().Count(enrollment => enrollment.GroupId == grp.Id && enrollment.Status == EnrollmentStatus.Active),
            grp.Status,
            EF.Property<uint>(grp, "Version"));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
