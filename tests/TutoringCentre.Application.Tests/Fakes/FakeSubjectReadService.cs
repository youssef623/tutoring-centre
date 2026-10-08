using TutoringCentre.Application.Academics.Subjects;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for subject reads. Records the last <c>includeArchived</c> flag it was asked for, so a handler test can assert it was passed through.</summary>
public sealed class FakeSubjectReadService : ISubjectReadService
{
    public List<SubjectDto> Subjects { get; } = [];

    public bool? LastIncludeArchivedRequested { get; private set; }

    public Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        LastIncludeArchivedRequested = includeArchived;
        return Task.FromResult<IReadOnlyList<SubjectDto>>(Subjects);
    }

    public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Subjects.FirstOrDefault(subject => subject.Id == id));
}
