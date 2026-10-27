# `tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs`

## Purpose

Declares `PhoneNumberTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 95 lines and 3,061 bytes. Source hash: `cf2e520c7895baa73ea3c1e9f6de9c0dd0587c2b974a2709e5107fafa1cded52`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L5): `public sealed class PhoneNumberTests`
- [Line 21](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L21): `public void Create_WithAcceptedForm_ProducesTheCanonicalValue(string input, string expected)`
- [Line 41](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L41): `public void Create_WithRejectedForm_FailsWithPhoneInvalid(string? input)`
- [Line 51](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L51): `public void Create_NeverThrows()`
- [Line 59](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L59): `public void TwoInstances_FromDifferentSpellingsOfOneNumber_AreEqualWithEqualHashCodes()`
- [Line 69](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L69): `public void ToString_EqualsValue()`
- [Line 77](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L77): `public void Masked_ForEgyptianNumber_ShowsCountryCodeAndLastFourDigits()`
- [Line 85](../../../../../tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs#L85): `public void Masked_ForInternationalNumber_ShowsOnlyLastFourDigitsOfSubscriberNumber()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 5 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
