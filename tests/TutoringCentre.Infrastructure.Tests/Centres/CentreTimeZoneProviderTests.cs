using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Centres;

/// <summary>
/// Task 51.6: proves the adapter's hand-written centre predicate follows the actor — platform.centres has no EF
/// query filter and no row-level security, so nothing underneath narrows it automatically. Nile and Maadi are
/// both Africa/Cairo, so a third centre in a different zone is created: with two centres sharing a zone, a
/// lookup that ignored the actor could still pass.
/// </summary>
public sealed class CentreTimeZoneProviderTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string London = "Europe/London";

    [Fact]
    public async Task GetForCurrentCentreAsync_ActorsOfTwoCentres_EachGetTheirOwnCentresZone()
    {
        var londonCentreId = await CreateCentreAsync("Thames Tutoring", "thames-tutoring", London);

        var nileZone = await ResolveAsync(StaffIn(NileCentreId), provider => provider.GetForCurrentCentreAsync(CancellationToken.None));
        var londonZone = await ResolveAsync(StaffIn(londonCentreId), provider => provider.GetForCurrentCentreAsync(CancellationToken.None));

        Assert.Equal("Africa/Cairo", nileZone.Id);
        Assert.Equal(London, londonZone.Id);
    }

    [Fact]
    public async Task GetForCurrentCentreAsync_CalledTwiceInOneScope_ReadsTheCentreOnce()
    {
        var londonCentreId = await CreateCentreAsync("Thames Tutoring", "thames-tutoring", London);

        var (first, second) = await ResolveAsync(StaffIn(londonCentreId), async provider =>
        {
            var firstZone = await provider.GetForCurrentCentreAsync(CancellationToken.None);

            // Centre has no time-zone mutator; change the row underneath the scope to prove the second call
            // answers from the scope's cache rather than reading the row again.
            await Fixture.ScalarAsync<int>($"update platform.centres set time_zone_id = 'Africa/Cairo' where id = '{londonCentreId}' returning 1");
            var secondZone = await provider.GetForCurrentCentreAsync(CancellationToken.None);
            return (firstZone, secondZone);
        });

        Assert.Equal(London, first.Id);
        Assert.Same(first, second);
        Assert.Equal("Africa/Cairo", (await ResolveAsync(StaffIn(londonCentreId), provider => provider.GetForCurrentCentreAsync(CancellationToken.None))).Id);
    }

    private static StaffActor StaffIn(Guid centreId) => new(Guid.NewGuid(), centreId, StaffRole.Secretary);

    private async Task<T> ResolveAsync<T>(Actor actor, Func<ICentreTimeZoneProvider, Task<T>> call)
    {
        await using var scope = Fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(actor);
        return await call(scope.ServiceProvider.GetRequiredService<ICentreTimeZoneProvider>());
    }

    private async Task<Guid> CreateCentreAsync(string name, string slug, string timeZoneId)
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(
            new SystemActor(null), new CreateCentreCommand(name, slug, timeZoneId, SupportedLocale.En));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.CentreId;
    }
}
