using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Attendance;

public sealed class CardTokenTests
{
    private static readonly string Text = "TCM1." + new string('A', 43);

    [Fact]
    public void Generate_HasRequiredFormatAndThousandDistinctValues()
    {
        var tokens = Enumerable.Range(0, 1000).Select(_ => CardToken.Generate().Reveal()).ToArray();
        Assert.All(tokens, token =>
        {
            Assert.Equal(48, token.Length);
            Assert.Matches(@"\ATCM1\.[A-Za-z0-9_-]{43}\z", token);
        });
        Assert.Equal(1000, tokens.Distinct().Count());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" \t")]
    [InlineData(null)]
    [InlineData("TCM2.")]
    [InlineData("TCMX.")]
    [InlineData("TCM1", 43)]
    [InlineData("TCM1.", 42)]
    [InlineData("TCM1.", 44)]
    [InlineData("TCM1.", 42, "+")]
    [InlineData("TCM1.", 42, "/")]
    [InlineData("TCM1.", 42, "=")]
    public void Parse_Invalid_ReturnsFieldValidation(string? prefix, int length = 43, string suffix = "")
    {
        var input = prefix is null || string.IsNullOrWhiteSpace(prefix) ? prefix : prefix + new string('A', length) + suffix;
        var result = CardToken.Parse(input);
        Assert.True(result.IsFailure);
        Assert.Equal("card.invalid_format", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.Contains("token", result.Error.Fields!.Keys);
    }

    [Fact]
    public void Parse_TrimsAndHasValueEquality()
    {
        var token = CardToken.Parse(" \t" + Text + "\r\n").Value;
        Assert.Equal(Text, token.Reveal());
        Assert.Equal(CardToken.Parse(Text).Value, token);
        Assert.Equal(CardToken.Parse(Text).Value.GetHashCode(), token.GetHashCode());
    }

    [Fact]
    public void Hash_MatchesIndependentKnownAnswerAndReturnsFreshBytes()
    {
        // Node crypto.createHash('sha256').update('TCM1.' + 'A'.repeat(43), 'utf8').digest('hex').
        var token = CardToken.Parse(Text).Value;
        var hash = token.Hash();
        Assert.Equal(32, hash.Length);
        Assert.Equal("429120B2CF3782BC27DF9C4D2086BC5E6443E9DB8215BFCF5845896DBECCD178", Convert.ToHexString(hash));
        Assert.Equal(hash, CardToken.Parse(Text).Value.Hash());
        hash[0] ^= 255;
        Assert.NotEqual(hash, token.Hash());
    }

    [Fact]
    public void ToString_AndInterpolation_HideBody()
    {
        var token = CardToken.Parse(Text).Value;
        Assert.Equal("TCM1.…", token.ToString());
        Assert.Equal("TCM1.…", $"{token}");
        Assert.DoesNotContain(Text[5..], token.ToString());
    }
}
