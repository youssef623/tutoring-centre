# `tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves UnitOfWork.ReconcileOrphanedStudentParentLinks through the real production handlers, not the Day 38 test-only ones in StudentParentTestRequests.cs: a link removed through UnlinkParentCommand must actually disappear from the database, not just from the aggregate's in-memory collection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 59 lines and 3,516 bytes. Source hash: `e5b27f7ae416f6ff91e8e8a8df832046004554f1e154d00dc626cbb2e581f6c5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using TutoringCentre.Application.Academics.Students.Commands.LinkParent;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Tenancy;`
- Line 8: `using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;`
- Line 9: `using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;`
- Line 10: `using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;`
- Line 11: `using RealUnlinkParentCommand = TutoringCentre.Application.Academics.Students.Commands.UnlinkParent.UnlinkParentCommand;`
- Line 12: `using Unit = TutoringCentre.Application.Common.Cqrs.Unit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L21): `public sealed class UnlinkParentReconciliationSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L24): `public async Task UnlinkParent_ThroughRealHandler_RemovesTheLinkRow()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L55): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);`
- [Line 57](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs#L57): `private async Task<uint> ReadVersionAsync(Guid studentId) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
using RealRegisterStudentCommand = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentCommand;
using RealRegisterStudentParent = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentParent;
using RealRegisterStudentResult = TutoringCentre.Application.Academics.Students.Commands.RegisterStudent.RegisterStudentResult;
using RealUnlinkParentCommand = TutoringCentre.Application.Academics.Students.Commands.UnlinkParent.UnlinkParentCommand;
using Unit = TutoringCentre.Application.Common.Cqrs.Unit;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Proves UnitOfWork.ReconcileOrphanedStudentParentLinks through the real production handlers, not the
/// Day 38 test-only ones in StudentParentTestRequests.cs: a link removed through UnlinkParentCommand must
/// actually disappear from the database, not just from the aggregate's in-memory collection.
/// </summary>
public sealed class UnlinkParentReconciliationSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task UnlinkParent_ThroughRealHandler_RemovesTheLinkRow()
    {
        var actor = await StaffActorInAsync(NileCentreId);

        var registered = await Fixture.SendAsAsync<RealRegisterStudentCommand, RealRegisterStudentResult>(
            actor,
            new RealRegisterStudentCommand(
                "Youssef Omar", null, null, new RealRegisterStudentParent(null, "Omar Hassan", "01012345678"), ParentRelationship.Father));
        Assert.True(registered.IsSuccess);

        var secondParentId = await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        var versionAfterRegister = await ReadVersionAsync(registered.Value.StudentId);
        var linked = await Fixture.SendAsAsync<LinkParentCommand, Unit>(
            actor, new LinkParentCommand(registered.Value.StudentId, secondParentId, ParentRelationship.Mother, versionAfterRegister));
        Assert.True(linked.IsSuccess, linked.Error?.Message);

        var versionAfterLink = await ReadVersionAsync(registered.Value.StudentId);
        var unlinked = await Fixture.SendAsAsync<RealUnlinkParentCommand, Unit>(
            actor, new RealUnlinkParentCommand(registered.Value.StudentId, secondParentId, versionAfterLink));
        Assert.True(unlinked.IsSuccess, unlinked.Error?.Message);

        Assert.Equal(
            0,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{registered.Value.StudentId}' and parent_id = '{secondParentId}'"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.student_parents where student_id = '{registered.Value.StudentId}'"));
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Owner);

    private async Task<uint> ReadVersionAsync(Guid studentId) =>
        uint.Parse(await Fixture.ScalarAsync<string>($"select xmin::text from academics.students where id = '{studentId}'"), CultureInfo.InvariantCulture);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
