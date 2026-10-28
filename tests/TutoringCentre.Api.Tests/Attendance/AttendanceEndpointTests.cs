using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class AttendanceEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(BeforeSession);
    }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }

    [Fact]
    public async Task FiveRoutes_MarkRepeatRosterCorrectStaleAndHistoryPaging()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await Arrange(owner.Client);
        factory.Time.FixAt(InSession);
        var sessions = await Get(owner.Client, "/api/attendance/sessions");
        Assert.Equal(Date, sessions.GetProperty("date").GetString());
        Assert.Equal("Africa/Cairo", sessions.GetProperty("timeZoneId").GetString());
        Assert.Equal("open", Assert.Single(sessions.GetProperty("items").EnumerateArray()).GetProperty("window").GetString());
        using var created = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        Assert.Equal(2, (await Json(created)).GetProperty("created").GetInt32());
        using var repeated = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        Assert.Equal(2, (await Json(repeated)).GetProperty("unchanged").GetInt32());
        var roster = await Get(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance");
        Assert.Equal(3, roster.GetProperty("items").GetArrayLength());
        Assert.Single(roster.GetProperty("items").EnumerateArray(), row => row.GetProperty("record").ValueKind == JsonValueKind.Null);
        var record = await Record(owner.Client, scenario);
        var url = $"/api/attendance/{record.GetProperty("recordId").GetGuid()}/correct";
        var correction = new { status = "absent", reason = "Teacher tapped wrong row", version = record.GetProperty("version").GetUInt32() };
        using var corrected = await Post(owner.Client, url, correction);
        Assert.Equal(HttpStatusCode.NoContent, corrected.StatusCode);
        Assert.DoesNotContain(factory.AttendanceLogs.Snapshot(), entry =>
            entry.RenderMessage(System.Globalization.CultureInfo.InvariantCulture).Contains(correction.reason, StringComparison.Ordinal)
            || entry.Properties.Values.Any(value => value.ToString().Contains(correction.reason, StringComparison.Ordinal)));
        using var stale = await Post(owner.Client, url, correction);
        await Problem(stale, HttpStatusCode.Conflict, "concurrency.stale");
        var page = await Get(owner.Client, $"/api/attendance?from={Date}&to={Date}&pageSize=1");
        var first = Assert.Single(page.GetProperty("items").EnumerateArray()).GetProperty("recordId").GetGuid();
        var cursor = page.GetProperty("nextCursor").GetString();
        Assert.NotNull(cursor);
        var next = await Get(owner.Client, $"/api/attendance?from={Date}&to={Date}&pageSize=1&cursor={Uri.EscapeDataString(cursor)}");
        Assert.NotEqual(first, Assert.Single(next.GetProperty("items").EnumerateArray()).GetProperty("recordId").GetGuid());
        Assert.Equal(JsonValueKind.Null, next.GetProperty("nextCursor").ValueKind);
    }

    [Theory]
    [InlineData("source")]
    [InlineData("enrollmentId")]
    [InlineData("centreId")]
    public async Task Marks_RejectUnknownNestedFields(string field)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var mark = new Dictionary<string, object> { ["studentId"] = Guid.NewGuid(), ["status"] = "present", [field] = "untrusted" };
        using var response = await Put(owner.Client, $"/api/sessions/{Guid.NewGuid()}/attendance", new { marks = new[] { mark } });
        await Problem(response, HttpStatusCode.BadRequest, "request.malformed");
    }

    [Theory]
    [InlineData("from=2027-04-21&to=2027-04-20")]
    [InlineData("from=2027-01-01&to=2027-04-04")]
    [InlineData("from=2027-04-20&to=2027-04-20&pageSize=0")]
    [InlineData("from=2027-04-20&to=2027-04-20&pageSize=101")]
    [InlineData("from=2027-04-20&to=2027-04-20&cursor=bad")]
    public async Task History_RejectsInvalidQuery(string query)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri($"/api/attendance?{query}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True((await Json(response)).TryGetProperty("errors", out _));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task Marks_RejectInvalidBatchSize(int count)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await Put(owner.Client, $"/api/sessions/{Guid.NewGuid()}/attendance",
            new { marks = Enumerable.Range(0, count).Select(_ => new { studentId = Guid.NewGuid(), status = "present" }).ToArray() });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-31, "attendance.session_not_open")]
    [InlineData(181, "attendance.window_closed")]
    public async Task Marks_RejectClosedWindow(int minutes, string code)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await Arrange(owner.Client);
        factory.Time.FixAt(InSession.AddMinutes(minutes));
        using var response = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        await Problem(response, HttpStatusCode.UnprocessableEntity, code);
    }

    [Fact]
    public async Task UnknownSessionAndRecord_ReturnUniform404()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var roster = await owner.Client.GetAsync(new Uri($"/api/sessions/{Guid.NewGuid()}/attendance", UriKind.Relative));
        await Problem(roster, HttpStatusCode.NotFound, "attendance.session_not_found");
        using var mark = await Put(owner.Client, $"/api/sessions/{Guid.NewGuid()}/attendance", new { marks = new[] { new { studentId = Guid.NewGuid(), status = "present" } } });
        await Problem(mark, HttpStatusCode.NotFound, "attendance.session_not_found");
        using var correction = await Post(owner.Client, $"/api/attendance/{Guid.NewGuid()}/correct", new { status = "present", reason = "Valid reason", version = 1 });
        await Problem(correction, HttpStatusCode.NotFound, "attendance.not_found");
    }

    [Fact]
    public async Task MissingSessionAndCsrf_AreRefused()
    {
        using var anonymous = factory.CreateClient();
        using var response = await anonymous.GetAsync(new Uri("/api/attendance/sessions", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var csrf = await owner.Client.PutAsJsonAsync(new Uri($"/api/sessions/{Guid.NewGuid()}/attendance", UriKind.Relative), new { marks = Array.Empty<object>() });
        Assert.Equal(HttpStatusCode.Forbidden, csrf.StatusCode);
    }

    [Theory]
    [InlineData("duplicate")]
    [InlineData("unexpected")]
    [InlineData("cancelled")]
    [InlineData("corrected")]
    [InlineData("earlyCorrection")]
    [InlineData("unchangedCorrection")]
    [InlineData("badReason")]
    public async Task Rules_ReturnContractCodesAndPreserveRows(string scenarioName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await Arrange(owner.Client);
        if (scenarioName == "cancelled")
        {
            var session = Assert.Single((await Get(owner.Client, $"/api/timetable?weekStart={Date}&groupId={scenario.GroupId}")).GetProperty("sessions").EnumerateArray());
            using var cancelled = await Post(owner.Client, $"/api/sessions/{scenario.SessionId}/cancel", new { version = session.GetProperty("version").GetUInt32() });
            Assert.Equal(HttpStatusCode.NoContent, cancelled.StatusCode);
        }
        factory.Time.FixAt(scenarioName == "earlyCorrection" ? InSession.AddMinutes(-10) : InSession);
        if (scenarioName is "duplicate" or "unexpected" or "cancelled")
        {
            object body = scenarioName == "duplicate"
                ? new { marks = new[] { new { studentId = scenario.Students[0], status = "present" }, new { studentId = scenario.Students[0], status = "absent" } } }
                : scenarioName == "unexpected" ? new { marks = new[] { new { studentId = Guid.NewGuid(), status = "present" } } } : Marks(scenario);
            using var response = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", body);
            if (scenarioName == "duplicate") { Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); }
            else { await Problem(response, HttpStatusCode.UnprocessableEntity, scenarioName == "unexpected" ? "attendance.student_not_in_session" : "attendance.session_cancelled"); }
            Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from attendance.attendance_records"));
            if (scenarioName == "cancelled")
            {
                var session = Assert.Single((await Get(owner.Client, "/api/attendance/sessions")).GetProperty("items").EnumerateArray());
                Assert.Equal("cancelled", session.GetProperty("status").GetString());
            }
            return;
        }
        using var marked = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        var record = await Record(owner.Client, scenario);
        using var correction = await Post(owner.Client, $"/api/attendance/{record.GetProperty("recordId").GetGuid()}/correct", new
        {
            status = scenarioName == "unchangedCorrection" ? "present" : "absent",
            reason = scenarioName == "badReason" ? " " : "Valid correction reason", version = record.GetProperty("version").GetUInt32(),
        });
        if (scenarioName == "corrected")
        {
            Assert.Equal(HttpStatusCode.NoContent, correction.StatusCode);
            using var remark = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
            await Problem(remark, HttpStatusCode.UnprocessableEntity, "attendance.record_corrected");
        }
        else if (scenarioName == "badReason") { Assert.Equal(HttpStatusCode.BadRequest, correction.StatusCode); }
        else { await Problem(correction, HttpStatusCode.UnprocessableEntity, scenarioName == "earlyCorrection" ? "attendance.correction_too_early" : "attendance.correction_unchanged"); }
    }

    [Fact]
    public async Task Roster_ExcludesLateEnrollment_RetainsWithdrawnAfterStart()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await Arrange(owner.Client);
        factory.Time.FixAt(InSession);
        using var marked = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        factory.Time.FixAt(InSession.AddMinutes(1));
        var enrollments = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/enrollments")).GetProperty("items");
        var enrollment = Assert.Single(enrollments.EnumerateArray(), row => row.GetProperty("enrollmentId").GetGuid() == scenario.Enrollments[0]);
        using var withdrawn = await Post(owner.Client, $"/api/enrollments/{scenario.Enrollments[0]}/withdraw", new { version = enrollment.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, withdrawn.StatusCode);
        var late = (await Get(owner.Client, "/api/students?pageSize=100")).GetProperty("items").EnumerateArray()
            .Select(row => row.GetProperty("id").GetGuid()).First(id => !scenario.Students.Contains(id));
        await Create(owner.Client, $"/api/groups/{scenario.GroupId}/enrollments", new { studentId = late });
        var roster = (await Get(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance")).GetProperty("items");
        Assert.Equal(3, roster.GetArrayLength());
        Assert.Contains(roster.EnumerateArray(), row => row.GetProperty("studentId").GetGuid() == scenario.Students[0] && row.GetProperty("record").ValueKind == JsonValueKind.Object);
        Assert.DoesNotContain(roster.EnumerateArray(), row => row.GetProperty("studentId").GetGuid() == late);
    }

    [Fact]
    public async Task NoSelectedCentre_Returns403EvenWithDate()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        using var response = await teacher.Client.GetAsync(new Uri($"/api/attendance/sessions?date={Date}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("2028-04-21")]
    [InlineData("2026-04-18")]
    public async Task Sessions_RejectDatesOutside366Days(string date)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri($"/api/attendance/sessions?date={date}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ConcurrentFirstMarks_Return409AndPersistOneBatch()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var scenario = await Arrange(owner.Client);
        factory.Time.FixAt(InSession);
        factory.AttendanceRace.Enable();
        try
        {
            var responses = await Task.WhenAll(
                Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario)),
                Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario)));
            using var success = Assert.Single(responses, response => response.StatusCode == HttpStatusCode.OK);
            using var conflict = Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Conflict);
            await Problem(conflict, HttpStatusCode.Conflict, "attendance.concurrent_mark");
            Assert.Equal(2L, await factory.ScalarAsync<long>("select count(*) from attendance.attendance_records"));
        }
        finally { factory.AttendanceRace.Disable(); }
    }
}
