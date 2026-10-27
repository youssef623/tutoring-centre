# `src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs`

## Purpose

Builds active group rosters and a student enrollment history through tenant-filtered joins. TeacherScope filters the group before reading a roster; a nonexistent student returns null, whereas a valid student with no permitted groups yields an empty list. The roster cap equals the domain maximum capacity and assumes the write invariant remains intact.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 101 lines and 4,388 bytes. Source hash: `39ad64e85e65b8de51b9169047137fbd88abdd9aa5ad997d786a6de7639b1467`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L17): `internal sealed class EnrollmentReadService(AppDbContext db) : IEnrollmentReadService`
- [Line 19](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L19): `public async Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)`
- [Line 54](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L54): `public async Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)`
- [Line 90](../../../../../src/TutoringCentre.Infrastructure/ReadServices/EnrollmentReadService.cs#L90): `private IQueryable<Group>? ScopedGroups(TeacherScope scope)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../onboarding-work/extend_backend_purposes.py#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:143](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L143)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Enrollment: no-tracking projections joined to groups, students, subjects and teachers for their
/// names. Tenancy comes from the EF query filter and row-level security; ownership comes from
/// <see cref="TeacherScope"/>, applied as a predicate on the group row in the same SQL. A
/// <see cref="TeacherScope.None"/> scope reads no group.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class EnrollmentReadService(AppDbContext db) : IEnrollmentReadService
{
    public async Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        var groups = ScopedGroups(scope);
        if (groups is null)
        {
            return null;
        }

        var group = await groups
            .Where(group => group.Id == groupId)
            .Select(group => new { group.Id, group.Name, group.Capacity })
            .SingleOrDefaultAsync(ct);
        if (group is null)
        {
            return null;
        }

        var items = await (
                from enrollment in db.Set<Enrollment>().AsNoTracking()
                join student in db.Set<Student>().AsNoTracking() on enrollment.StudentId equals student.Id
                where enrollment.GroupId == groupId && enrollment.Status == EnrollmentStatus.Active
                orderby student.SearchName, student.Id
                select new RosterEntryDto(
                    enrollment.Id,
                    student.Id,
                    student.FullName,
                    enrollment.EnrolledAt,
                    EF.Property<uint>(enrollment, "Version")))
            .Take(Group.MaxCapacity)
            .ToListAsync(ct);

        // The capacity invariant keeps active enrollments at or below MaxCapacity, so the capped list is the whole roster.
        return new GroupRosterDto(group.Id, group.Name, group.Capacity, items.Count, items);
    }

    public async Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        if (!await db.Set<Student>().AsNoTracking().AnyAsync(student => student.Id == studentId, ct))
        {
            return null;
        }

        var groups = ScopedGroups(scope);
        if (groups is null)
        {
            return [];
        }

        return await (
                from enrollment in db.Set<Enrollment>().AsNoTracking()
                join grp in groups on enrollment.GroupId equals grp.Id
                join subject in db.Set<Subject>().AsNoTracking() on grp.SubjectId equals subject.Id
                join teacher in db.Set<Teacher>().AsNoTracking() on grp.TeacherId equals teacher.Id
                where enrollment.StudentId == studentId
                orderby enrollment.Status == EnrollmentStatus.Active ? 0 : 1, enrollment.EnrolledAt descending, enrollment.Id descending
                select new StudentEnrollmentDto(
                    enrollment.Id,
                    grp.Id,
                    grp.Name,
                    subject.Name,
                    teacher.FullName,
                    enrollment.Status,
                    enrollment.EnrolledAt,
                    enrollment.EndedAt,
                    enrollment.EndReason,
                    EF.Property<uint>(enrollment, "Version")))
            .ToListAsync(ct);
    }

    /// <summary>The groups the scope allows, or null when it allows none. Fails closed on any unknown kind.</summary>
    private IQueryable<Group>? ScopedGroups(TeacherScope scope)
```

The excerpt stops at line 90; use the original file for the remaining 11 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
