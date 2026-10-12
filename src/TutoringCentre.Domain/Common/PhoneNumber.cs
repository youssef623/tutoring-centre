using System.Text.RegularExpressions;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// A phone number in canonical E.164 form. The only way to obtain one is <see cref="Create"/>, which enforces
/// the accepted formats. Immutable and equal by value: two instances built from different spellings of the
/// same number compare equal.
/// </summary>
public sealed partial class PhoneNumber : IEquatable<PhoneNumber>
{
    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>The canonical E.164 form, e.g. "+201012345678".</summary>
    public string Value { get; }

    /// <summary>
    /// A display form that hides every digit except a leading prefix and the last four. For a "+20" number the
    /// prefix is "+20" (the Egyptian country code); for any other number the prefix is just the leading "+" and
    /// its first digit, since this type does not know other countries' code lengths. Everything between the
    /// prefix and the last four digits becomes one "•" per hidden digit.
    /// </summary>
    public string Masked
    {
        get
        {
            var prefixLength = Value.StartsWith("+20", StringComparison.Ordinal) ? 3 : 2;
            var hiddenCount = Value.Length - prefixLength - 4;
            return Value[..prefixLength] + new string('•', hiddenCount) + Value[^4..];
        }
    }

    public static Result<PhoneNumber> Create(string? input)
    {
        var canonical = Canonicalize(input);
        return canonical is null
            ? Result<PhoneNumber>.Failure(Error.Validation("phone.invalid", "Phone number is not valid."))
            : Result<PhoneNumber>.Success(new PhoneNumber(canonical));
    }

    public override string ToString() => Value;

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    private static string? Canonicalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var mapped = MapArabicDigits(input.Trim());
        var cleaned = SeparatorPattern().Replace(mapped, string.Empty);

        if (cleaned.StartsWith("00", StringComparison.Ordinal))
        {
            cleaned = "+" + cleaned[2..];
        }
        else if (LocalEgyptianMobilePattern().IsMatch(cleaned))
        {
            cleaned = "+20" + cleaned[1..];
        }
        else if (EgyptianMobileWithoutPlusPattern().IsMatch(cleaned))
        {
            cleaned = "+" + cleaned;
        }

        var isValid = cleaned.StartsWith("+20", StringComparison.Ordinal)
            ? EgyptianMobilePattern().IsMatch(cleaned)
            : GeneralInternationalPattern().IsMatch(cleaned);

        return isValid ? cleaned : null;
    }

    private static string MapArabicDigits(string value)
    {
        var buffer = new char[value.Length];
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            buffer[i] = c switch
            {
                >= '٠' and <= '٩' => (char)('0' + (c - '٠')), // Arabic-Indic
                >= '۰' and <= '۹' => (char)('0' + (c - '۰')), // Extended Arabic-Indic
                _ => c,
            };
        }

        return new string(buffer);
    }

    // Spaces, hyphens, dots and parentheses carry no information once digits are mapped.
    [GeneratedRegex(@"[\s\-.()]")]
    private static partial Regex SeparatorPattern();

    // "01" + one of the mobile prefixes 0/1/2/5 + eight digits: the local 11-digit form.
    [GeneratedRegex(@"^01[0125]\d{8}$")]
    private static partial Regex LocalEgyptianMobilePattern();

    // The same number with the country code but no leading "+".
    [GeneratedRegex(@"^201[0125]\d{8}$")]
    private static partial Regex EgyptianMobileWithoutPlusPattern();

    // "+20" + mobile prefix 0/1/2/5 + eight digits.
    [GeneratedRegex(@"^\+201[0125]\d{8}$")]
    private static partial Regex EgyptianMobilePattern();

    // "+" + a non-zero digit + 7 to 14 more digits: 8 to 15 digits in total.
    [GeneratedRegex(@"^\+[1-9]\d{7,14}$")]
    private static partial Regex GeneralInternationalPattern();
}
