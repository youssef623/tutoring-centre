using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.IssueStudentCard;
using TutoringCentre.Application.Attendance.Commands.IssueGroupCards;
using TutoringCentre.Application.Attendance.Commands.RevokeStudentCard;
using TutoringCentre.Application.Attendance.Queries.GetStudentCard;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class StudentCardUseCaseTests(PostgresFixture fixture) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task IssueAndReissue_StoreOnlyHash_KeepHistoryAndAuditWithoutSecrets()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-issue");
        var first = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId));
        Assert.True(first.IsSuccess, first.Error?.Code);
        var expected = Convert.ToHexString(CardToken.Parse(first.Value.Token).Value.Hash()).ToLowerInvariant();
        Assert.Equal(expected, await Fixture.ScalarAsync<string>($"select encode(token_hash, 'hex') from attendance.student_cards where id = '{first.Value.CardId}'"));
        Assert.Equal(32, await Fixture.ScalarAsync<int>("select octet_length(token_hash) from attendance.student_cards limit 1"));
        await using (var scope = Clocked.CreateAsyncScope())
        {
            scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(scenario.Owner);
            var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unit.BeginAsync(true, CancellationToken.None);
            var found = await scope.ServiceProvider.GetRequiredService<IStudentCardRepository>()
                .FindByTokenHashAsync(CardToken.Parse(first.Value.Token).Value.Hash(), CancellationToken.None);
            Assert.Equal(first.Value.CardId, found!.Id);
            await unit.CommitAsync(CancellationToken.None);
        }
        Assert.Equal(0, await Fixture.ScalarAsync<long>("""
            select count(*) from information_schema.columns
            where table_schema = 'attendance' and table_name = 'student_cards'
                and data_type in ('text', 'character varying') and column_name not in ('status', 'revocation_reason')
            """));
        Time.Advance(TimeSpan.FromMinutes(1));
        var second = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId));
        Assert.True(second.IsSuccess, second.Error?.Code);
        Assert.Equal("reissued", await Fixture.ScalarAsync<string>($"select revocation_reason from attendance.student_cards where id = '{first.Value.CardId}'"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards where status = 'active'"));
        var status = await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(scenario.Owner, new(scenario.StudentId));
        Assert.Equal(second.Value.CardId, status.Value!.CardId);
        Assert.Null(status.Value.RevocationReason);
        Assert.Null(status.Value.RevokedAt);
        var revoked = await Clocked.SendAsAsync<RevokeStudentCardCommand, Unit>(scenario.Owner, new(second.Value.CardId, CardRevocationReason.Lost, status.Value.Version));
        Assert.True(revoked.IsSuccess, revoked.Error?.Code);
        var after = (await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(scenario.Owner, new(scenario.StudentId))).Value!;
        Assert.Equal("revoked", after.Status);
        Assert.Equal("lost", after.RevocationReason);
        Assert.Equal(Time.GetUtcNow(), after.RevokedAt);
        Assert.False(string.IsNullOrWhiteSpace(after.IssuedByName));
        var audit = await Fixture.ScalarAsync<string>("select json_agg(changes)::text from audit.audit_entries where entity_type = 'student_card'");
        Assert.DoesNotContain("hash", audit, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", audit, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(first.Value.Token, audit);
        Assert.Equal(4, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'student_card'"));
        var json = JsonSerializer.Serialize(status.Value);
        Assert.DoesNotContain("hash", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ForeignRevokeAndLookup_AreNotFound_AndRowUnchanged()
    {
        var nile = await ArrangeAttendanceAsync(NileCentreId, "card-nile");
        var maadi = await ArrangeAttendanceAsync(MaadiCentreId, "card-maadi");
        var issued = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(nile.Owner, new(nile.StudentId));
        Assert.True(issued.IsSuccess, issued.Error?.Code);
        var status = await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(nile.Owner, new(nile.StudentId));
        var denied = await Clocked.SendAsAsync<RevokeStudentCardCommand, Unit>(maadi.Owner, new(issued.Value.CardId, CardRevocationReason.Lost, status.Value!.Version));
        Assert.Equal("card.not_found", denied.Error!.Code);
        Assert.Equal("student.not_found", (await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(maadi.Owner, new(nile.StudentId))).Error!.Code);
        Assert.Equal("active", await Fixture.ScalarAsync<string>("select status from attendance.student_cards limit 1"));
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(maadi.Owner);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(true, CancellationToken.None);
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IStudentCardRepository>()
            .FindByTokenHashAsync(CardToken.Parse(issued.Value.Token).Value.Hash(), CancellationToken.None));
        await unit.CommitAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StaleRevoke_RollsBack_AndRepeatedRevokeReturnsAlreadyRevoked()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-stale");
        var issued = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId));
        var status = (await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(scenario.Owner, new(scenario.StudentId))).Value!;
        var stale = await Clocked.SendAsAsync<RevokeStudentCardCommand, Unit>(scenario.Owner, new(issued.Value.CardId, CardRevocationReason.Damaged, status.Version + 1));
        Assert.Equal("concurrency.stale", stale.Error!.Code);
        Assert.Equal("active", (await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(scenario.Owner, new(scenario.StudentId))).Value!.Status);
        Assert.True((await Clocked.SendAsAsync<RevokeStudentCardCommand, Unit>(scenario.Owner, new(issued.Value.CardId, CardRevocationReason.Damaged, status.Version))).IsSuccess);
        var repeated = await Clocked.SendAsAsync<RevokeStudentCardCommand, Unit>(scenario.Owner, new(issued.Value.CardId, CardRevocationReason.Lost, status.Version));
        Assert.Equal("card.already_revoked", repeated.Error!.Code);
    }

    [Fact]
    public async Task BulkIssueAndReissue_OneActiveEach_OutsiderRollsBackAll()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-bulk");
        var other = await Fixture.SeedStudentAsync(NileCentreId, "Second Card Student");
        Assert.True((await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(scenario.Owner, new(scenario.GroupId, other))).IsSuccess);
        var first = await Clocked.SendAsAsync<IssueGroupCardsCommand, IssuedGroupCards>(scenario.Owner, new(scenario.GroupId, [scenario.StudentId, other]));
        Assert.True(first.IsSuccess, first.Error?.Code);
        var failed = await Clocked.SendAsAsync<IssueGroupCardsCommand, IssuedGroupCards>(scenario.Owner, new(scenario.GroupId, [scenario.StudentId, Guid.NewGuid()]));
        Assert.Equal("card.student_not_in_group", failed.Error!.Code);
        Assert.Contains("studentIds[1]", failed.Error.Fields!.Keys);
        Assert.Equal(2, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards"));
        var second = await Clocked.SendAsAsync<IssueGroupCardsCommand, IssuedGroupCards>(scenario.Owner, new(scenario.GroupId, [scenario.StudentId, other]));
        Assert.True(second.IsSuccess, second.Error?.Code);
        Assert.Equal(4, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards"));
        Assert.Equal(2, await Fixture.ScalarAsync<long>("select count(*) from attendance.student_cards where status = 'active'"));
    }

    [Fact]
    public async Task NoCard_IsSuccessfulNull_ArchivedStudentCannotIssue()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "card-empty");
        var status = await Clocked.QueryAsAsync<GetStudentCardQuery, StudentCardDto?>(scenario.Owner, new(scenario.StudentId));
        Assert.True(status.IsSuccess);
        Assert.Null(status.Value);
        await using var connection = new Npgsql.NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new Npgsql.NpgsqlCommand($"update academics.students set status = 'archived' where id = '{scenario.StudentId}'", connection);
        await command.ExecuteNonQueryAsync();
        var issued = await Clocked.SendAsAsync<IssueStudentCardCommand, IssuedCard>(scenario.Owner, new(scenario.StudentId));
        Assert.Equal("card.student_archived", issued.Error!.Code);
    }
}
