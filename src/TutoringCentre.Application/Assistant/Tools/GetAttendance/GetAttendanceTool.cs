using TutoringCentre.Application.Attendance.Contracts;
using System.Globalization;
using System.Text.Json;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Queries.GetAttendance;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Assistant.Contracts;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Tools.GetAttendance;

/// <summary>Executed by the gateway inside the freshly resolved family actor scope.</summary>
public sealed class GetAttendanceTool(Dispatcher dispatcher, IAssistantCalendar calendar) : ITool
{
    public ToolDescriptor Descriptor { get; } = new("get_attendance",
        "Read recorded attendance for a family child. Missing records do not mean absent.",
        JsonDocument.Parse("""
        {"type":"object","additionalProperties":false,"required":["childHandle"],"properties":{
          "childHandle":{"type":"string","pattern":"^(child_[0-9]+|me)$","maxLength":32},
          "date":{"type":"string","format":"date"},"from":{"type":"string","format":"date"},"to":{"type":"string","format":"date"}}}
        """).RootElement.Clone());
    public async Task<Result<ToolResult>> ExecuteAsync(ToolContext context, ToolArguments arguments, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(arguments);
        var handle = arguments.GetString("childHandle");
        var child = context.Handles.Resolve(handle);
        if (child.IsFailure) return Result<ToolResult>.Failure(Error.Validation("tool.handle_unknown", "Child handle is unknown."));
        var date = arguments.GetOptionalString("date");
        var fromText = arguments.GetOptionalString("from");
        var toText = arguments.GetOptionalString("to");
        if (date is not null && (fromText is not null || toText is not null)) return Invalid();
        var local = await calendar.GetAsync(ct);
        var zone = local.Zone;
        var today = local.Today;
        var from = Parse(date ?? fromText, today);
        var to = Parse(date ?? toText, from);
        if (to < from || to.DayNumber - from.DayNumber > 62) return Invalid();
        var page = await dispatcher.QueryAsync<GetAttendanceQuery, AttendancePageDto>(new(child.Value, null, from, to, PageSize: 31), ct);
        if (page.IsFailure) return Result<ToolResult>.Failure(page.Error!);
        var records = page.Value.Items.Select(row => new GetAttendanceRecord(row.LocalDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeZoneInfo.ConvertTime(row.StartsAt, zone).ToString("HH:mm", CultureInfo.InvariantCulture), row.Status, row.GroupName)).ToArray();
        return Result<ToolResult>.Success(new(new GetAttendanceToolResult(context.Handles.Children[handle].Name,
            from.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            records, page.Value.NextCursor is not null, records.Length == 0 ? GetAttendanceToolResult.EmptyNote : null)));
    }
    private static DateOnly Parse(string? value, DateOnly fallback) => value is null ? fallback : DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    private static Result<ToolResult> Invalid() => Result<ToolResult>.Failure(Error.Validation("tool.invalid_arguments", "Use a date or a range of at most 62 days."));
}
public sealed record GetAttendanceRecord(string Date, string StartsAt, string Status, string Group);
public sealed record GetAttendanceToolResult(string Child, string From, string To, IReadOnlyList<GetAttendanceRecord> Records, bool Truncated, string? Note)
{
    public const string EmptyNote = "No record means attendance has not been recorded (yet); it does not mean absent.";
}
