using System.Globalization;

namespace TutoringCentre.Domain.Common;

/// <summary>Signed representation; fee and monthly-obligation boundaries impose their own sign rules.</summary>
public readonly record struct Money : IComparable<Money>
{
    public const decimal Maximum = 99999999.99m;
    public const string Currency = "EGP";
    public decimal Amount { get; }
    private Money(decimal amount) => Amount = amount;
    public static Result<Money> Create(decimal amount) => amount < -Maximum || amount > Maximum || ((decimal.GetBits(amount)[3] >> 16) & 0xff) > 2
        ? Result<Money>.Failure(Error.Validation("billing.money_invalid", "Use at most two decimal places within the supported range."))
        : Result<Money>.Success(new(amount));
    public static Result<Money> Parse(string value) => decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var amount)
        ? Create(amount) : Result<Money>.Failure(Error.Validation("billing.money_invalid", "A decimal amount is required."));
    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;
    public Result<Money> Add(Money other) => Create(Amount + other.Amount);
    public int CompareTo(Money other) => Amount.CompareTo(other.Amount);
    public override string ToString() => Amount.ToString("0.00", CultureInfo.InvariantCulture);
}
