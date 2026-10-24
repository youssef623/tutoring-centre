using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;

/// <summary>Intent to end an enrollment and free its seat, guarded by the version the client last read.</summary>
public sealed record WithdrawEnrollmentCommand(Guid EnrollmentId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.EnrollmentsManage;
}
