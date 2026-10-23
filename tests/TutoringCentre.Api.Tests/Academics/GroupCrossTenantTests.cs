using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 45.7: tenant isolation for groups at the HTTP boundary, every case in both directions. Ordinary probes
/// attack the victim's group directly on all six ID routes with its real current version, so a 409 could never
/// mask a missing guard. Reference probes put the victim centre's subject or teacher ID inside the attacker's
/// own request: the handler must resolve it inside the attacker's centre and answer not-found, with no row
/// written on either side.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class GroupCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task EveryIdRoute_OnTheOtherCentresGroup_Returns404AndLeavesTheRowUnchanged(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerTeacherId = (await CreateReferencesAsync(attacker.Client, "Attacker")).TeacherId;
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim Group", victimRefs);
        var version = await GetGroupVersionAsync(victim.Client, victimGroupId);
        var url = $"/api/groups/{victimGroupId}";
        Task<string> SnapshotAsync() => SnapshotGroupAsync(victimGroupId);

        await CrossTenantProbe.RunAsync(attacker.Client, HttpMethod.Get, url, null, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, url, new { name = "Hacked Name", version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"{url}/capacity", new { capacity = 2, version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"{url}/teacher", new { teacherId = attackerTeacherId, version },
            HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"{url}/archive", new { version }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);

        // Restore needs an archived victim: archived by the victim itself, then probed with its new real version.
        using (var archived = await AntiforgeryTestHelper.SendAsJsonAsync(
            victim.Client, HttpMethod.Post, new Uri($"{url}/archive", UriKind.Relative), new { version }))
        {
            Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);
        }

        var archivedVersion = await GetGroupVersionAsync(victim.Client, victimGroupId);
        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"{url}/restore", new { version = archivedVersion }, HttpStatusCode.NotFound, "group.not_found", SnapshotAsync);
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task List_ContainsNoneOfTheOtherCentresGroups(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim Only Group", victimRefs);

        using var response = await attacker.Client.GetAsync(new Uri("/api/groups?includeArchived=true", UriKind.Relative));

        var items = (await ReadJsonAsync(response)).RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(items, item => item.GetProperty("id").GetGuid() == victimGroupId);
        Assert.DoesNotContain(items, item => item.GetProperty("name").GetString() == "Victim Only Group");
        Assert.DoesNotContain(items, item => item.GetProperty("subjectId").GetGuid() == victimRefs.SubjectId);

        using var filtered = await attacker.Client.GetAsync(new Uri($"/api/groups?subjectId={victimRefs.SubjectId}&includeArchived=true", UriKind.Relative));
        Assert.Equal(0, (await ReadJsonAsync(filtered)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Create_WithTheOtherCentresSubject_Returns404SubjectNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerRefs = await CreateReferencesAsync(attacker.Client, "Attacker");
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, "/api/groups",
            new { name = "Reference Probe", subjectId = victimRefs.SubjectId, teacherId = attackerRefs.TeacherId, capacity = 20 },
            HttpStatusCode.NotFound, "subject.not_found",
            SnapshotAllGroupsAsync, () => SnapshotSubjectAsync(victimRefs.SubjectId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Create_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerRefs = await CreateReferencesAsync(attacker.Client, "Attacker");
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, "/api/groups",
            new { name = "Reference Probe", subjectId = attackerRefs.SubjectId, teacherId = victimRefs.TeacherId, capacity = 20 },
            HttpStatusCode.NotFound, "teacher.not_found",
            SnapshotAllGroupsAsync, () => SnapshotTeacherAsync(victimRefs.TeacherId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task AssignTeacher_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerRefs = await CreateReferencesAsync(attacker.Client, "Attacker");
        var attackerGroupId = await CreateGroupAsync(attacker.Client, "Attacker Group", attackerRefs);
        var version = await GetGroupVersionAsync(attacker.Client, attackerGroupId);
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Put, $"/api/groups/{attackerGroupId}/teacher",
            new { teacherId = victimRefs.TeacherId, version },
            HttpStatusCode.NotFound, "teacher.not_found",
            () => SnapshotGroupAsync(attackerGroupId), () => SnapshotTeacherAsync(victimRefs.TeacherId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Create_WithANameUsedInTheOtherCentre_Returns201(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimRefs = await CreateReferencesAsync(victim.Client, "Victim");
        await CreateGroupAsync(victim.Client, "Shared Group Name", victimRefs);
        var attackerRefs = await CreateReferencesAsync(attacker.Client, "Attacker");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            attacker.Client, new Uri("/api/groups", UriKind.Relative),
            new { name = "Shared Group Name", subjectId = attackerRefs.SubjectId, teacherId = attackerRefs.TeacherId, capacity = 20 });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<References> CreateReferencesAsync(HttpClient client, string prefix)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{prefix} Subject" });
        var teacherId = await CreateAsync(client, "/api/teachers", new { fullName = $"{prefix} Teacher", phone = (string?)null });
        return new References(subjectId, teacherId);
    }

    private static Task<Guid> CreateGroupAsync(HttpClient client, string name, References references) =>
        CreateAsync(client, "/api/groups", new { name, subjectId = references.SubjectId, teacherId = references.TeacherId, capacity = 20 });

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<uint> GetGroupVersionAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));
        return (await ReadJsonAsync(response)).RootElement.GetProperty("version").GetUInt32();
    }

    private Task<string> SnapshotGroupAsync(Guid id) =>
        factory.ScalarAsync<string>(
            $"select name || '|' || capacity || '|' || teacher_id || '|' || status || '|' || xmin from academics.groups where id = '{id}'");

    // Every group in every centre: a reference attack on create must write no row anywhere.
    private Task<string> SnapshotAllGroupsAsync() =>
        factory.ScalarAsync<string>("select count(*)::text || '|' || coalesce(string_agg(id::text, ',' order by id), '') from academics.groups");

    private Task<string> SnapshotSubjectAsync(Guid id) =>
        factory.ScalarAsync<string>($"select name || '|' || status || '|' || xmin from academics.subjects where id = '{id}'");

    private Task<string> SnapshotTeacherAsync(Guid id) =>
        factory.ScalarAsync<string>($"select full_name || '|' || status || '|' || xmin from academics.teachers where id = '{id}'");

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }

    private sealed record References(Guid SubjectId, Guid TeacherId);
}
