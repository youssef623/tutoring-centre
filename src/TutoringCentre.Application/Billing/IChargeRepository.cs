using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Application.Billing;

public interface IChargeRepository
{
    Task<bool> ExistsForMonthAsync(Guid studentId, Guid subjectId, BillingMonth month, CancellationToken ct);
    void Add(Charge charge);
    /// <summary>Atomic insert: only a conflict on the partial monthly obligation key yields false.</summary>
    Task<bool> TryAddMonthlyAsync(Charge charge, CancellationToken ct);
}
