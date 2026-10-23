using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory group repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeGroupRepository : IGroupRepository
{
    public List<Group> Existing { get; } = [];

    public List<Group> Added { get; } = [];

    public Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(group =>
            group.NormalizedName == normalizedName && group.Id != excludingId));

    public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.SubjectId == subjectId && group.Status == GroupStatus.Active));

    public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.TeacherId == teacherId && group.Status == GroupStatus.Active));

    public void Add(Group group) => Added.Add(group);
}
