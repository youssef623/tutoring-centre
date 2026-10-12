using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 39.9: both callers pass the application-level uniqueness check; only ux_parents_centre_phone can
/// arbitrate who actually wins. The ten spellings all normalize to the same E.164 number, proving the index —
/// not string equality — is what the race is decided on. Each attempt dispatches in its own scope.
/// </summary>
public sealed class ParentRaceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private static readonly string[] Spellings =
    [
        "01012345678",
        "010 1234 5678",
        "010-1234-5678",
        "+201012345678",
        "00201012345678",
        "201012345678",
        "٠١٠١٢٣٤٥٦٧٨",
        "+20 (101) 234-5678",
        "+20 101 234 5678",
        "0020 101 234 5678",
    ];

    [Fact]
    public async Task Create_TenParallelCreatesWithTheSamePhoneInDifferentSpellings_LeavesExactlyOneRowAndNineCleanConflicts()
    {
        var gate = new TaskCompletionSource();

        async Task<Result<CreateParentResult>> AttemptAsync(string spelling)
        {
            await gate.Task;
            var actor = new StaffActor(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);
            return await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(actor, new CreateParentCommand("Omar Hassan", spelling));
        }

        var tasks = Spellings.Select(spelling => Task.Run(() => AttemptAsync(spelling))).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var losers = results.Where(result => result.IsFailure).ToList();
        Assert.Equal(Spellings.Length - 1, losers.Count);
        Assert.All(losers, result => Assert.Equal("parent.phone_taken", result.Error!.Code));
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from academics.parents"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>("select count(*) from academics.parents where phone = '+201012345678'"));
    }
}
