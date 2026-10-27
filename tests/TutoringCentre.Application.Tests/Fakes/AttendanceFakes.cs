using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Tests.Fakes;

public sealed class FakeAttendanceRosterSource : IAttendanceRosterSource
{
    public Dictionary<Guid, AttendanceSessionFacts> Sessions { get; } = [];
    public Dictionary<Guid, IReadOnlyList<ExpectedStudent>> Students { get; } = [];
    public List<string> Calls { get; } = [];

    public Task<AttendanceSessionFacts?> GetSessionAsync(Guid sessionId, CancellationToken ct)
    {
        Calls.Add("session");
        return Task.FromResult(Sessions.GetValueOrDefault(sessionId));
    }

    public Task<IReadOnlyList<ExpectedStudent>> GetExpectedStudentsAsync(Guid sessionId, CancellationToken ct)
    {
        Calls.Add("expected");
        return Task.FromResult(Students.GetValueOrDefault(sessionId) ?? (IReadOnlyList<ExpectedStudent>)[]);
    }

    public Task<IReadOnlyList<SessionOnDate>> ListSessionsOnDateAsync(DateOnly localDate, Guid? teacherId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<SessionOnDate>>([]);
}

public sealed class FakeTeachingScope : ITeachingScope
{
    public TeachingScope Scope { get; set; } = TeachingScope.None;
    public int Calls { get; private set; }

    public Task<TeachingScope> GetAsync(CancellationToken ct)
    {
        Calls++;
        return Task.FromResult(Scope);
    }
}

public sealed class FakeAttendanceRecordRepository : IAttendanceRecordRepository
{
    public List<AttendanceRecord> Existing { get; } = [];
    public List<AttendanceRecord> Added { get; } = [];
    public IEnumerable<AttendanceRecord> Records => Existing.Concat(Added);
    public uint? ExpectedVersion { get; private set; }
    public int SessionReads { get; private set; }

    public Task<IReadOnlyList<AttendanceRecord>> GetForSessionAsync(Guid sessionId, CancellationToken ct)
    {
        SessionReads++;
        return Task.FromResult<IReadOnlyList<AttendanceRecord>>(Records.Where(record => record.SessionId == sessionId).ToArray());
    }

    public Task<AttendanceRecord?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct)
    {
        ExpectedVersion = expectedVersion;
        return Task.FromResult(Records.SingleOrDefault(record => record.Id == id));
    }

    public void Add(AttendanceRecord record) => Added.Add(record);
}
