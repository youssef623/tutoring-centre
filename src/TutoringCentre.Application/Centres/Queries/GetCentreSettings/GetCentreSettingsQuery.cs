using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Centres.Queries.GetCentreSettings;

/// <summary>The settings behind the centre settings page. No parameters: scope comes from the actor.</summary>
public sealed record GetCentreSettingsQuery : IQuery<CentreSettingsDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.CentreSettingsManage;
}
