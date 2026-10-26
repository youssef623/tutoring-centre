using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;

/// <summary>Intent to take a student out of the active list without deleting their history, guarded by the version the client last read.</summary>
public sealed record ArchiveStudentCommand(Guid StudentId, uint Version) : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
