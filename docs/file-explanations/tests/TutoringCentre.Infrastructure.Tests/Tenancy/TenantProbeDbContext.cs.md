# `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs`

## Purpose

Source summary (intent, not independent execution proof): Extends the production AppDbContext (Task 20.3) with the test-only TenantProbe mapping, so the production tenant query filter convention, interceptors and unit of work all apply to it unchanged.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,187 bytes. Source hash: `f54fab5112fea9773de3dab667b8135fa26955819c44c0b8484916f84ce0c843`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs#L11): `internal sealed class TenantProbeDbContext : AppDbContext`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs#L16): `public TenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:50](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs#L50)
- [tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs:19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs#L19)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs:36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Extends the production AppDbContext (Task 20.3) with the test-only TenantProbe mapping, so the production
/// tenant query filter convention, interceptors and unit of work all apply to it unchanged.
/// </summary>
internal sealed class TenantProbeDbContext : AppDbContext
{
    // Reuses AppDbContext's own options type rather than DbContextOptions&lt;TenantProbeDbContext&gt;: EF's DI
    // activation refuses a non-generic DbContextOptions constructor parameter once a second DbContext type is
    // registered in the same container, so this is built by hand (PostgresFixture), not through AddDbContext.
    public TenantProbeDbContext(DbContextOptions<AppDbContext> options, ICurrentActor currentActor)
        : base(options, currentActor)
    {
    }

    protected override void ExtendModel(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TenantProbeConfiguration());
        builder.ApplyConfiguration(new TenantProbeChildConfiguration());
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
