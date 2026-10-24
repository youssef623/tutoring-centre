using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory enrollment repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeEnrollmentRepository : IEnrollmentRepository
{
    public List<Enrollment> Existing { get; } = [];

    public List<Enrollment> Added { get; } = [];

    public Task<Enrollment?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(enrollment => enrollment.Id == id));

    public Task<bool> ActiveExistsAsync(Guid studentId, Guid groupId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.StudentId == studentId && enrollment.GroupId == groupId));

    public Task<bool> AnyActiveForGroupAsync(Guid groupId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.GroupId == groupId));

    public Task<bool> AnyActiveForStudentAsync(Guid studentId, CancellationToken ct) =>
        Task.FromResult(Active().Any(enrollment => enrollment.StudentId == studentId));

    public void Add(Enrollment enrollment) => Added.Add(enrollment);

    private IEnumerable<Enrollment> Active() =>
        Existing.Concat(Added).Where(enrollment => enrollment.Status == EnrollmentStatus.Active);
}
