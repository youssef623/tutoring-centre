using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Domain.Tests.Billing;
public sealed class BillingCalendarTests
{
    [Theory][InlineData(2024, 2, 29)][InlineData(2025, 2, 28)][InlineData(2026, 4, 30)]
    public void Day31Clamps(int year, int month, int day) => Assert.Equal(new DateOnly(year, month, day), BillingCalendar.DueDate(BillingMonth.Of(new(year, month, 10)), 31));
    [Fact] public void DecemberRollsIntoJanuary() => Assert.Equal("2027-01", BillingMonth.Of(new(2026, 12, 31)).Next().ToString());
    [Fact] public void DefaultDueDateIsDayFive() => Assert.Equal(new DateOnly(2026, 2, 5), BillingCalendar.DueDate(BillingMonth.Of(new(2026, 2, 28))));
}
