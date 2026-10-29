using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class ScanEndpointTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Fact]
    public async Task TeacherScan_RecordRepeatAndManualAbsence_UsesQr()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        var card = await Issue(owner.Client, scenario.Students[0]);
        Factory.Time.FixAt(InSession);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(Factory, "nile-centre");
        using var absent = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", new { marks = new[] { new { studentId = scenario.Students[0], status = "absent" } } });
        Assert.Equal(HttpStatusCode.OK, absent.StatusCode);
        using var first = await Scan(teacher.Client, scenario.SessionId, card.GetProperty("token").GetString()!);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal("recorded", (await Json(first)).GetProperty("outcome").GetString());
        Factory.Time.FixAt(InSession.AddMinutes(16));
        using var second = await Scan(teacher.Client, scenario.SessionId, card.GetProperty("token").GetString()!);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal("alreadyRecorded", (await Json(second)).GetProperty("outcome").GetString());
        Assert.Equal("present", (await Json(second)).GetProperty("status").GetString());
        Assert.Equal("qr", (await Record(owner.Client, scenario)).GetProperty("source").GetString());
    }

    [Theory]
    [InlineData("format", 400, "card.invalid_format")]
    [InlineData("field", 400, "request.malformed")]
    [InlineData("unknown", 404, "card.not_recognised")]
    [InlineData("session", 404, "attendance.session_not_found")]
    [InlineData("scope", 404, "attendance.session_not_found")]
    [InlineData("revoked", 422, "card.revoked")]
    [InlineData("early", 422, "attendance.session_not_open")]
    [InlineData("closed", 422, "attendance.window_closed")]
    [InlineData("cancelled", 422, "attendance.session_cancelled")]
    [InlineData("unexpected", 422, "attendance.student_not_in_session")]
    [InlineData("corrected", 422, "attendance.record_corrected")]
    public async Task Failures_OutcomeTable(string failure, int status, string code)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client, failure != "scope");
        var card = await Issue(owner.Client, scenario.Students[0]);
        var token = card.GetProperty("token").GetString()!;
        var cardStatus = (await Get(owner.Client, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        if (failure == "revoked") { using var revoked = await Post(owner.Client, $"/api/student-cards/{cardStatus.GetProperty("cardId").GetGuid()}/revoke", new { reason = "lost", version = cardStatus.GetProperty("version").GetUInt32() }); Assert.Equal(HttpStatusCode.NoContent, revoked.StatusCode); }
        if (failure == "cancelled")
        {
            var session = (await Get(owner.Client, $"/api/timetable?weekStart={Date}&groupId={scenario.GroupId}")).GetProperty("sessions")[0];
            using var cancelled = await Post(owner.Client, $"/api/sessions/{scenario.SessionId}/cancel", new { version = session.GetProperty("version").GetUInt32() });
            Assert.Equal(HttpStatusCode.NoContent, cancelled.StatusCode);
        }
        if (failure == "unexpected")
        {
            var outsider = (await Get(owner.Client, "/api/students?pageSize=100")).GetProperty("items").EnumerateArray().First(s => !scenario.Students.Contains(s.GetProperty("id").GetGuid())).GetProperty("id").GetGuid();
            token = (await Issue(owner.Client, outsider)).GetProperty("token").GetString()!;
        }
        Factory.Time.FixAt(failure == "early" ? InSession.AddHours(-1) : failure == "closed" ? InSession.AddHours(4) : InSession);
        if (failure == "corrected")
        {
            using var marked = await Put(owner.Client, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
            var record = await Record(owner.Client, scenario);
            using var corrected = await Post(owner.Client, $"/api/attendance/{record.GetProperty("recordId").GetGuid()}/correct", new { status = "excused", reason = "Office decision", version = record.GetProperty("version").GetUInt32() });
            Assert.Equal(HttpStatusCode.NoContent, corrected.StatusCode);
        }
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(Factory, "nile-centre");
        object body = failure == "field" ? new { token, studentId = scenario.Students[0] } : new { token = failure == "format" ? "TCM1.short" : failure == "unknown" ? TutoringCentre.Domain.Attendance.CardToken.Generate().Reveal() : token };
        using var response = await Post(teacher.Client, $"/api/sessions/{(failure == "session" ? Guid.NewGuid() : scenario.SessionId)}/scans", body);
        await Problem(response, (HttpStatusCode)status, code);
    }

    [Fact]
    public async Task Scan_RequiresSessionCentreAndCsrf()
    {
        using var anonymous = Factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var missingSession = await anonymous.PostAsync(new Uri($"/api/sessions/{Guid.NewGuid()}/scans", UriKind.Relative),
            System.Net.Http.Json.JsonContent.Create(new { token = "TCM1.short" }));
        Assert.Equal(HttpStatusCode.Unauthorized, missingSession.StatusCode);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var missingCsrf = await owner.Client.PostAsync(new Uri($"/api/sessions/{Guid.NewGuid()}/scans", UriKind.Relative), System.Net.Http.Json.JsonContent.Create(new { token = "TCM1.short" }));
        Assert.Equal(HttpStatusCode.Forbidden, missingCsrf.StatusCode);
        using var teacher = await SignInHelper.SignInAsync(Factory, "teacher@both.test");
        using var noCentre = await Scan(teacher.Client, Guid.NewGuid(), TutoringCentre.Domain.Attendance.CardToken.Generate().Reveal());
        await Problem(noCentre, HttpStatusCode.Forbidden, "tenant.not_selected");
    }
}
