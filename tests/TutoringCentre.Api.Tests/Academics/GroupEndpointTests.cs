using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 45.5: the eight group endpoints' contract through the full HTTP pipeline — status, code, body shape and
/// headers, never messages — plus the one outcome Day 44 added to Month 2's subject archive endpoint, and the two
/// outcomes scheduling (Day 52) added to reassigning and archiving a group (Task 53.4). Every test creates its own
/// subjects, teachers and groups, so the seeded demo data never decides a result.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class GroupEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri GroupsUri = new("/api/groups", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    // The two schedule cases fix the clock; every other case runs on real time, as before.
    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200WithFullShape()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateSubjectAsync(owner.Client, "Physics Endpoint");
        var teacherId = await CreateTeacherAsync(owner.Client, "Amina Hassan");

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "Physics – Grade 10 أ", subjectId, teacherId, capacity = 20 });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();
        Assert.Equal($"/api/groups/{id}", createResponse.Headers.Location?.OriginalString);

        using var getResponse = await owner.Client.GetAsync(createResponse.Headers.Location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var group = body.RootElement;
        Assert.Equal(id, group.GetProperty("id").GetGuid());
        Assert.Equal("Physics – Grade 10 أ", group.GetProperty("name").GetString());
        Assert.Equal(subjectId, group.GetProperty("subjectId").GetGuid());
        Assert.Equal("Physics Endpoint", group.GetProperty("subjectName").GetString());
        Assert.Equal(teacherId, group.GetProperty("teacherId").GetGuid());
        Assert.Equal("Amina Hassan", group.GetProperty("teacherName").GetString());
        Assert.Equal(20, group.GetProperty("capacity").GetInt32());
        Assert.Equal(0, group.GetProperty("enrolledCount").GetInt32());
        Assert.Equal("active", group.GetProperty("status").GetString());
        Assert.True(group.GetProperty("version").GetUInt32() > 0);

        // Groups carry no contact data: exactly the ten GroupDto fields, nothing else.
        Assert.Equal(10, group.EnumerateObject().Count());
    }

    [Fact]
    public async Task Create_NameTakenIgnoringCase_Returns409NameTakenWithErrorsName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "grade 10 physics", subjectId, teacherId, capacity = 20 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("group.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    [InlineData(500)]
    public async Task Create_CapacityOutOfRange_Returns400WithErrorsCapacityAndCreatesNoRow(int capacity)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var countBefore = await CountGroupsAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "Capacity Probe", subjectId, teacherId, capacity });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("capacity").GetArrayLength() > 0);
        Assert.Equal(countBefore, await CountGroupsAsync(owner.Client));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyName_Returns400WithErrorsName(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, GroupsUri, new { name, subjectId, teacherId, capacity = 20 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData("status")]
    [InlineData("enrolledCount")]
    public async Task Create_WithOverPostedField_Returns400RequestMalformedAndCreatesNoRow(string extraField)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var countBefore = await CountGroupsAsync(owner.Client);
        var body = new Dictionary<string, object>
        {
            ["name"] = "Over Posted",
            ["subjectId"] = subjectId,
            ["teacherId"] = teacherId,
            ["capacity"] = 20,
            [extraField] = extraField == "status" ? "archived" : 5,
        };

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, GroupsUri, body);

        var responseBody = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", responseBody.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await CountGroupsAsync(owner.Client));
    }

    [Fact]
    public async Task Create_UnknownSubject_Returns404SubjectNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var teacherId = await CreateTeacherAsync(owner.Client, "Amina Hassan");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "No Subject", subjectId = Guid.NewGuid(), teacherId, capacity = 20 });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "subject.not_found");
    }

    [Fact]
    public async Task Create_UnknownTeacher_Returns404TeacherNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateSubjectAsync(owner.Client, "Physics Endpoint");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "No Teacher", subjectId, teacherId = Guid.NewGuid(), capacity = 20 });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "teacher.not_found");
    }

    [Fact]
    public async Task Create_ArchivedSubject_Returns422SubjectArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        await ArchiveSubjectAsync(owner.Client, subjectId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "Archived Subject", subjectId, teacherId, capacity = 20 });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.subject_archived");
    }

    [Fact]
    public async Task Create_ArchivedTeacher_Returns422TeacherArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        await ArchiveTeacherAsync(owner.Client, teacherId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, GroupsUri, new { name = "Archived Teacher", subjectId, teacherId, capacity = 20 });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.teacher_archived");
    }

    [Fact]
    public async Task Rename_HappyPathThenStaleVersion()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        var version = await GetVersionAsync(owner.Client, id);

        using var renamed = await SendAsync(owner.Client, HttpMethod.Put, $"/api/groups/{id}", new { name = "Grade 11 Physics", version });
        Assert.Equal(HttpStatusCode.NoContent, renamed.StatusCode);
        Assert.Equal("Grade 11 Physics", (await GetGroupAsync(owner.Client, id)).GetProperty("name").GetString());

        using var stale = await SendAsync(owner.Client, HttpMethod.Put, $"/api/groups/{id}", new { name = "Someone Else", version });
        await AssertProblemAsync(stale, HttpStatusCode.Conflict, "concurrency.stale");
    }

    [Fact]
    public async Task Rename_ToAnotherGroupsName_Returns409NameTaken()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        var id = await CreateGroupAsync(owner.Client, "Grade 11 Physics", subjectId, teacherId);

        using var response = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}", new { name = "Grade 10 Physics", version = await GetVersionAsync(owner.Client, id) });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("group.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/capacity")]
    [InlineData("/teacher")]
    public async Task Put_WithSubjectIdInBody_Returns400RequestMalformedAndLeavesSubjectUnchanged(string suffix)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var otherSubjectId = await CreateSubjectAsync(owner.Client, "Chemistry Endpoint");
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        var version = await GetVersionAsync(owner.Client, id);
        object body = suffix switch
        {
            "" => new { name = "Grade 10 Chemistry", subjectId = otherSubjectId, version },
            "/capacity" => new { capacity = 25, subjectId = otherSubjectId, version },
            _ => new { teacherId, subjectId = otherSubjectId, version },
        };

        using var response = await SendAsync(owner.Client, HttpMethod.Put, $"/api/groups/{id}{suffix}", body);

        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
        var group = await GetGroupAsync(owner.Client, id);
        Assert.Equal(subjectId, group.GetProperty("subjectId").GetGuid());
        Assert.Equal(version, group.GetProperty("version").GetUInt32());
    }

    [Fact]
    public async Task ChangeCapacity_HappyPathThenZeroReturns400WithErrorsCapacity()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var raised = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}/capacity", new { capacity = 35, version = await GetVersionAsync(owner.Client, id) });
        Assert.Equal(HttpStatusCode.NoContent, raised.StatusCode);
        Assert.Equal(35, (await GetGroupAsync(owner.Client, id)).GetProperty("capacity").GetInt32());

        using var zero = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}/capacity", new { capacity = 0, version = await GetVersionAsync(owner.Client, id) });
        var body = await ReadJsonAsync(zero);
        Assert.Equal(HttpStatusCode.BadRequest, zero.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("capacity").GetArrayLength() > 0);
    }

    [Fact]
    public async Task AssignTeacher_ActiveTeacher_Returns204AndGetShowsTheNewTeacher()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var newTeacherId = await CreateTeacherAsync(owner.Client, "Mona Said");
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var response = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}/teacher", new { teacherId = newTeacherId, version = await GetVersionAsync(owner.Client, id) });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var group = await GetGroupAsync(owner.Client, id);
        Assert.Equal(newTeacherId, group.GetProperty("teacherId").GetGuid());
        Assert.Equal("Mona Said", group.GetProperty("teacherName").GetString());
    }

    [Fact]
    public async Task AssignTeacher_ArchivedTeacher_Returns422TeacherArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var archivedTeacherId = await CreateTeacherAsync(owner.Client, "Mona Said");
        await ArchiveTeacherAsync(owner.Client, archivedTeacherId);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var response = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}/teacher", new { teacherId = archivedTeacherId, version = await GetVersionAsync(owner.Client, id) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.teacher_archived");
        Assert.Equal(teacherId, (await GetGroupAsync(owner.Client, id)).GetProperty("teacherId").GetGuid());
    }

    [Fact]
    public async Task AssignTeacher_UnknownTeacher_Returns404TeacherNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var response = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}/teacher", new { teacherId = Guid.NewGuid(), version = await GetVersionAsync(owner.Client, id) });

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "teacher.not_found");
    }

    [Fact]
    public async Task ArchiveThenRestore_AndTheirRuleViolations()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);

        using var archived = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/archive", new { version = await GetVersionAsync(owner.Client, id) });
        Assert.Equal(HttpStatusCode.NoContent, archived.StatusCode);
        Assert.Equal("archived", (await GetGroupAsync(owner.Client, id)).GetProperty("status").GetString());

        using var archiveAgain = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/archive", new { version = await GetVersionAsync(owner.Client, id) });
        await AssertProblemAsync(archiveAgain, HttpStatusCode.UnprocessableEntity, "group.already_archived");

        using var renameArchived = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{id}", new { name = "Read Only", version = await GetVersionAsync(owner.Client, id) });
        await AssertProblemAsync(renameArchived, HttpStatusCode.UnprocessableEntity, "group.archived_read_only");

        using var restored = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/restore", new { version = await GetVersionAsync(owner.Client, id) });
        Assert.Equal(HttpStatusCode.NoContent, restored.StatusCode);

        using var restoreAgain = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/restore", new { version = await GetVersionAsync(owner.Client, id) });
        await AssertProblemAsync(restoreAgain, HttpStatusCode.UnprocessableEntity, "group.not_archived");
    }

    [Fact]
    public async Task Restore_AfterItsTeacherWasArchived_Returns422TeacherArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        await ArchiveGroupAsync(owner.Client, id);
        await ArchiveTeacherAsync(owner.Client, teacherId);

        using var response = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/restore", new { version = await GetVersionAsync(owner.Client, id) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.teacher_archived");
    }

    [Fact]
    public async Task Restore_AfterItsSubjectWasArchived_Returns422SubjectArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        await ArchiveGroupAsync(owner.Client, id);
        await ArchiveSubjectAsync(owner.Client, subjectId);

        using var response = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/restore", new { version = await GetVersionAsync(owner.Client, id) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.subject_archived");
    }

    [Fact]
    public async Task Get_UnknownId_Returns404GroupNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri($"/api/groups/{Guid.NewGuid()}", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task List_FiltersBySubjectAndByTeacher_AndHidesArchivedUnlessAsked()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var physicsId = await CreateSubjectAsync(owner.Client, "Physics Endpoint");
        var chemistryId = await CreateSubjectAsync(owner.Client, "Chemistry Endpoint");
        var aminaId = await CreateTeacherAsync(owner.Client, "Amina Hassan");
        var monaId = await CreateTeacherAsync(owner.Client, "Mona Said");
        var physicsAmina = await CreateGroupAsync(owner.Client, "Physics A", physicsId, aminaId);
        var physicsMona = await CreateGroupAsync(owner.Client, "Physics B", physicsId, monaId);
        var chemistryMona = await CreateGroupAsync(owner.Client, "Chemistry A", chemistryId, monaId);
        await ArchiveGroupAsync(owner.Client, physicsMona);

        Assert.Equal(new[] { physicsAmina }, await ListIdsAsync(owner.Client, $"?subjectId={physicsId}"));
        Assert.Equal(new[] { physicsAmina, physicsMona }, await ListIdsAsync(owner.Client, $"?subjectId={physicsId}&includeArchived=true"));
        Assert.Equal(new[] { chemistryMona }, await ListIdsAsync(owner.Client, $"?teacherId={monaId}"));
        Assert.Equal(new[] { physicsMona }, await ListIdsAsync(owner.Client, $"?subjectId={physicsId}&teacherId={monaId}&includeArchived=true"));
    }

    [Fact]
    public async Task SubjectArchive_WithAnActiveGroup_Returns422HasActiveGroupsAndSubjectStaysActive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var groupId = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        var subjectVersion = await GetSubjectVersionAsync(owner.Client, subjectId);

        using var refused = await SendAsync(owner.Client, HttpMethod.Post, $"/api/subjects/{subjectId}/archive", new { version = subjectVersion });

        await AssertProblemAsync(refused, HttpStatusCode.UnprocessableEntity, "subject.has_active_groups");
        using (var subjectResponse = await owner.Client.GetAsync(new Uri($"/api/subjects/{subjectId}", UriKind.Relative)))
        {
            var subject = await ReadJsonAsync(subjectResponse);
            Assert.Equal("active", subject.RootElement.GetProperty("status").GetString());
            Assert.Equal(subjectVersion, subject.RootElement.GetProperty("version").GetUInt32());
        }

        // Once its only group is archived, the same subject archives normally.
        await ArchiveGroupAsync(owner.Client, groupId);
        using var allowed = await SendAsync(owner.Client, HttpMethod.Post, $"/api/subjects/{subjectId}/archive", new { version = subjectVersion });
        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
    }

    [Fact]
    public async Task ChangeCapacity_BelowTheEnrolledCount_Returns422CapacityBelowEnrolledAndCapacityStays()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        await EnrollNewStudentAsync(owner.Client, id, "Capacity Student One", "01099991001");
        await EnrollNewStudentAsync(owner.Client, id, "Capacity Student Two", "01099991002");
        var version = await GetVersionAsync(owner.Client, id);

        using var refused = await SendAsync(owner.Client, HttpMethod.Put, $"/api/groups/{id}/capacity", new { capacity = 1, version });

        await AssertProblemAsync(refused, HttpStatusCode.UnprocessableEntity, "group.capacity_below_enrolled");
        var group = await GetGroupAsync(owner.Client, id);
        Assert.Equal(20, group.GetProperty("capacity").GetInt32());
        Assert.Equal(version, group.GetProperty("version").GetUInt32());

        // Exactly the enrolled count is allowed: every seat taken, none over.
        using var allowed = await SendAsync(owner.Client, HttpMethod.Put, $"/api/groups/{id}/capacity", new { capacity = 2, version });
        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
    }

    [Fact]
    public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndGroupStaysActive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var id = await CreateGroupAsync(owner.Client, "Grade 10 Physics", subjectId, teacherId);
        var enrollmentId = await EnrollNewStudentAsync(owner.Client, id, "Archive Group Student", "01099991003");
        var version = await GetVersionAsync(owner.Client, id);

        using var refused = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/archive", new { version });

        await AssertProblemAsync(refused, HttpStatusCode.UnprocessableEntity, "group.has_active_enrollments");
        Assert.Equal("active", (await GetGroupAsync(owner.Client, id)).GetProperty("status").GetString());

        // Once its only enrollment is withdrawn, the same group archives normally.
        using (var rosterResponse = await owner.Client.GetAsync(new Uri($"/api/groups/{id}/roster", UriKind.Relative)))
        {
            var enrollmentVersion = (await ReadJsonAsync(rosterResponse)).RootElement.GetProperty("items")[0].GetProperty("version").GetUInt32();
            using var withdrawn = await SendAsync(owner.Client, HttpMethod.Post, $"/api/enrollments/{enrollmentId}/withdraw", new { version = enrollmentVersion });
            Assert.Equal(HttpStatusCode.NoContent, withdrawn.StatusCode);
        }

        using var allowed = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{id}/archive", new { version = await GetVersionAsync(owner.Client, id) });
        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
    }

    [Fact]
    public async Task List_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(GroupsUri);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var countBefore = await CountGroupsAsync(owner.Client);

        using var response = await owner.Client.PostAsJsonAsync(GroupsUri, new { name = "No Token", subjectId, teacherId, capacity = 20 });

        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.csrf_invalid");
        Assert.Equal(countBefore, await CountGroupsAsync(owner.Client));
    }

    // Day 52's two added outcomes. The clock is fixed so the sessions involved are in the future on every run.
    [Fact]
    public async Task AssignTeacher_ToATeacherBusyAtOneOfTheGroupsSessions_Returns409TeacherDoubleBookedAndKeepsTeacherAndSessions()
    {
        factory.Time.FixAt(ScheduleNow);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var busyTeacherId = await CreateTeacherAsync(owner.Client, "Busy Teacher");
        var groupId = await CreateGroupAsync(owner.Client, "Reassign Scheduled", subjectId, teacherId);
        var busyGroupId = await CreateGroupAsync(owner.Client, "Busy Teachers Group", subjectId, busyTeacherId);
        await AddSaturdayRuleAsync(owner.Client, groupId, "18:00");
        await AddSaturdayRuleAsync(owner.Client, busyGroupId, "18:30");
        var sessionsBefore = await SnapshotGroupSessionsAsync(groupId);

        using var response = await SendAsync(
            owner.Client, HttpMethod.Put, $"/api/groups/{groupId}/teacher", new { teacherId = busyTeacherId, version = await GetVersionAsync(owner.Client, groupId) });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "schedule.teacher_double_booked");
        Assert.Equal(teacherId, (await GetGroupAsync(owner.Client, groupId)).GetProperty("teacherId").GetGuid());
        Assert.Equal(sessionsBefore, await SnapshotGroupSessionsAsync(groupId));
    }

    [Fact]
    public async Task Archive_WithAScheduleRuleInEffect_Returns422HasActiveSchedule()
    {
        factory.Time.FixAt(ScheduleNow);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (subjectId, teacherId) = await CreateReferencesAsync(owner.Client);
        var groupId = await CreateGroupAsync(owner.Client, "Archive Scheduled", subjectId, teacherId);
        await AddSaturdayRuleAsync(owner.Client, groupId, "18:00");

        using var response = await SendAsync(owner.Client, HttpMethod.Post, $"/api/groups/{groupId}/archive", new { version = await GetVersionAsync(owner.Client, groupId) });

        await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, "group.has_active_schedule");
        Assert.Equal("active", (await GetGroupAsync(owner.Client, groupId)).GetProperty("status").GetString());
    }

    // Tuesday 20 April 2027, 10:00 in Cairo; a rule from that day generates Saturdays 24 April to 15 May.
    private static readonly DateTimeOffset ScheduleNow = new(2027, 4, 20, 8, 0, 0, TimeSpan.Zero);

    private static async Task AddSaturdayRuleAsync(HttpClient client, Guid groupId, string startTime)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative),
            new { dayOfWeek = "saturday", startTime, durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private Task<string> SnapshotGroupSessionsAsync(Guid groupId) =>
        factory.ScalarAsync<string>(
            $"select count(*)::text || ':' || coalesce(string_agg(id::text || ':' || teacher_id::text || ':' || status || ':' || xmin::text, ',' order by id), '') from academics.class_sessions where group_id = '{groupId}'");

    private static async Task<(Guid SubjectId, Guid TeacherId)> CreateReferencesAsync(HttpClient client) =>
        (await CreateSubjectAsync(client, "Physics Endpoint"), await CreateTeacherAsync(client, "Amina Hassan"));

    private static async Task<Guid> CreateSubjectAsync(HttpClient client, string name)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri("/api/subjects", UriKind.Relative), new { name });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateTeacherAsync(HttpClient client, string fullName)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri("/api/teachers", UriKind.Relative), new { fullName, phone = (string?)null });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, GroupsUri, new { name, subjectId, teacherId, capacity = 20 });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> EnrollNewStudentAsync(HttpClient client, Guid groupId, string fullName, string parentPhone)
    {
        using var studentResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, new Uri("/api/students", UriKind.Relative),
            new { fullName, phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = $"{fullName} Parent", phone = parentPhone }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, studentResponse.StatusCode);
        var studentId = (await ReadJsonAsync(studentResponse)).RootElement.GetProperty("id").GetGuid();

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task ArchiveGroupAsync(HttpClient client, Guid id)
    {
        using var response = await SendAsync(client, HttpMethod.Post, $"/api/groups/{id}/archive", new { version = await GetVersionAsync(client, id) });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task ArchiveSubjectAsync(HttpClient client, Guid id)
    {
        using var response = await SendAsync(client, HttpMethod.Post, $"/api/subjects/{id}/archive", new { version = await GetSubjectVersionAsync(client, id) });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task ArchiveTeacherAsync(HttpClient client, Guid id)
    {
        using var getResponse = await client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));
        var version = (await ReadJsonAsync(getResponse)).RootElement.GetProperty("version").GetUInt32();
        using var response = await SendAsync(client, HttpMethod.Post, $"/api/teachers/{id}/archive", new { version });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static async Task<JsonElement> GetGroupAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.Clone();
    }

    private static async Task<uint> GetVersionAsync(HttpClient client, Guid id) =>
        (await GetGroupAsync(client, id)).GetProperty("version").GetUInt32();

    private static async Task<uint> GetSubjectVersionAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));
        return (await ReadJsonAsync(response)).RootElement.GetProperty("version").GetUInt32();
    }

    private static async Task<List<Guid>> ListIdsAsync(HttpClient client, string query)
    {
        using var response = await client.GetAsync(new Uri($"/api/groups{query}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("id").GetGuid()).ToList();
    }

    private static async Task<int> CountGroupsAsync(HttpClient client)
    {
        using var response = await client.GetAsync(new Uri("/api/groups?includeArchived=true", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private static Task<HttpResponseMessage> SendAsync(HttpClient client, HttpMethod method, string url, object body) =>
        AntiforgeryTestHelper.SendAsJsonAsync(client, method, new Uri(url, UriKind.Relative), body);

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
