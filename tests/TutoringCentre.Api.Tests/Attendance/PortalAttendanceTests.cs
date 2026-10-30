using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
namespace TutoringCentre.Api.Tests.Attendance;
[Collection(ApiCollection.Name)]
public sealed class PortalAttendanceTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync() { await factory.ResetAsync(); Assert.Equal(0, await SeedCommand.RunAsync(factory.Services)); }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }
    [Theory] [InlineData("parent")] [InlineData("student")]
    public async Task PortalHistoryAndRosterPermissionContract(string role)
    {
        using var actor = await PortalAsync(role);
        using var history = await actor.Client.GetAsync(new Uri("/api/attendance?from=2026-10-01&to=2026-10-31", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, history.StatusCode);
        foreach (var route in new[] { "/api/attendance/sessions", $"/api/sessions/{Guid.NewGuid()}/attendance", $"/api/conversations/{Guid.NewGuid()}/timeline" })
        {
            using var denied = await actor.Client.GetAsync(new Uri(route, UriKind.Relative));
            Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        }
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='maadi-hub'");
        var foreign = await factory.ScalarAsync<Guid>($"select id from academics.students where centre_id='{centre}' order by id limit 1");
        var bodies = new List<string>();
        foreach(var id in new[] { foreign, Guid.NewGuid() })
        {
            using var response = await actor.Client.GetAsync(new Uri($"/api/attendance?from=2026-10-01&to=2026-10-31&studentId={id}", UriKind.Relative));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var node = System.Text.Json.Nodes.JsonNode.Parse(await response.Content.ReadAsStringAsync())!.AsObject();
            node.Remove("traceId"); node.Remove("correlationId"); bodies.Add(node.ToJsonString());
        }
        Assert.Equal(bodies[0], bodies[1]);
    }
    [Theory] [InlineData("parent")] [InlineData("student")]
    public async Task OwnChildHasRecordedRowsAndForeignGroupCannotWiden(string role)
    {
        factory.Time.FixAt(AttendanceHttp.BeforeSession);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var own = await AttendanceHttp.Arrange(owner.Client);
        var foreign = await AttendanceHttp.Arrange(owner.Client, false);
        factory.Time.FixAt(AttendanceHttp.InSession);
        using var marked = await AttendanceHttp.Put(owner.Client, $"/api/sessions/{own.SessionId}/attendance", AttendanceHttp.Marks(own));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        var subject = role == "student" ? own.Students[0] : await factory.ScalarAsync<Guid>($"select parent_id from academics.student_parents where student_id='{own.Students[0]}' limit 1");
        using var portal = await PortalAsync(role, subject);
        var page = await AttendanceHttp.Get(portal.Client, $"/api/attendance?from={AttendanceHttp.Date}&to={AttendanceHttp.Date}&studentId={own.Students[0]}");
        Assert.All(page.GetProperty("items").EnumerateArray(), row => Assert.Equal(own.Students[0], row.GetProperty("studentId").GetGuid()));
        Assert.NotEmpty(page.GetProperty("items").EnumerateArray());
        var narrowed = await AttendanceHttp.Get(portal.Client, $"/api/attendance?from={AttendanceHttp.Date}&to={AttendanceHttp.Date}&groupId={foreign.GroupId}");
        Assert.Empty(narrowed.GetProperty("items").EnumerateArray());
    }
    private async Task<SignedInClient> PortalAsync(string role, Guid? selectedSubject = null)
    {
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
        var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
        var subject = await factory.ScalarAsync<Guid>($"select id from academics.{(role == "parent" ? "parents" : "students")} where centre_id='{centre}' order by id limit 1");
        subject = selectedSubject ?? subject;
        await factory.ExecuteAsync($"""
            update identity.memberships set role='{role}' where centre_id='{centre}' and user_id='{user}';
            insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
            values ('{Guid.NewGuid()}','{centre}','{user}','{role}',{(role == "parent" ? $"'{subject}'" : "null")},{(role == "student" ? $"'{subject}'" : "null")},now(),'{owner}');
            """);
        return await SignInHelper.SignInAsync(factory, "secretary@nile.test");
    }

}
