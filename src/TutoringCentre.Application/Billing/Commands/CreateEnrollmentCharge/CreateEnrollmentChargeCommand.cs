using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Billing.Commands.CreateEnrollmentCharge;
public sealed record CreateEnrollmentChargeCommand(Guid EnrollmentId) : ICommand<Unit>, ITenantScoped, IRequirePermission
{ public string RequiredPermission => Permissions.BillingGenerate; }
