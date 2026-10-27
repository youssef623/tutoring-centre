# `src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs`

## Purpose

Reads parent details and exact canonical-phone matches without tracking. The children query joins filtered links to filtered students, orders by name, and caps children at fifty. Phone value objects are converted to output strings after materialization to avoid unsupported SQL member translation.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 2,830 bytes. Source hash: `8a03e14b7450ce7201cc26f2924cf2f7c97a278eee3890eeb5b7549605b9f255`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Academics.Parents;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs#L18): `internal sealed class ParentReadService(AppDbContext db) : IParentReadService`
- [Line 22](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs#L22): `public async Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct)`
- [Line 52](../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs#L52): `public async Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:12](../../../../onboarding-work/extend_backend_purposes.py#L12)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:137](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L137)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Parent: no-tracking projections straight to DTOs. No tenant predicate written by hand — the
/// EF query filter and row-level security scope <c>parents</c>, <c>student_parents</c> and <c>students</c> the
/// same way, so the detail join can never reach another centre's row on any of the three tables. The phone
/// column is selected as the mapped <see cref="PhoneNumber"/> and turned into its E.164 string only after
/// materialisation, never inside the SQL-translated part of a projection.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ParentReadService(AppDbContext db) : IParentReadService
{
    private const int ChildrenCap = 50;

    public async Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct)
    {
        var parent = await db.Set<Parent>()
            .AsNoTracking()
            .Where(parent => parent.Id == id)
            .Select(parent => new { parent.Id, parent.FullName, parent.Phone, Version = EF.Property<uint>(parent, "Version") })
            .SingleOrDefaultAsync(ct);

        if (parent is null)
        {
            return null;
        }

        var children = await db.Set<StudentParentLink>()
            .AsNoTracking()
            .Where(link => link.ParentId == id)
            .Join(
                db.Set<Student>().AsNoTracking(),
                link => link.StudentId,
                student => student.Id,
                (link, student) => new { link, student })
            .OrderBy(joined => joined.student.FullName)
            .Take(ChildrenCap)
            .Select(joined => new ParentChildDto(
                joined.student.Id, joined.student.FullName, joined.student.Status, joined.link.Relationship, joined.link.IsPrimary))
            .ToListAsync(ct);

        return new ParentDetailDto(parent.Id, parent.FullName, parent.Phone.Value, parent.Version, children);
    }

    public async Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct)
    {
        var parent = await db.Set<Parent>()
            .AsNoTracking()
            .Where(parent => parent.Phone == phone)
            .Select(parent => new { parent.Id, parent.FullName, parent.Phone, Version = EF.Property<uint>(parent, "Version") })
            .SingleOrDefaultAsync(ct);

        return parent is null ? null : new ParentDto(parent.Id, parent.FullName, parent.Phone.Value, parent.Version);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
