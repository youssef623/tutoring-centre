using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;

/// <summary>Intent to put an archived student back on the active list, guarded by the version the client last read.</summary>
public sealed record RestoreStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
