using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Parent entity or EF types.</summary>
public interface IParentReadService
{
    /// <summary>One parent with their children, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct);

    /// <summary>The parent holding this phone in the acting centre, or null.</summary>
    Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct);
}

/// <summary>The one shape every plain parent read returns. No centre, no search name, no timestamps — those are persistence details.</summary>
public sealed record ParentDto(Guid Id, string FullName, string Phone, uint Version);

/// <summary>A parent plus the students linked to them, for the parent detail page.</summary>
public sealed record ParentDetailDto(Guid Id, string FullName, string Phone, uint Version, IReadOnlyList<ParentChildDto> Children);

/// <summary>One student linked to a parent, from the parent's point of view.</summary>
public sealed record ParentChildDto(Guid StudentId, string FullName, StudentStatus Status, ParentRelationship Relationship, bool IsPrimary);

/// <summary>A lookup's answer: the parent holding the looked-up phone, or null — absence is data, never a 404.</summary>
public sealed record ParentLookupDto(ParentDto? Parent);
