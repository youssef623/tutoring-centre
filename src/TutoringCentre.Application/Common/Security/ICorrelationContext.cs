namespace TutoringCentre.Application.Common.Security;

/// <summary>Read-only access to the current scope's correlation ID (HTTP request or job). Null when none was set.</summary>
public interface ICorrelationContext
{
    string? CorrelationId { get; }
}
