using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Subjects;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Subject entity or EF types.</summary>
public interface ISubjectReadService
{
    /// <summary>Subjects ordered by name, scoped to the acting centre. Capped at 500 rows; there is no paging.</summary>
    Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct);

    /// <summary>One subject by ID, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct);
}

/// <summary>The one shape every subject read returns. No centre, no normalized name, no timestamps — those are persistence details.</summary>
public sealed record SubjectDto(Guid Id, string Name, SubjectStatus Status, uint Version);
