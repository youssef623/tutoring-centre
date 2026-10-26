using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory teacher repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeTeacherRepository : ITeacherRepository
{
    public List<Teacher> Existing { get; } = [];

    public List<Teacher> Added { get; } = [];

    public Task<Teacher?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(teacher => teacher.Id == id));

    public Task<Teacher?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(teacher => teacher.Id == id));

    public Task<bool> UserLinkedAsync(Guid userId, Guid? excludingTeacherId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(teacher =>
            teacher.UserId == userId && teacher.Id != excludingTeacherId));

    public void Add(Teacher teacher) => Added.Add(teacher);
}
