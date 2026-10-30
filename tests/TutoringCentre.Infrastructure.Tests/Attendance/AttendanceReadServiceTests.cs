using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Persistence;
using Xunit.Abstractions;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class AttendanceReadServiceTests(PostgresFixture fixture, ITestOutputHelper output) : AttendanceCommandTestBase(fixture)
{
    [Fact]
    public async Task History_AccessCountsMarkerAndSubstituteScope()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "reads");
        Time.SetUtcNow(scenario.StartsAt.AddMinutes(-10));
        Assert.True((await MarkAsync(scenario.Owner, scenario)).IsSuccess);
        await using var scope = Clocked.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(scenario.Owner);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: true, CancellationToken.None);
        var reads = scope.ServiceProvider.GetRequiredService<IAttendanceReadService>();
        var date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(scenario.StartsAt, "Africa/Cairo").DateTime);
        var filter = new AttendanceFilter(scenario.StudentId, scenario.GroupId, date, date);
        Assert.Single((await reads.ListAsync(filter, AttendanceAccess.All, null, 50, CancellationToken.None)).Items);
        Assert.Single((await reads.ListAsync(filter, AttendanceAccess.Children([scenario.StudentId]), null, 50, CancellationToken.None)).Items);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.Children([Guid.NewGuid()]), null, 50, CancellationToken.None)).Items);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.Children([]), null, 50, CancellationToken.None)).Items);
        Assert.Single((await reads.ListAsync(filter, AttendanceAccess.OwnSessions(scenario.TeacherId), null, 50, CancellationToken.None)).Items);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.OwnSessions(scenario.SubstituteId), null, 50, CancellationToken.None)).Items);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.None, null, 50, CancellationToken.None)).Items);
        var records = await reads.ListForSessionAsync(scenario.SessionId, CancellationToken.None);
        Assert.NotNull(Assert.Single(records).Record.MarkedByName);
        Assert.NotEqual(0u, records[0].Record.Version);
        var counts = await reads.CountForSessionsAsync([scenario.SessionId, Guid.NewGuid()], CancellationToken.None);
        Assert.Equal(1, counts[scenario.SessionId]);
        Assert.Single(counts);
        await unit.CommitAsync(CancellationToken.None);
        await ReassignSessionAsync(scenario, scenario.SubstituteId);
        await unit.BeginAsync(readOnly: true, CancellationToken.None);
        Assert.Empty((await reads.ListAsync(filter, AttendanceAccess.OwnSessions(scenario.TeacherId), null, 50, CancellationToken.None)).Items);
        Assert.Single((await reads.ListAsync(filter, AttendanceAccess.OwnSessions(scenario.SubstituteId), null, 50, CancellationToken.None)).Items);
        await unit.CommitAsync(CancellationToken.None);
    }

    [Fact]
    public async Task None_ReturnsWithoutDatabaseConnection()
    {
        // No transaction or actor: a database query here would violate tenant setup.
        await using var scope = Clocked.CreateAsyncScope();
        var reads = scope.ServiceProvider.GetRequiredService<IAttendanceReadService>();
        var page = await reads.ListAsync(new(null, null, DefaultToday, DefaultToday), AttendanceAccess.None, null, 50, CancellationToken.None);
        Assert.Empty(page.Items);
        Assert.Null(page.NextCursor);
    }

    [Fact]
    public async Task History_LogsSessionTeacherPredicate_AndNoneExecutesNoQuery()
    {
        var scenario = await ArrangeAttendanceAsync(NileCentreId, "sql-scope");
        var capture = new Commands();
        await using var services = Fixture.CreateServiceProvider(services =>
            services.AddDbContext<AppDbContext>(options => options.AddInterceptors(capture)));
        await using var scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(scenario.Owner);
        var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(readOnly: true, CancellationToken.None);
        var reads = scope.ServiceProvider.GetRequiredService<IAttendanceReadService>();
        var filter = new AttendanceFilter(null, null, DefaultToday, DefaultToday.AddDays(7));
        capture.Sql.Clear();
        await reads.ListAsync(filter, AttendanceAccess.None, null, 50, CancellationToken.None);
        Assert.Empty(capture.Sql);
        await reads.ListAsync(filter, AttendanceAccess.OwnSessions(scenario.TeacherId), null, 50, CancellationToken.None);
        var sql = Assert.Single(capture.Sql);
        output.WriteLine(sql);
        Assert.Contains("teacher_id =", sql, StringComparison.Ordinal);
        Assert.Contains("academics.class_sessions", sql, StringComparison.Ordinal);
        await unit.CommitAsync(CancellationToken.None);
    }

    private sealed class Commands : DbCommandInterceptor
    {
        public List<string> Sql { get; } = [];
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
