using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>
/// "Is there already a parent with this phone in my centre?" — the question the registration form asks
/// before creating a duplicate. Absence is a normal answer (a null inside a success), never a 404. No centre:
/// scope comes from the actor.
/// </summary>
public sealed record LookupParentByPhoneQuery(string Phone) : IQuery<ParentLookupDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
