using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.7: the ownership guard on enrollment, as black-box behaviour through teacher@both.test's real session.
/// The roster inherits its group's rule: 200 for the own group, 404 (never 403) for another teacher's. A student
/// enrolled in both an own and another teacher's group shows the teacher only the own enrollment. Writes are 403
/// even on the own group, because permission is evaluated before ownership, and they change nothing.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentTeacherScopeTests(ApiFactory factory) : IAsyncLifetime
{
    private const string TwoCentreTeacherEmail = "teacher@both.test";

    private static int _nextPhoneSuffix = 50000000;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task LinkedTeacher_RosterOfTheOwnGroup_Returns200WithItsStudents()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}/roster", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var roster = (await ReadJsonAsync(response)).RootElement;
        Assert.Equal(arranged.OwnGroupId, roster.GetProperty("groupId").GetGuid());
        Assert.Equal(arranged.StudentId, Assert.Single(roster.GetProperty("items").EnumerateArray()).GetProperty("studentId").GetGuid());
    }

    [Fact]
    public async Task LinkedTeacher_RosterOfAnotherTeachersGroup_Returns404GroupNotFound()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OtherGroupId}/roster", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task LinkedTeacher_StudentEnrolledInOwnAndOtherGroup_ListsOnlyTheOwnEnrollment()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var teacherResponse = await teacher.Client.GetAsync(new Uri($"/api/students/{arranged.StudentId}/enrollments", UriKind.Relative));
        using var ownerResponse = await owner.Client.GetAsync(new Uri($"/api/students/{arranged.StudentId}/enrollments", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, teacherResponse.StatusCode);
        var only = Assert.Single((await ReadJsonAsync(teacherResponse)).RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(arranged.OwnEnrollmentId, only.GetProperty("enrollmentId").GetGuid());
        Assert.Equal(arranged.OwnGroupId, only.GetProperty("groupId").GetGuid());

        // The same route as the owner shows both, so the teacher's single item is the scope, not missing data.
        Assert.Equal(2, (await ReadJsonAsync(ownerResponse)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData("enroll")]
    [InlineData("withdraw")]
    [InlineData("transfer")]
    public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)
    {
        var arranged = await ArrangeAsync();
        var version = await GetRosterVersionAsOwnerAsync(arranged.OwnGroupId, arranged.OwnEnrollmentId);
        var newStudentId = await RegisterStudentAsOwnerAsync("Scope Write Student");
        var before = await SnapshotEnrollmentsAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        var (url, body) = route switch
        {
            "enroll" => ($"/api/groups/{arranged.OwnGroupId}/enrollments", (object)new { studentId = newStudentId }),
            "withdraw" => ($"/api/enrollments/{arranged.OwnEnrollmentId}/withdraw", new { version }),
            _ => ($"/api/enrollments/{arranged.OwnEnrollmentId}/transfer", new { targetGroupId = arranged.OwnSecondGroupId, version }),
        };

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(teacher.Client, new Uri(url, UriKind.Relative), body);

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.permission_denied");
        Assert.Equal(before, await SnapshotEnrollmentsAsync());
    }

    /// <summary>
    /// As the Nile owner, over HTTP: one subject, two teachers, the first linked to teacher@both.test and owning
    /// two groups, the second owning one; one student enrolled in an own group and in the other teacher's group.
    /// Any teacher record already linked to that user in Nile (the demo seed links one) is unlinked first, so
    /// "own" means exactly the groups arranged here.
    /// </summary>
    private async Task<Arrangement> ArrangeAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var teacherUserId = await FindUserIdAsync(owner.Client, TwoCentreTeacherEmail);

        using (var teachersResponse = await owner.Client.GetAsync(new Uri("/api/teachers?includeArchived=true", UriKind.Relative)))
        {
            var teachers = await ReadJsonAsync(teachersResponse);
            foreach (var existing in teachers.RootElement.GetProperty("items").EnumerateArray())
            {
                if (existing.GetProperty("userId").ValueKind == JsonValueKind.String && existing.GetProperty("userId").GetGuid() == teacherUserId)
                {
                    await SetTeacherAccountAsync(owner.Client, existing.GetProperty("id").GetGuid(), null);
                }
            }
        }

        var subjectId = await CreateAsync(owner.Client, "/api/subjects", new { name = "Scope Enrollment Physics" });
        var linkedTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Linked Teacher", phone = (string?)null });
        var otherTeacherId = await CreateAsync(owner.Client, "/api/teachers", new { fullName = "Scope Other Teacher", phone = (string?)null });
        await SetTeacherAccountAsync(owner.Client, linkedTeacherId, teacherUserId);
        var ownGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Own Group", subjectId, teacherId = linkedTeacherId, capacity = 20 });
        var ownSecondGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Own Second Group", subjectId, teacherId = linkedTeacherId, capacity = 20 });
        var otherGroupId = await CreateAsync(
            owner.Client, "/api/groups", new { name = "Scope Other Group", subjectId, teacherId = otherTeacherId, capacity = 20 });
        var studentId = await RegisterStudentAsync(owner.Client, "Scope Shared Student");
        var ownEnrollmentId = await CreateAsync(owner.Client, $"/api/groups/{ownGroupId}/enrollments", new { studentId });
        await CreateAsync(owner.Client, $"/api/groups/{otherGroupId}/enrollments", new { studentId });

        return new Arrangement(ownGroupId, ownSecondGroupId, otherGroupId, studentId, ownEnrollmentId);
    }

    private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)
    {
        using var response = await ownerClient.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("email").GetString() == email).GetProperty("userId").GetGuid();
    }

    private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)
    {
        using var getResponse = await ownerClient.GetAsync(new Uri($"/api/teachers/{teacherId}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            ownerClient, HttpMethod.Put, new Uri($"/api/teachers/{teacherId}/account", UriKind.Relative), new { userId, version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<Guid> RegisterStudentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        return await RegisterStudentAsync(owner.Client, fullName);
    }

    private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>
        CreateAsync(
            client, "/api/students",
            new
            {
                fullName, phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = $"{fullName} Parent", phone = UniquePhone() }, relationship = "father",
            });

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private async Task<uint> GetRosterVersionAsOwnerAsync(Guid groupId, Guid enrollmentId)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}/roster", UriKind.Relative));
        return (await ReadJsonAsync(response)).RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("enrollmentId").GetGuid() == enrollmentId).GetProperty("version").GetUInt32();
    }

    private Task<string> SnapshotEnrollmentsAsync() =>
        factory.ScalarAsync<string>(
            "select count(*)::text || '|' || coalesce(string_agg(id::text || ':' || status || ':' || xmin::text, ',' order by id), '') from academics.enrollments");

    private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)
    {
        var body = await ReadJsonAsync(response);
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }

    private sealed record Arrangement(Guid OwnGroupId, Guid OwnSecondGroupId, Guid OtherGroupId, Guid StudentId, Guid OwnEnrollmentId);
}
