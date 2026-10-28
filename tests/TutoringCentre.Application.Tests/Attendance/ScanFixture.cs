using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.RecordScan;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Attendance;

internal sealed class ScanFixture
{
    public AttendanceHandlerFixture Attendance { get; } = new();
    public ScanCards Cards { get; } = new();
    public ScanLogger Log { get; } = new();
    public CardToken Token { get; } = CardToken.Generate();
    public StudentCard Card { get; }
    public ScanFixture()
    {
        Card = StudentCard.Issue(Attendance.Staff.CentreId!.Value, Attendance.Students[0].StudentId,
            Token, Attendance.Staff.UserId, Attendance.Clock.UtcNow).Value;
        Cards.Items.Add(Card);
    }
    public Task<Result<ScanResult>> Scan(Guid? sessionId = null) => new RecordScanHandler(
        Attendance.Roster, Attendance.Scope, Cards, Attendance.Records, Attendance.Actor, Attendance.Clock, Log)
        .HandleAsync(new(sessionId ?? Attendance.Session.SessionId, Token.Reveal()), CancellationToken.None);
    public void Revoke() => Card.Revoke(CardRevocationReason.Lost, Attendance.Staff.UserId, Attendance.Clock.UtcNow);
    public AttendanceRecord Existing(AttendanceStatus status)
    {
        var a = Attendance;
        var student = a.Students[0];
        var record = AttendanceRecord.Create(a.Staff.CentreId!.Value, a.Session.SessionId, student.StudentId,
            student.EnrollmentId, status, AttendanceSource.Manual, a.Staff.UserId,
            new(a.Session.StartsAt, a.Session.EndsAt, false), a.Clock.UtcNow).Value;
        a.Records.Existing.Add(record);
        return record;
    }
}

internal sealed class ScanCards : IStudentCardRepository
{
    public List<StudentCard> Items { get; } = [];
    public int Reads { get; private set; }
    public Task<StudentCard?> FindByTokenHashAsync(byte[] tokenHash, CancellationToken ct)
    {
        Reads++;
        return Task.FromResult(Items.SingleOrDefault(card => card.Matches(tokenHash)));
    }
    public Task<StudentCard?> GetActiveAsync(Guid studentId, CancellationToken ct) => throw new NotSupportedException();
    public Task<IReadOnlyList<StudentCard>> GetActiveAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken ct) => throw new NotSupportedException();
    public Task<StudentCard?> GetByIdAsync(Guid cardId, uint expectedVersion, CancellationToken ct) => throw new NotSupportedException();
    public void Add(StudentCard card) => Items.Add(card);
}

internal sealed class ScanLogger : ILogger<RecordScanHandler>
{
    public List<(LogLevel Level, IReadOnlyList<KeyValuePair<string, object?>> State)> Entries { get; } = [];
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Entries.Add((logLevel, ((IEnumerable<KeyValuePair<string, object?>>)state!).ToArray()));
}
