# `tests/TutoringCentre.Api.Tests/Health/ReadinessPrivilegeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): The API test half of Task 19.7's eight tests: the runtime-role tripwire (Day 19) fails readiness closed.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessPrivilegeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 873 bytes. Source hash: `bcfb1c9b11156365ec23bd07c0b25404b6ad9990d25c6487e5d08044af73b202`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using Microsoft.AspNetCore.Hosting;`
- Line 3: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessPrivilegeTests.cs#L9): `public sealed class ReadinessPrivilegeTests(ApiFactory factory)`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Health/ReadinessPrivilegeTests.cs#L12): `public async Task GetReady_WithOwnerLoginAsRuntimeConnection_Returns503()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using Microsoft.AspNetCore.Hosting;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Health;

/// <summary>The API test half of Task 19.7's eight tests: the runtime-role tripwire (Day 19) fails readiness closed.</summary>
[Collection(ApiCollection.Name)]
public sealed class ReadinessPrivilegeTests(ApiFactory factory)
{
    [Fact]
    public async Task GetReady_WithOwnerLoginAsRuntimeConnection_Returns503()
    {
        await using var ownerFactory = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Postgres", factory.OwnerConnectionString));
        using var client = ownerFactory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
