# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 21.5: test-only requests that each depend on exactly one isolation layer, so a layer's own test can be proven to hold even when the other layer is bypassed or absent.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 50 lines and 2,466 bytes. Source hash: `94ec211ad23ce916f7d0319db0224b43541dab4013a8e93a31c8fe758e9a8ed6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L14): `internal sealed record ListProbeCentresIgnoringEfFilterQuery : IQuery<List<Guid>>;`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L17): `internal sealed record ListProbeCentresQuery : IQuery<List<Guid>>;`
- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L20): `internal sealed record ListProbeCentresByRawSqlQuery : IQuery<List<Guid>>;`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L22): `internal sealed class ListProbeCentresIgnoringEfFilterQueryHandler(AppDbContext db)`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L25): `public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresIgnoringEfFilterQuery query, CancellationToken cancellationToken)`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L32): `internal sealed class ListProbeCentresQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresQuery, List<Guid>>`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L34): `public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresQuery query, CancellationToken cancellationToken)`
- [Line 41](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L41): `internal sealed class ListProbeCentresByRawSqlQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresByRawSqlQuery, List<Guid>>`
- [Line 43](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs#L43): `public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresByRawSqlQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:181](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L181)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:182](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L182)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:183](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L183)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs:21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs:39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L39)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs:52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L52)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.5: test-only requests that each depend on exactly one isolation layer, so a layer's own test can be
/// proven to hold even when the other layer is bypassed or absent.
/// </summary>

/// <summary>Bypasses the EF query filter (Task 20.2) on purpose — only row-level security (Task 21.3) can still narrow this.</summary>
internal sealed record ListProbeCentresIgnoringEfFilterQuery : IQuery<List<Guid>>;

/// <summary>A normal, filtered EF query — relies only on the Task 20.2 filter, not on row-level security.</summary>
internal sealed record ListProbeCentresQuery : IQuery<List<Guid>>;

/// <summary>Raw SQL: never goes through the EF filter at all (it can't) — only row-level security narrows this.</summary>
internal sealed record ListProbeCentresByRawSqlQuery : IQuery<List<Guid>>;

internal sealed class ListProbeCentresIgnoringEfFilterQueryHandler(AppDbContext db)
    : IQueryHandler<ListProbeCentresIgnoringEfFilterQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresIgnoringEfFilterQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Set<TenantProbe>().IgnoreQueryFilters().Select(probe => probe.CentreId).ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}

internal sealed class ListProbeCentresQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Set<TenantProbe>().Select(probe => probe.CentreId).ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}

internal sealed class ListProbeCentresByRawSqlQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresByRawSqlQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresByRawSqlQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Database
            .SqlQueryRaw<Guid>("select centre_id from probe.tenant_probes")
            .ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
