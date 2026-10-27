# `tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs`

## Purpose

Declares `RolePermissionsTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 4,556 bytes. Source hash: `87b7449245a2b493b1cd42008d6dbf1ed5cfefdcd03c96b19d3c4872f3d26a8e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L5): `public sealed class RolePermissionsTests`
- [Line 57](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L57): `public void Holds_MatchesTheMatrixCellLiterally(StaffRole role, string permission, bool expected)`
- [Line 63](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L63): `public void Owner_HoldsEveryPermission()`
- [Line 72](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L72): `public void For_OnlyReturnsPermissionsFromTheCatalogue(StaffRole role)`
- [Line 81](../../../../../tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs#L81): `public void Holds_WithAnUnknownPermission_IsDeniedForEveryRole(StaffRole role)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class RolePermissionsTests
{
    // Day 28 contract matrix, written out literally. Never derive these from RolePermissions itself.
    [Theory]
    [InlineData(StaffRole.Owner, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Owner, Permissions.SubjectsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.StaffView, true)]
    [InlineData(StaffRole.Owner, Permissions.StaffManage, true)]
    [InlineData(StaffRole.Owner, Permissions.AuditView, true)]
    [InlineData(StaffRole.Owner, Permissions.CentreSettingsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.StudentsView, true)]
    [InlineData(StaffRole.Owner, Permissions.StudentsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.TeachersView, true)]
    [InlineData(StaffRole.Owner, Permissions.TeachersManage, true)]
    [InlineData(StaffRole.Owner, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Owner, Permissions.GroupsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Owner, Permissions.EnrollmentsManage, true)]
    [InlineData(StaffRole.Owner, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Owner, Permissions.ScheduleManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.SubjectsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.StaffView, false)]
    [InlineData(StaffRole.Secretary, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.AuditView, false)]
    [InlineData(StaffRole.Secretary, Permissions.CentreSettingsManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.StudentsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.StudentsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.TeachersView, true)]
    [InlineData(StaffRole.Secretary, Permissions.TeachersManage, false)]
    [InlineData(StaffRole.Secretary, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.GroupsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Secretary, Permissions.EnrollmentsManage, true)]
    [InlineData(StaffRole.Secretary, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Secretary, Permissions.ScheduleManage, true)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.SubjectsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffView, false)]
    [InlineData(StaffRole.Teacher, Permissions.StaffManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.AuditView, false)]
    [InlineData(StaffRole.Teacher, Permissions.CentreSettingsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.StudentsView, false)]
    [InlineData(StaffRole.Teacher, Permissions.StudentsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.TeachersView, false)]
    [InlineData(StaffRole.Teacher, Permissions.TeachersManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.GroupsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.GroupsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.EnrollmentsView, true)]
    [InlineData(StaffRole.Teacher, Permissions.EnrollmentsManage, false)]
    [InlineData(StaffRole.Teacher, Permissions.ScheduleView, true)]
    [InlineData(StaffRole.Teacher, Permissions.ScheduleManage, false)]
    public void Holds_MatchesTheMatrixCellLiterally(StaffRole role, string permission, bool expected)
    {
        Assert.Equal(expected, RolePermissions.Holds(role, permission));
    }

    [Fact]
    public void Owner_HoldsEveryPermission()
    {
        Assert.All(Permissions.All, permission => Assert.True(RolePermissions.Holds(StaffRole.Owner, permission)));
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public void For_OnlyReturnsPermissionsFromTheCatalogue(StaffRole role)
    {
        Assert.All(RolePermissions.For(role), permission => Assert.Contains(permission, Permissions.All));
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public void Holds_WithAnUnknownPermission_IsDeniedForEveryRole(StaffRole role)
    {
        Assert.False(RolePermissions.Holds(role, "nonexistent.permission"));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
