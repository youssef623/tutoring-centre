using TutoringCentre.Domain.Common;
namespace TutoringCentre.Domain.Billing;

public static class BillingCalendar
{
    public const int DueDay = 5;
    public static DateOnly DueDate(BillingMonth month, int day = DueDay)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(day, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(day, 31);
        return new(month.FirstDay.Year, month.FirstDay.Month, Math.Min(day, DateTime.DaysInMonth(month.FirstDay.Year, month.FirstDay.Month)));
    }
}
