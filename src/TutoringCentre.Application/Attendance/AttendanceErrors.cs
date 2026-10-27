using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance;

internal static class AttendanceErrors
{
    public static Error SessionNotFound() => Error.NotFound("attendance.session_not_found", "This session could not be found.");
    public static Error NotFound() => Error.NotFound("attendance.not_found", "This attendance record could not be found.");

    public static Error? MarkingWindow(AttendanceSessionTiming timing, DateTimeOffset now)
    {
        if (timing.IsCancelled)
        {
            return Error.Rule("attendance.session_cancelled", "Attendance cannot be marked for a cancelled session.");
        }

        return AttendanceWindow.StateAt(timing, now) switch
        {
            AttendanceWindowState.NotOpen => Error.Rule("attendance.session_not_open", "The attendance window has not opened yet."),
            AttendanceWindowState.Closed => Error.Rule("attendance.window_closed", "The attendance window has closed."),
            _ => null,
        };
    }

    public static Error AtMark(Error error, int index, string field = "status") => error with
    {
        Fields = new Dictionary<string, string[]> { [$"marks[{index}].{field}"] = [error.Message] },
    };
}
