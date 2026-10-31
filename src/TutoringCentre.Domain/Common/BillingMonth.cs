using System.Globalization;

namespace TutoringCentre.Domain.Common;

public readonly record struct BillingMonth
{
    public DateOnly FirstDay { get; }
    private BillingMonth(DateOnly date) => FirstDay = new(date.Year, date.Month, 1);
    public static BillingMonth Of(DateOnly localDate) => new(localDate);
    public static bool operator <=(BillingMonth left, BillingMonth right) => left.FirstDay <= right.FirstDay;
    public static bool operator >=(BillingMonth left, BillingMonth right) => left.FirstDay >= right.FirstDay;
    public static bool operator <(BillingMonth left, BillingMonth right) => left.FirstDay < right.FirstDay;
    public static bool operator >(BillingMonth left, BillingMonth right) => left.FirstDay > right.FirstDay;
    public BillingMonth Next() => Of(FirstDay.AddMonths(1));
    public override string ToString() => FirstDay.ToString("yyyy-MM", CultureInfo.InvariantCulture);
}
