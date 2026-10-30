using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Identity;

/// <summary>An immutable association between a login and one tenant-owned portal subject.</summary>
public sealed class PortalLink : Entity, ITenantOwned
{
    private PortalLink() { }

    private PortalLink(Guid userId, Guid centreId, Guid subjectId, Guid createdBy, bool parent)
    {
        CentreId = centreId;
        UserId = userId;
        CreatedBy = createdBy;
        RoleKey = parent ? "parent" : "student";
        ParentId = parent ? subjectId : null;
        StudentId = parent ? null : subjectId;
    }

    public Guid CentreId { get; }
    public Guid UserId { get; }
    public string RoleKey { get; } = null!;
    public Guid? ParentId { get; }
    public Guid? StudentId { get; }
    public Guid CreatedBy { get; }

    public static Result<PortalLink> CreateForParent(Guid userId, Guid centreId, Guid parentId, Guid createdBy) =>
        Create(userId, centreId, parentId, createdBy, true);

    public static Result<PortalLink> CreateForStudent(Guid userId, Guid centreId, Guid studentId, Guid createdBy) =>
        Create(userId, centreId, studentId, createdBy, false);

    private static Result<PortalLink> Create(Guid userId, Guid centreId, Guid subjectId, Guid createdBy, bool parent)
    {
        if (userId == Guid.Empty || centreId == Guid.Empty || subjectId == Guid.Empty || createdBy == Guid.Empty)
            return Result<PortalLink>.Failure(Error.Validation("portal.ids_required", "All identifiers are required."));

        return Result<PortalLink>.Success(new PortalLink(userId, centreId, subjectId, createdBy, parent));
    }
}
