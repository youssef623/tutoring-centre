using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class CardCrossTenantTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Theory]
    [InlineData("nile", "status")]
    [InlineData("maadi", "status")]
    [InlineData("nile", "issue")]
    [InlineData("maadi", "issue")]
    [InlineData("nile", "revoke")]
    [InlineData("maadi", "revoke")]
    [InlineData("nile", "bulk")]
    [InlineData("maadi", "bulk")]
    public async Task ForeignIds_NotFoundAndRowsUnchanged(string centre, string operation)
    {
        using var nile = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        using var maadi = await SignInHelper.SignInAsMaadiOwnerAsync(Factory);
        var victim = centre == "nile" ? nile.Client : maadi.Client;
        var attacker = centre == "nile" ? maadi.Client : nile.Client;
        var scenario = await Arrange(victim);
        var card = await Issue(victim, scenario.Students[0]);
        var status = (await Get(victim, $"/api/students/{scenario.Students[0]}/card")).GetProperty("card");
        var url = operation == "revoke" ? $"/api/student-cards/{card.GetProperty("cardId").GetGuid()}/revoke"
            : operation == "bulk" ? $"/api/groups/{scenario.GroupId}/cards" : $"/api/students/{scenario.Students[0]}/card";
        object? body = operation == "revoke" ? new { reason = "lost", version = status.GetProperty("version").GetUInt32() }
            : operation == "bulk" ? new { studentIds = scenario.Students } : null;
        await CrossTenantProbe.RunAsync(attacker, operation == "status" ? HttpMethod.Get : HttpMethod.Post,
            url, body, HttpStatusCode.NotFound, operation == "revoke" ? "card.not_found" : operation == "bulk" ? "group.not_found" : "student.not_found",
            () => Factory.ScalarAsync<string>("select string_agg(id::text || status || xmin::text, ',' order by id) from attendance.student_cards"));
    }
}
