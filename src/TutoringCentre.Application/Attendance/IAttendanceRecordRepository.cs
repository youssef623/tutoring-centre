using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Attendance;

public interface IAttendanceRecordRepository
{
    Task<IReadOnlyList<AttendanceRecord>> GetForSessionAsync(Guid sessionId, CancellationToken ct);
    Task<AttendanceRecord?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);
    bool IsAtVersion(AttendanceRecord record, uint expectedVersion);
    void Add(AttendanceRecord record);
}
