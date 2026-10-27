# `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 39.9: both callers pass the application-level uniqueness check; only ux_parents_centre_phone can arbitrate who actually wins. The ten spellings all normalize to the same E.164 number, proving the index — not string equality — is what the race is decided on. Each attempt dispatches in its own scope.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 56 lines and 2,433 bytes. Source hash: `05e6e73caff951c98c60da5d597b5b4d29365a8127f91dcf8a746316287eb187`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L15): `public sealed class ParentRaceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs#L32): `public async Task Create_TenParallelCreatesWithTheSamePhoneInDifferentSpellings_LeavesExactlyOneRowAndNineCleanConflicts()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
