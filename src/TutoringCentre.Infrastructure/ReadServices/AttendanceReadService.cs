using TutoringCentre.Application.Attendance.Contracts;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class AttendanceReadService(AppDbContext db) : IAttendanceReadService
{
    public async Task<IReadOnlyList<SessionAttendanceRecordDto>> ListForSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var rows = await (from record in db.Set<AttendanceRecord>().AsNoTracking()
                          join user in db.Set<ApplicationUser>().AsNoTracking() on record.MarkedByUserId equals user.Id into markers
                          from marker in markers.DefaultIfEmpty()
                          where record.SessionId == sessionId
                          select new { record, Name = (string?)marker.DisplayName, Version = EF.Property<uint>(record, "Version") }).ToListAsync(ct);
        return rows.Select(row => new SessionAttendanceRecordDto(row.record.StudentId,
            new(row.record.Id, row.record.Status.ToString().ToLowerInvariant(), row.record.Source.ToString().ToLowerInvariant(),
                row.record.MarkedAt, row.Name, row.record.CorrectionCount, row.Version))).ToArray();
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountForSessionsAsync(IReadOnlyCollection<Guid> sessionIds, CancellationToken ct)
    {
        if (sessionIds.Count == 0) { return new Dictionary<Guid, int>(); }
        return await db.Set<AttendanceRecord>().AsNoTracking().Where(record => sessionIds.Contains(record.SessionId))
            .GroupBy(record => record.SessionId).Select(group => new { Id = group.Key, Count = group.Count() })
            .ToDictionaryAsync(row => row.Id, row => row.Count, ct);
    }

    public async Task<AttendancePageDto> ListAsync(AttendanceFilter filter, AttendanceAccess access, string? cursor, int pageSize, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ArgumentNullException.ThrowIfNull(access);
        if (!access.IsAll && access.TeacherId is null && (access.StudentIds is null || access.StudentIds.Count == 0)) { return new([], null); }

        var records = db.Set<AttendanceRecord>().AsNoTracking().AsQueryable();
        if (access.StudentIds is { } children) { records = records.Where(record => children.Contains(record.StudentId)); }
        if (filter.StudentId is { } studentId) { records = records.Where(record => record.StudentId == studentId); }
        var sessions = db.Set<ClassSession>().AsNoTracking().Where(session => session.LocalDate >= filter.From && session.LocalDate <= filter.To);
        if (filter.GroupId is { } groupId) { sessions = sessions.Where(session => session.GroupId == groupId); }
        if (access.TeacherId is not null) { sessions = sessions.Where(session => session.TeacherId == access.TeacherId); }
        var query = from record in records
                    join session in sessions on record.SessionId equals session.Id
                    join studyGroup in db.Set<Group>().AsNoTracking() on session.GroupId equals studyGroup.Id
                    join subject in db.Set<Subject>().AsNoTracking() on studyGroup.SubjectId equals subject.Id
                    join student in db.Set<Student>().AsNoTracking() on record.StudentId equals student.Id
                    select new { record, session.LocalDate, session.StartsAt, session.GroupId, GroupName = studyGroup.Name,
                        SubjectName = subject.Name, StudentName = student.FullName };
        if (cursor is not null && AttendanceCursor.TryDecode(cursor, out var after))
        {
            query = query.Where(row => row.LocalDate < after.Date
                || (row.LocalDate == after.Date && row.StartsAt < after.StartsAt)
                || (row.LocalDate == after.Date && row.StartsAt == after.StartsAt && row.record.Id.CompareTo(after.Id) < 0));
        }

        var rows = await query.OrderByDescending(row => row.LocalDate).ThenByDescending(row => row.StartsAt)
            .ThenByDescending(row => row.record.Id).Take(pageSize + 1).ToListAsync(ct);
        var page = rows.Take(pageSize).ToArray();
        var next = rows.Count > pageSize ? AttendanceCursor.Encode(page[^1].LocalDate, page[^1].StartsAt, page[^1].record.Id) : null;
        return new(page.Select(row => new AttendanceHistoryItemDto(row.record.Id, row.record.SessionId, row.LocalDate, row.StartsAt,
            row.GroupId, row.GroupName, row.SubjectName, row.record.StudentId, row.StudentName,
            row.record.Status.ToString().ToLowerInvariant(), row.record.Source.ToString().ToLowerInvariant(), row.record.CorrectionCount)).ToArray(), next);
    }
}
