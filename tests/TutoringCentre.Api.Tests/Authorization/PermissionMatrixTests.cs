using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Authorization;

/// <summary>
/// Task 30.8 (extended Task 32.11, Task 43.14, Task 45.9, Task 49.9, Task 53.4): one table of role x endpoint ->
/// expected status, executed over real HTTP against Nile's three roles, now including the seven teacher endpoints
/// (21 more literal cases), the eight group endpoints (24 more), the five enrollment endpoints (15 more) and the six
/// schedule endpoints (18 more). The matrix records the permission decision only: Teacher is allowed on the two
/// group reads, the two enrollment reads and the two schedule reads (and reads a group it owns here), while what a
/// teacher may see is ownership, proven separately by GroupTeacherScopeTests, EnrollmentTeacherScopeTests and
/// ScheduleTeacherScopeTests. The schedule cases run on a fixed application clock, so no case depends on today.
/// Allowed cases assert a non-403 success status;
/// denied cases assert 403 auth.permission_denied and an unchanged database, compared against the exact
/// pre-request state, not a snapshot taken after the request already ran. Values are written literally here,
/// never derived from <c>RolePermissions</c> — that would prove the matrix agrees with itself, not with the
/// contract.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class PermissionMatrixTests(ApiFactory factory) : IAsyncLifetime
{
    /// <summary>The fourteen routes this matrix classifies, in the exact raw pattern <see cref="EndpointInventoryTests"/> reads off the running app.</summary>
    internal static readonly string[] MatrixRoutes =
    [
        "GET /api/portal/chat/messages",
        "POST /api/portal/chat/messages",
        "GET /api/conversations",
        "GET /api/conversations/{conversationId:guid}/messages",
        "GET /api/conversations/{conversationId:guid}/timeline",
        "GET /api/portal-accounts",
        "POST /api/portal-accounts",
        "POST /api/portal-accounts/{linkId:guid}/deactivate",
        "POST /api/portal-accounts/{linkId:guid}/reactivate",
        "GET /api/subjects",
        "GET /api/subjects/{id:guid}",
        "POST /api/subjects",
        "PUT /api/subjects/{id:guid}",
        "POST /api/subjects/{id:guid}/archive",
        "POST /api/subjects/{id:guid}/restore",
        "GET /api/staff",
        "POST /api/staff",
        "PUT /api/staff/{membershipId:guid}/role",
        "POST /api/staff/{membershipId:guid}/deactivate",
        "POST /api/staff/{membershipId:guid}/reactivate",
        "GET /api/audit",
        "GET /api/centre/settings",
        "PUT /api/centre/settings",
        "POST /api/parents",
        "GET /api/parents/{id:guid}",
        "PUT /api/parents/{id:guid}",
        "POST /api/parents/lookup",
        "GET /api/students",
        "GET /api/students/{id:guid}",
        "POST /api/students",
        "PUT /api/students/{id:guid}",
        "POST /api/students/{id:guid}/archive",
        "POST /api/students/{id:guid}/restore",
        "POST /api/students/{id:guid}/parents",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/unlink",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/make-primary",
        "GET /api/teachers",
        "GET /api/teachers/{id:guid}",
        "POST /api/teachers",
        "PUT /api/teachers/{id:guid}",
        "PUT /api/teachers/{id:guid}/account",
        "POST /api/teachers/{id:guid}/archive",
        "POST /api/teachers/{id:guid}/restore",
        "GET /api/groups",
        "GET /api/groups/{id:guid}",
        "POST /api/groups",
        "PUT /api/groups/{id:guid}",
        "PUT /api/groups/{id:guid}/capacity",
        "PUT /api/groups/{id:guid}/teacher",
        "POST /api/groups/{id:guid}/archive",
        "POST /api/groups/{id:guid}/restore",
        "POST /api/groups/{groupId:guid}/enrollments",
        "GET /api/groups/{groupId:guid}/roster",
        "GET /api/students/{studentId:guid}/enrollments",
        "POST /api/enrollments/{id:guid}/withdraw",
        "POST /api/enrollments/{id:guid}/transfer",
        "GET /api/groups/{groupId:guid}/schedule-rules",
        "POST /api/groups/{groupId:guid}/schedule-rules",
        "POST /api/schedule-rules/{id:guid}/end",
        "POST /api/schedule/extend",
        "GET /api/timetable",
        "POST /api/sessions/{id:guid}/cancel",
        "GET /api/attendance/sessions",
        "GET /api/sessions/{sessionId:guid}/attendance",
        "PUT /api/sessions/{sessionId:guid}/attendance",
        "POST /api/attendance/{recordId:guid}/correct",
        "GET /api/attendance",
        "GET /api/students/{studentId:guid}/card",
        "POST /api/students/{studentId:guid}/card",
        "POST /api/groups/{groupId:guid}/cards",
        "POST /api/student-cards/{cardId:guid}/revoke",
        "POST /api/sessions/{sessionId:guid}/scans",
    ];

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NotFound)]
    [InlineData("secretary@nile.test", HttpStatusCode.NotFound)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task AssistantTimelinePermission(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);
        using var response = await signedIn.Client.GetAsync(new Uri($"/api/conversations/{Guid.NewGuid()}/timeline", UriKind.Relative));
        Assert.Equal(expected, response.StatusCode);
    }
    [Theory]
    [InlineData("owner@nile.test", "status", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "status", HttpStatusCode.OK)]
    [InlineData("teacher", "status", HttpStatusCode.Forbidden)]
    [InlineData("parent", "status", HttpStatusCode.Forbidden)]
    [InlineData("student", "status", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "issue", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", "issue", HttpStatusCode.Created)]
    [InlineData("teacher", "issue", HttpStatusCode.Forbidden)]
    [InlineData("parent", "issue", HttpStatusCode.Forbidden)]
    [InlineData("student", "issue", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "bulk", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "bulk", HttpStatusCode.OK)]
    [InlineData("teacher", "bulk", HttpStatusCode.Forbidden)]
    [InlineData("parent", "bulk", HttpStatusCode.Forbidden)]
    [InlineData("student", "bulk", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "revoke", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", "revoke", HttpStatusCode.NoContent)]
    [InlineData("teacher", "revoke", HttpStatusCode.Forbidden)]
    [InlineData("parent", "revoke", HttpStatusCode.Forbidden)]
    [InlineData("student", "revoke", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "scan", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "scan", HttpStatusCode.OK)]
    [InlineData("teacher", "scan", HttpStatusCode.OK)]
    [InlineData("parent", "scan", HttpStatusCode.Forbidden)]
    [InlineData("student", "scan", HttpStatusCode.Forbidden)]
    public async Task CardsAndScans(string role, string operation, HttpStatusCode expected)
    {
        factory.Time.FixAt(AttendanceHttp.BeforeSession);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await AttendanceHttp.Arrange(owner.Client);
        using var issued = await AntiforgeryTestHelper.PostAsync(owner.Client, new($"/api/students/{scenario.Students[0]}/card", UriKind.Relative), null);
        Assert.Equal(HttpStatusCode.Created, issued.StatusCode);
        var card = await AttendanceHttp.Json(issued);
        var status = (await AttendanceHttp.Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        var before = await factory.ScalarAsync<string>("select string_agg(id::text || status || xmin::text, ',' order by id) from attendance.student_cards");
        factory.Time.FixAt(AttendanceHttp.InSession);
        using var actor = await SignInAsync(role);
        using var response = operation switch
        {
            "status" => await actor.Client.GetAsync(new Uri($"/api/students/{scenario.Students[0]}/card", UriKind.Relative)),
            "issue" => await AntiforgeryTestHelper.PostAsync(actor.Client, new($"/api/students/{scenario.Students[0]}/card", UriKind.Relative), null),
            "bulk" => await AttendanceHttp.Post(actor.Client, $"/api/groups/{scenario.GroupId}/cards", new { studentIds = scenario.Students }),
            "revoke" => await AttendanceHttp.Post(actor.Client, $"/api/student-cards/{card.GetProperty("cardId").GetGuid()}/revoke", new { reason = "lost", version = status.GetProperty("version").GetUInt32() }),
            _ => await AttendanceHttp.Post(actor.Client, $"/api/sessions/{scenario.SessionId}/scans", new { token = card.GetProperty("token").GetString() }),
        };
        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(before, await factory.ScalarAsync<string>("select string_agg(id::text || status || xmin::text, ',' order by id) from attendance.student_cards"));
        }
    }

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    [Theory]
    [InlineData("owner@nile.test", "sessions", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "sessions", HttpStatusCode.OK)]
    [InlineData("teacher", "sessions", HttpStatusCode.OK)]
    [InlineData("parent", "sessions", HttpStatusCode.Forbidden)]
    [InlineData("student", "sessions", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "roster", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "roster", HttpStatusCode.OK)]
    [InlineData("teacher", "roster", HttpStatusCode.OK)]
    [InlineData("parent", "roster", HttpStatusCode.Forbidden)]
    [InlineData("student", "roster", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "mark", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "mark", HttpStatusCode.OK)]
    [InlineData("teacher", "mark", HttpStatusCode.OK)]
    [InlineData("parent", "mark", HttpStatusCode.Forbidden)]
    [InlineData("student", "mark", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "correct", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", "correct", HttpStatusCode.NoContent)]
    [InlineData("teacher", "correct", HttpStatusCode.Forbidden)]
    [InlineData("parent", "correct", HttpStatusCode.Forbidden)]
    [InlineData("student", "correct", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "history", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "history", HttpStatusCode.OK)]
    [InlineData("teacher", "history", HttpStatusCode.OK)]
    [InlineData("parent", "history", HttpStatusCode.OK)]
    [InlineData("student", "history", HttpStatusCode.OK)]
    public async Task Attendance(string role, string operation, HttpStatusCode expected)
    {
        factory.Time.FixAt(AttendanceHttp.BeforeSession);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await AttendanceHttp.Arrange(owner.Client);
        factory.Time.FixAt(AttendanceHttp.InSession);
        using var signedIn = await SignInAsync(role);
        HttpResponseMessage response;
        if (operation == "mark")
        {
            response = await AttendanceHttp.Put(signedIn.Client, $"/api/sessions/{scenario.SessionId}/attendance", AttendanceHttp.Marks(scenario));
        }
        else if (operation == "correct")
        {
            using var marked = await AttendanceHttp.Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", AttendanceHttp.Marks(scenario));
            Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
            var record = await AttendanceHttp.Record(owner.Client, scenario);
            response = await AttendanceHttp.Post(signedIn.Client, $"/api/attendance/{record.GetProperty("recordId").GetGuid()}/correct",
                new { status = "absent", reason = "Matrix correction", version = record.GetProperty("version").GetUInt32() });
        }
        else
        {
            var url = operation == "sessions" ? "/api/attendance/sessions" : operation == "roster"
                ? $"/api/sessions/{scenario.SessionId}/attendance" : $"/api/attendance?from={AttendanceHttp.Date}&to={AttendanceHttp.Date}";
            response = await signedIn.Client.GetAsync(new Uri(url, UriKind.Relative));
        }
        using (response)
        {
            Assert.Equal(expected, response.StatusCode);
            if (expected == HttpStatusCode.Forbidden)
            {
                if ((role is "parent" or "student") && (operation is "sessions" or "roster"))
                    await AttendanceHttp.Problem(response, HttpStatusCode.Forbidden, "attendance.not_available_to_portal");
                else
                    await AssertPermissionDeniedAsync(response);
                Assert.Equal(0L, await factory.ScalarAsync<long>("select coalesce(sum(correction_count), 0) from attendance.attendance_records"));
            }
        }
    }

    // The schedule cases fix the clock; every other case runs on real time, as before.
    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/subjects", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsGetById(string role, HttpStatusCode expected)
    {
        var (id, _) = await CreateSubjectAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsCreate(string role, HttpStatusCode expected)
    {
        var countBefore = await CountSubjectsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/subjects", UriKind.Relative), new { name = $"Matrix Create {Guid.NewGuid():N}" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountSubjectsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsRename(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateSubjectAsOwnerAsync($"Matrix Rename {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "Renamed By Attempt", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsArchive(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateSubjectAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task SubjectsRestore(string role, HttpStatusCode expected)
    {
        var (id, archivedVersion) = await CreateArchivedSubjectAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"archived|{archivedVersion}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StaffList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StaffCreate(string role, HttpStatusCode expected)
    {
        var usersBefore = await factory.ScalarAsync<long>("select count(*) from identity.users");
        var membershipsBefore = await factory.ScalarAsync<long>("select count(*) from identity.memberships");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/staff", UriKind.Relative),
            new { email = $"matrix-create-{Guid.NewGuid():N}@nile.test", displayName = "Matrix Create", role = "teacher", preferredLocale = "en" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(usersBefore, await factory.ScalarAsync<long>("select count(*) from identity.users"));
            Assert.Equal(membershipsBefore, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StaffChangeRole(string role, HttpStatusCode expected)
    {
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync($"matrix-role-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "secretary", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|active|{version}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StaffDeactivate(string role, HttpStatusCode expected)
    {
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync($"matrix-deact-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|active|{version}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StaffReactivate(string role, HttpStatusCode expected)
    {
        var (membershipId, deactivatedVersion) = await CreateDeactivatedStaffMemberAsOwnerAsync($"matrix-react-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/reactivate", UriKind.Relative), new { version = deactivatedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|inactive|{deactivatedVersion}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task AuditList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/audit", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task CentreSettingsGet(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/centre/settings", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task CentreSettingsUpdate(string role, HttpStatusCode expected)
    {
        var before = await SnapshotNileCentreAsync();
        var version = await GetNileCentreVersionAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri("/api/centre/settings", UriKind.Relative),
            new { name = "Matrix Attempt", defaultLocale = "en", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(before, await SnapshotNileCentreAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ParentsCreate(string role, HttpStatusCode expected)
    {
        var countBefore = await CountParentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/parents", UriKind.Relative),
            new { fullName = $"Matrix Create {Guid.NewGuid():N}", phone = $"015{Random.Shared.Next(10000000, 99999999)}" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountParentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ParentsGetById(string role, HttpStatusCode expected)
    {
        var (id, _) = await CreateParentAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/parents/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ParentsUpdate(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateParentAsOwnerAsync($"Matrix Update {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotParentAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/parents/{id}", UriKind.Relative),
            new { fullName = "Renamed By Attempt", phone = "01012340000", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotParentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ParentsLookup(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/parents/lookup", UriKind.Relative), new { phone = "01012340001" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/students", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsGetById(string role, HttpStatusCode expected)
    {
        var (id, _, _) = await RegisterStudentAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsRegister(string role, HttpStatusCode expected)
    {
        var countBefore = await CountStudentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/students", UriKind.Relative),
            new
            {
                fullName = $"Matrix Register {Guid.NewGuid():N}", phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = "Matrix Parent", phone = UniquePhone("012") }, relationship = "father",
            });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountStudentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsUpdate(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Update {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotStudentAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/students/{id}", UriKind.Relative),
            new { fullName = "Renamed By Attempt", phone = (string?)null, schoolYear = (string?)null, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsArchive(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsRestore(string role, HttpStatusCode expected)
    {
        var (id, _, archivedVersion) = await RegisterArchivedStudentAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"archived|{archivedVersion}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsLinkParent(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Link {Guid.NewGuid():N}");
        var secondParentId = await RegisterSecondParentAsOwnerAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = secondParentId, relationship = "mother", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsUnlinkParent(string role, HttpStatusCode expected)
    {
        var (id, firstParentId, secondParentId, version) = await RegisterStudentWithTwoParentsAsOwnerAsync($"Matrix Unlink {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/unlink", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
            Assert.True(firstParentId != Guid.Empty);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task StudentsMakePrimary(string role, HttpStatusCode expected)
    {
        var (id, _, secondParentId, version) = await RegisterStudentWithTwoParentsAsOwnerAsync($"Matrix MakePrimary {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/make-primary", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/teachers", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersGetById(string role, HttpStatusCode expected)
    {
        var (id, _) = await CreateTeacherAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersCreate(string role, HttpStatusCode expected)
    {
        var countBefore = await CountTeachersAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/teachers", UriKind.Relative), new { fullName = $"Matrix Create {Guid.NewGuid():N}", phone = (string?)null });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountTeachersAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersUpdate(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateTeacherAsOwnerAsync($"Matrix Update {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}", UriKind.Relative),
            new { fullName = "Updated By Attempt", phone = (string?)null, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotTeacherAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersSetAccount(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateTeacherAsOwnerAsync($"Matrix SetAccount {Guid.NewGuid():N}");
        var eligibleUserId = await CreateEligibleTeacherUserAsOwnerAsync($"matrix-account-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/teachers/{id}/account", UriKind.Relative), new { userId = eligibleUserId, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotTeacherAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersArchive(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateTeacherAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotTeacherAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task TeachersRestore(string role, HttpStatusCode expected)
    {
        var (id, archivedVersion) = await CreateArchivedTeacherAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"archived|{archivedVersion}", await SnapshotTeacherAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/groups", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsGetById(string role, HttpStatusCode expected)
    {
        // The seeded Nile group the two-centre teacher owns, so the teacher's 200 is the permission decision,
        // not a scope decision; out-of-scope reads are GroupTeacherScopeTests' question.
        var id = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsCreate(string role, HttpStatusCode expected)
    {
        var (subjectId, _) = await CreateSubjectAsOwnerAsync($"Matrix Group Subject {Guid.NewGuid():N}");
        var (teacherId, _) = await CreateTeacherAsOwnerAsync($"Matrix Group Teacher {Guid.NewGuid():N}");
        var countBefore = await CountGroupsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/groups", UriKind.Relative),
            new { name = $"Matrix Create {Guid.NewGuid():N}", subjectId, teacherId, capacity = 20 });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountGroupsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsRename(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateGroupAsOwnerAsync($"Matrix Rename {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotGroupAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/groups/{id}", UriKind.Relative), new { name = $"Renamed By Attempt {Guid.NewGuid():N}", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotGroupAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsChangeCapacity(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateGroupAsOwnerAsync($"Matrix Capacity {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotGroupAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/groups/{id}/capacity", UriKind.Relative), new { capacity = 30, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotGroupAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsAssignTeacher(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateGroupAsOwnerAsync($"Matrix Assign {Guid.NewGuid():N}");
        var (newTeacherId, _) = await CreateTeacherAsOwnerAsync($"Matrix New Teacher {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotGroupAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/groups/{id}/teacher", UriKind.Relative), new { teacherId = newTeacherId, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotGroupAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsArchive(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateGroupAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotGroupAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/groups/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotGroupAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task GroupsRestore(string role, HttpStatusCode expected)
    {
        var (id, archivedVersion) = await CreateArchivedGroupAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotGroupAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/groups/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotGroupAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task EnrollmentsEnroll(string role, HttpStatusCode expected)
    {
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        var (studentId, _, _) = await RegisterStudentAsOwnerAsync($"Matrix Enroll {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotEnrollmentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotEnrollmentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task EnrollmentsRoster(string role, HttpStatusCode expected)
    {
        // The seeded Nile group the two-centre teacher owns, so the teacher's 200 is the permission decision,
        // not a scope decision; out-of-scope rosters are EnrollmentTeacherScopeTests' question.
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/groups/{groupId}/roster", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task EnrollmentsListForStudent(string role, HttpStatusCode expected)
    {
        var (_, studentId) = await EnrollNewStudentInTeachersOwnGroupAsOwnerAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/students/{studentId}/enrollments", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task EnrollmentsWithdraw(string role, HttpStatusCode expected)
    {
        var (enrollmentId, _) = await EnrollNewStudentInTeachersOwnGroupAsOwnerAsync();
        var version = await GetEnrollmentVersionAsync(enrollmentId);
        var snapshotBefore = await SnapshotEnrollmentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/enrollments/{enrollmentId}/withdraw", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotEnrollmentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task EnrollmentsTransfer(string role, HttpStatusCode expected)
    {
        var (sourceGroupId, targetGroupId) = await CreateSameSubjectGroupsAsOwnerAsync($"Matrix Transfer {Guid.NewGuid():N}");
        var (studentId, _, _) = await RegisterStudentAsOwnerAsync($"Matrix Transfer {Guid.NewGuid():N}");
        var enrollmentId = await EnrollAsOwnerAsync(sourceGroupId, studentId);
        var version = await GetEnrollmentVersionAsync(enrollmentId);
        var snapshotBefore = await SnapshotEnrollmentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/enrollments/{enrollmentId}/transfer", UriKind.Relative), new { targetGroupId, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotEnrollmentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleListRules(string role, HttpStatusCode expected)
    {
        // The seeded Nile group the two-centre teacher owns: the teacher's 200 is the permission decision, not a scope
        // decision; out-of-scope groups are ScheduleTeacherScopeTests' question.
        FixScheduleClock();
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleAddRule(string role, HttpStatusCode expected)
    {
        FixScheduleClock();
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        var snapshotBefore = await SnapshotScheduleAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative), MatrixRule);

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotScheduleAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleEndRule(string role, HttpStatusCode expected)
    {
        FixScheduleClock();
        var ruleId = await AddRuleToTeachersOwnGroupAsOwnerAsync();
        var version = await factory.ScalarAsync<uint>($"select xmin from academics.schedule_rules where id = '{ruleId}'");
        var snapshotBefore = await SnapshotScheduleAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/schedule-rules/{ruleId}/end", UriKind.Relative), new { lastDate = "2027-05-01", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotScheduleAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleExtend(string role, HttpStatusCode expected)
    {
        FixScheduleClock();
        await AddRuleToTeachersOwnGroupAsOwnerAsync();
        var snapshotBefore = await SnapshotScheduleAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/schedule/extend", UriKind.Relative), new { through = "2027-06-15" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotScheduleAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleTimetable(string role, HttpStatusCode expected)
    {
        FixScheduleClock();
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/timetable?weekStart=2027-04-24", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    [InlineData("parent", HttpStatusCode.Forbidden)]
    [InlineData("student", HttpStatusCode.Forbidden)]
    public async Task ScheduleCancelSession(string role, HttpStatusCode expected)
    {
        FixScheduleClock();
        var ruleId = await AddRuleToTeachersOwnGroupAsOwnerAsync();
        var sessionId = await factory.ScalarAsync<Guid>(
            $"select id from academics.class_sessions where schedule_rule_id = '{ruleId}' order by local_date limit 1");
        var version = await factory.ScalarAsync<uint>($"select xmin from academics.class_sessions where id = '{sessionId}'");
        var snapshotBefore = await SnapshotScheduleAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/sessions/{sessionId}/cancel", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotScheduleAsync());
        }
    }

    private static readonly object MatrixRule =
        new { dayOfWeek = "saturday", startTime = "18:00", durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" };

    // Tuesday 20 April 2027, 10:00 in Cairo: the rule above generates four Saturdays from 24 April.
    private void FixScheduleClock() => factory.Time.FixAt(new DateTimeOffset(2027, 4, 20, 8, 0, 0, TimeSpan.Zero));

    private async Task<Guid> AddRuleToTeachersOwnGroupAsOwnerAsync()
    {
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative), MatrixRule);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private Task<string> SnapshotScheduleAsync() =>
        factory.ScalarAsync<string>(
            """
            select (select count(*)::text || ':' || coalesce(string_agg(id::text || ':' || coalesce(effective_to::text, '-') || ':' || xmin::text, ',' order by id), '')
                    from academics.schedule_rules)
                || '|' ||
                   (select count(*)::text || ':' || coalesce(string_agg(id::text || ':' || status || ':' || xmin::text, ',' order by id), '')
                    from academics.class_sessions)
            """);

    private async Task<(Guid EnrollmentId, Guid StudentId)> EnrollNewStudentInTeachersOwnGroupAsOwnerAsync()
    {
        var groupId = await GetTwoCentreTeachersOwnNileGroupIdAsync();
        var (studentId, _, _) = await RegisterStudentAsOwnerAsync($"Matrix Enrolled {Guid.NewGuid():N}");
        return (await EnrollAsOwnerAsync(groupId, studentId), studentId);
    }

    private async Task<Guid> EnrollAsOwnerAsync(Guid groupId, Guid studentId)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private async Task<(Guid SourceGroupId, Guid TargetGroupId)> CreateSameSubjectGroupsAsOwnerAsync(string name)
    {
        var (subjectId, _) = await CreateSubjectAsOwnerAsync($"{name} Subject");
        var (teacherId, _) = await CreateTeacherAsOwnerAsync($"{name} Teacher");
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var sourceResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/groups", UriKind.Relative), new { name = $"{name} Source", subjectId, teacherId, capacity = 20 });
        using var targetResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/groups", UriKind.Relative), new { name = $"{name} Target", subjectId, teacherId, capacity = 20 });
        return (
            (await ReadJsonAsync(sourceResponse)).RootElement.GetProperty("id").GetGuid(),
            (await ReadJsonAsync(targetResponse)).RootElement.GetProperty("id").GetGuid());
    }

    private Task<uint> GetEnrollmentVersionAsync(Guid enrollmentId) =>
        factory.ScalarAsync<uint>($"select xmin from academics.enrollments where id = '{enrollmentId}'");

    private Task<string> SnapshotEnrollmentsAsync() =>
        factory.ScalarAsync<string>(
            "select count(*)::text || '|' || coalesce(string_agg(id::text || ':' || status || ':' || xmin::text, ',' order by id), '') from academics.enrollments");

    private Task<Guid> GetTwoCentreTeachersOwnNileGroupIdAsync() =>
        factory.ScalarAsync<Guid>(
            """
            select g.id from academics.groups g
            join academics.teachers t on t.id = g.teacher_id
            join identity.users u on u.id = t.user_id
            join platform.centres c on c.id = g.centre_id
            where u.email = 'teacher@both.test' and c.slug = 'nile-centre'
            order by g.id limit 1
            """);

    private async Task<(Guid Id, uint Version)> CreateGroupAsOwnerAsync(string name)
    {
        var (subjectId, _) = await CreateSubjectAsOwnerAsync($"{name} Subject");
        var (teacherId, _) = await CreateTeacherAsOwnerAsync($"{name} Teacher");
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/groups", UriKind.Relative), new { name, subjectId, teacherId, capacity = 20 });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, uint ArchivedVersion)> CreateArchivedGroupAsOwnerAsync(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateGroupAsOwnerAsync(name);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/groups/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, body.RootElement.GetProperty("version").GetUInt32());
    }

    private Task<string> SnapshotGroupAsync(Guid id) =>
        factory.ScalarAsync<string>(
            $"select name || '|' || capacity || '|' || teacher_id || '|' || status || '|' || xmin from academics.groups where id = '{id}'");

    private Task<long> CountGroupsAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.groups where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    // Random.Shared.Next(10000000, 99999999) collides often enough across this many call sites in one run to
    // occasionally produce a 409 parent.phone_taken where a test expects 201/204 — an interlocked counter
    // never repeats within a run, unlike eight random digits.
    private static int _nextPhoneSuffix = 10000000;

    private static string UniquePhone(string prefix) => prefix + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private Task<long> CountStudentsAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.students where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    private async Task<(Guid Id, Guid ParentId, uint Version)> RegisterStudentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/students", UriKind.Relative),
            new { fullName, phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Matrix Target Parent", phone = UniquePhone("012") }, relationship = "father" });
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();
        var parentId = createBody.RootElement.GetProperty("parentId").GetGuid();
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, parentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, Guid ParentId, uint ArchivedVersion)> RegisterArchivedStudentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, parentId, version) = await RegisterStudentAsOwnerAsync(fullName);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, parentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<Guid> RegisterSecondParentAsOwnerAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/students", UriKind.Relative),
            new
            {
                fullName = $"Matrix Temp Holder {Guid.NewGuid():N}", phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = "Matrix Second Parent", phone = UniquePhone("012") }, relationship = "mother",
            });
        var body = await ReadJsonAsync(createResponse);
        return body.RootElement.GetProperty("parentId").GetGuid();
    }

    private async Task<(Guid Id, Guid FirstParentId, Guid SecondParentId, uint Version)> RegisterStudentWithTwoParentsAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, firstParentId, version) = await RegisterStudentAsOwnerAsync(fullName);
        var secondParentId = await RegisterSecondParentAsOwnerAsync();
        using var linkResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = secondParentId, relationship = "mother", version });
        Assert.Equal(HttpStatusCode.NoContent, linkResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, firstParentId, secondParentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private Task<string> SnapshotStudentAsync(Guid id) =>
        factory.ScalarAsync<string>($"select status || '|' || xmin from academics.students where id = '{id}'");

    private Task<long> CountParentsAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.parents where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    private async Task<(Guid Id, uint Version)> CreateParentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/parents", UriKind.Relative),
            new { fullName, phone = $"011{Random.Shared.Next(10000000, 99999999)}" });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private Task<string> SnapshotParentAsync(Guid id) =>
        factory.ScalarAsync<string>($"select full_name || '|' || phone || '|' || xmin from academics.parents where id = '{id}'");

    private Task<string> SnapshotNileCentreAsync() =>
        factory.ScalarAsync<string>("select name || '|' || default_locale || '|' || xmin from platform.centres where slug = 'nile-centre'");

    private async Task<uint> GetNileCentreVersionAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri("/api/centre/settings", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private async Task<SignedInClient> SignInAsync(string role)
    {
        if (role is "parent" or "student")
        {
            var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
            var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
            var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
            var subject = await factory.ScalarAsync<Guid>($"select id from academics.{(role == "parent" ? "parents" : "students")} where centre_id='{centre}' order by id limit 1");
            await factory.ExecuteAsync($"""
                update identity.memberships set role='{role}' where centre_id='{centre}' and user_id='{user}';
                insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
                values ('{Guid.NewGuid()}','{centre}','{user}','{role}',{(role == "parent" ? $"'{subject}'" : "null")},{(role == "student" ? $"'{subject}'" : "null")},now(),'{owner}');
                """);
            return await SignInHelper.SignInAsync(factory, "secretary@nile.test");
        }
        return role == "teacher"
            ? await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre")
            : await SignInHelper.SignInAsync(factory, role);
    }

    private static async Task AssertPermissionDeniedAsync(HttpResponseMessage response)
    {
        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    private async Task<(Guid Id, uint Version)> CreateSubjectAsOwnerAsync(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, new Uri("/api/subjects", UriKind.Relative), new { name });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, uint ArchivedVersion)> CreateArchivedSubjectAsOwnerAsync(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsOwnerAsync(name);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid MembershipId, uint Version)> CreateStaffMemberAsOwnerAsync(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/staff", UriKind.Relative), new { email, displayName = "Matrix Target", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(createResponse);
        var membershipId = body.RootElement.GetProperty("membershipId").GetGuid();
        using var listResponse = await owner.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var version = listBody.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("membershipId").GetGuid() == membershipId).GetProperty("version").GetUInt32();
        return (membershipId, version);
    }

    private async Task<(Guid MembershipId, uint DeactivatedVersion)> CreateDeactivatedStaffMemberAsOwnerAsync(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync(email);
        using var deactivateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
        using var listResponse = await owner.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var deactivatedVersion = listBody.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("membershipId").GetGuid() == membershipId).GetProperty("version").GetUInt32();
        return (membershipId, deactivatedVersion);
    }

    private Task<string> SnapshotSubjectAsync(Guid id) =>
        factory.ScalarAsync<string>($"select status || '|' || xmin from academics.subjects where id = '{id}'");

    private Task<string> SnapshotMembershipAsync(Guid membershipId) =>
        factory.ScalarAsync<string>($"select role || '|' || status || '|' || xmin from identity.memberships where id = '{membershipId}'");

    private async Task<int> CountSubjectsAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri("/api/subjects?includeArchived=true", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private async Task<(Guid Id, uint Version)> CreateTeacherAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/teachers", UriKind.Relative), new { fullName, phone = (string?)null });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, uint ArchivedVersion)> CreateArchivedTeacherAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsOwnerAsync(fullName);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/teachers/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/teachers/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, body.RootElement.GetProperty("version").GetUInt32());
    }

    /// <summary>Creates a real Nile Teacher-role staff member through /api/staff, so TeachersSetAccount's allowed case links to a genuinely eligible user.</summary>
    private async Task<Guid> CreateEligibleTeacherUserAsOwnerAsync(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/staff", UriKind.Relative), new { email, displayName = "Matrix Teacher Account", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(createResponse);
        return body.RootElement.GetProperty("userId").GetGuid();
    }

    private Task<string> SnapshotTeacherAsync(Guid id) =>
        factory.ScalarAsync<string>($"select status || '|' || xmin from academics.teachers where id = '{id}'");

    private Task<long> CountTeachersAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.teachers where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
