using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>
/// Task 30.7: a Maadi owner cannot see or touch a Nile membership, and vice versa — proven in both directions
/// with CrossTenantProbe, using the victim's real membership ID and real current version so a 409 could never
/// mask a leak. identity.memberships carries no RLS and no EF query filter (Day 29), so this is the only proof.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class StaffCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotTouchMaadisOwnMembership() => RunDirectionAsync(
        attackerEmail: "owner@nile.test", victimListerEmail: "owner@maadi.test", victimEmail: "owner@maadi.test", victimOnlyEmailFragment: "maadi");

    [Fact]
    public Task MaadiOwner_CannotTouchNilesSecretaryMembership() => RunDirectionAsync(
        attackerEmail: "owner@maadi.test", victimListerEmail: "owner@nile.test", victimEmail: "secretary@nile.test", victimOnlyEmailFragment: "nile");

    private async Task RunDirectionAsync(string attackerEmail, string victimListerEmail, string victimEmail, string victimOnlyEmailFragment)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victimOwner = await SignInHelper.SignInAsync(factory, victimListerEmail);

        var (victimMembershipId, victimVersion) = await FindMemberAsync(victimOwner.Client, victimEmail);

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select role || '|' || status || '|' || xmin from identity.memberships where id = '{victimMembershipId}'");

        // Role change.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/staff/{victimMembershipId}/role",
            new { role = "teacher", version = victimVersion },
            HttpStatusCode.NotFound, "staff.not_found", SnapshotAsync);

        // Deactivate.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/staff/{victimMembershipId}/deactivate",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "staff.not_found", SnapshotAsync);

        // Reactivate.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/staff/{victimMembershipId}/reactivate",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "staff.not_found", SnapshotAsync);

        // The attacker's own list contains no membership whose email belongs only to the victim's centre.
        using var attackerListResponse = await attacker.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var attackerList = await ReadJsonAsync(attackerListResponse);
        var attackerEmails = attackerList.RootElement.GetProperty("items").EnumerateArray()
            .Select(item => item.GetProperty("email").GetString()!).ToList();
        Assert.DoesNotContain(attackerEmails, email => email.Contains(victimOnlyEmailFragment, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<(Guid MembershipId, uint Version)> FindMemberAsync(HttpClient client, string email)
    {
        using var response = await client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        var member = body.RootElement.GetProperty("items").EnumerateArray().Single(item => item.GetProperty("email").GetString() == email);
        return (member.GetProperty("membershipId").GetGuid(), member.GetProperty("version").GetUInt32());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
