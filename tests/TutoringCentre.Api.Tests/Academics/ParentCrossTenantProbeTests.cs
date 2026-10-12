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
