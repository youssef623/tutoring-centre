using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory subject repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeSubjectRepository : ISubjectRepository
{
    public List<Subject> Existing { get; } = [];

    public List<Subject> Added { get; } = [];

    public Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(subject => subject.Id == id));

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(subject =>
            subject.NormalizedName == normalizedName && subject.Id != excludingId));

    public void Add(Subject subject) => Added.Add(subject);
}
