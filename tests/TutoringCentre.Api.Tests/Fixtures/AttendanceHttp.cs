using System.Net;
using System.Text.Json;

namespace TutoringCentre.Api.Tests.Fixtures;

internal static class AttendanceHttp
{
    public const string Date = "2027-04-20";
    public static readonly DateTimeOffset BeforeSession = new(2027, 4, 20, 8, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset InSession = new(2027, 4, 20, 16, 0, 0, TimeSpan.Zero);
    public static Task<HttpResponseMessage> Post(HttpClient client, string url, object body) =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, new(url, UriKind.Relative), body);
    public static Task<HttpResponseMessage> Put(HttpClient client, string url, object body) =>
        AntiforgeryTestHelper.SendAsJsonAsync(client, HttpMethod.Put, new(url, UriKind.Relative), body);
    public static async Task<JsonElement> Json(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
    public static async Task<JsonElement> Get(HttpClient client, string url)
    {
        using var response = await client.GetAsync(new Uri(url, UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await Json(response);
    }
    public static async Task<Guid> Create(HttpClient client, string url, object body)
    {
        using var response = await Post(client, url, body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await Json(response)).GetProperty("id").GetGuid();
    }
    public static async Task Problem(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(code, (await Json(response)).GetProperty("code").GetString());
    }
    public static async Task<Scenario> Arrange(HttpClient owner, bool own = true, string startTime = "18:00")
    {
        var staff = (await Get(owner, "/api/staff")).GetProperty("items").EnumerateArray()
            .Single(row => row.GetProperty("email").GetString() == "teacher@both.test").GetProperty("userId").GetGuid();
        var teachers = (await Get(owner, "/api/teachers")).GetProperty("items").EnumerateArray();
        var teacher = teachers.First(row => own
            ? row.GetProperty("userId").ValueKind == JsonValueKind.String && row.GetProperty("userId").GetGuid() == staff
            : row.GetProperty("userId").ValueKind == JsonValueKind.Null).GetProperty("id").GetGuid();
        var subject = (await Get(owner, "/api/subjects")).GetProperty("items")[0].GetProperty("id").GetGuid();
        var group = await Create(owner, "/api/groups", new { name = $"Attendance {Guid.NewGuid():N}", subjectId = subject, teacherId = teacher, capacity = 20 });
        var students = (await Get(owner, "/api/students?pageSize=100")).GetProperty("items").EnumerateArray()
            .Skip(own ? 0 : 3).Take(3).Select(row => row.GetProperty("id").GetGuid()).ToArray();
        var enrollments = new List<Guid>();
        foreach (var studentId in students) { enrollments.Add(await Create(owner, $"/api/groups/{group}/enrollments", new { studentId })); }
        await Create(owner, $"/api/groups/{group}/schedule-rules", new
        {
            dayOfWeek = "tuesday", startTime, durationMinutes = 60, effectiveFrom = Date,
        });
        var session = Assert.Single((await Get(owner, $"/api/timetable?weekStart={Date}&groupId={group}")).GetProperty("sessions").EnumerateArray());
        return new(group, session.GetProperty("id").GetGuid(), students, enrollments.ToArray());
    }
    public static object Marks(Scenario scenario, string status = "present") =>
        new { marks = scenario.Students.Take(2).Select(studentId => new { studentId, status }).ToArray() };
    public static async Task<JsonElement> Record(HttpClient client, Scenario scenario) =>
        (await Get(client, $"/api/sessions/{scenario.SessionId}/attendance")).GetProperty("items").EnumerateArray()
            .First(row => row.GetProperty("record").ValueKind == JsonValueKind.Object).GetProperty("record");
    internal sealed record Scenario(Guid GroupId, Guid SessionId, Guid[] Students, Guid[] Enrollments);
}
