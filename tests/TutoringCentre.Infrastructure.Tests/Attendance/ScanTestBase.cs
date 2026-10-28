using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.IssueStudentCard;
using TutoringCentre.Application.Attendance.Commands.RecordScan;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public abstract class ScanTestBase(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    private protected async Task<IssuedCard> IssueAsync(Scenario scenario)
    {
        var result = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId));
        Assert.True(result.IsSuccess, result.Error?.Code);
        return result.Value;
    }

    private protected Task<Result<ScanResult>> ScanAsync(Scenario scenario, string token, Actor? actor = null) =>
        Clocked.SendAsAsync<RecordScanCommand, ScanResult>(actor ?? scenario.Owner, new(scenario.SessionId, token));
}
