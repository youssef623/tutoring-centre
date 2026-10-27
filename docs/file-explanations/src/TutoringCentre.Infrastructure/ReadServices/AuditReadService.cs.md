# `src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs`

## Purpose

Audit reads apply filters and descending occurrence-plus-ID keyset paging, left join only actor display names, and convert stored JSON changes into DTO values. One extra row determines the next cursor. The adapter preserves audit rows without a current actor-user match and does not return actor email.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 121 lines and 4,970 bytes. Source hash: `d19d41de0f9f3b6edecabec9bfba983a55782aede4fa20ca84ae41ad37f2c240`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Text.Json;`
- Line 3: `using Microsoft.EntityFrameworkCore;`
- Line 4: `using TutoringCentre.Application.Audit;`
- Line 5: `using TutoringCentre.Infrastructure.Audit;`
- Line 6: `using TutoringCentre.Infrastructure.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L19): `internal sealed class AuditReadService(AppDbContext db) : IAuditReadService`
- [Line 21](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L21): `public async Task<AuditPageDto> GetPageAsync(`
- [Line 83](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L83): `private static AuditEntryDto ToDto(AuditEntry entry, string? actorDisplayName) => new(`
- [Line 95](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L95): `private static List<AuditChangeDto> ParseChanges(string changesJson)`
- [Line 113](../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L113): `private static string? ReadValue(JsonElement element) => element.ValueKind switch`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:157](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L157)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Audit;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of the audit log: a no-tracking projection over audit.audit_entries, joined to users only for the
/// actor's display name (never their email). No tenant predicate written by hand — the EF query filter and
/// row-level security (Day 31) scope this the same as <see cref="SubjectReadService"/>. Orders newest first by
/// (occurredAt, id), both descending, and never by timestamp alone, so a cursor resumes unambiguously even when
/// two rows share a timestamp.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AuditReadService(AppDbContext db) : IAuditReadService
{
    public async Task<AuditPageDto> GetPageAsync(
        string? entityType,
        Guid? entityId,
        Guid? actorUserId,
        DateTimeOffset? occurredFrom,
        DateTimeOffset? occurredTo,
        string? cursor,
        int pageSize,
        CancellationToken ct)
    {
        var entries = db.Set<AuditEntry>().AsNoTracking().AsQueryable();

        if (entityType is not null && AuditEntityTypeNames.TryParse(entityType, out var parsedEntityType))
        {
            entries = entries.Where(entry => entry.EntityType == parsedEntityType);
        }

        if (entityId is not null)
        {
            entries = entries.Where(entry => entry.EntityId == entityId.Value);
        }

        if (actorUserId is not null)
        {
            entries = entries.Where(entry => entry.ActorUserId == actorUserId.Value);
        }

        if (occurredFrom is not null)
        {
            entries = entries.Where(entry => entry.OccurredAt >= occurredFrom.Value);
        }

        if (occurredTo is not null)
        {
            entries = entries.Where(entry => entry.OccurredAt <= occurredTo.Value);
        }

        if (cursor is not null && AuditCursor.TryDecode(cursor, out var after))
        {
            entries = entries.Where(entry =>
                entry.OccurredAt < after.OccurredAt
                || (entry.OccurredAt == after.OccurredAt && entry.Id.CompareTo(after.Id) < 0));
        }

        var rows = await (
            from entry in entries
            join user in db.Set<ApplicationUser>().AsNoTracking() on entry.ActorUserId equals user.Id into actorUsers
            from actorUser in actorUsers.DefaultIfEmpty()
            orderby entry.OccurredAt descending, entry.Id descending
            select new { entry, ActorDisplayName = (string?)actorUser.DisplayName })
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = rows.Count > pageSize;
        var page = hasNextPage ? rows.Take(pageSize).ToList() : rows;

        var items = page.Select(row => ToDto(row.entry, row.ActorDisplayName)).ToList();
        var nextCursor = hasNextPage ? AuditCursor.Encode(page[^1].entry.OccurredAt, page[^1].entry.Id) : null;

        return new AuditPageDto(items, nextCursor);
    }

    private static AuditEntryDto ToDto(AuditEntry entry, string? actorDisplayName) => new(
        entry.Id,
        entry.OccurredAt,
        entry.ActorType.ToString().ToLowerInvariant(),
        entry.ActorUserId,
        actorDisplayName,
        entry.Action.ToString().ToLowerInvariant(),
        AuditEntityTypeNames.ToDatabase(entry.EntityType),
```

The excerpt stops at line 90; use the original file for the remaining 31 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
