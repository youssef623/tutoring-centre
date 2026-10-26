using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;

/// <summary>Intent to bring an archived teacher back, guarded by the version the client last read. The kept account link takes effect again.</summary>
public sealed record RestoreTeacherCommand(Guid TeacherId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
