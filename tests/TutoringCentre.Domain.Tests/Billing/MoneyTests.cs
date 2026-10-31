using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Domain.Tests.Billing;
public sealed class MoneyTests
{
    [Theory]
    [InlineData("0")][InlineData("-10.25")][InlineData("300.00")][InlineData("99999999.99")]
    public void SignedRepresentationRoundTrips(string value)
    { var money = Money.Parse(value); Assert.True(money.IsSuccess); Assert.Equal(money.Value, Money.Parse(money.Value.ToString()).Value); }
    [Theory]
    [InlineData("10.123")][InlineData("1.2300")][InlineData("100000000")][InlineData("-100000000")][InlineData("NaN")]
    public void InvalidPrecisionOrRangeIsRejected(string value) => Assert.True(Money.Parse(value).IsFailure);
    [Fact] public void ExactAddition() => Assert.Equal(0.3m, Money.Create(0.1m).Value.Add(Money.Create(0.2m).Value).Value.Amount);
    [Fact] public void AdditionChecksRange() => Assert.True(Money.Create(Money.Maximum).Value.Add(Money.Create(0.01m).Value).IsFailure);
    [Fact] public void CompareIsExact() => Assert.True(Money.Create(0.1m).Value.CompareTo(Money.Create(0.2m).Value) < 0);
    [Theory][InlineData(0)][InlineData(-1)]
    public void MonthlyChargeRejectsNonpositiveFee(int amount) => Assert.True(Charge.CreateMonthly(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), BillingMonth.Of(new(2026, 11, 1)), Money.Create(amount).Value).IsFailure);
    [Fact] public void MonthlyChargeSnapshotsAmountAndDueDate()
    { var c = Charge.CreateMonthly(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), BillingMonth.Of(new(2026, 11, 15)), Money.Create(300).Value).Value; Assert.Equal("300.00", c.Amount.ToString()); Assert.Equal(new DateOnly(2026, 11, 5), c.DueDate); Assert.Equal(ChargeKind.Monthly, c.Kind); }
}
