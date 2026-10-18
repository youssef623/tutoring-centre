using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

/// <summary>Intent to take a teacher out of use without deleting it, guarded by the version the client last read. The account link is kept.</summary>
public sealed record ArchiveTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
