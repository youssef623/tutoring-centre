using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class ScanRaceHttpTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Fact]
    public async Task FiveClientsTwoUsers_RetryOnce_OneRecordedOneRow_FiveSessions()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var clients = new List<SignedInClient>();
        try
        {
            for (var index = 0; index < 5; index++) clients.Add(index % 2 == 0
                ? await SignInHelper.SignInAsNileOwnerAsync(Factory)
                : await SignInHelper.SignInAsNileSecretaryAsync(Factory));
            for (var race = 0; race < 5; race++)
            {
                Factory.Time.FixAt(BeforeSession);
                var scenario = await Arrange(owner.Client, startTime: $"{18 + race:00}:00");
                var issued = await Issue(owner.Client, scenario.Students[0]);
                Factory.Time.FixAt(InSession.AddHours(race));
                Factory.AttendanceRace.Enable(5);
                try
                {
                    async Task<string> ScanWithRetry(SignedInClient client)
                    {
                        using var first = await Scan(client.Client, scenario.SessionId, issued.GetProperty("token").GetString()!);
                        if (first.StatusCode == HttpStatusCode.Conflict)
                        {
                            Assert.Equal("attendance.concurrent_mark", (await Json(first)).GetProperty("code").GetString());
                            using var retry = await Scan(client.Client, scenario.SessionId, issued.GetProperty("token").GetString()!);
                            Assert.Equal(HttpStatusCode.OK, retry.StatusCode);
                            return (await Json(retry)).GetProperty("outcome").GetString()!;
                        }
                        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
                        return (await Json(first)).GetProperty("outcome").GetString()!;
                    }
                    var outcomes = await Task.WhenAll(clients.Select(ScanWithRetry));
                    Assert.Equal(1L, await Factory.ScalarAsync<long>($"select count(*) from attendance.attendance_records where session_id = '{scenario.SessionId}'"));
                    Assert.Single(outcomes, outcome => outcome == "recorded");
                    Assert.Equal(4, outcomes.Count(outcome => outcome == "alreadyRecorded"));
                }
                finally { Factory.AttendanceRace.Disable(); }
            }
        }
        finally { foreach (var client in clients) client.Dispose(); }
    }
}
