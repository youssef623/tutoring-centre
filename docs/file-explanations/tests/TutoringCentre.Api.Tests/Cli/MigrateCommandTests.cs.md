# `tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs`

## Purpose

Declares `MigrateCommandTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 813 bytes. Source hash: `73d13acf8eebd44090365d7e5e14737586c9f7fcc28424745850f5a4a0cdba25`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Cli;`
- Line 2: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs#L7): `public sealed class MigrateCommandTests(ApiFactory factory)`
- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs#L10): `public async Task RunAsync_CalledTwice_SecondRunIsANoOp()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class MigrateCommandTests(ApiFactory factory)
{
    [Fact]
    public async Task RunAsync_CalledTwice_SecondRunIsANoOp()
    {
        var historyCountBefore = await factory.ScalarAsync<long>("select count(*) from platform.__ef_migrations_history");

        var firstExitCode = await MigrateCommand.RunAsync(factory.Services);
        var secondExitCode = await MigrateCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(
            historyCountBefore,
            await factory.ScalarAsync<long>("select count(*) from platform.__ef_migrations_history"));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
