using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Academics.Students;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Student entity or EF types.</summary>
public interface IStudentReadService
{
    /// <summary>A page of students matching the search criteria, scoped to the acting centre.</summary>
    Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct);

    /// <summary>One student with their parents, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct);
}

/// <summary>What the list query passes to the read service. No centre: scope comes from the actor.</summary>
public sealed record StudentSearchCriteria(string? Search, bool IncludeArchived, string? Cursor, int PageSize);

/// <summary>One row of the student list, with the primary parent's contact details for display.</summary>
public sealed record StudentListItemDto(
    Guid Id, string FullName, string? SchoolYear, StudentStatus Status, string? PrimaryParentName, string? PrimaryParentPhone);

public sealed record StudentPageDto(IReadOnlyList<StudentListItemDto> Items, string? NextCursor);

/// <summary>A student plus every linked parent, for the student detail page. Parents list the primary first.</summary>
public sealed record StudentDetailDto(
    Guid Id, string FullName, string? Phone, string? SchoolYear, StudentStatus Status, uint Version, IReadOnlyList<StudentParentDto> Parents);

public sealed record StudentParentDto(Guid ParentId, string FullName, string Phone, ParentRelationship Relationship, bool IsPrimary);
