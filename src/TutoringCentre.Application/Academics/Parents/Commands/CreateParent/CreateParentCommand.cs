using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Commands.CreateParent;

/// <summary>Intent to create a parent in the acting actor's own centre. The phone is raw input here; PhoneNumber.Create parses it once, in the handler.</summary>
public sealed record CreateParentCommand(string FullName, string Phone) : ICommand<CreateParentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}
