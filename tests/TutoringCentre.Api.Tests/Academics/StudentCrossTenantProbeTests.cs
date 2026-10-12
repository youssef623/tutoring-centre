using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 41.5: owner A cannot see or touch B's students, proven in both directions with CrossTenantProbe, using
/// a real victim student and its real current version so a 409 could never mask a leak. Reference probes
/// cover the second kind of attack: the attacker's own, legitimate URL with the victim's id planted inside it
/// (a body field or a route segment), which an ordinary cross-tenant probe — always aimed at the victim's own
/// URL — cannot exercise.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class StudentCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisStudents() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test", victimParentPhone: "01212340010", namePrefix: "Maadi Victim");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesStudents() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test", victimParentPhone: "01512340011", namePrefix: "Nile Victim");

    [Fact]
    public Task NileOwner_ReferenceAttacks_CannotReachMaadisParentOrStudent() =>
        RunReferenceDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test", victimParentPhone: "01212340020");

    [Fact]
    public Task MaadiOwner_ReferenceAttacks_CannotReachNilesParentOrStudent() =>
        RunReferenceDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test", victimParentPhone: "01512340021");

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail, string victimParentPhone, string namePrefix)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        var (victimStudentId, _, victimVersion) = await RegisterAsync(victim.Client, $"{namePrefix} Student", $"{namePrefix} Parent", victimParentPhone);

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || status || '|' || xmin from academics.students where id = '{victimStudentId}'");

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/students/{victimStudentId}", body: null,
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/students/{victimStudentId}",
            new { fullName = "Hacked Name", phone = (string?)null, schoolYear = (string?)null, version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/archive",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/restore",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents",
            new { parentId = Guid.NewGuid(), relationship = "mother", version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents/{Guid.NewGuid()}/unlink",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents/{Guid.NewGuid()}/make-primary",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        using var listResponse = await attacker.Client.GetAsync(new Uri("/api/students?pageSize=100", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var names = listBody.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("fullName").GetString()).ToList();
        Assert.DoesNotContain($"{namePrefix} Student", names);
    }

    private async Task RunReferenceDirectionAsync(string attackerEmail, string victimEmail, string victimParentPhone)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        var (_, victimParentId, _) = await RegisterAsync(victim.Client, "Reference Victim Student", "Reference Victim Parent", victimParentPhone);
        var (attackerStudentId, attackerParentId, attackerVersion) = await RegisterAsync(
            attacker.Client, "Reference Attacker Student", "Reference Attacker Parent", $"010{Random.Shared.Next(10000000, 99999999)}");

        Task<string> SnapshotAttackerStudentAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || xmin from academics.students where id = '{attackerStudentId}'");
        Task<string> SnapshotVictimParentAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || xmin from academics.parents where id = '{victimParentId}'");

        // Link the attacker's own student to the victim's parent id: the URL is the attacker's own, but the
        // body references a parent they cannot see.
        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{attackerStudentId}/parents",
            new { parentId = victimParentId, relationship = "mother", version = attackerVersion },
            HttpStatusCode.NotFound, "parent.not_found", SnapshotAttackerStudentAsync, SnapshotVictimParentAsync);

        // Register a brand-new student whose nested "existing parent" form references the victim's parent id.
        Task<string> CountAttackerStudentsAsync() =>
            factory.ScalarAsync<string>("select count(*)::text from academics.students");
        var studentsBefore = await CountAttackerStudentsAsync();

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, "/api/students",
            new { fullName = "Smuggled Via Reference", phone = (string?)null, schoolYear = (string?)null, parent = new { id = victimParentId }, relationship = "father" },
            HttpStatusCode.NotFound, "parent.not_found", CountAttackerStudentsAsync, SnapshotVictimParentAsync);

        Assert.Equal(studentsBefore, await CountAttackerStudentsAsync());

        // Registering a NEW parent whose phone happens to exist only in the victim's centre succeeds: phone
        // uniqueness is per centre, never global, so this is not a leak.
        using var newParentSameWording = await AntiforgeryTestHelper.PostAsJsonAsync(
            attacker.Client, new Uri("/api/students", UriKind.Relative),
            new { fullName = "Legitimate New Registration", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Coincidental Namesake", phone = victimParentPhone }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, newParentSameWording.StatusCode);

        Assert.True(attackerParentId != Guid.Empty);
    }

    private static async Task<(Guid StudentId, Guid ParentId, uint Version)> RegisterAsync(
        HttpClient client, string fullName, string parentFullName, string parentPhone)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri("/api/students", UriKind.Relative),
            new { fullName, phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = parentFullName, phone = parentPhone }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var body = await ReadJsonAsync(createResponse);
        var studentId = body.RootElement.GetProperty("id").GetGuid();
        var parentId = body.RootElement.GetProperty("parentId").GetGuid();

        using var getResponse = await client.GetAsync(new Uri($"/api/students/{studentId}", UriKind.Relative));
        var getBody = await ReadJsonAsync(getResponse);
        var version = getBody.RootElement.GetProperty("version").GetUInt32();

        return (studentId, parentId, version);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
