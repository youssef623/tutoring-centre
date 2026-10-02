using Microsoft.EntityFrameworkCore;
using Npgsql;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Centres;

/// <summary>
/// TOCTOU (time-of-check to time-of-use): the application checks "slug free?" and then inserts. Two requests can both
/// see "free". The unique index is the real guarantee. Either outcome for the loser is acceptable today; Month 2
/// translates the database exception into a 409.
/// </summary>
public sealed class CentreSlugRaceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    private enum Outcome
    {
        Created,
        SlugTakenResult,
        UniqueViolationException,
    }

    [Fact]
    public async Task SendAsync_TwoParallelCreatesWithSameSlug_LeavesExactlyOneRow()
    {
        var command = new CreateCentreCommand("Race Centre", "race-centre", "Africa/Cairo", SupportedLocale.En);
        var gate = new TaskCompletionSource();

        async Task<Outcome> AttemptAsync()
        {
            await gate.Task;
            try
            {
                var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);
                return result.IsSuccess ? Outcome.Created : Outcome.SlugTakenResult;
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: "23505" })
            {
                return Outcome.UniqueViolationException;
            }
        }

        var first = Task.Run(AttemptAsync);
        var second = Task.Run(AttemptAsync);
        gate.SetResult();
        var outcomes = await Task.WhenAll(first, second);

        Assert.Equal(1, outcomes.Count(outcome => outcome == Outcome.Created));
        Assert.Equal(1, outcomes.Count(outcome => outcome != Outcome.Created));
        Assert.Equal(1, await Fixture.CountCentresAsync());
    }
}
