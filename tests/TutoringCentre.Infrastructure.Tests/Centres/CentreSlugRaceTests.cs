using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Centres;

/// <summary>
/// TOCTOU (time-of-check to time-of-use): the application checks "slug free?" and then inserts. Two requests can both
/// see "free". The unique index is the real guarantee. Task 25.3/25.4 closes the gap this test used to document:
/// the loser now always receives a translated centre.slug_taken Result, never an unhandled exception.
/// </summary>
public sealed class CentreSlugRaceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task SendAsync_TwoParallelCreatesWithSameSlug_LeavesExactlyOneRowAndLoserGetsSlugTakenConflict()
    {
        var command = new CreateCentreCommand("Race Centre", "race-centre", "Africa/Cairo", SupportedLocale.En);
        var gate = new TaskCompletionSource();

        async Task<Result<CreateCentreResult>> AttemptAsync()
        {
            await gate.Task;
            return await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);
        }

        var first = Task.Run(AttemptAsync);
        var second = Task.Run(AttemptAsync);
        gate.SetResult();
        var results = await Task.WhenAll(first, second);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var loser = Assert.Single(results, result => result.IsFailure);
        Assert.Equal("centre.slug_taken", loser.Error!.Code);
        Assert.Equal(1, await Fixture.CountCentresAsync());
    }
}
