using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory parent repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add
/// calls. <see cref="PhoneExistsCallCount"/> lets a test prove the uniqueness query never runs when the phone
/// is unchanged on update.
/// </summary>
public sealed class FakeParentRepository : IParentRepository
{
    public List<Parent> Existing { get; } = [];

    public List<Parent> Added { get; } = [];

    public int PhoneExistsCallCount { get; private set; }

    public Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(parent => parent.Id == id));

    public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(parent => parent.Id == id));

    public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).FirstOrDefault(parent => parent.Phone.Equals(phone)));

    public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct)
    {
        PhoneExistsCallCount++;
        return Task.FromResult(Existing.Concat(Added).Any(parent => parent.Phone.Equals(phone) && parent.Id != excludingId));
    }

    public void Add(Parent parent) => Added.Add(parent);
}
