# `tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs`

## Purpose

Declares `SystemInfoQueryTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 922 bytes. Source hash: `cf9052c44f51ab74d334fe77a613dcdb2cbd60a7c23d209462307239e6c93f04`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Platform;`
- Line 3: `using TutoringCentre.Application.Platform.Queries.GetSystemInfo;`
- Line 4: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs#L8): `public sealed class SystemInfoQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs#L11): `public async Task QueryAsync_MigratedDatabase_ReportsLatestMigrationAsUpToDate()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Platform.Queries.GetSystemInfo;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Platform;

public sealed class SystemInfoQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task QueryAsync_MigratedDatabase_ReportsLatestMigrationAsUpToDate()
    {
        var result = await Fixture.QueryAsAsync<GetSystemInfoQuery, SystemInfoDto>(new AnonymousActor(), new GetSystemInfoQuery());

        Assert.True(result.IsSuccess);
        Assert.EndsWith("_AddSchedule", result.Value.LatestMigration, StringComparison.Ordinal);
        Assert.True(result.Value.DatabaseUpToDate);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.ApplicationVersion));
        Assert.False(result.Value.DemoMode);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
