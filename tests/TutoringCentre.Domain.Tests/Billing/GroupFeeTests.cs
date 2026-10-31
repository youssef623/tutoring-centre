using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Domain.Tests.Billing;
public sealed class GroupFeeTests
{
    [Fact]
    public void FeeScheduleKeepsCurrentMonthAndReplacesOnlySameFutureMonth()
    {
        var group = Group.Create(Guid.NewGuid(), "EVAL", Guid.NewGuid(), Guid.NewGuid(), 20).Value;
        var month = BillingMonth.Of(new(2026, 11, 1));
        Assert.Equal(0m, group.MonthlyFee.Amount);
        Assert.True(group.SetMonthlyFee(Money.Create(-1).Value).IsFailure);
        group.SetMonthlyFee(Money.Create(300).Value, month);
        group.SetMonthlyFee(Money.Create(400).Value, month.Next());
        group.SetMonthlyFee(Money.Create(450).Value, month.Next());
        Assert.Equal(2, group.MonthlyFees.Count);
        Assert.Equal(300m, group.MonthlyFees.Single(x => x.EffectiveMonth == month).Fee.Amount);
        Assert.Equal(450m, group.MonthlyFees.Single(x => x.EffectiveMonth == month.Next()).Fee.Amount);
    }
}
