# `src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs`

## Purpose

Reads teachers with optional linked-account display names through a left join and computes active group counts. It deliberately exposes no linked-account email. List results are capped at five hundred, while user-to-teacher resolution requires an active teacher so archived records cannot obtain Own read scope.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 3,566 bytes. Source hash: `4a28d969d4309583b6d1341779361461b77f08c1bdc2c59b1d6ef3841b464b58`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Teachers;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Infrastructure.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L17): `internal sealed class TeacherReadService(AppDbContext db) : ITeacherReadService`
- [Line 21](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L21): `public async Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)`
- [Line 42](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L42): `public async Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct)`
- [Line 61](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L61): `public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>`
- [Line 68](../../../../../src/TutoringCentre.Infrastructure/ReadServices/TeacherReadService.cs#L68): `private static TeacherDto ToDto(Teacher teacher, string? userDisplayName, int activeGroupCount, uint version) => new(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:16](../../../../onboarding-work/extend_backend_purposes.py#L16)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:132](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L132)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Teacher: no-tracking projections, left-joined to <c>identity.users</c> only for the linked
/// account's display name (never its email — that stays behind <c>staff.view</c>). No tenant predicate written
/// by hand — the EF query filter and row-level security scope <c>teachers</c> the same way they scope
/// <c>subjects</c>; the join to <c>identity.users</c> exposes one column and nothing else.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherReadService(AppDbContext db) : ITeacherReadService
{
    private const int ListCap = 500;

    public async Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        var rows = await (
            from teacher in db.Set<Teacher>().AsNoTracking()
            join user in db.Set<ApplicationUser>().AsNoTracking() on teacher.UserId equals user.Id into linkedUsers
            from linkedUser in linkedUsers.DefaultIfEmpty()
            where includeArchived || teacher.Status == TeacherStatus.Active
            orderby teacher.SearchName
            select new
            {
                teacher,
                UserDisplayName = (string?)linkedUser.DisplayName,
                ActiveGroupCount = db.Set<Group>().Count(taught => taught.TeacherId == teacher.Id && taught.Status == GroupStatus.Active),
                Version = EF.Property<uint>(teacher, "Version"),
            })
            .Take(ListCap)
            .ToListAsync(ct);

        return rows.Select(row => ToDto(row.teacher, row.UserDisplayName, row.ActiveGroupCount, row.Version)).ToList();
    }

    public async Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var row = await (
            from teacher in db.Set<Teacher>().AsNoTracking()
            join user in db.Set<ApplicationUser>().AsNoTracking() on teacher.UserId equals user.Id into linkedUsers
            from linkedUser in linkedUsers.DefaultIfEmpty()
            where teacher.Id == id
            select new
            {
                teacher,
                UserDisplayName = (string?)linkedUser.DisplayName,
                ActiveGroupCount = db.Set<Group>().Count(taught => taught.TeacherId == teacher.Id && taught.Status == GroupStatus.Active),
                Version = EF.Property<uint>(teacher, "Version"),
            })
            .SingleOrDefaultAsync(ct);

        return row is null ? null : ToDto(row.teacher, row.UserDisplayName, row.ActiveGroupCount, row.Version);
    }

    public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>
        db.Set<Teacher>()
            .AsNoTracking()
            .Where(teacher => teacher.UserId == userId && teacher.Status == TeacherStatus.Active)
            .Select(teacher => (Guid?)teacher.Id)
            .SingleOrDefaultAsync(ct);

    private static TeacherDto ToDto(Teacher teacher, string? userDisplayName, int activeGroupCount, uint version) => new(
        teacher.Id,
        teacher.FullName,
        teacher.Phone?.Value,
        teacher.Status,
        teacher.UserId,
        userDisplayName,
        activeGroupCount,
        version);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
