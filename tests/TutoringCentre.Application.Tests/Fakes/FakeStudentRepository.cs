using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory student repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeStudentRepository : IStudentRepository
{
    public List<Student> Existing { get; } = [];

    public List<Student> Added { get; } = [];

    public Task<Student?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(student => student.Id == id));

    public Task<Student?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(student => student.Id == id));

    public void Add(Student student) => Added.Add(student);
}
