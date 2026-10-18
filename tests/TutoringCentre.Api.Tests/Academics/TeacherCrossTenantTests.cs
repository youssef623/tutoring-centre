using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 43.14: owner A cannot see or touch B's teachers, proven in both directions with CrossTenantProbe
/// across all five ID routes, using a real victim teacher and its real current version so a 409 could never
/// mask a leak. The account link gets its own reference probe: linking the attacker's own teacher to a user
/// who belongs to the victim's centre must answer teacher.user_not_eligible, the same answer as any other
/// ineligible user, never disclosing that the ID itself resolved to someone real.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class TeacherCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisTeachers() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesTeachers() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test");

    [Fact]
    public async Task SetAccount_WithAnotherCentresUserId_ReturnsUserNotEligibleAndChangesNothing()
    {
        using var nileOwner = await SignInHelper.SignInAsync(factory, "owner@nile.test");
        using var maadiOwner = await SignInHelper.SignInAsync(factory, "owner@maadi.test");

        var (teacherId, version) = await CreateTeacherAsync(nileOwner.Client, "IDOR Target");

        using var maadiMeResponse = await maadiOwner.Client.GetAsync(new Uri("/api/me", UriKind.Relative));
        var maadiMe = await ReadJsonAsync(maadiMeResponse);
        var maadiUserId = maadiMe.RootElement.GetProperty("userId").GetGuid();

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative),
            new { userId = maadiUserId, version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("teacher.user_not_eligible", body.RootElement.GetProperty("code").GetString());

        using var getResponse = await nileOwner.Client.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var getBody = await ReadJsonAsync(getResponse);
        Assert.True(getBody.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        var (victimTeacherId, victimVersion) = await CreateTeacherAsync(victim.Client, "Victim Teacher");

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || status from academics.teachers where id = '{victimTeacherId}'");

        // GET by ID.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/teachers/{victimTeacherId}", body: null,
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // PUT with a valid body and the victim's real current version — never an invented one.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/teachers/{victimTeacherId}",
            new { fullName = "Hacked Name", phone = (string?)null, version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // Account link.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/teachers/{victimTeacherId}/account",
            new { userId = (Guid?)null, version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // Archive.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/teachers/{victimTeacherId}/archive", new { version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // Restore.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/teachers/{victimTeacherId}/restore", new { version = victimVersion },
            HttpStatusCode.NotFound, "teacher.not_found", SnapshotAsync);

        // The attacker's own list contains none of the victim's IDs or names.
        using var attackerListResponse = await attacker.Client.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative));
        var attackerList = await ReadJsonAsync(attackerListResponse);
        var attackerItems = attackerList.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.DoesNotContain(attackerItems, item => item.GetProperty("id").GetGuid() == victimTeacherId);
    }

    private static async Task<(Guid Id, uint Version)> CreateTeacherAsync(HttpClient client, string fullName)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri("/api/teachers", UriKind.Relative), new { fullName, phone = (string?)null });
        var location = createResponse.Headers.Location!;
        using var getResponse = await client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
