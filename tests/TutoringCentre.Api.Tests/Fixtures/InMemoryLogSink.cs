using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Captures Serilog events in memory so tests can assert on what the API logged.</summary>
public sealed class InMemoryLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        _events.Enqueue(logEvent);
    }

    public IReadOnlyList<LogEvent> Snapshot() => [.. _events];

    /// <summary>Polls until an event matches (the request-logging line can be written just after the response is sent).</summary>
    public async Task<LogEvent?> WaitForAsync(Func<LogEvent, bool> predicate, TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(5));
        while (DateTime.UtcNow < deadline)
        {
            var match = _events.FirstOrDefault(predicate);
            if (match is not null)
            {
                return match;
            }

            await Task.Delay(25);
        }

        return null;
    }

    public static string? Text(LogEvent logEvent, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        return logEvent.Properties.TryGetValue(propertyName, out var value) && value is ScalarValue { Value: string text } ? text : null;
    }
}
