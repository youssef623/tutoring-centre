# `src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs`

## Purpose

Translates EF applied and pending migration metadata into the application SchemaStatus record. It returns the last applied migration and pending count, preventing EF migration types from leaking through the application port.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 19 lines and 883 bytes. Source hash: `24cedac997dadcd5059c3b0f3128a67f1ccf17b65a806a9f97168e8fb162aed4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using TutoringCentre.Application.Platform;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs#L10): `internal sealed class SystemInfoReadService(AppDbContext db) : ISystemInfoReadService`
- [Line 12](../../../../../src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs#L12): `public async Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:15](../../../../onboarding-work/extend_backend_purposes.py#L15)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:154](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L154)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Platform;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>Read side of platform status. Returns only DTO-shaped data; EF migration types never leave this class.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SystemInfoReadService(AppDbContext db) : ISystemInfoReadService
{
    public async Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct)
    {
        var applied = await db.Database.GetAppliedMigrationsAsync(ct);
        var pending = await db.Database.GetPendingMigrationsAsync(ct);

        return new SchemaStatus(applied.LastOrDefault(), pending.Count());
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
