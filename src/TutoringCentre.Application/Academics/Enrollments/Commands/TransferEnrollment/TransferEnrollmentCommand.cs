using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

/// <summary>Intent to move an active enrollment to another group of the same subject, guarded by the version the client last read.</summary>
public sealed record TransferEnrollmentCommand(Guid EnrollmentId, Guid TargetGroupId, uint Version)
    : ICommand<TransferEnrollmentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
