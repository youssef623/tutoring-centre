namespace TutoringCentre.Application.Identity.Portal;

public sealed record PortalSubject(PortalSubjectKind Kind, Guid Id)
{
    public string? RoleKey => Kind switch
    {
        PortalSubjectKind.Parent => "parent",
        PortalSubjectKind.Student => "student",
        _ => null,
    };
}
