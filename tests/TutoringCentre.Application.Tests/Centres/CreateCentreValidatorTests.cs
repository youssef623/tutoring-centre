using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class CreateCentreValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReportsErrorOnName()
    {
        // Arrange
        var command = new CreateCentreCommand("", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_SlugOverSixtyCharacters_ReportsErrorOnSlug()
    {
        // Arrange
        var command = new CreateCentreCommand("Nile Tutoring Centre", new string('a', 61), "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Slug");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        // Arrange
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);

        // Act
        var result = new CreateCentreValidator().Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}
