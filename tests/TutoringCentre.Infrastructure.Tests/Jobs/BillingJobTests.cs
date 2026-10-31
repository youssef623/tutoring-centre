using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Jobs;
using TutoringCentre.Infrastructure.Tests.Academics;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Jobs;
public sealed class BillingJobTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    [Fact]
    public async Task FanOutEnqueuesCentreIdentifiersOnly()
    {
        var client = new Client(); await new CreateChargesFanOutJob(Clocked.GetRequiredService<ISystemScopeRunner>(), client).ExecuteAsync(null, CancellationToken.None);
        Assert.Equal(new[] { NileCentreId, MaadiCentreId }.Order(), client.Jobs.Select(x => (Guid)x.Args[0]).Order());
        Assert.All(client.Jobs, job => { Assert.Equal(typeof(CreateCentreChargesJob), job.Type); Assert.Null(job.Args[1]); });
    }
    [Fact]
    public async Task ChildFailuresAreIndependentAndRepeatIsSafe()
    {
        var job = new CreateCentreChargesJob(Clocked.GetRequiredService<ISystemScopeRunner>());
        await Assert.ThrowsAsync<InvalidOperationException>(() => job.ExecuteAsync(Guid.NewGuid(), null, CancellationToken.None));
        await job.ExecuteAsync(MaadiCentreId, null, CancellationToken.None); await job.ExecuteAsync(MaadiCentreId, null, CancellationToken.None);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from billing.charges"));
    }
    [Fact]
    public void RegistrationUsesFirstDayCairo0030WithoutAServer()
    {
        var storage = JobsStorage.Create(Fixture.AppConnectionString, prepareSchema: false); RecurringJobs.Register(new RecurringJobManager(storage));
        using var connection = storage.GetConnection(); var values = connection.GetAllEntriesFromHash("recurring-job:billing.monthly-charges");
        Assert.Equal("30 0 1 * *", values["Cron"]); Assert.Equal("Africa/Cairo", values["TimeZoneId"]); Assert.Empty(storage.GetMonitoringApi().Servers());
    }
    private sealed class Client : IBackgroundJobClient
    {
        public List<Job> Jobs { get; } = [];
        public string Create(Job job, IState state) { Jobs.Add(job); return "job-probe"; }
        public bool ChangeState(string jobId, IState state, string expectedState) => throw new NotSupportedException();
    }
}
