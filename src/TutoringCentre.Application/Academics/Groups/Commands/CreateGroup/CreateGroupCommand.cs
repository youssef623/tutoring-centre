using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>Intent to open a group for an active subject and an active teacher in the acting actor's own centre.</summary>
public sealed record CreateGroupCommand(string Name, Guid SubjectId, Guid TeacherId, int Capacity, decimal MonthlyFee = 0)
    : ICommand<CreateGroupResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.GroupsManage;
}
