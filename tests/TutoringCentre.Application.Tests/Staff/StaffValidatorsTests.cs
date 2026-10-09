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
    {
        var result = new DeactivateStaffValidator().Validate(new DeactivateStaffCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "MembershipId");
    }
}

public sealed class ReactivateStaffValidatorTests
{
    [Fact]
    public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()
    {
        var result = new ReactivateStaffValidator().Validate(new ReactivateStaffCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "MembershipId");
    }
}
