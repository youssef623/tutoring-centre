using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Attendance;

public interface IStudentCardRepository
{
    Task<StudentCard?> GetActiveAsync(Guid studentId, CancellationToken ct);
    Task<IReadOnlyList<StudentCard>> GetActiveAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken ct);
    Task<StudentCard?> GetByIdAsync(Guid cardId, uint expectedVersion, CancellationToken ct);
    Task<StudentCard?> FindByTokenHashAsync(byte[] tokenHash, CancellationToken ct);
    void Add(StudentCard card);
}
