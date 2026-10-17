using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

/// <summary>
/// Intent to replace a teacher's name and phone, guarded by the version the client last read. Full
/// replacement: a missing phone clears it. The account link is never touched here. No centre: scope comes
/// from the actor.
/// </summary>
public sealed record UpdateTeacherCommand(Guid TeacherId, string FullName, string? Phone, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
