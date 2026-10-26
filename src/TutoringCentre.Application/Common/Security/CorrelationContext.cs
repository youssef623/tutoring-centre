namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Holds the correlation ID for one scope. Starts null; trusted edge code (Api middleware) calls <see cref="Set"/>
/// at most once. Non-HTTP callers (seed, jobs, tests) never call it, so <see cref="CorrelationId"/> stays null.
/// </summary>
public sealed class CorrelationContext : ICorrelationContext
{
    private bool _isSet;

    public string? CorrelationId { get; private set; }

    /// <exception cref="ArgumentException"><paramref name="correlationId"/> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">The correlation ID for this scope has already been set.</exception>
    public void Set(string correlationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(correlationId);

        if (_isSet)
        {
            throw new InvalidOperationException("The correlation ID for this scope has already been set and cannot be replaced.");
        }

        CorrelationId = correlationId;
        _isSet = true;
    }
}
