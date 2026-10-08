using TutoringCentre.Application.Academics.Subjects;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Day 24 placeholder, registered only so the Subject query handlers (every one depends on
/// <see cref="ISubjectReadService"/>) resolve at container-validation time. Nothing calls it today: no endpoint
/// exposes a Subject use case until Day 26, and Day 24's own handler tests use a fake, not this registration.
/// The real implementation arrives Day 25.
/// </summary>
internal sealed class SubjectReadService : ISubjectReadService
{
    private const string NotImplementedMessage = "The Subject read service is not implemented until Day 25.";

    public Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct) =>
        throw new NotImplementedException(NotImplementedMessage);

    public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>
        throw new NotImplementedException(NotImplementedMessage);
}
