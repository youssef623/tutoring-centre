using System.Net;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class AttendanceCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(BeforeSession);
    }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }

    [Theory]
    [InlineData("nile", "roster")]
    [InlineData("maadi", "roster")]
    [InlineData("nile", "mark")]
    [InlineData("maadi", "mark")]
    [InlineData("nile", "correct")]
    [InlineData("maadi", "correct")]
    [InlineData("nile", "student")]
    [InlineData("maadi", "student")]
    [InlineData("nile", "group")]
    [InlineData("maadi", "group")]
    public async Task OtherCentre_IsInvisibleAndUnchanged(string victimCentre, string operation)
    {
        using var nile = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var maadi = await SignInHelper.SignInAsMaadiOwnerAsync(factory);
        var victim = victimCentre == "nile" ? nile.Client : maadi.Client;
        var attacker = victimCentre == "nile" ? maadi.Client : nile.Client;
        var scenario = await Arrange(victim);
        factory.Time.FixAt(InSession);
        using var marked = await Put(victim, $"/api/sessions/{scenario.SessionId}/attendance", Marks(scenario));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        var record = await Record(victim, scenario);
        var id = record.GetProperty("recordId").GetGuid();
        Task<string> Snapshot() => factory.ScalarAsync<string>($"select status || '|' || correction_count || '|' || xmin::text from attendance.attendance_records where id = '{id}'");
        if (operation is "student" or "group")
        {
            var before = await Snapshot();
            var filter = operation == "student" ? $"studentId={scenario.Students[0]}" : $"groupId={scenario.GroupId}";
            var page = await Get(attacker, $"/api/attendance?from={Date}&to={Date}&{filter}");
            Assert.Empty(page.GetProperty("items").EnumerateArray());
            Assert.Equal(before, await Snapshot());
            return;
        }
        var method = operation == "roster" ? HttpMethod.Get : operation == "mark" ? HttpMethod.Put : HttpMethod.Post;
        var url = operation == "correct" ? $"/api/attendance/{id}/correct" : $"/api/sessions/{scenario.SessionId}/attendance";
        object? body = operation == "roster" ? null : operation == "mark" ? Marks(scenario, "absent")
            : new { status = "absent", reason = "Valid correction reason", version = record.GetProperty("version").GetUInt32() };
        await CrossTenantProbe.RunAsync(attacker, method, url, body, HttpStatusCode.NotFound,
            operation == "correct" ? "attendance.not_found" : "attendance.session_not_found", Snapshot);
    }
}
