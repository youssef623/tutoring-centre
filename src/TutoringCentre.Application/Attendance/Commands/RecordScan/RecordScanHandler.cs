using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.RecordScan;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed partial class RecordScanHandler(IAttendanceRosterSource roster, ITeachingScope teachingScope,
    IStudentCardRepository cards, IAttendanceRecordRepository records, ICurrentActor actor, IClock clock,
    ILogger<RecordScanHandler> logger) : ICommandHandler<RecordScanCommand, ScanResult>
{
    public async Task<Result<ScanResult>> HandleAsync(RecordScanCommand command, CancellationToken cancellationToken)
    {
        Guid? cardId = null;
        var result = await RecordAsync(command, cancellationToken);
        LogScan(logger, result.IsSuccess ? result.Value.Outcome.ToString() : result.Error!.Code, command.SessionId, cardId);
        return result;

        async Task<Result<ScanResult>> RecordAsync(RecordScanCommand request, CancellationToken ct)
        {
            // 3. Establish the tenant-scoped session before checking ownership.
            var session = await roster.GetSessionAsync(request.SessionId, ct);
            if (session is null) return Result<ScanResult>.Failure(AttendanceErrors.SessionNotFound());

            // 4. Hide other teachers' sessions using the same not-found result.
            if (!(await teachingScope.GetAsync(ct)).Allows(session.TeacherId))
                return Result<ScanResult>.Failure(AttendanceErrors.SessionNotFound());

            // 5. No card information is available outside an open session.
            var timing = new AttendanceSessionTiming(session.StartsAt, session.EndsAt, session.IsCancelled);
            var now = clock.UtcNow;
            var windowError = AttendanceErrors.MarkingWindow(timing, now);
            if (windowError is not null) return Result<ScanResult>.Failure(windowError);

            // 6. One indexed, tenant-scoped hash lookup. Validation already parsed the token.
            var card = await cards.FindByTokenHashAsync(CardToken.Parse(request.Token).Value.Hash(), ct);
            if (card is null)
                return Result<ScanResult>.Failure(Error.NotFound("card.not_recognised", "This card is not recognised."));
            cardId = card.Id;

            // 7. Only a known card in this centre can disclose its revoked status.
            if (card.Status != StudentCardStatus.Active)
                return Result<ScanResult>.Failure(Error.Rule("card.revoked", "This card has been revoked."));

            // 8. Resolve the student's name and enrollment only after card checks.
            var student = (await roster.GetExpectedStudentsAsync(session.SessionId, ct))
                .SingleOrDefault(student => student.StudentId == card.StudentId);
            if (student is null)
                return Result<ScanResult>.Failure(Error.Rule("attendance.student_not_in_session", "This student is not expected at this session."));

            var status = ScanPolicy.StatusFor(timing, now);
            // 9. Corrected decisions take precedence, including corrected arrivals.
            var record = (await records.GetForSessionAsync(session.SessionId, ct))
                .SingleOrDefault(record => record.StudentId == student.StudentId);
            var byUserId = actor.Actor is StaffActor staff ? (Guid?)staff.UserId : null;
            if (record is not null)
            {
                if (record.CorrectionCount == 0 && record.Status is AttendanceStatus.Present or AttendanceStatus.Late)
                    return Result<ScanResult>.Success(new(ScanOutcome.AlreadyRecorded, student.StudentId,
                        student.FullName, record.Status, record.MarkedAt));

                var marked = record.Mark(status, AttendanceSource.Qr, byUserId, timing, now);
                if (marked.IsFailure) return Result<ScanResult>.Failure(marked.Error!);
            }
            else
            {
                var created = AttendanceRecord.Create(actor.Actor.CentreId!.Value, session.SessionId,
                    student.StudentId, student.EnrollmentId, status, AttendanceSource.Qr, byUserId, timing, now);
                if (created.IsFailure) return Result<ScanResult>.Failure(created.Error!);
                record = created.Value;
                records.Add(record);
            }

            return Result<ScanResult>.Success(new(ScanOutcome.Recorded, student.StudentId,
                student.FullName, record.Status, record.MarkedAt));
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Scan {Outcome} session {SessionId} card {CardId}")]
    private static partial void LogScan(ILogger logger, string outcome, Guid sessionId, Guid? cardId);
}
