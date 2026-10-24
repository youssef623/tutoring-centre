using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.7: tenant isolation for enrollment at the HTTP boundary, every case in both directions. Enrollment joins
/// three resources (group, student, enrollment), so each of the three is the foreign one in at least one case,
/// and mixed cases put one foreign ID inside an otherwise own request — the "everything foreign" case alone would
/// be stopped by the group lookup and prove nothing about the student or target-group lookups. Each case answers
/// the 404 of the first foreign thing the handler meets (group before student; enrollment before target group),
/// and the rows of both centres are compared before and after through the superuser connection.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    private static int _nextPhoneSuffix = 40000000;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_OwnGroupWithTheOtherCentresStudent_Returns404StudentNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerGroupId = await CreateGroupAsync(attacker.Client, "Attacker");
        var victimStudentId = await RegisterStudentAsync(victim.Client, "Victim Student");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{attackerGroupId}/enrollments", new { studentId = victimStudentId },
            HttpStatusCode.NotFound, "student.not_found",
            () => SnapshotGroupAsync(attackerGroupId), () => SnapshotStudentAsync(victimStudentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_TheOtherCentresGroupWithOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerStudentId = await RegisterStudentAsync(attacker.Client, "Attacker Student");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{victimGroupId}/enrollments", new { studentId = attackerStudentId },
            HttpStatusCode.NotFound, "group.not_found",
            () => SnapshotStudentAsync(attackerStudentId), () => SnapshotGroupAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_TheOtherCentresGroupWithThatCentresOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimStudentId = await RegisterStudentAsync(victim.Client, "Victim Student");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{victimGroupId}/enrollments", new { studentId = victimStudentId },
            HttpStatusCode.NotFound, "group.not_found",
            () => SnapshotGroupAsync(victimGroupId), () => SnapshotStudentAsync(victimStudentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Withdraw_TheOtherCentresEnrollmentWithItsRealVersion_Returns404EnrollmentNotFoundAndLeavesItActive(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimEnrollmentId = await EnrollAsync(victim.Client, victimGroupId, await RegisterStudentAsync(victim.Client, "Victim Student"));
        var version = await GetRosterVersionAsync(victim.Client, victimGroupId, victimEnrollmentId);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/enrollments/{victimEnrollmentId}/withdraw", new { version },
            HttpStatusCode.NotFound, "enrollment.not_found", () => SnapshotGroupAsync(victimGroupId));
        Assert.Equal("active", await EnrollmentStatusAsync(victimEnrollmentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Transfer_TheOtherCentresEnrollmentIntoOwnGroup_Returns404EnrollmentNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerGroupId = await CreateGroupAsync(attacker.Client, "Attacker");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimEnrollmentId = await EnrollAsync(victim.Client, victimGroupId, await RegisterStudentAsync(victim.Client, "Victim Student"));
        var version = await GetRosterVersionAsync(victim.Client, victimGroupId, victimEnrollmentId);

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/enrollments/{victimEnrollmentId}/transfer", new { targetGroupId = attackerGroupId, version },
            HttpStatusCode.NotFound, "enrollment.not_found",
            () => SnapshotGroupAsync(attackerGroupId), () => SnapshotGroupAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Transfer_TheOtherCentresEnrollmentWithinThatCentre_Returns404EnrollmentNotFoundAndChangesNothing(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var (victimSourceId, victimTargetId) = await CreateSameSubjectGroupsAsync(victim.Client, "Victim");
        var victimEnrollmentId = await EnrollAsync(victim.Client, victimSourceId, await RegisterStudentAsync(victim.Client, "Victim Student"));
        var version = await GetRosterVersionAsync(victim.Client, victimSourceId, victimEnrollmentId);

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/enrollments/{victimEnrollmentId}/transfer", new { targetGroupId = victimTargetId, version },
            HttpStatusCode.NotFound, "enrollment.not_found",
            () => SnapshotGroupAsync(victimSourceId), () => SnapshotGroupAsync(victimTargetId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Transfer_OwnEnrollmentIntoTheOtherCentresGroup_Returns404GroupNotFoundAndTheEnrollmentStaysActive(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerGroupId = await CreateGroupAsync(attacker.Client, "Attacker");
        var attackerEnrollmentId = await EnrollAsync(attacker.Client, attackerGroupId, await RegisterStudentAsync(attacker.Client, "Attacker Student"));
        var version = await GetRosterVersionAsync(attacker.Client, attackerGroupId, attackerEnrollmentId);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/enrollments/{attackerEnrollmentId}/transfer", new { targetGroupId = victimGroupId, version },
            HttpStatusCode.NotFound, "group.not_found",
            () => SnapshotGroupAsync(attackerGroupId), () => SnapshotGroupAsync(victimGroupId));
        Assert.Equal("active", await EnrollmentStatusAsync(attackerEnrollmentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Roster_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        await EnrollAsync(victim.Client, victimGroupId, await RegisterStudentAsync(victim.Client, "Victim Student"));

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/groups/{victimGroupId}/roster", null,
            HttpStatusCode.NotFound, "group.not_found", () => SnapshotGroupAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task StudentEnrollments_OfTheOtherCentresStudent_Returns404StudentNotFound(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimStudentId = await RegisterStudentAsync(victim.Client, "Victim Student");
        await EnrollAsync(victim.Client, victimGroupId, victimStudentId);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/students/{victimStudentId}/enrollments", null,
            HttpStatusCode.NotFound, "student.not_found", () => SnapshotStudentAsync(victimStudentId));
    }

    private static async Task<Guid> CreateGroupAsync(HttpClient client, string prefix)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{prefix} Subject {Guid.NewGuid():N}" });
        var teacherId = await CreateAsync(client, "/api/teachers", new { fullName = $"{prefix} Teacher", phone = (string?)null });
        return await CreateAsync(client, "/api/groups", new { name = $"{prefix} Group {Guid.NewGuid():N}", subjectId, teacherId, capacity = 20 });
    }

    private static async Task<(Guid SourceId, Guid TargetId)> CreateSameSubjectGroupsAsync(HttpClient client, string prefix)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{prefix} Subject {Guid.NewGuid():N}" });
        var teacherId = await CreateAsync(client, "/api/teachers", new { fullName = $"{prefix} Teacher", phone = (string?)null });
        var sourceId = await CreateAsync(client, "/api/groups", new { name = $"{prefix} Source {Guid.NewGuid():N}", subjectId, teacherId, capacity = 20 });
        var targetId = await CreateAsync(client, "/api/groups", new { name = $"{prefix} Target {Guid.NewGuid():N}", subjectId, teacherId, capacity = 20 });
        return (sourceId, targetId);
    }

    private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>
        CreateAsync(
            client, "/api/students",
            new
            {
                fullName, phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = $"{fullName} Parent", phone = UniquePhone() }, relationship = "father",
            });

    private static Task<Guid> EnrollAsync(HttpClient client, Guid groupId, Guid studentId) =>
        CreateAsync(client, $"/api/groups/{groupId}/enrollments", new { studentId });

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<uint> GetRosterVersionAsync(HttpClient client, Guid groupId, Guid enrollmentId)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{groupId}/roster", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("enrollmentId").GetGuid() == enrollmentId).GetProperty("version").GetUInt32();
    }

    // The group row plus every enrollment row in it, with status, end reason and row version: any enrollment
    // written, ended or touched in the group changes this string, and so does any change to the group itself.
    private Task<string> SnapshotGroupAsync(Guid groupId) =>
        factory.ScalarAsync<string>(
            $"""
            select g.capacity || '|' || g.status || '|' || g.xmin::text || '|' || (
                select count(*)::text || ':' || coalesce(string_agg(e.id::text || ':' || e.status || ':' || coalesce(e.end_reason, '-') || ':' || e.xmin::text, ',' order by e.id), '')
                from academics.enrollments e where e.group_id = g.id)
            from academics.groups g where g.id = '{groupId}'
            """);

    // The student row plus every enrollment row of that student, in any group.
    private Task<string> SnapshotStudentAsync(Guid studentId) =>
        factory.ScalarAsync<string>(
            $"""
            select s.status || '|' || s.xmin::text || '|' || (
                select count(*)::text || ':' || coalesce(string_agg(e.id::text || ':' || e.status || ':' || e.xmin::text, ',' order by e.id), '')
                from academics.enrollments e where e.student_id = s.id)
            from academics.students s where s.id = '{studentId}'
            """);

    private Task<string> EnrollmentStatusAsync(Guid enrollmentId) =>
        factory.ScalarAsync<string>($"select status from academics.enrollments where id = '{enrollmentId}'");

    private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
