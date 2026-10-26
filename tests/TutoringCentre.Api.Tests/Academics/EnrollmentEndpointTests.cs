using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.6: the five enrollment endpoints' contract through the full HTTP pipeline — status, code, body shape
/// and headers, never messages. Every test builds its own subjects, teachers, groups, students and enrollments
/// through the earlier endpoints over HTTP, so each one also exercises them, and the seeded demo data never
/// decides a result. Every error row of the Day 49 outcome table has its own case.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    // An interlocked counter never repeats within a run, unlike random digits, so no parent phone ever collides.
    private static int _nextPhoneSuffix = 30000000;

    private static readonly string[] ExpectedRosterOrder = ["ahmed Samir", "Mona Hassan", "Zeinab Ali", "إبراهيم علي", "أحمد سمير"];

    private static readonly string[] RosterEntryFields = ["enrolledAt", "enrollmentId", "studentId", "studentName", "version"];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Enroll_Returns201WithIdAndLocationAtTheRoster_AndTheRosterListsTheStudent()
    {
        // Only the owner may create teachers, so the owner arranges the group; the secretary does the rest.
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Happy");
        var studentId = await RegisterStudentAsync(secretary.Client, "Salma Fathy");

        using var response = await PostAsync(secretary.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/groups/{groupId}/roster", response.Headers.Location?.OriginalString);
        var body = await ReadJsonAsync(response);
        var enrollmentId = body.RootElement.GetProperty("id").GetGuid();
        Assert.Single(body.RootElement.EnumerateObject());

        using var rosterResponse = await secretary.Client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, rosterResponse.StatusCode);
        var entry = Assert.Single((await ReadJsonAsync(rosterResponse)).RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(enrollmentId, entry.GetProperty("enrollmentId").GetGuid());
        Assert.Equal(studentId, entry.GetProperty("studentId").GetGuid());
    }

    [Fact]
    public async Task Enroll_SameStudentAgain_Returns409AlreadyEnrolledAndWritesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Twice");
        var studentId = await RegisterStudentAsync(owner.Client, "Twice Student");
        await EnrollAsync(owner.Client, groupId, studentId);

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "enrollment.already_enrolled");
        Assert.Equal(1, await CountEnrollmentRowsAsync(groupId));
    }

    [Theory]
    [InlineData("groupId")]
    [InlineData("status")]
    public async Task Enroll_WithAFieldBeyondStudentId_Returns400RequestMalformedAndWritesNoRow(string extraField)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Over Posted");
        var otherGroupId = await CreateGroupAsync(owner.Client, "Enroll Over Posted Other");
        var studentId = await RegisterStudentAsync(owner.Client, "Over Posted Student");
        var body = new Dictionary<string, object>
        {
            ["studentId"] = studentId,
            [extraField] = extraField == "groupId" ? otherGroupId : "active",
        };

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", body);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
        Assert.Equal(0, await CountEnrollmentRowsAsync(otherGroupId));
    }

    [Fact]
    public async Task Enroll_MalformedJson_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Malformed");

        using var response = await PostRawAsync(owner.Client, $"/api/groups/{groupId}/enrollments", "{ \"studentId\": ");

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task Enroll_EmptyStudentId_Returns400WithErrorsStudentId()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Empty Id");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId = Guid.Empty });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("studentId").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Enroll_UnknownGroup_Returns404GroupNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var studentId = await RegisterStudentAsync(owner.Client, "No Group Student");

        using var response = await PostAsync(owner.Client, $"/api/groups/{Guid.NewGuid()}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task Enroll_UnknownStudent_Returns404StudentNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Unknown Student");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId = Guid.NewGuid() });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "student.not_found");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_IntoAFullGroup_Returns409GroupFullAndWritesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Full", capacity: 1);
        await EnrollAsync(owner.Client, groupId, await RegisterStudentAsync(owner.Client, "Seat Holder"));
        var latecomer = await RegisterStudentAsync(owner.Client, "Latecomer");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId = latecomer });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "group.full");
        Assert.Equal(1, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_IntoAnArchivedGroup_Returns422GroupArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Archived Group");
        await ArchiveGroupAsync(owner.Client, groupId);
        var studentId = await RegisterStudentAsync(owner.Client, "Archived Group Student");

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.archived");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_AnArchivedStudent_Returns422StudentArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Archived Student");
        var studentId = await RegisterStudentAsync(owner.Client, "Archived Student");
        await ArchiveStudentAsync(owner.Client, studentId);

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "enrollment.student_archived");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll No Token");
        var studentId = await RegisterStudentAsync(owner.Client, "No Token Student");

        using var response = await owner.Client.PostAsJsonAsync(new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.csrf_invalid");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_AsTeacher_Returns403PermissionDenied()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll As Teacher");
        var studentId = await RegisterStudentAsync(owner.Client, "Teacher Attempt Student");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await PostAsync(teacher.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.permission_denied");
        Assert.Equal(0, await CountEnrollmentRowsAsync(groupId));
    }

    [Fact]
    public async Task Enroll_BeforeSelectingACentre_Returns403TenantNotSelected()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        Assert.Null(teacher.CentreId);

        using var response = await PostAsync(teacher.Client, $"/api/groups/{Guid.NewGuid()}/enrollments", new { studentId = Guid.NewGuid() });

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "tenant.not_selected");
    }

    [Fact]
    public async Task Roster_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(new Uri($"/api/groups/{Guid.NewGuid()}/roster", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Roster_IsOrderedByNormalizedStudentName_HasTheGroupsFactsAndNamesOnly()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Roster Order", capacity: 12);
        // Enrolled out of order. "إبراهيم" sorts before "أحمد" only once the hamza forms are folded (raw
        // U+0625 sorts after U+0623), and "ahmed" sorts before "Mona" only once case is folded.
        var names = new[] { "Zeinab Ali", "أحمد سمير", "Mona Hassan", "إبراهيم علي", "ahmed Samir" };
        foreach (var name in names)
        {
            await EnrollAsync(owner.Client, groupId, await RegisterStudentAsync(owner.Client, name));
        }

        var roster = await GetRosterAsync(owner.Client, groupId);

        Assert.Equal(groupId, roster.GetProperty("groupId").GetGuid());
        Assert.Equal("Roster Order", roster.GetProperty("groupName").GetString());
        Assert.Equal(12, roster.GetProperty("capacity").GetInt32());
        Assert.Equal(5, roster.GetProperty("enrolledCount").GetInt32());
        var items = roster.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(
            ExpectedRosterOrder,
            items.Select(item => item.GetProperty("studentName").GetString()));

        // Names only: exactly the five roster-entry fields, no phone or parent of any kind.
        var entry = items[0];
        Assert.Equal(
            RosterEntryFields,
            entry.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
        Assert.True(entry.GetProperty("version").GetUInt32() > 0);
        Assert.Equal(5, roster.EnumerateObject().Count());
    }

    [Fact]
    public async Task Roster_UnknownGroup_Returns404GroupNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri($"/api/groups/{Guid.NewGuid()}/roster", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task GroupList_EnrolledCount_EqualsTheRosterLength()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateSubjectAsync(owner.Client, "Count Subject");
        var teacherId = await CreateTeacherAsync(owner.Client, "Count Teacher");
        var groupId = await CreateGroupAsync(owner.Client, "Count Group", subjectId, teacherId);
        for (var i = 0; i < 3; i++)
        {
            await EnrollAsync(owner.Client, groupId, await RegisterStudentAsync(owner.Client, $"Count Student {i}"));
        }

        var leaving = await RegisterStudentAsync(owner.Client, "Count Leaver");
        var leavingEnrollmentId = await EnrollAsync(owner.Client, groupId, leaving);
        using (var withdrawn = await PostAsync(
            owner.Client, $"/api/enrollments/{leavingEnrollmentId}/withdraw", new { version = await GetRosterVersionAsync(owner.Client, groupId, leavingEnrollmentId) }))
        {
            Assert.Equal(HttpStatusCode.NoContent, withdrawn.StatusCode);
        }

        var rosterLength = (await GetRosterAsync(owner.Client, groupId)).GetProperty("items").GetArrayLength();
        using var listResponse = await owner.Client.GetAsync(new Uri($"/api/groups?subjectId={subjectId}", UriKind.Relative));
        var listed = Assert.Single((await ReadJsonAsync(listResponse)).RootElement.GetProperty("items").EnumerateArray());
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/groups/{groupId}", UriKind.Relative));

        Assert.Equal(3, rosterLength);
        Assert.Equal(rosterLength, listed.GetProperty("enrolledCount").GetInt32());
        Assert.Equal(rosterLength, (await ReadJsonAsync(getResponse)).RootElement.GetProperty("enrolledCount").GetInt32());
    }

    [Fact]
    public async Task StudentEnrollments_ListsActiveFirstThenEnded_WithTheFullShape()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateSubjectAsync(owner.Client, "History Subject");
        var teacherId = await CreateTeacherAsync(owner.Client, "Amina Hassan");
        var endedGroupId = await CreateGroupAsync(owner.Client, "History Ended", subjectId, teacherId);
        var activeGroupId = await CreateGroupAsync(owner.Client, "History Active", subjectId, teacherId);
        var studentId = await RegisterStudentAsync(owner.Client, "History Student");

        // The active enrollment is the older one, so newest-first alone would list the ended one first: only
        // "active first" explains the expected order.
        var activeId = await EnrollAsync(owner.Client, activeGroupId, studentId);
        var endedId = await EnrollAsync(owner.Client, endedGroupId, studentId);
        using (var withdrawn = await PostAsync(
            owner.Client, $"/api/enrollments/{endedId}/withdraw", new { version = await GetRosterVersionAsync(owner.Client, endedGroupId, endedId) }))
        {
            Assert.Equal(HttpStatusCode.NoContent, withdrawn.StatusCode);
        }

        using var response = await owner.Client.GetAsync(new Uri($"/api/students/{studentId}/enrollments", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await ReadJsonAsync(response)).RootElement;
        Assert.Single(body.EnumerateObject());
        var items = body.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(new[] { activeId, endedId }, items.Select(item => item.GetProperty("enrollmentId").GetGuid()));

        var active = items[0];
        Assert.Equal(activeGroupId, active.GetProperty("groupId").GetGuid());
        Assert.Equal("History Active", active.GetProperty("groupName").GetString());
        Assert.Equal("History Subject", active.GetProperty("subjectName").GetString());
        Assert.Equal("Amina Hassan", active.GetProperty("teacherName").GetString());
        Assert.Equal("active", active.GetProperty("status").GetString());
        Assert.Equal(JsonValueKind.Null, active.GetProperty("endedAt").ValueKind);
        Assert.Equal(JsonValueKind.Null, active.GetProperty("endReason").ValueKind);
        Assert.True(active.GetProperty("version").GetUInt32() > 0);
        Assert.Equal(10, active.EnumerateObject().Count());

        var ended = items[1];
        Assert.Equal("ended", ended.GetProperty("status").GetString());
        Assert.Equal("withdrawn", ended.GetProperty("endReason").GetString());
        Assert.Equal(JsonValueKind.String, ended.GetProperty("endedAt").ValueKind);
    }

    [Fact]
    public async Task StudentEnrollments_UnknownStudent_Returns404StudentNotFound_AndAKnownStudentWithNoneReturnsEmptyItems()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var studentId = await RegisterStudentAsync(owner.Client, "Never Enrolled");

        using var unknown = await owner.Client.GetAsync(new Uri($"/api/students/{Guid.NewGuid()}/enrollments", UriKind.Relative));
        using var empty = await owner.Client.GetAsync(new Uri($"/api/students/{studentId}/enrollments", UriKind.Relative));

        await AssertProblemAsync(unknown, HttpStatusCode.NotFound, "student.not_found");
        Assert.Equal(HttpStatusCode.OK, empty.StatusCode);
        Assert.Equal(0, (await ReadJsonAsync(empty)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task StudentDetail_StaysUnchanged_AndEmbedsNoEnrollments()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Detail Unchanged");
        var studentId = await RegisterStudentAsync(owner.Client, "Detail Student");
        var before = await GetStudentPropertyNamesAsync(owner.Client, studentId);
        await EnrollAsync(owner.Client, groupId, studentId);

        var after = await GetStudentPropertyNamesAsync(owner.Client, studentId);

        Assert.Equal(before, after);
        Assert.DoesNotContain(after, name => name.Contains("nroll", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Withdraw_Returns204_ThenAgainReturns422AlreadyEnded_AndTheRowStaysAsHistory()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Withdraw Happy");
        var studentId = await RegisterStudentAsync(secretary.Client, "Withdraw Student");
        var enrollmentId = await EnrollAsync(secretary.Client, groupId, studentId);
        var version = await GetRosterVersionAsync(secretary.Client, groupId, enrollmentId);

        using var first = await PostAsync(secretary.Client, $"/api/enrollments/{enrollmentId}/withdraw", new { version });

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(0, (await GetRosterAsync(secretary.Client, groupId)).GetProperty("items").GetArrayLength());
        var history = await ListStudentEnrollmentsAsync(secretary.Client, studentId);
        var row = Assert.Single(history);
        Assert.Equal(enrollmentId, row.GetProperty("enrollmentId").GetGuid());
        Assert.Equal("ended", row.GetProperty("status").GetString());
        Assert.Equal("withdrawn", row.GetProperty("endReason").GetString());
        Assert.Equal(1, await CountEnrollmentRowsAsync(groupId));

        using var second = await PostAsync(
            secretary.Client, $"/api/enrollments/{enrollmentId}/withdraw", new { version = row.GetProperty("version").GetUInt32() });

        await AssertProblemAsync(second, HttpStatusCode.UnprocessableEntity, "enrollment.already_ended");
    }

    [Fact]
    public async Task Withdraw_StaleVersion_Returns409ConcurrencyStaleAndTheEnrollmentStaysActive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Withdraw Stale");
        var enrollmentId = await EnrollAsync(owner.Client, groupId, await RegisterStudentAsync(owner.Client, "Stale Student"));
        var version = await GetRosterVersionAsync(owner.Client, groupId, enrollmentId);

        using var response = await PostAsync(owner.Client, $"/api/enrollments/{enrollmentId}/withdraw", new { version = version + 1 });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "concurrency.stale");
        Assert.Equal("active", await SnapshotEnrollmentStatusAsync(enrollmentId));
    }

    [Fact]
    public async Task Withdraw_UnknownEnrollment_Returns404EnrollmentNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(owner.Client, $"/api/enrollments/{Guid.NewGuid()}/withdraw", new { version = 1 });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "enrollment.not_found");
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{ \"version\": 1, \"reason\": \"withdrawn\" }")]
    public async Task Withdraw_MissingVersionOrUnknownField_Returns400RequestMalformed(string json)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostRawAsync(owner.Client, $"/api/enrollments/{Guid.NewGuid()}/withdraw", json);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Fact]
    public async Task Transfer_ToASameSubjectGroup_Returns200WithTheNewId_EndsTheOldAsTransferredAndStartsTheNew()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Happy");
        var studentId = await RegisterStudentAsync(secretary.Client, "Transfer Student");
        var oldId = await EnrollAsync(secretary.Client, sourceId, studentId);

        using var response = await PostAsync(
            secretary.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = targetId, version = await GetRosterVersionAsync(secretary.Client, sourceId, oldId) });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await ReadJsonAsync(response)).RootElement;
        Assert.Single(body.EnumerateObject());
        var newId = body.GetProperty("id").GetGuid();
        Assert.NotEqual(oldId, newId);

        Assert.Equal(0, (await GetRosterAsync(secretary.Client, sourceId)).GetProperty("items").GetArrayLength());
        var targetEntry = Assert.Single((await GetRosterAsync(secretary.Client, targetId)).GetProperty("items").EnumerateArray());
        Assert.Equal(newId, targetEntry.GetProperty("enrollmentId").GetGuid());

        var history = await ListStudentEnrollmentsAsync(secretary.Client, studentId);
        Assert.Equal(new[] { newId, oldId }, history.Select(item => item.GetProperty("enrollmentId").GetGuid()));
        Assert.Equal("active", history[0].GetProperty("status").GetString());
        Assert.Equal(targetId, history[0].GetProperty("groupId").GetGuid());
        Assert.Equal("ended", history[1].GetProperty("status").GetString());
        Assert.Equal("transferred", history[1].GetProperty("endReason").GetString());
        Assert.Equal(sourceId, history[1].GetProperty("groupId").GetGuid());
    }

    [Fact]
    public async Task Transfer_ToAGroupOfAnotherSubject_Returns422SubjectMismatchAndChangesNothing()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, _, teacherId) = await CreateTransferGroupsAsync(owner.Client, "Transfer Mismatch");
        var otherSubjectGroupId = await CreateGroupAsync(owner.Client, "Transfer Mismatch Chemistry", await CreateSubjectAsync(owner.Client, "Transfer Chemistry"), teacherId);
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Mismatch Student"));
        var before = await SnapshotEnrollmentsAsync(sourceId, otherSubjectGroupId);

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = otherSubjectGroupId, version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "enrollment.subject_mismatch");
        Assert.Equal(before, await SnapshotEnrollmentsAsync(sourceId, otherSubjectGroupId));
    }

    [Fact]
    public async Task Transfer_ToItsOwnGroup_Returns422SameGroup()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, _, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Same");
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Same Group Student"));

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = sourceId, version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "enrollment.same_group");
        Assert.Equal("active", await SnapshotEnrollmentStatusAsync(oldId));
    }

    [Fact]
    public async Task Transfer_AnEndedEnrollment_Returns422AlreadyEnded()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Ended");
        var studentId = await RegisterStudentAsync(owner.Client, "Ended Transfer Student");
        var oldId = await EnrollAsync(owner.Client, sourceId, studentId);
        using (var withdrawn = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/withdraw", new { version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) }))
        {
            Assert.Equal(HttpStatusCode.NoContent, withdrawn.StatusCode);
        }

        var endedVersion = Assert.Single(await ListStudentEnrollmentsAsync(owner.Client, studentId)).GetProperty("version").GetUInt32();

        using var response = await PostAsync(owner.Client, $"/api/enrollments/{oldId}/transfer", new { targetGroupId = targetId, version = endedVersion });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "enrollment.already_ended");
        Assert.Equal(0, await CountEnrollmentRowsAsync(targetId));
    }

    [Fact]
    public async Task Transfer_ToAnArchivedGroup_Returns422GroupArchivedAndTheStudentStaysInTheSource()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Archived");
        await ArchiveGroupAsync(owner.Client, targetId);
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Archived Target Student"));

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = targetId, version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.archived");
        Assert.Equal("active", await SnapshotEnrollmentStatusAsync(oldId));
        Assert.Equal(0, await CountEnrollmentRowsAsync(targetId));
    }

    [Fact]
    public async Task Transfer_ToAFullGroup_Returns409GroupFullAndTheStudentStaysInTheSource()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Full", targetCapacity: 1);
        await EnrollAsync(owner.Client, targetId, await RegisterStudentAsync(owner.Client, "Target Seat Holder"));
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Full Target Student"));
        var before = await SnapshotEnrollmentsAsync(sourceId, targetId);

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = targetId, version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "group.full");
        Assert.Equal(before, await SnapshotEnrollmentsAsync(sourceId, targetId));
        Assert.Equal(oldId, Assert.Single((await GetRosterAsync(owner.Client, sourceId)).GetProperty("items").EnumerateArray()).GetProperty("enrollmentId").GetGuid());
    }

    [Fact]
    public async Task Transfer_ToAGroupTheStudentIsAlreadyIn_Returns409AlreadyEnrolled()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Already");
        var studentId = await RegisterStudentAsync(owner.Client, "Already In Target");
        var oldId = await EnrollAsync(owner.Client, sourceId, studentId);
        await EnrollAsync(owner.Client, targetId, studentId);
        var before = await SnapshotEnrollmentsAsync(sourceId, targetId);

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{oldId}/transfer",
            new { targetGroupId = targetId, version = await GetRosterVersionAsync(owner.Client, sourceId, oldId) });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "enrollment.already_enrolled");
        Assert.Equal(before, await SnapshotEnrollmentsAsync(sourceId, targetId));
    }

    [Fact]
    public async Task Transfer_StaleVersion_Returns409ConcurrencyStaleAndRollsBackBothChanges()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Stale");
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Stale Transfer Student"));
        var version = await GetRosterVersionAsync(owner.Client, sourceId, oldId);
        var before = await SnapshotEnrollmentsAsync(sourceId, targetId);

        using var response = await PostAsync(owner.Client, $"/api/enrollments/{oldId}/transfer", new { targetGroupId = targetId, version = version + 1 });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "concurrency.stale");
        Assert.Equal(before, await SnapshotEnrollmentsAsync(sourceId, targetId));
    }

    [Fact]
    public async Task Transfer_UnknownEnrollment_Returns404EnrollmentNotFound_AndUnknownTarget_Returns404GroupNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (sourceId, targetId, _) = await CreateTransferGroupsAsync(owner.Client, "Transfer Unknown");
        var oldId = await EnrollAsync(owner.Client, sourceId, await RegisterStudentAsync(owner.Client, "Unknown Target Student"));
        var version = await GetRosterVersionAsync(owner.Client, sourceId, oldId);

        using var unknownEnrollment = await PostAsync(owner.Client, $"/api/enrollments/{Guid.NewGuid()}/transfer", new { targetGroupId = targetId, version });
        using var unknownTarget = await PostAsync(owner.Client, $"/api/enrollments/{oldId}/transfer", new { targetGroupId = Guid.NewGuid(), version });

        await AssertProblemAsync(unknownEnrollment, HttpStatusCode.NotFound, "enrollment.not_found");
        await AssertProblemAsync(unknownTarget, HttpStatusCode.NotFound, "group.not_found");
        Assert.Equal("active", await SnapshotEnrollmentStatusAsync(oldId));
    }

    [Fact]
    public async Task Transfer_EmptyTargetGroupId_Returns400WithErrorsTargetGroupId()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(owner.Client, $"/api/enrollments/{Guid.NewGuid()}/transfer", new { targetGroupId = Guid.Empty, version = 1 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("targetGroupId").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Transfer_WithAStudentIdInTheBody_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await PostAsync(
            owner.Client, $"/api/enrollments/{Guid.NewGuid()}/transfer", new { targetGroupId = Guid.NewGuid(), version = 1, studentId = Guid.NewGuid() });

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    private static async Task<(Guid SourceId, Guid TargetId, Guid TeacherId)> CreateTransferGroupsAsync(HttpClient client, string prefix, int targetCapacity = 20)
    {
        var subjectId = await CreateSubjectAsync(client, $"{prefix} Subject");
        var teacherId = await CreateTeacherAsync(client, $"{prefix} Teacher");
        var sourceId = await CreateGroupAsync(client, $"{prefix} Source", subjectId, teacherId);
        var targetId = await CreateGroupAsync(client, $"{prefix} Target", subjectId, teacherId, targetCapacity);
        return (sourceId, targetId, teacherId);
    }

    private static async Task<Guid> CreateGroupAsync(HttpClient client, string name, int capacity = 20)
    {
        var subjectId = await CreateSubjectAsync(client, $"{name} Subject");
        var teacherId = await CreateTeacherAsync(client, $"{name} Teacher");
        return await CreateGroupAsync(client, name, subjectId, teacherId, capacity);
    }

    private static Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId, int capacity = 20) =>
        CreateAsync(client, "/api/groups", new { name, subjectId, teacherId, capacity });

    private static Task<Guid> CreateSubjectAsync(HttpClient client, string name) => CreateAsync(client, "/api/subjects", new { name });

    private static Task<Guid> CreateTeacherAsync(HttpClient client, string fullName) =>
        CreateAsync(client, "/api/teachers", new { fullName, phone = (string?)null });

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
        using var response = await PostAsync(client, url, body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task ArchiveGroupAsync(HttpClient client, Guid id)
    {
        using var getResponse = await client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await PostAsync(client, $"/api/groups/{id}/archive", new { version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task ArchiveStudentAsync(HttpClient client, Guid id)
    {
        using var getResponse = await client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await PostAsync(client, $"/api/students/{id}/archive", new { version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<JsonElement> GetRosterAsync(HttpClient client, Guid groupId)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{groupId}/roster", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone();
    }

    private static async Task<uint> GetRosterVersionAsync(HttpClient client, Guid groupId, Guid enrollmentId) =>
        (await GetRosterAsync(client, groupId)).GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("enrollmentId").GetGuid() == enrollmentId).GetProperty("version").GetUInt32();

    private static async Task<List<JsonElement>> ListStudentEnrollmentsAsync(HttpClient client, Guid studentId)
    {
        using var response = await client.GetAsync(new Uri($"/api/students/{studentId}/enrollments", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone().GetProperty("items").EnumerateArray().ToList();
    }

    private static async Task<List<string>> GetStudentPropertyNamesAsync(HttpClient client, Guid studentId)
    {
        using var response = await client.GetAsync(new Uri($"/api/students/{studentId}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.EnumerateObject().Select(property => property.Name).ToList();
    }

    private Task<long> CountEnrollmentRowsAsync(Guid groupId) =>
        factory.ScalarAsync<long>($"select count(*) from academics.enrollments where group_id = '{groupId}'");

    private Task<string> SnapshotEnrollmentStatusAsync(Guid enrollmentId) =>
        factory.ScalarAsync<string>($"select status from academics.enrollments where id = '{enrollmentId}'");

    // Every enrollment row of both groups with its status and row version: a refused transfer changes none of them.
    private Task<string> SnapshotEnrollmentsAsync(Guid firstGroupId, Guid secondGroupId) =>
        factory.ScalarAsync<string>(
            $"""
            select count(*)::text || '|' || coalesce(string_agg(id::text || ':' || status || ':' || xmin::text, ',' order by id), '')
            from academics.enrollments where group_id in ('{firstGroupId}', '{secondGroupId}')
            """);

    private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private static Task<HttpResponseMessage> PostAsync(HttpClient client, string url, object body) =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);

    private static async Task<HttpResponseMessage> PostRawAsync(HttpClient client, string url, string json)
    {
        var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.Relative))
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        return await client.SendAsync(request);
    }

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
}
