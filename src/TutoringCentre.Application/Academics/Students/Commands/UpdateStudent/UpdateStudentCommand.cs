using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>
/// Intent to replace a student's name, phone and school year, guarded by the version the client last read.
/// Full replacement: a missing phone or school year clears it, it does not mean "keep the old one". Parent
/// links are untouched. No centre: scope comes from the actor.
/// </summary>
public sealed record UpdateStudentCommand(Guid StudentId, string FullName, string? Phone, string? SchoolYear, uint Version)
    : ICommand<Unit>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
