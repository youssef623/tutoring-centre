using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 25.7: both callers pass the application-level uniqueness check; only the unique index can arbitrate who
/// actually wins. Each attempt dispatches in its own scope (its own context and connection), never sharing one.
/// </summary>
public sealed class SubjectRaceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const int Attempts = 10;

    [Fact]
    public async Task Create_TenParallelCreatesWithSameName_LeavesExactlyOneRowAndNineCleanConflicts()
    {
        var gate = new TaskCompletionSource();

        async Task<Result<CreateSubjectResult>> AttemptAsync()
        {
            await gate.Task;
            return await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(
                new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner), new CreateSubjectCommand("Mathematics"));
        }

        var tasks = Enumerable.Range(0, Attempts).Select(_ => Task.Run(AttemptAsync)).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var losers = results.Where(result => result.IsFailure).ToList();
        Assert.Equal(Attempts - 1, losers.Count);
        Assert.All(losers, result => Assert.Equal("subject.name_taken", result.Error!.Code));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.subjects"));
    }
}
