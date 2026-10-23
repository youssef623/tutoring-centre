using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;

/// <summary>Intent to change a group's number of seats only, guarded by the version the client last read.</summary>
public sealed record ChangeGroupCapacityCommand(Guid GroupId, int Capacity, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
