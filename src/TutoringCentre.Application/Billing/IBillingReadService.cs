using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Billing;
public sealed record StudentChargeDto(Guid Id, string BillingMonth, string Kind, string SubjectName, string GroupName, string Amount, DateOnly DueDate);
public sealed record StudentChargesPage(IReadOnlyList<StudentChargeDto> Items, string? NextCursor);
public interface IBillingReadService
{
    Task<StudentChargesPage> GetAsync(Guid studentId, string? cursor, int pageSize, CancellationToken ct);
}
