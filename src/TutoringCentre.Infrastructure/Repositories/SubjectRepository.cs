using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Repositories;

/// <summary>
/// Day 24 placeholder, registered only so the Subject command handlers (every one depends on
/// <see cref="ISubjectRepository"/>) resolve at container-validation time. Nothing calls it today: no endpoint
/// exposes a Subject use case until Day 26, and Day 24's own handler tests use a fake, not this registration.
/// The real EF implementation, backed by the AppDbContext added on Day 23, arrives Day 25.
/// </summary>
internal sealed class SubjectRepository : ISubjectRepository
{
    private const string NotImplementedMessage = "The Subject repository is not implemented until Day 25.";

    public Task<Subject?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        throw new NotImplementedException(NotImplementedMessage);

    public Task<bool> NameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct) =>
        throw new NotImplementedException(NotImplementedMessage);

    public void Add(Subject subject) => throw new NotImplementedException(NotImplementedMessage);
}
