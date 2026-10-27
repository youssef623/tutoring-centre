# `tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs`

## Purpose

Declares `CreateStaffValidatorTests`, `ChangeStaffRoleValidatorTests`, `DeactivateStaffValidatorTests`, `ReactivateStaffValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 109 lines and 3,994 bytes. Source hash: `8f11d16934eb446f0bd6d24ac023b9e7193b1ec87fb8008c93ad9cade49f30c3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;`
- Line 2: `using TutoringCentre.Application.Staff.Commands.CreateStaff;`
- Line 3: `using TutoringCentre.Application.Staff.Commands.DeactivateStaff;`
- Line 4: `using TutoringCentre.Application.Staff.Commands.ReactivateStaff;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L9): `public sealed class CreateStaffValidatorTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L12): `public void Validate_InvalidEmail_ReportsErrorOnEmail()`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L21): `public void Validate_EmailOverTwoHundredFiftySixCharacters_ReportsErrorOnEmail()`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L31): `public void Validate_EmptyDisplayName_ReportsErrorOnDisplayName()`
- [Line 40](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L40): `public void Validate_DisplayNameOverOneHundredTwentyCharacters_ReportsErrorOnDisplayName()`
- [Line 49](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L49): `public void Validate_UndefinedRole_ReportsErrorOnRole()`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L58): `public void Validate_UnsupportedLocale_ReportsErrorOnPreferredLocale()`
- [Line 67](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L67): `public void Validate_ValidCommand_HasNoErrors()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L75): `public sealed class ChangeStaffRoleValidatorTests`
- [Line 78](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L78): `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`
- [Line 87](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L87): `public sealed class DeactivateStaffValidatorTests`
- [Line 90](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L90): `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`
- [Line 99](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L99): `public sealed class ReactivateStaffValidatorTests`
- [Line 102](../../../../../tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs#L102): `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

public sealed class CreateStaffValidatorTests
{
    [Fact]
    public void Validate_InvalidEmail_ReportsErrorOnEmail()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("not-an-email", "Name", StaffRole.Teacher, "en"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmailOverTwoHundredFiftySixCharacters_ReportsErrorOnEmail()
    {
        var longLocal = new string('a', 250);
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand($"{longLocal}@nile.test", "Name", StaffRole.Teacher, "en"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyDisplayName_ReportsErrorOnDisplayName()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("person@nile.test", "", StaffRole.Teacher, "en"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "DisplayName");
    }

    [Fact]
    public void Validate_DisplayNameOverOneHundredTwentyCharacters_ReportsErrorOnDisplayName()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("person@nile.test", new string('a', 121), StaffRole.Teacher, "en"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "DisplayName");
    }

    [Fact]
    public void Validate_UndefinedRole_ReportsErrorOnRole()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("person@nile.test", "Name", (StaffRole)99, "en"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Role");
    }

    [Fact]
    public void Validate_UnsupportedLocale_ReportsErrorOnPreferredLocale()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("person@nile.test", "Name", StaffRole.Teacher, "fr"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "PreferredLocale");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = new CreateStaffValidator().Validate(new CreateStaffCommand("person@nile.test", "Name", StaffRole.Teacher, "en"));

        Assert.True(result.IsValid);
    }
}

public sealed class ChangeStaffRoleValidatorTests
{
    [Fact]
    public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()
    {
        var result = new ChangeStaffRoleValidator().Validate(new ChangeStaffRoleCommand(Guid.Empty, StaffRole.Teacher, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "MembershipId");
    }
}

public sealed class DeactivateStaffValidatorTests
{
    [Fact]
    public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()
```

The excerpt stops at line 90; use the original file for the remaining 19 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
