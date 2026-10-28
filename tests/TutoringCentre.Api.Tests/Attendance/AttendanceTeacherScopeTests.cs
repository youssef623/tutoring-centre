using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class AttendanceTeacherScopeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(BeforeSession);
    }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }

    [Theory]
    [InlineData("list")]
    [InlineData("own")]
    [InlineData("otherRead")]
    [InlineData("otherWrite")]
    [InlineData("history")]
    [InlineData("correct")]
    public async Task LinkedTeacher_UsesSessionOwnership(string operation)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var own = await Arrange(owner.Client);
        var other = await Arrange(owner.Client, false);
        factory.Time.FixAt(InSession);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        if (operation == "list")
        {
            var sessions = await Get(teacher.Client, "/api/attendance/sessions");
            Assert.Equal(own.SessionId, Assert.Single(sessions.GetProperty("items").EnumerateArray()).GetProperty("sessionId").GetGuid());
        }
        else if (operation == "own")
        {
            using var mark = await Put(teacher.Client, $"/api/sessions/{own.SessionId}/attendance", Marks(own));
            Assert.Equal(HttpStatusCode.OK, mark.StatusCode);
            Assert.Equal(3, (await Get(teacher.Client, $"/api/sessions/{own.SessionId}/attendance")).GetProperty("items").GetArrayLength());
        }
        else if (operation == "otherRead")
        {
            using var response = await teacher.Client.GetAsync(new Uri($"/api/sessions/{other.SessionId}/attendance", UriKind.Relative));
            await Problem(response, HttpStatusCode.NotFound, "attendance.session_not_found");
        }
        else if (operation == "otherWrite")
        {
            using var response = await Put(teacher.Client, $"/api/sessions/{other.SessionId}/attendance", Marks(other));
            await Problem(response, HttpStatusCode.NotFound, "attendance.session_not_found");
        }
        else
        {
            using var mark = await Put(owner.Client, $"/api/sessions/{own.SessionId}/attendance", Marks(own));
            Assert.Equal(HttpStatusCode.OK, mark.StatusCode);
            var record = await Record(owner.Client, own);
            if (operation == "correct")
            {
                using var response = await Post(teacher.Client, $"/api/attendance/{record.GetProperty("recordId").GetGuid()}/correct",
                    new { status = "absent", reason = "Teacher correction refused", version = record.GetProperty("version").GetUInt32() });
                await Problem(response, HttpStatusCode.Forbidden, "auth.permission_denied");
            }
            else
            {
                using var otherMarks = await Put(owner.Client, $"/api/sessions/{other.SessionId}/attendance", Marks(other));
                Assert.Equal(HttpStatusCode.OK, otherMarks.StatusCode);
                var page = await Get(teacher.Client, $"/api/attendance?from={Date}&to={Date}");
                Assert.Equal(2, page.GetProperty("items").GetArrayLength());
                Assert.All(page.GetProperty("items").EnumerateArray(), row => Assert.Equal(own.SessionId, row.GetProperty("sessionId").GetGuid()));
                var empty = await Get(teacher.Client, $"/api/attendance?from={Date}&to={Date}&groupId={other.GroupId}");
                Assert.Empty(empty.GetProperty("items").EnumerateArray());
                var student = await Get(teacher.Client, $"/api/attendance?from={Date}&to={Date}&studentId={other.Students[0]}");
                Assert.Empty(student.GetProperty("items").EnumerateArray());
            }
        }
    }

    [Fact]
    public async Task History_ExcludesSubstituteSessionInsideOwnedGroup()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var own = await Arrange(owner.Client);
        var otherTeacher = await Create(owner.Client, "/api/teachers", new { fullName = "Attendance Substitute", phone = (string?)null });
        var group = await Get(owner.Client, $"/api/groups/{own.GroupId}");
        var regularTeacher = group.GetProperty("teacherId").GetGuid();
        using var reassigned = await Put(owner.Client, $"/api/groups/{own.GroupId}/teacher",
            new { teacherId = otherTeacher, version = group.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, reassigned.StatusCode);
        factory.Time.FixAt(InSession);
        group = await Get(owner.Client, $"/api/groups/{own.GroupId}");
        using var restored = await Put(owner.Client, $"/api/groups/{own.GroupId}/teacher",
            new { teacherId = regularTeacher, version = group.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, restored.StatusCode);
        using var marked = await Put(owner.Client, $"/api/sessions/{own.SessionId}/attendance", Marks(own));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        var page = await Get(teacher.Client, $"/api/attendance?from={Date}&to={Date}&groupId={own.GroupId}");
        Assert.Empty(page.GetProperty("items").EnumerateArray());
        Assert.Equal(2, (await Get(owner.Client, $"/api/attendance?from={Date}&to={Date}&groupId={own.GroupId}")).GetProperty("items").GetArrayLength());
    }
}
