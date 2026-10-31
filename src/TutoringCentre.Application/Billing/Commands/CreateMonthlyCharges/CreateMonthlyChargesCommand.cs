using TutoringCentre.Domain.Common;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
public sealed record CreateMonthlyChargesCommand(BillingMonth? Month = null) : ICommand<CreateMonthlyChargesResult>, ITenantScoped, IRequirePermission
{ public string RequiredPermission => Permissions.BillingGenerate; }
public sealed record CreateMonthlyChargesResult(int Created, int Skipped);
