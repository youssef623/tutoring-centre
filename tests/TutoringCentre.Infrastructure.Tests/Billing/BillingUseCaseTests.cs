using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Billing;
using TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
using TutoringCentre.Application.Billing.Queries.GetStudentCharges;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Jobs;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Academics;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Billing;

public sealed class BillingUseCaseTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private static readonly BillingMonth Month = BillingMonth.Of(new(2027, 3, 1));
    private async Task<(Guid Group, Guid Student, Guid Enrollment, Guid Subject, Guid Teacher)> ArrangeAsync(decimal fee = 300)
    {
        var world = await ArrangeWorldAsync(NileCentreId, "billing");
        var source = await GetGroupAsync(world.Owner.Actor, world.G1);
        var group = await Clocked.SendAsAsync<CreateGroupCommand, CreateGroupResult>(new SystemActor(NileCentreId), new("Fee group", source.SubjectId, world.T1, 20, fee));
        Assert.True(group.IsSuccess, group.Error?.Code);
        var student = (await SeedStudentsAsync(NileCentreId, 1, "EVAL Billing"))[0];
        var enrollment = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(new SystemActor(NileCentreId), new(group.Value.GroupId, student));
        Assert.True(enrollment.IsSuccess, enrollment.Error?.Code);
        return (group.Value.GroupId, student, enrollment.Value.EnrollmentId, source.SubjectId, world.T1);
    }
    private Task<TutoringCentre.Domain.Common.Result<CreateMonthlyChargesResult>> Generate() => Clocked.SendAsAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new SystemActor(NileCentreId), new());
    private Task<long> Count() => Fixture.ScalarAsync<long>("select count(*) from billing.charges");
    private async Task ExecuteSql(string sql)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString); await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection); await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task MonthlyBatchRepeatsWithoutChangingSnapshots()
    {
        var world = await ArrangeAsync();
        var first = await Generate(); Assert.True(first.IsSuccess, first.Error?.Code); Assert.Equal(new(1, 0), first.Value);
        var repeat = await Generate(); Assert.Equal(new(0, 1), repeat.Value);
        var dto = await GetGroupAsync(new SystemActor(NileCentreId), world.Group);
        Assert.True((await Clocked.SendAsAsync<RenameGroupCommand, TutoringCentre.Application.Common.Cqrs.Unit>(new SystemActor(NileCentreId), new(world.Group, dto.Name, dto.Version, 450))).IsSuccess);
        await Generate(); Assert.Equal(300m, await Fixture.ScalarAsync<decimal>("select amount from billing.charges"));
        Assert.Equal(1, await Count());
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type='charge' and action='created'"));
    }
    [Fact]
    public async Task TwoBatchesRaceSafely()
    {
        await ArrangeAsync(); var results = await Task.WhenAll(Generate(), Generate());
        Assert.All(results, r => Assert.True(r.IsSuccess, r.Error?.Code)); Assert.Equal(1, results.Sum(x => x.Value.Created)); Assert.Equal(1, await Count());
    }
    [Fact]
    public async Task TwentyConcurrentMonthlyInsertsLeaveExactlyOneRow()
    {
        var w = await ArrangeAsync();
        var inserted = await Task.WhenAll(Enumerable.Range(0, 20).Select(async _ =>
        {
            await using var scope = Clocked.CreateAsyncScope(); var services = scope.ServiceProvider;
            services.GetRequiredService<CurrentActorContext>().Set(new SystemActor(NileCentreId));
            var uow = services.GetRequiredService<IUnitOfWork>(); await uow.BeginAsync(false, CancellationToken.None);
            var charge = Charge.CreateMonthly(NileCentreId, w.Student, w.Enrollment, w.Group, w.Subject, w.Teacher, Month, Money.Create(300).Value).Value;
            var added = await services.GetRequiredService<IChargeRepository>().TryAddMonthlyAsync(charge, CancellationToken.None);
            Assert.True((await uow.SaveChangesAsync(CancellationToken.None)).IsSuccess); await uow.CommitAsync(CancellationToken.None); return added;
        }));
        Assert.Equal(1, inserted.Count(x => x)); Assert.Equal(1, await Count());
    }
    [Fact]
    public async Task FeeEditDoesNotRepriceAnUnchargedCurrentEnrollment()
    {
        var w = await ArrangeAsync(); var group = await GetGroupAsync(new SystemActor(NileCentreId), w.Group);
        await Clocked.SendAsAsync<RenameGroupCommand, TutoringCentre.Application.Common.Cqrs.Unit>(new SystemActor(NileCentreId), new(w.Group, group.Name, group.Version, 450));
        await Generate(); Assert.Equal(300m, await Fixture.ScalarAsync<decimal>("select amount from billing.charges"));
        var next = await Clocked.SendAsAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new SystemActor(NileCentreId), new(Month.Next()));
        Assert.True(next.IsSuccess); Assert.Equal(450m, await Fixture.ScalarAsync<decimal>("select amount from billing.charges order by billing_month desc limit 1"));
    }
    [Fact]
    public async Task EnrollmentRelayTwiceCreatesOneCurrentMonthCharge()
    {
        await ArrangeAsync(); var relay = Clocked.GetRequiredService<OutboxRelayService>();
        await relay.ProcessBatchAsync(); await relay.ProcessBatchAsync(); Assert.Equal(1, await Count());
    }
    [Fact]
    public async Task ZeroFeeHasNoChargeableFactOrCharge()
    {
        await ArrangeAsync(0); var result = await Generate(); Assert.Equal(new(0, 0), result.Value);
        await Clocked.GetRequiredService<OutboxRelayService>().ProcessBatchAsync(); Assert.Equal(0, await Count());
    }
    [Theory]
    [InlineData(StaffRole.Owner)][InlineData(StaffRole.Secretary)][InlineData(StaffRole.Teacher)][InlineData(StaffRole.Parent)][InlineData(StaffRole.Student)]
    public async Task GenerationIsDeniedToEveryNormalRole(StaffRole role)
    {
        var result = await Clocked.SendAsAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new StaffActor(Guid.NewGuid(), NileCentreId, role), new());
        Assert.True(result.IsFailure); Assert.Equal("auth.permission_denied", result.Error!.Code);
    }
    [Fact]
    public async Task ChargeReadIsExactAndForeignStudentIsUniformlyNotFound()
    {
        var w = await ArrangeAsync(); await Generate();
        var page = await Clocked.QueryAsAsync<GetStudentChargesQuery, StudentChargesPage>(new SystemActor(NileCentreId), new(w.Student, null, 1));
        var row = Assert.Single(page.Value.Items); Assert.Equal("300.00", row.Amount); Assert.Equal("2027-03", row.BillingMonth); Assert.Equal(new DateOnly(2027, 3, 5), row.DueDate);
        var foreign = await Clocked.QueryAsAsync<GetStudentChargesQuery, StudentChargesPage>(new SystemActor(MaadiCentreId), new(w.Student));
        Assert.Equal("student.not_found", foreign.Error!.Code);
    }
    [Fact]
    public async Task CairoFirstDayIsPreviousUtcMonth()
    {
        await ArrangeAsync(); Time.SetUtcNow(new DateTimeOffset(2027, 3, 31, 22, 30, 0, TimeSpan.Zero));
        await new CreateCentreChargesJob(Clocked.GetRequiredService<ISystemScopeRunner>()).ExecuteAsync(NileCentreId, null, CancellationToken.None);
        Assert.Equal(new DateOnly(2027, 4, 1), await Fixture.ScalarAsync<DateOnly>("select billing_month from billing.charges"));
    }
    [Fact]
    public async Task RuntimeRoleCannotUpdateOrDeleteAndWithoutTenantSeesNothing()
    {
        await ArrangeAsync(); await Generate();
        Assert.Equal(0, await PostgresFixture.ScalarAsync<long>(Fixture.AppConnectionString, "select count(*) from billing.charges"));
        foreach (var sql in new[] { "update billing.charges set amount=1", "delete from billing.charges" })
        {
            await using var c = new NpgsqlConnection(Fixture.AppConnectionString); await c.OpenAsync();
            await using var command = new NpgsqlCommand(sql, c); var error = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync()); Assert.Equal("42501", error.SqlState);
        }
    }
    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task SingleAndListRosterUseTheSameLatestEnrollmentIncludingEqualStartTies(bool equalStart)
    {
        var w = await ArrangeAsync(); var other = await Clocked.SendAsAsync<CreateGroupCommand, CreateGroupResult>(new SystemActor(NileCentreId), new("Transfer group", w.Subject, w.Teacher, 20, 400));
        if (!equalStart) Time.Advance(TimeSpan.FromMinutes(1));
        var newer = await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(new SystemActor(NileCentreId), new(other.Value.GroupId, w.Student));
        await using var scope = Clocked.CreateAsyncScope(); scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(NileCentreId));
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>(); await uow.BeginAsync(true, CancellationToken.None);
        var winner = !equalStart || newer.Value.EnrollmentId.CompareTo(w.Enrollment) > 0 ? newer.Value.EnrollmentId : w.Enrollment;
        var loser = winner == w.Enrollment ? newer.Value.EnrollmentId : w.Enrollment;
        var roster = scope.ServiceProvider.GetRequiredService<IBillingRosterSource>(); Assert.Null(await roster.GetChargeableAsync(loser, Month, CancellationToken.None));
        Assert.Equal(winner, (await roster.ListChargeableAsync(Month, CancellationToken.None)).Single().EnrollmentId);
        await uow.CommitAsync(CancellationToken.None);
    }

    [Theory]
    [InlineData("student")][InlineData("group")][InlineData("ended")]
    public async Task InactiveOrNonoverlappingEnrollmentIsExcluded(string excluded)
    {
        var w = await ArrangeAsync();
        await ExecuteSql(excluded switch
        {
            "student" => $"update academics.students set status='archived' where id='{w.Student}'",
            "group" => $"update academics.groups set status='archived' where id='{w.Group}'",
            _ => $"update academics.enrollments set enrolled_at='2027-02-01Z', ended_at='2027-03-01 00:00:00+02', status='ended', end_reason='withdrawn' where id='{w.Enrollment}'"
        });
        Assert.Equal(0, (await Generate()).Value.Created); Assert.Equal(0, await Count());
    }
    [Fact]
    public async Task MidMonthStartIsIncludedAndForeignCentreIsNotBilled()
    {
        Time.Advance(TimeSpan.FromDays(14)); await ArrangeAsync(); Assert.Equal(1, (await Generate()).Value.Created);
        Assert.Equal(0, (await Clocked.SendAsAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new SystemActor(MaadiCentreId), new())).Value.Created);
    }
    [Theory]
    [InlineData("student")][InlineData("group")]
    public async Task CompositeForeignKeysRejectWrongStudentOrForeignGroup(string wrong)
    {
        var w = await ArrangeAsync();
        var student = (await SeedStudentsAsync(NileCentreId, 1, "EVAL Other"))[0];
        var subject = await Fixture.SeedSubjectAsync(MaadiCentreId, "EVAL Foreign"); var teacher = await Fixture.SeedTeacherAsync(MaadiCentreId, "EVAL Foreign");
        var foreignGroup = await Fixture.SeedGroupAsync(MaadiCentreId, "EVAL Foreign", subject, teacher);
        await using var scope = Clocked.CreateAsyncScope(); var services = scope.ServiceProvider; services.GetRequiredService<CurrentActorContext>().Set(new SystemActor(NileCentreId));
        var uow = services.GetRequiredService<IUnitOfWork>(); await uow.BeginAsync(false, CancellationToken.None);
        var charge = Charge.CreateMonthly(NileCentreId, wrong == "student" ? student : w.Student, w.Enrollment, wrong == "group" ? foreignGroup : w.Group, w.Subject, w.Teacher, Month, Money.Create(300).Value).Value;
        var error = await Assert.ThrowsAsync<PostgresException>(() => services.GetRequiredService<IChargeRepository>().TryAddMonthlyAsync(charge, CancellationToken.None));
        Assert.Equal("23503", error.SqlState); await uow.RollbackAsync(CancellationToken.None);
    }
    [Fact]
    public async Task UnrelatedInsertionFailureRollsBackWholeBatch()
    {
        var w = await ArrangeAsync(); var student = (await SeedStudentsAsync(NileCentreId, 1, "EVAL Second"))[0];
        await Clocked.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(new SystemActor(NileCentreId), new(w.Group, student));
        await ExecuteSql("""
            create function billing.fail_second_probe() returns trigger language plpgsql as $$
            begin if (select count(*) from billing.charges) > 0 then raise exception 'probe failure'; end if; return new; end $$;
            create trigger fail_second_probe before insert on billing.charges for each row execute function billing.fail_second_probe();
            """);
        try { await Assert.ThrowsAsync<PostgresException>(async () => await Generate()); Assert.Equal(0, await Count()); }
        finally { await ExecuteSql("drop trigger fail_second_probe on billing.charges; drop function billing.fail_second_probe();"); }
    }
    [Fact]
    public async Task PagingHasNoMissingOrDuplicatedObligations()
    {
        var w = await ArrangeAsync(); await Generate();
        await Clocked.SendAsAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new SystemActor(NileCentreId), new(Month.Next()));
        var first = (await Clocked.QueryAsAsync<GetStudentChargesQuery, StudentChargesPage>(new SystemActor(NileCentreId), new(w.Student, null, 1))).Value;
        var second = (await Clocked.QueryAsAsync<GetStudentChargesQuery, StudentChargesPage>(new SystemActor(NileCentreId), new(w.Student, first.NextCursor, 1))).Value;
        Assert.Equal("2027-04", Assert.Single(first.Items).BillingMonth); Assert.Equal("2027-03", Assert.Single(second.Items).BillingMonth);
        Assert.NotEqual(first.Items[0].Id, second.Items[0].Id); Assert.Null(second.NextCursor);
    }
    [Fact]
    public async Task ForeignTenantRawSqlCannotSeeNileCharge()
    {
        await ArrangeAsync(); await Generate(); await using var connection = new NpgsqlConnection(Fixture.AppConnectionString); await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();
        await using (var set = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, tx)) { set.Parameters.AddWithValue("centre", MaadiCentreId.ToString()); await set.ExecuteNonQueryAsync(); }
        await using var read = new NpgsqlCommand("select count(*) from billing.charges", connection, tx); Assert.Equal(0L, (long)(await read.ExecuteScalarAsync())!); await tx.RollbackAsync();
    }
    [Fact]
    public async Task TransferAndRepeatedOutboxDeliveryDoNotCreateAnotherObligation()
    {
        var w = await ArrangeAsync(); await Generate();
        var target = await Clocked.SendAsAsync<CreateGroupCommand, CreateGroupResult>(new SystemActor(NileCentreId), new("EVAL Transfer destination", w.Subject, w.Teacher, 20, 400));
        var version = await Fixture.ScalarAsync<uint>($"select xmin from academics.enrollments where id='{w.Enrollment}'"); Time.Advance(TimeSpan.FromMinutes(1));
        var moved = await Clocked.SendAsAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(new SystemActor(NileCentreId), new(w.Enrollment, target.Value.GroupId, version));
        Assert.True(moved.IsSuccess, moved.Error?.Code);
        var relay = Clocked.GetRequiredService<OutboxRelayService>(); await relay.ProcessBatchAsync(); await relay.ProcessBatchAsync();
        Assert.Equal(1, await Count()); Assert.Equal(300m, await Fixture.ScalarAsync<decimal>("select amount from billing.charges"));
        Assert.Equal(2, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages where type='academics.enrollment-started.v1'"));
    }
    [Fact]
    public async Task BillingConsumerFailureLeavesEnrollmentCommittedAndRetryable()
    {
        var w = await ArrangeAsync();
        await ExecuteSql("create function billing.fail_consumer_probe() returns trigger language plpgsql as $$ begin raise exception 'probe failure'; end $$; create trigger fail_consumer_probe before insert on billing.charges for each row execute function billing.fail_consumer_probe();");
        try
        {
            await Clocked.GetRequiredService<OutboxRelayService>().ProcessBatchAsync(); Assert.Equal(0, await Count());
            Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from academics.enrollments where id='{w.Enrollment}' and status='active'"));
        }
        finally { await ExecuteSql("drop trigger fail_consumer_probe on billing.charges; drop function billing.fail_consumer_probe();"); }
        Time.Advance(TimeSpan.FromMinutes(2)); await Clocked.GetRequiredService<OutboxRelayService>().ProcessBatchAsync(); Assert.Equal(1, await Count());
    }
}
