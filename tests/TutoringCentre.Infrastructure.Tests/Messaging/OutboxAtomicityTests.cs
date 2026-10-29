using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public sealed class OutboxAtomicityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private static readonly DateTimeOffset Now = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Case1_FailedCommand_RollsBackBusinessChangeAndOutbox()
    {
        var result = await SendAsync(Guid.NewGuid(), fail: true);
        Assert.True(result.IsFailure);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
    }

    [Fact]
    public async Task Case2_SuccessfulCommand_CommitsActorCentreCorrelationAndClock()
    {
        var result = await SendAsync(Guid.NewGuid(), fail: false);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from probe.tenant_probes"));
        Assert.Equal(NileCentreId, await Fixture.ScalarAsync<Guid>("select centre_id from jobs.outbox_messages"));
        Assert.Equal("test:atomicity", await Fixture.ScalarAsync<string>("select correlation_id from jobs.outbox_messages"));
        Assert.Equal(Now.UtcDateTime, await Fixture.ScalarAsync<DateTime>("select occurred_at from jobs.outbox_messages"));
        Assert.Equal(Now.UtcDateTime, await Fixture.ScalarAsync<DateTime>("select available_at from jobs.outbox_messages"));
        Assert.Equal(0, await Fixture.ScalarAsync<int>("select attempts from jobs.outbox_messages"));
    }

    [Fact]
    public async Task Enqueue_WithoutCentre_ThrowsBeforeWriting()
    {
        await using var scope = Fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(null));
        Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IOutbox>().Enqueue(new DeliveryMessage(Guid.NewGuid())));
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages"));
    }

    private async Task<Result<Unit>> SendAsync(Guid id, bool fail)
    {
        await using var provider = Fixture.CreateServiceProvider(services =>
        {
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(Now));
            services.AddOutboxTestCommands();
        });
        return await provider.GetRequiredService<ISystemScopeRunner>().RunAsync(NileCentreId, "test:atomicity",
            (services, ct) => services.GetRequiredService<Dispatcher>().SendAsync<EnqueueProbeCommand, Unit>(new EnqueueProbeCommand(id, fail), ct));
    }
}
