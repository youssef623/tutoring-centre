using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Holds two empty marking reads so both HTTP requests attempt their first insert.</summary>
public sealed class AttendanceMarkRace : DbCommandInterceptor
{
    private TaskCompletionSource? _both;
    private int _arrivals;
    public void Enable() { _arrivals = 0; _both = new(TaskCreationOptions.RunContinuationsAsynchronously); }
    public void Disable() { _both?.TrySetResult(); _both = null; }
    public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result, CancellationToken cancellationToken = default)
    {
        var both = _both;
        if (both is not null && command.CommandText.Contains("attendance.attendance_records", StringComparison.Ordinal)
            && command.CommandText.Contains("for update", StringComparison.OrdinalIgnoreCase))
        {
            if (Interlocked.Increment(ref _arrivals) == 2) { both.TrySetResult(); }
            await both.Task.WaitAsync(TimeSpan.FromSeconds(20), cancellationToken);
        }
        return result;
    }
}
