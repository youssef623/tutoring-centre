using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>One effective-month price, not a charge. Editing the same future month replaces its proposed price.</summary>
public sealed class GroupMonthlyFee : Entity, ITenantOwned
{
    private GroupMonthlyFee() { }
    internal GroupMonthlyFee(Guid centreId, Guid groupId, BillingMonth month, Money fee)
    { CentreId = centreId; GroupId = groupId; EffectiveMonth = month; Fee = fee; }
    public Guid CentreId { get; private set; }
    public Guid GroupId { get; private set; }
    public BillingMonth EffectiveMonth { get; private set; }
    public Money Fee { get; private set; }
    internal void Replace(Money fee) => Fee = fee;
}
