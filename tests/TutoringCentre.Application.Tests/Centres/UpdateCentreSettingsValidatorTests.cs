using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class UpdateCentreSettingsValidatorTests
{
    private readonly UpdateCentreSettingsValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_Succeeds()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("Nile Learning Centre", SupportedLocale.En, 0));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("", SupportedLocale.En, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.Name));
    }

    [Fact]
    public void Validate_With121CharacterName_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand(new string('a', 121), SupportedLocale.En, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.Name));
    }

    [Fact]
    public void Validate_WithUnknownLocale_Fails()
    {
        var result = _validator.Validate(new UpdateCentreSettingsCommand("Nile Learning Centre", (SupportedLocale)99, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(UpdateCentreSettingsCommand.DefaultLocale));
    }
}
