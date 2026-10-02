using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Centres;

public sealed class CentreTests
{
    private const string ValidName = "Nour Academy";
    private const string ValidSlug = "nour-academy";
    private const string Cairo = "Africa/Cairo";

    [Fact]
    public void Create_WithValidInput_SucceedsAndTrimsName()
    {
        // Act
        var result = Centre.Create("  Nour Academy  ", ValidSlug, Cairo, SupportedLocale.Ar);

        // Assert
        Assert.True(result.IsSuccess);
        var centre = result.Value;
        Assert.Equal("Nour Academy", centre.Name);
        Assert.Equal(ValidSlug, centre.Slug);
        Assert.Equal(Cairo, centre.TimeZoneId);
        Assert.Equal(SupportedLocale.Ar, centre.DefaultLocale);
        Assert.NotEqual(Guid.Empty, centre.Id);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsNameRequired() =>
        AssertValidationFailure(Centre.Create("", ValidSlug, Cairo, SupportedLocale.Ar), "centre.name_required");

    [Fact]
    public void Create_With121CharacterName_ReturnsNameTooLong() =>
        AssertValidationFailure(
            Centre.Create(new string('a', 121), ValidSlug, Cairo, SupportedLocale.Ar),
            "centre.name_too_long");

    [Theory]
    [InlineData("ab")]
    [InlineData("Bad Slug")]
    [InlineData("-abc")]
    [InlineData("abc--def")]
    public void Create_WithInvalidSlug_ReturnsSlugInvalid(string slug) =>
        AssertValidationFailure(Centre.Create(ValidName, slug, Cairo, SupportedLocale.Ar), "centre.slug_invalid");

    [Fact]
    public void Create_WithUnknownTimeZone_ReturnsTimeZoneInvalid() =>
        AssertValidationFailure(
            Centre.Create(ValidName, ValidSlug, "Mars/Base", SupportedLocale.Ar),
            "centre.time_zone_invalid");

    [Fact]
    public void Create_WithCairoTimeZone_Succeeds()
    {
        var result = Centre.Create(ValidName, ValidSlug, Cairo, SupportedLocale.En);

        Assert.True(result.IsSuccess);
        Assert.Equal(Cairo, result.Value.TimeZoneId);
    }

    // Pins down: IsNullOrWhiteSpace catches whitespace-only input, not just the empty string.
    [Fact]
    public void Create_WithWhitespaceOnlyName_ReturnsNameRequired() =>
        AssertValidationFailure(Centre.Create("   ", ValidSlug, Cairo, SupportedLocale.Ar), "centre.name_required");

    // Pins down: the length boundary is inclusive — exactly NameMaxLength characters must succeed, not just <120.
    [Fact]
    public void Create_With120CharacterName_Succeeds()
    {
        var name = new string('a', Centre.NameMaxLength);

        var result = Centre.Create(name, ValidSlug, Cairo, SupportedLocale.Ar);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
    }

    private static void AssertValidationFailure(Result<Centre> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(expectedCode, result.Error.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }
}
