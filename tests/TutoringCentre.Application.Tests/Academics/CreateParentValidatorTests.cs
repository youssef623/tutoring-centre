using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateParentValidatorTests
{
    [Fact]
    public void Validate_EmptyFullName_ReportsErrorOnFullName()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("", "01012345678"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_EmptyPhone_ReportsErrorOnPhone()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("Omar Hassan", ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_ValidInput_HasNoErrors()
    {
        var result = new CreateParentValidator().Validate(new CreateParentCommand("Omar Hassan", "01012345678"));

        Assert.True(result.IsValid);
    }
}
