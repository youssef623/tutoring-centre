# `tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 26.8: a probe that cannot fail proves nothing. This proves CrossTenantProbe actually can — using a real subject as a convenient, already-seeded resource (the helper itself still knows nothing about subjects).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 50 lines and 2,158 bytes. Source hash: `bfee61916f80508a446427ed9faee207ac8878af09cc0d6772296d5002e376a7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L12): `public sealed class CrossTenantProbeSelfTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L23): `public async Task RunAsync_AttackerIsActuallyAMemberOfTheSameCentre_FailsInsteadOfPassingSilently()`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L43): `private static async Task<Guid> GetAnyNileSubjectIdAsync(HttpClient nileOwnerClient)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Task 26.8: a probe that cannot fail proves nothing. This proves CrossTenantProbe actually can — using a real
/// subject as a convenient, already-seeded resource (the helper itself still knows nothing about subjects).
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class CrossTenantProbeSelfTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RunAsync_AttackerIsActuallyAMemberOfTheSameCentre_FailsInsteadOfPassingSilently()
    {
        // Not a real attacker: a legitimate Nile owner probing a Nile subject. The probe is told to expect the
        // isolation outcome (404) anyway — proving that when isolation does NOT hold (here, correctly: nothing
        // is actually broken), the helper reports a failure rather than silently agreeing with whatever happened.
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await GetAnyNileSubjectIdAsync(nileOwner.Client);

        var exception = await Record.ExceptionAsync(() => CrossTenantProbe.RunAsync(
            nileOwner.Client,
            HttpMethod.Get,
            $"/api/subjects/{subjectId}",
            body: null,
            expectedStatus: HttpStatusCode.NotFound,
            expectedCode: "subject.not_found",
            snapshotVictimRowAsync: () => Task.FromResult(string.Empty)));

        Assert.NotNull(exception);
    }

    private static async Task<Guid> GetAnyNileSubjectIdAsync(HttpClient nileOwnerClient)
    {
        using var response = await nileOwnerClient.GetAsync(new Uri("/api/subjects", UriKind.Relative));
        var text = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(text);
        return json.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
