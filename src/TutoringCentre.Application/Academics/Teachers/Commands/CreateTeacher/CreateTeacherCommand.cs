using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

/// <summary>
/// Intent to add a teacher record in the acting actor's own centre. No userId: linking an account is a
/// separate, separately audited action (SetTeacherAccountCommand). Teacher names are not unique, so two
/// teachers may share a name.
/// </summary>
public sealed record CreateTeacherCommand(string FullName, string? Phone) : ICommand<CreateTeacherResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersManage;
}
