using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AttendanceRecordRepository(AppDbContext db) : IAttendanceRecordRepository
{
    // Existing marks serialize before being read, so unversioned re-marking uses the latest state and xmin.
    // No row is locked for a first mark; the unique index still arbitrates concurrent creation.
    public async Task<IReadOnlyList<AttendanceRecord>> GetForSessionAsync(Guid sessionId, CancellationToken ct) =>
        await db.Set<AttendanceRecord>().FromSqlInterpolated($"""
            select *, xmin from attendance.attendance_records where session_id = {sessionId} order by id for update
            """).ToListAsync(ct);

    public async Task<AttendanceRecord?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        var record = await db.Set<AttendanceRecord>().SingleOrDefaultAsync(record => record.Id == id, ct);
        if (record is not null)
        {
            db.Entry(record).Property("Version").OriginalValue = expectedVersion;
        }

        return record;
    }

    public void Add(AttendanceRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        db.Add(record);
    }
}
