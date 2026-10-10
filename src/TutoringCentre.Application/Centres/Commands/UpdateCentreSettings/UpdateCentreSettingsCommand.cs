using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;

/// <summary>Intent to change the acting actor's own centre's name and default locale, guarded by the version the client last read. No centre id: scope comes from the actor.</summary>
public sealed record UpdateCentreSettingsCommand(string Name, SupportedLocale DefaultLocale, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.CentreSettingsManage;
}
