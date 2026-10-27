# `tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs`

## Purpose

Declares `MembershipTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 134 lines and 3,953 bytes. Source hash: `ea9cb349b61c3e4794af2e5006831ee02aa48e603b94edb4b83018bb7f6b136a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L5): `public sealed class MembershipTests`
- [Line 11](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L11): `public void Create_WithValidInput_SucceedsAsActiveWithTheGivenRole()`
- [Line 26](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L26): `public void Create_WithEmptyUserId_ReturnsUserRequired()`
- [Line 37](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L37): `public void Create_WithEmptyCentreId_ReturnsCentreRequired()`
- [Line 48](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L48): `public void ChangeRole_WhenActive_SucceedsAndChangesTheRole()`
- [Line 62](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L62): `public void ChangeRole_WhenInactive_FailsAndLeavesTheRoleUnchanged()`
- [Line 78](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L78): `public void ChangeRole_ToTheSameRole_FailsAndLeavesTheRoleUnchanged()`
- [Line 93](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L93): `public void Deactivate_WhenActive_SucceedsAndBecomesInactive()`
- [Line 107](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L107): `public void Deactivate_WhenAlreadyInactive_ReturnsAlreadyInactive()`
- [Line 122](../../../../../tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs#L122): `public void Activate_WhenAlreadyActive_ReturnsAlreadyActive()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class MembershipTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public void Create_WithValidInput_SucceedsAsActiveWithTheGivenRole()
    {
        // Act
        var result = Membership.Create(UserId, CentreId, StaffRole.Teacher);

        // Assert
        Assert.True(result.IsSuccess);
        var membership = result.Value;
        Assert.Equal(UserId, membership.UserId);
        Assert.Equal(CentreId, membership.CentreId);
        Assert.Equal(StaffRole.Teacher, membership.Role);
        Assert.Equal(MembershipStatus.Active, membership.Status);
    }

    [Fact]
    public void Create_WithEmptyUserId_ReturnsUserRequired()
    {
        // Act
        var result = Membership.Create(Guid.Empty, CentreId, StaffRole.Owner);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.user_required", result.Error!.Code);
    }

    [Fact]
    public void Create_WithEmptyCentreId_ReturnsCentreRequired()
    {
        // Act
        var result = Membership.Create(UserId, Guid.Empty, StaffRole.Owner);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.centre_required", result.Error!.Code);
    }

    [Fact]
    public void ChangeRole_WhenActive_SucceedsAndChangesTheRole()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;

        // Act
        var result = membership.ChangeRole(StaffRole.Secretary);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StaffRole.Secretary, membership.Role);
    }

    [Fact]
    public void ChangeRole_WhenInactive_FailsAndLeavesTheRoleUnchanged()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;
        membership.Deactivate();

        // Act
        var result = membership.ChangeRole(StaffRole.Secretary);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.inactive", result.Error!.Code);
        Assert.Equal(StaffRole.Teacher, membership.Role);
    }

    [Fact]
    public void ChangeRole_ToTheSameRole_FailsAndLeavesTheRoleUnchanged()
    {
        // Arrange
        var membership = Membership.Create(UserId, CentreId, StaffRole.Teacher).Value;

        // Act
        var result = membership.ChangeRole(StaffRole.Teacher);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("membership.role_unchanged", result.Error!.Code);
        Assert.Equal(StaffRole.Teacher, membership.Role);
    }
```

The excerpt stops at line 90; use the original file for the remaining 44 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
