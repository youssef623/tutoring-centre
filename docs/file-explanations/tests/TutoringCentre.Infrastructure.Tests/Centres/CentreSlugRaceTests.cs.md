# `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): TOCTOU (time-of-check to time-of-use): the application checks "slug free?" and then inserts. Two requests can both see "free". The unique index is the real guarantee. Task 25.3/25.4 closes the gap this test used to document: the loser now always receives a translated centre.slug_taken Result, never an unhandled exception.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,722 bytes. Source hash: `6e9d99f35fb10aafe3c6701b8d8ae283f35d772115a38f29568b6ed2b2b5e3cb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs#L14): `public sealed class CentreSlugRaceTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs#L17): `public async Task SendAsync_TwoParallelCreatesWithSameSlug_LeavesExactlyOneRowAndLoserGetsSlugTakenConflict()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
