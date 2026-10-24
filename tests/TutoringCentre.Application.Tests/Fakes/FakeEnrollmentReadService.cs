using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for enrollment reads. Each row carries its group's teacher, so the scope is applied the way
/// the real adapter applies it (rows outside it are invisible); records the last scope it was asked for.
/// </summary>
public sealed class FakeEnrollmentReadService : IEnrollmentReadService
{
    public List<(Guid TeacherId, GroupRosterDto Roster)> Rosters { get; } = [];

    public HashSet<Guid> StudentIds { get; } = [];

    public List<(Guid StudentId, Guid TeacherId, StudentEnrollmentDto Enrollment)> StudentEnrollments { get; } = [];

    public TeacherScope? LastScope { get; private set; }

    public Task<GroupRosterDto?> GetRosterAsync(Guid groupId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        var match = Rosters
            .Where(row => row.Roster.GroupId == groupId && scope.Allows(row.TeacherId))
            .Select(row => row.Roster)
            .FirstOrDefault();
        return Task.FromResult(match);
    }

    public Task<IReadOnlyList<StudentEnrollmentDto>?> ListForStudentAsync(Guid studentId, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        if (!StudentIds.Contains(studentId))
        {
            return Task.FromResult<IReadOnlyList<StudentEnrollmentDto>?>(null);
        }

        IReadOnlyList<StudentEnrollmentDto> visible = StudentEnrollments
            .Where(row => row.StudentId == studentId && scope.Allows(row.TeacherId))
            .Select(row => row.Enrollment)
            .ToList();
        return Task.FromResult<IReadOnlyList<StudentEnrollmentDto>?>(visible);
    }
}
