namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Thrown by <see cref="Interceptors.TenantWriteGuardInterceptor"/> when a save would cross a tenant boundary.
/// Nothing below the dispatcher catches it; the global exception handler turns it into a generic 500.
/// </summary>
public sealed class TenantViolationException : Exception
{
    public TenantViolationException(Type entityType)
        : base($"Save rejected: '{entityType.Name}' crosses a tenant boundary.")
    {
    }
}
