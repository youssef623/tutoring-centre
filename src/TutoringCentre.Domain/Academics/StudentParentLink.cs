using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Academics;

/// <summary>
/// Records that a parent is linked to a student, in what relationship, and whether that parent is the primary
/// contact. A child entity of <see cref="Student"/>: only the aggregate root creates or changes a link, which is
/// why construction and the primary flag are internal to the Domain assembly rather than public.
/// </summary>
public sealed class StudentParentLink : Entity, ITenantOwned
{
    internal StudentParentLink(Guid centreId, Guid studentId, Guid parentId, ParentRelationship relationship, bool isPrimary)
    {
        CentreId = centreId;
        StudentId = studentId;
        ParentId = parentId;
        Relationship = relationship;
        IsPrimary = isPrimary;
    }

    // For EF Core materialisation (Day 38) only; EF then sets every property from the row.
    private StudentParentLink()
    {
    }

    public Guid CentreId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid ParentId { get; private set; }

    public ParentRelationship Relationship { get; private set; }

    public bool IsPrimary { get; private set; }

    internal void MarkPrimary() => IsPrimary = true;

    internal void ClearPrimary() => IsPrimary = false;
}
