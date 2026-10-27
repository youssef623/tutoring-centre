# `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 25.9: the first Tenant A → Tenant B tests on real business data. Every assertion has two parts — the attacker's (Maadi's) result, and the victim's (Nile's) row, read back through the superuser connection, never through the application. Maadi always uses S's real ID and real version, never an invented one.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 138 lines and 6,849 bytes. Source hash: `8604d32645020c9a98422fc32ee5a54b9e00b080f36ec9203985de116f05e6d8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Subjects;`
- Line 2: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 3: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`
- Line 11: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 12: `using TutoringCentre.Infrastructure.Tests.Tenancy;`
- Line 16: `using RealRenameSubjectCommand = TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject.RenameSubjectCommand;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L25): `public sealed class SubjectCrossTenantTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 28](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L28): `public async Task Get_MaadiOwnerForNileSubject_ReturnsNotFound()`
- [Line 39](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L39): `public async Task Rename_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 54](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L54): `public async Task Archive_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 68](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L68): `public async Task Restore_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- [Line 82](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L82): `public async Task List_MaadiOwner_ContainsNoNileSubject()`
- [Line 93](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L93): `public async Task Create_MaadiOwnerWithNilesExactName_Succeeds()`
- [Line 103](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L103): `public async Task CentrelessActor_AllSixRequests_ReturnTenantNotSelected()`
- [Line 123](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L123): `private async Task<StaffActor> NileOwnerAsync() => new(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);`
- [Line 125](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L125): `private async Task<StaffActor> MaadiOwnerAsync() => new(await Fixture.SeedUserAsync(), MaadiCentreId, StaffRole.Owner);`
- [Line 127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L127): `private async Task<Guid> CreateNileSubjectAsync(string name)`
- [Line 133](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L133): `private async Task<uint> ReadVersionAsNileAsync(Guid subjectId)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
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
/// Task 25.9: the first Tenant A → Tenant B tests on real business data. Every assertion has two parts — the
/// attacker's (Maadi's) result, and the victim's (Nile's) row, read back through the superuser connection, never
/// through the application. Maadi always uses S's real ID and real version, never an invented one.
/// </summary>
public sealed class SubjectCrossTenantTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task Get_MaadiOwnerForNileSubject_ReturnsNotFound()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");

        var result = await Fixture.QueryAsAsync<GetSubjectQuery, SubjectDto>(await MaadiOwnerAsync(), new GetSubjectQuery(subjectId));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task Rename_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<RealRenameSubjectCommand, Unit>(
            await MaadiOwnerAsync(), new RealRenameSubjectCommand(subjectId, "Hacked", version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and name = 'Mathematics'"));
    }

    [Fact]
    public async Task Archive_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<ArchiveSubjectCommand, Unit>(await MaadiOwnerAsync(), new ArchiveSubjectCommand(subjectId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and status = 'active'"));
    }

    [Fact]
    public async Task Restore_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()
    {
        var subjectId = await CreateNileSubjectAsync("Mathematics");
        var version = await ReadVersionAsNileAsync(subjectId);

        var result = await Fixture.SendAsAsync<RestoreSubjectCommand, Unit>(await MaadiOwnerAsync(), new RestoreSubjectCommand(subjectId, version));

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(
            1, await Fixture.ScalarAsync<long>($"select count(*) from academics.subjects where id = '{subjectId}' and status = 'active'"));
    }

    [Fact]
    public async Task List_MaadiOwner_ContainsNoNileSubject()
    {
        await CreateNileSubjectAsync("Mathematics");

        var result = await Fixture.QueryAsAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(await MaadiOwnerAsync(), new ListSubjectsQuery(true));

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value, subject => subject.Name == "Mathematics");
    }
```

The excerpt stops at line 90; use the original file for the remaining 48 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
