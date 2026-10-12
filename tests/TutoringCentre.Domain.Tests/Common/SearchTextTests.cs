using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class SearchTextTests
{
    [Fact]
    public void Normalize_NullValue_ReturnsEmpty() => Assert.Equal(string.Empty, SearchText.Normalize(null));

    [Theory]
    [InlineData("أحمد")]
    [InlineData("احمد")]
    [InlineData("أَحْمَد")]
    [InlineData("أحـمد")]
    public void Normalize_AhmedSpellingVariants_AllProduceTheSameString(string input)
    {
        Assert.Equal(SearchText.Normalize("احمد"), SearchText.Normalize(input));
    }

    [Fact]
    public void Normalize_IbrahimWithAndWithoutInitialHamza_AreEqual() =>
        Assert.Equal(SearchText.Normalize("ابراهيم"), SearchText.Normalize("إبراهيم"));

    [Fact]
    public void Normalize_FatimaWithTaaMarbutaAndHaa_AreEqual() =>
        Assert.Equal(SearchText.Normalize("فاطمه"), SearchText.Normalize("فاطمة"));

    [Fact]
    public void Normalize_MustafaWithAlifMaksuraAndYaa_AreEqual() =>
        Assert.Equal(SearchText.Normalize("مصطفي"), SearchText.Normalize("مصطفى"));

    [Fact]
    public void Normalize_TrimsAndCollapsesWhitespaceAndLowersCase() =>
        Assert.Equal("omar hassan", SearchText.Normalize("  Omar   Hassan "));

    [Fact]
    public void Normalize_IsCaseInsensitive() => Assert.Equal(SearchText.Normalize("omar"), SearchText.Normalize("OMAR"));

    [Theory]
    [InlineData("أحمد")]
    [InlineData("  Omar   Hassan ")]
    [InlineData("فاطمة")]
    [InlineData("مصطفى")]
    [InlineData("")]
    public void Normalize_IsIdempotent(string input)
    {
        var once = SearchText.Normalize(input);
        var twice = SearchText.Normalize(once);

        Assert.Equal(once, twice);
    }

    // Mutation check: delete the hamza mapping (أ,إ,آ,ٱ → ا); the Ahmed-variants test fails.
}
