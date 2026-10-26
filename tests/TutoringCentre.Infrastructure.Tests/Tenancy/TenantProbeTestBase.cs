using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Shared setup for Tasks 20.5/20.6: seeds Nile and Maadi fresh for every test (tables start empty — Day 19's
/// PostgresTestBase), and offers the one helper every filter/guard test uses to set its actor.
/// </summary>
public abstract class TenantProbeTestBase(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    protected Guid NileCentreId { get; private set; }

    protected Guid MaadiCentreId { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        NileCentreId = await CreateCentreAsync("Nile Tutoring Centre", "nile-centre");
        MaadiCentreId = await CreateCentreAsync("Maadi Hub", "maadi-hub");
    }

    /// <summary>Opens a probe scope as the given actor — the context is created, then the actor is set, matching
    /// how a real request can build AppDbContext before login runs (Task 20.1).</summary>
    private protected Task<ProbeScope> CreateProbeScope(Actor actor) => Fixture.CreateProbeScope(actor);

    private async Task<Guid> CreateCentreAsync(string name, string slug)
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(
            new SystemActor(null), new CreateCentreCommand(name, slug, "Africa/Cairo", SupportedLocale.Ar));
        return result.Value.CentreId;
    }
}
