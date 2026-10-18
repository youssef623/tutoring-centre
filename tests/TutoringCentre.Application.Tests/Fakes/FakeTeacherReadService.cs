using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for teacher reads. Records the last <c>includeArchived</c> flag it was asked for, so a handler test can assert it was passed through.</summary>
public sealed class FakeTeacherReadService : ITeacherReadService
{
    public List<TeacherDto> Teachers { get; } = [];

    public bool? LastIncludeArchivedRequested { get; private set; }

    public Task<IReadOnlyList<TeacherDto>> ListAsync(bool includeArchived, CancellationToken ct)
    {
        LastIncludeArchivedRequested = includeArchived;
        return Task.FromResult<IReadOnlyList<TeacherDto>>(Teachers);
    }

    public Task<TeacherDto?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Teachers.FirstOrDefault(teacher => teacher.Id == id));

    public Task<Guid?> FindIdByUserAsync(Guid userId, CancellationToken ct) =>
        Task.FromResult(Teachers.FirstOrDefault(teacher => teacher.UserId == userId && teacher.Status == TeacherStatus.Active)?.Id);
}
