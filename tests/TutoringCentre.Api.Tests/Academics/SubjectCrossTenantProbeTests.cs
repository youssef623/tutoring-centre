using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 26.10: the week's first demo as an automated test — owner A cannot see or touch B's subjects, proven in
/// both directions with CrossTenantProbe, using a real victim subject and its real current version so a 409
/// could never mask a leak.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisSubjects() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesSubjects() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test");

    [Fact]
    public async Task TwoCentreTeacher_SeesNilesListThenMaadisAfterSwitching_NeverAMix()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        var nileNames = await ListNamesAsync(teacher.Client);
        Assert.Equal(3, nileNames.Count);
        Assert.DoesNotContain("Mathematics", nileNames);
        Assert.DoesNotContain("Physics", nileNames);

        await teacher.SwitchCentreAsync("maadi-hub");

        var maadiNames = await ListNamesAsync(teacher.Client);
        Assert.Equal(2, maadiNames.Count);
        Assert.DoesNotContain("رياضيات", maadiNames);
        Assert.DoesNotContain("فيزياء", maadiNames);
        Assert.DoesNotContain("كيمياء", maadiNames);
    }

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        using var victimListResponse = await victim.Client.GetAsync(new Uri("/api/subjects", UriKind.Relative));
        var victimList = await ReadJsonAsync(victimListResponse);
        var firstVictimSubject = victimList.RootElement.GetProperty("items")[0];
        var victimSubjectId = firstVictimSubject.GetProperty("id").GetGuid();
        var victimSubjectName = firstVictimSubject.GetProperty("name").GetString()!;
        var victimVersion = firstVictimSubject.GetProperty("version").GetUInt32();

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select name || '|' || status from academics.subjects where id = '{victimSubjectId}'");

        // GET by ID.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/subjects/{victimSubjectId}", body: null,
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // PUT with a valid body and the victim's real current version — never an invented one.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/subjects/{victimSubjectId}",
            new { name = "Hacked Name", version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // Archive.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/subjects/{victimSubjectId}/archive", new { version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // Restore.
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/subjects/{victimSubjectId}/restore", new { version = victimVersion },
            HttpStatusCode.NotFound, "subject.not_found", SnapshotAsync);

        // The attacker's own list contains none of the victim's IDs or names.
        using var attackerListResponse = await attacker.Client.GetAsync(new Uri("/api/subjects?includeArchived=true", UriKind.Relative));
        var attackerList = await ReadJsonAsync(attackerListResponse);
        var attackerItems = attackerList.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.DoesNotContain(attackerItems, item => item.GetProperty("id").GetGuid() == victimSubjectId);
        Assert.DoesNotContain(attackerItems, item => item.GetProperty("name").GetString() == victimSubjectName);

        // Creating a subject with the victim's exact name succeeds — uniqueness is per centre, not global, so
        // this must never surface a cross-tenant conflict signal.
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            attacker.Client, new Uri("/api/subjects", UriKind.Relative), new { name = victimSubjectName });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    }

    private static async Task<List<string>> ListNamesAsync(HttpClient client)
    {
        using var response = await client.GetAsync(new Uri("/api/subjects", UriKind.Relative));
        var json = await ReadJsonAsync(response);
        return json.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("name").GetString()!).ToList();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
