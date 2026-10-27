using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Commands.MarkAttendance;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class MarkAttendanceHandler(IAttendanceRosterSource roster, ITeachingScope teachingScope,
    IAttendanceRecordRepository records, ICurrentActor actor, IClock clock)
    : ICommandHandler<MarkAttendanceCommand, MarkAttendanceResult>
{
    public async Task<Result<MarkAttendanceResult>> HandleAsync(MarkAttendanceCommand command, CancellationToken cancellationToken)
    {
        var session = await roster.GetSessionAsync(command.SessionId, cancellationToken);
        if (session is null || !(await teachingScope.GetAsync(cancellationToken)).Allows(session.TeacherId))
        {
            return Result<MarkAttendanceResult>.Failure(AttendanceErrors.SessionNotFound());
        }

        var timing = new AttendanceSessionTiming(session.StartsAt, session.EndsAt, session.IsCancelled);
        var now = clock.UtcNow;
        var windowError = AttendanceErrors.MarkingWindow(timing, now);
        if (windowError is not null)
        {
            return Result<MarkAttendanceResult>.Failure(windowError);
        }

        var expected = (await roster.GetExpectedStudentsAsync(session.SessionId, cancellationToken))
            .ToDictionary(student => student.StudentId);
        for (var index = 0; index < command.Marks.Count; index++)
        {
            if (!expected.ContainsKey(command.Marks[index].StudentId))
            {
                return Result<MarkAttendanceResult>.Failure(AttendanceErrors.AtMark(
                    Error.Rule("attendance.student_not_in_session", "This student is not expected at this session."), index, "studentId"));
            }
        }

        var existing = (await records.GetForSessionAsync(session.SessionId, cancellationToken))
            .ToDictionary(record => record.StudentId);
        var byUserId = actor.Actor is StaffActor staff ? (Guid?)staff.UserId : null;
        var created = 0;
        var updated = 0;
        var unchanged = 0;
        for (var index = 0; index < command.Marks.Count; index++)
        {
            var mark = command.Marks[index];
            if (existing.TryGetValue(mark.StudentId, out var record))
            {
                var result = record.Mark(mark.Status, AttendanceSource.Manual, byUserId, timing, now);
                if (result.IsFailure)
                {
                    return Result<MarkAttendanceResult>.Failure(AttendanceErrors.AtMark(result.Error!, index));
                }

                if (result.Value == MarkOutcome.Updated)
                {
                    updated++;
                }
                else
                {
                    unchanged++;
                }
            }
            else
            {
                var result = AttendanceRecord.Create(actor.Actor.CentreId!.Value, session.SessionId, mark.StudentId,
                    expected[mark.StudentId].EnrollmentId, mark.Status, AttendanceSource.Manual, byUserId, timing, now);
                if (result.IsFailure)
                {
                    return Result<MarkAttendanceResult>.Failure(AttendanceErrors.AtMark(result.Error!, index));
                }

                records.Add(result.Value);
                created++;
            }
        }

        return Result<MarkAttendanceResult>.Success(new(created, updated, unchanged));
    }
}
