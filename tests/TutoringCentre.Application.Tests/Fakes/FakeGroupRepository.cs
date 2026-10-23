using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory group repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls;
/// <see cref="ActiveEnrollmentCounts"/> is the count <see cref="GetForEnrollmentAsync"/> reports (0 when absent).
/// No lock: concurrency is proven against a real database, not here.
/// </summary>
public sealed class FakeGroupRepository : IGroupRepository
{
    public List<Group> Existing { get; } = [];

    public List<Group> Added { get; } = [];

    public Dictionary<Guid, int> ActiveEnrollmentCounts { get; } = [];

    public Task<Group?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<Group?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(group => group.Id == id));

    public Task<GroupForEnrollment?> GetForEnrollmentAsync(Guid groupId, CancellationToken ct)
    {
        var group = Existing.FirstOrDefault(group => group.Id == groupId);
        return Task.FromResult(group is null
            ? null
            : new GroupForEnrollment(group, ActiveEnrollmentCounts.GetValueOrDefault(groupId)));
    }

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(group =>
            group.NormalizedName == normalizedName && group.Id != excludingId));

    public Task<bool> AnyActiveForSubjectAsync(Guid subjectId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.SubjectId == subjectId && group.Status == GroupStatus.Active));

    public Task<bool> AnyActiveForTeacherAsync(Guid teacherId, CancellationToken ct) =>
        Task.FromResult(Existing.Any(group => group.TeacherId == teacherId && group.Status == GroupStatus.Active));

    public void Add(Group group) => Added.Add(group);
}
