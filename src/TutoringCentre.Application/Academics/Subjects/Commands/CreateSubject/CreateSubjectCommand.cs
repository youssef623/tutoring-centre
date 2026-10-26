using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

/// <summary>Intent to create a subject in the acting actor's own centre. Carries exactly the field its intent needs — the centre comes from the actor, never the caller.</summary>
public sealed record CreateSubjectCommand(string Name) : ICommand<CreateSubjectResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsManage;
}
