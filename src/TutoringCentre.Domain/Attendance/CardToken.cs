using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Attendance;

/// <summary>An opaque bearer secret. Reveal only when building the issue response.</summary>
public sealed partial class CardToken : IEquatable<CardToken>
{
    private readonly string text;

    private CardToken(string text) => this.text = text;

    public static CardToken Generate() => new("TCM1." + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        .TrimEnd('=').Replace('+', '-').Replace('/', '_'));

    public static Result<CardToken> Parse(string? input)
    {
        var value = input?.Trim();
        return value is not null && TokenPattern().IsMatch(value)
            ? Result<CardToken>.Success(new CardToken(value))
            : Result<CardToken>.Failure(Error.Validation("card.invalid_format", "Card token format is invalid.",
                new Dictionary<string, string[]> { ["token"] = ["Card token format is invalid."] }));
    }

    public byte[] Hash() => SHA256.HashData(Encoding.UTF8.GetBytes(text));
    public string Reveal() => text;
    public override string ToString() => "TCM1.…";
    public bool Equals(CardToken? other) => other is not null && text == other.text;
    public override bool Equals(object? obj) => Equals(obj as CardToken);
    public override int GetHashCode() => text.GetHashCode(StringComparison.Ordinal);

    [GeneratedRegex(@"\ATCM1\.[A-Za-z0-9_-]{43}\z")]
    private static partial Regex TokenPattern();
}
