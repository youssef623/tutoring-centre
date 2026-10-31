using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Billing;

namespace TutoringCentre.Application.Academics.Contracts;

public sealed record ChargeableEnrollment(Guid EnrollmentId, Guid StudentId, Guid GroupId, Guid SubjectId, Guid TeacherId, Money Fee);
public sealed record BillingNames(string SubjectName, string GroupName);
public interface IBillingRosterSource
{
    Task<IReadOnlyList<ChargeableEnrollment>> ListChargeableAsync(BillingMonth month, CancellationToken ct);
    Task<ChargeableEnrollment?> GetChargeableAsync(Guid enrollmentId, BillingMonth month, CancellationToken ct);
    Task<bool> StudentExistsAsync(Guid studentId, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, BillingNames>> GetGroupNamesAsync(IReadOnlyCollection<Guid> groupIds, CancellationToken ct);
}
