using TutoringCentre.Application.Academics.Students;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for student reads.</summary>
public sealed class FakeStudentReadService : IStudentReadService
{
    public List<StudentDetailDto> Details { get; } = [];

    public StudentPageDto Page { get; set; } = new([], null);

    public Task<StudentPageDto> ListAsync(StudentSearchCriteria criteria, CancellationToken ct) => Task.FromResult(Page);

    public Task<StudentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Details.FirstOrDefault(student => student.Id == id));
}
