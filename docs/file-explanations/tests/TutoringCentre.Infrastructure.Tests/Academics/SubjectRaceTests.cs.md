# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 25.7: both callers pass the application-level uniqueness check; only the unique index can arbitrate who actually wins. Each attempt dispatches in its own scope (its own context and connection), never sharing one.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,837 bytes. Source hash: `a5e548254d135bd381e98fd861554b98d94911398d46a1edc0ff507f034b10e0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs#L14): `public sealed class SubjectRaceTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs#L19): `public async Task Create_TenParallelCreatesWithSameName_LeavesExactlyOneRowAndNineCleanConflicts()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
            var actor = new StaffActor(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);
            return await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
