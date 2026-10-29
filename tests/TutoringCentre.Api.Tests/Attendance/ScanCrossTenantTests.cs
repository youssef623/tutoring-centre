using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class ScanCrossTenantTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Theory]
    [InlineData("nile")]
    [InlineData("maadi")]
    public async Task ForeignTokenInOwnOpenSession_UniformUnknown_NoRows(string centre)
    {
        using var nile = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var maadi = await SignInHelper.SignInAsMaadiOwnerAsync(Factory);
        var victim = centre == "nile" ? nile.Client : maadi.Client;
        var attacker = centre == "nile" ? maadi.Client : nile.Client;
        var victimSession = await Arrange(victim);
        var attackerSession = await Arrange(attacker);
        var card = await Issue(victim, victimSession.Students[0]);
        Factory.Time.FixAt(InSession);
        await CrossTenantProbe.RunAsync(attacker, HttpMethod.Post, $"/api/sessions/{attackerSession.SessionId}/scans",
            new { token = card.GetProperty("token").GetString() }, HttpStatusCode.NotFound, "card.not_recognised",
            async () => (await Factory.ScalarAsync<long>("select count(*) from attendance.attendance_records")).ToString(System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(0L, await Factory.ScalarAsync<long>("select count(*) from attendance.attendance_records"));
    }
}
