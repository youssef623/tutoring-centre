# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 25.8: optimistic concurrency proven end to end — the server never substitutes the current version for the client's. No test fetches a fresh version before its second write; that is exactly the anomaly under test.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 104 lines and 5,316 bytes. Source hash: `11a95c1bf1693b2690e9293235a9def529da1ed8dab36750273b401281f41654`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 5: `using TutoringCentre.Application.Common.Cqrs;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Domain.Identity;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 10: `using TutoringCentre.Infrastructure.Tests.Tenancy;`
- Line 14: `using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L22): `public sealed class SubjectStaleVersionTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L25): `public async Task Rename_WithTheVersionJustRead_Succeeds()`
- [Line 38](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L38): `public async Task Rename_AgainWithTheSameStaleVersion_ReturnsConcurrencyStaleAndKeepsTheFirstRename()`
- [Line 58](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L58): `public async Task Archive_WithAStaleVersion_ReturnsConcurrencyStale()`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L73): `public async Task Rename_TwoParallelCarryingTheSameVersion_ExactlyOneSucceeds()`
- [Line 97](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L97): `private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);`
- [Line 99](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L99): `private async Task<uint> ReadVersionAsync(StaffActor actor, Guid subjectId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

// Aliased for the same reason as SubjectUseCaseTests: Task 23.6's test-only RenameSubjectCommand, declared
// directly in this namespace, would otherwise shadow the real (production) one.
using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 25.8: optimistic concurrency proven end to end — the server never substitutes the current version for
/// the client's. No test fetches a fresh version before its second write; that is exactly the anomaly under test.
/// </summary>
public sealed class SubjectStaleVersionTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Rename_WithTheVersionJustRead_Succeeds()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);

        var result = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Rename_AgainWithTheSameStaleVersion_ReturnsConcurrencyStaleAndKeepsTheFirstRename()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        var firstRename = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));
        Assert.True(firstRename.IsSuccess);

        // Still v1 — the version the client originally read, not a freshly re-read one.
        var secondRename = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Pure Mathematics", v1));

        Assert.True(secondRename.IsFailure);
        Assert.Equal("concurrency.stale", secondRename.Error!.Code);
        var current = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(actor, new GetSubjectQuery(created.Value.SubjectId));
        Assert.Equal("Applied Mathematics", current.Value.Name);
    }

    [Fact]
    public async Task Archive_WithAStaleVersion_ReturnsConcurrencyStale()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            actor, new RealRenameSubjectCommand(created.Value.SubjectId, "Applied Mathematics", v1));

        var archived = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(actor, new ArchiveSubjectCommand(created.Value.SubjectId, v1));

        Assert.True(archived.IsFailure);
        Assert.Equal("concurrency.stale", archived.Error!.Code);
    }

    [Fact]
    public async Task Rename_TwoParallelCarryingTheSameVersion_ExactlyOneSucceeds()
    {
        var actor = await NileOwnerAsync();
        var created = await Fixture.SendAsAsync<CreateSubjectCommand, CreateSubjectResult>(actor, new CreateSubjectCommand("Mathematics"));
        var v1 = await ReadVersionAsync(actor, created.Value.SubjectId);
        var gate = new TaskCompletionSource();

        async Task<Result<Unit>> AttemptAsync(string name)
        {
            await gate.Task;
            return await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
                actor, new RealRenameSubjectCommand(created.Value.SubjectId, name, v1));
        }

        var first = Task.Run(() => AttemptAsync("Applied Mathematics"));
        var second = Task.Run(() => AttemptAsync("Pure Mathematics"));
        gate.SetResult();
        var results = await Task.WhenAll(first, second);
```

The excerpt stops at line 90; use the original file for the remaining 14 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
