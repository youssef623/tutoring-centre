using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

/// <summary>The student list behind the student directory page. Every filter is optional; the caller cannot pass a centre.</summary>
public sealed record ListStudentsQuery(string? Search, bool IncludeArchived, string? Cursor, int PageSize)
    : IQuery<StudentPageDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
