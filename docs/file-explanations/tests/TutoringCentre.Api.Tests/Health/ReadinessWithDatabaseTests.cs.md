# `tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs`

## Purpose

Declares `ReadinessWithDatabaseTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 640 bytes. Source hash: `5a7843d4b7932d08babbb6367ee179b8d6d0e2776530b6fe004ed38e80539247`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs#L7): `public sealed class ReadinessWithDatabaseTests(ApiFactory factory) : IAsyncLifetime`
- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs#L9): `public Task InitializeAsync() => factory.ResetAsync();`
- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs#L11): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs#L14): `public async Task GetReady_WithReachableDatabase_Returns200()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Health;

[Collection(ApiCollection.Name)]
public sealed class ReadinessWithDatabaseTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetReady_WithReachableDatabase_Returns200()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
