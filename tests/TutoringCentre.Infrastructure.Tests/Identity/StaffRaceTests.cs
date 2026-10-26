using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 29.12: the two concurrency invariants only a real database, under real concurrent transactions, can
/// prove — the last-active-owner lock and the membership uniqueness constraint. Each attempt dispatches in its
/// own scope (its own context and connection), never sharing one, exactly like <c>SubjectRaceTests</c>.
/// </summary>
public sealed class StaffRaceTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    private const int LastOwnerRaceRepetitions = 20;

    [Fact]
    public async Task ChangeRole_TwoOwnersDemotingEachOtherSimultaneously_ExactlyOneSucceedsAcrossTwentyRepetitions()
    {
        for (var iteration = 0; iteration < LastOwnerRaceRepetitions; iteration++)
        {
            // A fresh centre per repetition, not NileCentreId reused: the invariant under test is "at least
            // one active owner in the whole centre", so any owner left over from an earlier repetition would
            // silently count toward this one's total and mask the very race this test exists to catch.
            var centreId = await CreateRaceCentreAsync(iteration);
            var ownerA = await CreateStaffAsync(centreId, $"race-owner-a-{iteration}@nile.test", StaffRole.Owner);
            var ownerB = await CreateStaffAsync(centreId, $"race-owner-b-{iteration}@nile.test", StaffRole.Owner);
            var versionA = await ReadVersionAsync(ownerA, ownerA.MembershipId);
            var versionB = await ReadVersionAsync(ownerA, ownerB.MembershipId);
            var gate = new TaskCompletionSource();

            async Task<Result<Unit>> DemoteAsync(StaffActor actor, Guid targetMembershipId, uint version)
            {
                await gate.Task;
                return await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(
                    actor, new ChangeStaffRoleCommand(targetMembershipId, StaffRole.Teacher, version));
            }

            var aDemotesB = Task.Run(() => DemoteAsync(ownerA.Actor, ownerB.MembershipId, versionB));
            var bDemotesA = Task.Run(() => DemoteAsync(ownerB.Actor, ownerA.MembershipId, versionA));
            gate.SetResult();
            var results = await Task.WhenAll(aDemotesB, bDemotesA);

            Assert.Equal(1, results.Count(result => result.IsSuccess));
            var loser = Assert.Single(results, result => result.IsFailure);
            Assert.Equal("staff.last_owner", loser.Error!.Code);
            Assert.Equal(
                1,
                await Fixture.ScalarAsync<long>(
                    $"""
                    select count(*) from identity.memberships
                    where centre_id = '{centreId}' and role = 'owner' and status = 'active'
                      and id in ('{ownerA.MembershipId}', '{ownerB.MembershipId}')
                    """));
        }
    }

    private async Task<Guid> CreateRaceCentreAsync(int iteration)
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(
            new SystemActor(null), new CreateCentreCommand($"Race Centre {iteration}", $"race-centre-{iteration}", "Africa/Cairo", SupportedLocale.Ar));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.CentreId;
    }

    [Fact]
    public async Task Create_TwoParallelCreatesForTheSameEmailInTheSameCentre_LeavesExactlyOneMembershipAndTheLoserGetsAlreadyMember()
    {
        const string email = "race-duplicate@nile.test";

        // Pre-created in a different centre: the account already exists before the race starts, so the race
        // below exercises only the membership's own unique constraint (ux_memberships_centre_user), not
        // ASP.NET Identity's separate concurrent-insert race for a brand-new email — a distinct concern this
        // test is not about.
        await CreateStaffAsync(MaadiCentreId, email, StaffRole.Teacher, "Race Duplicate");

        var gate = new TaskCompletionSource();

        async Task<Result<CreateStaffResult>> AttemptAsync()
        {
            await gate.Task;
            return await Fixture.SendAsAsync<CreateStaffCommand, CreateStaffResult>(
                new SystemActor(NileCentreId), new CreateStaffCommand(email, "Race Duplicate", StaffRole.Secretary, "en"));
        }

        var first = Task.Run(AttemptAsync);
        var second = Task.Run(AttemptAsync);
        gate.SetResult();
        var results = await Task.WhenAll(first, second);

        Assert.Equal(1, results.Count(result => result.IsSuccess));
        var loser = Assert.Single(results, result => result.IsFailure);
        Assert.Equal("staff.already_member", loser.Error!.Code);
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from identity.memberships m join identity.users u on u.id = m.user_id
                where u.email = '{email}' and m.centre_id = '{NileCentreId}'
                """));
    }
}
