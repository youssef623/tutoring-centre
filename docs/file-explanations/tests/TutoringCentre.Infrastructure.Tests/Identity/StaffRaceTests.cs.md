# `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 29.12: the two concurrency invariants only a real database, under real concurrent transactions, can prove — the last-active-owner lock and the membership uniqueness constraint. Each attempt dispatches in its own scope (its own context and connection), never sharing one, exactly like SubjectRaceTests.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 107 lines and 5,476 bytes. Source hash: `7b0af7b7dd6a0aba11ec2fe0cbff6256c28069682c853655b36ac0c2f847f5b0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 5: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 6: `using TutoringCentre.Domain.Centres;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L18): `public sealed class StaffRaceTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)`
- [Line 23](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L23): `public async Task ChangeRole_TwoOwnersDemotingEachOtherSimultaneously_ExactlyOneSucceedsAcrossTwentyRepetitions()`
- [Line 63](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L63): `private async Task<Guid> CreateRaceCentreAsync(int iteration)`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs#L72): `public async Task Create_TwoParallelCreatesForTheSameEmailInTheSameCentre_LeavesExactlyOneMembershipAndTheLoserGetsAlreadyMember()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

```

The excerpt stops at line 90; use the original file for the remaining 17 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
