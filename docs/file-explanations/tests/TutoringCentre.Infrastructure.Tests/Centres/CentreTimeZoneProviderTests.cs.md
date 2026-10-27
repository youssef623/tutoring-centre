# `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 51.6: proves the adapter's hand-written centre predicate follows the actor — platform.centres has no EF query filter and no row-level security, so nothing underneath narrows it automatically. Nile and Maadi are both Africa/Cairo, so a third centre in a different zone is created: with two centres sharing a zone, a lookup that ignored the actor could still pass.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 3,623 bytes. Source hash: `6e84117dafce5f85c8e354ae7b6823bfa0e56819281c7bd7c20f34ae9bceb578`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.DependencyInjection;`
- Line 2: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 3: `using TutoringCentre.Application.Centres.Contracts;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Centres;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 8: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L18): `public sealed class CentreTimeZoneProviderTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L23): `public async Task GetForCurrentCentreAsync_ActorsOfTwoCentres_EachGetTheirOwnCentresZone()`
- [Line 35](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L35): `public async Task GetForCurrentCentreAsync_CalledTwiceInOneScope_ReadsTheCentreOnce()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L55): `private static StaffActor StaffIn(Guid centreId) => new(Guid.NewGuid(), centreId, StaffRole.Secretary);`
- [Line 57](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L57): `private async Task<T> ResolveAsync<T>(Actor actor, Func<ICentreTimeZoneProvider, Task<T>> call)`
- [Line 64](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs#L64): `private async Task<Guid> CreateCentreAsync(string name, string slug, string timeZoneId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
