# `tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 39.9: owner A cannot see or touch B's parents, proven in both directions with CrossTenantProbe, using a real victim parent and its real current version so a 409 could never mask a leak. The lookup and create checks confirm phone uniqueness and visibility are per-centre, never global.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 4,067 bytes. Source hash: `4b813101c300a41b2ef3b7af1d594d25f98a8fcdeaef54bd775fcb65415f53d4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L14): `public sealed class ParentCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L16): `public async Task InitializeAsync()`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L22): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 25](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L25): `public Task NileOwner_CannotSeeOrTouchMaadisParents() =>`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L29): `public Task MaadiOwner_CannotSeeOrTouchNilesParents() =>`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L32): `private async Task RunDirectionAsync(string attackerEmail, string victimEmail, string victimPhone)`
- [Line 72](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L72): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 39.9: owner A cannot see or touch B's parents, proven in both directions with CrossTenantProbe,
/// using a real victim parent and its real current version so a 409 could never mask a leak. The lookup and
/// create checks confirm phone uniqueness and visibility are per-centre, never global.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ParentCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisParents() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test", victimPhone: "01212345678");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesParents() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test", victimPhone: "01512345670");

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail, string victimPhone)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            victim.Client, new Uri("/api/parents", UriKind.Relative), new { fullName = "Victim Parent", phone = victimPhone });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var victimId = (await ReadJsonAsync(createResponse)).RootElement.GetProperty("id").GetGuid();

        using var victimGetResponse = await victim.Client.GetAsync(new Uri($"/api/parents/{victimId}", UriKind.Relative));
        var victimVersion = (await ReadJsonAsync(victimGetResponse)).RootElement.GetProperty("version").GetUInt32();

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || phone || '|' || xmin from academics.parents where id = '{victimId}'");

        // GET by ID.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/parents/{victimId}", body: null,
            HttpStatusCode.NotFound, "parent.not_found", SnapshotAsync);

        // PUT with a valid body and the victim's real current version — never an invented one.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/parents/{victimId}",
            new { fullName = "Hacked Name", phone = victimPhone, version = victimVersion },
            HttpStatusCode.NotFound, "parent.not_found", SnapshotAsync);

        // Looking up the victim's exact phone from the attacker's centre finds nothing — never a cross-tenant match.
        using var lookupResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            attacker.Client, new Uri("/api/parents/lookup", UriKind.Relative), new { phone = victimPhone });
        var lookupBody = await ReadJsonAsync(lookupResponse);
        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
        Assert.Equal(JsonValueKind.Null, lookupBody.RootElement.GetProperty("parent").ValueKind);

        // Creating a parent with the victim's exact phone succeeds — uniqueness is per centre, not global.
        using var attackerCreateResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            attacker.Client, new Uri("/api/parents", UriKind.Relative), new { fullName = "Attacker's Own Parent", phone = victimPhone });
        Assert.Equal(HttpStatusCode.Created, attackerCreateResponse.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
