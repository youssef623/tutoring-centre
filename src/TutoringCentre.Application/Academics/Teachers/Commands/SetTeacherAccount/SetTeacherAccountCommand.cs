using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;

/// <summary>
/// Intent to link a teacher record to a staff user's login, or to clear the link when <see cref="UserId"/> is
/// null. This is the action that gives the ownership guard (Day 44) its meaning. Guarded by the version the
/// client last read. No centre: scope comes from the actor.
/// </summary>
public sealed record SetTeacherAccountCommand(Guid TeacherId, Guid? UserId, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
