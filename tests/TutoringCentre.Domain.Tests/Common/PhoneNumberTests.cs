using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class PhoneNumberTests
{
    [Theory]
    [InlineData("01012345678", "+201012345678")]
    [InlineData("010 1234 5678", "+201012345678")]
    [InlineData("010-1234-5678", "+201012345678")]
    [InlineData("+201012345678", "+201012345678")]
    [InlineData("00201012345678", "+201012345678")]
    [InlineData("201012345678", "+201012345678")]
    [InlineData("٠١٠١٢٣٤٥٦٧٨", "+201012345678")]
    [InlineData("+20 (101) 234-5678", "+201012345678")]
    [InlineData("01112345678", "+201112345678")]
    [InlineData("01212345678", "+201212345678")]
    [InlineData("01512345678", "+201512345678")]
    [InlineData("+966501234567", "+966501234567")]
    [InlineData("00971501234567", "+971501234567")]
    public void Create_WithAcceptedForm_ProducesTheCanonicalValue(string input, string expected)
    {
        var result = PhoneNumber.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("0101234567")]
    [InlineData("010123456789")]
    [InlineData("01312345678")]
    [InlineData("0223456789")]
    [InlineData("+2010123")]
    [InlineData("+0123456789")]
    [InlineData("abc")]
    [InlineData("+20101234567a")]
    public void Create_WithRejectedForm_FailsWithPhoneInvalid(string? input)
    {
        var result = PhoneNumber.Create(input);

        Assert.True(result.IsFailure);
        Assert.Equal("phone.invalid", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Create_NeverThrows()
    {
        var result = PhoneNumber.Create("not a phone number at all");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void TwoInstances_FromDifferentSpellingsOfOneNumber_AreEqualWithEqualHashCodes()
    {
        var a = PhoneNumber.Create("01012345678").Value;
        var b = PhoneNumber.Create("+20 (101) 234-5678").Value;

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_EqualsValue()
    {
        var phone = PhoneNumber.Create("01012345678").Value;

        Assert.Equal(phone.Value, phone.ToString());
    }

    [Fact]
    public void Masked_ForEgyptianNumber_ShowsCountryCodeAndLastFourDigits()
    {
        var phone = PhoneNumber.Create("01012345678").Value;

        Assert.Equal("+20••••••5678", phone.Masked);
    }

    [Fact]
    public void Masked_ForInternationalNumber_ShowsOnlyLastFourDigitsOfSubscriberNumber()
    {
        var phone = PhoneNumber.Create("+966501234567").Value;

        Assert.EndsWith("4567", phone.Masked, StringComparison.Ordinal);
        Assert.DoesNotContain("501234", phone.Masked, StringComparison.Ordinal);
    }

    // Mutation check: remove "015" from LocalEgyptianMobilePattern's [0125] class; the "01512345678" row above
    // (prefix 015) alone fails.
}
