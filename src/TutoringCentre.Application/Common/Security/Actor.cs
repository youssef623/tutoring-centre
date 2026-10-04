using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Who is executing a use case and in which centre. Exactly one per request or job scope;
/// handlers read it, they never receive identity or tenant from request data.
/// </summary>
public abstract record Actor
{
    /// <summary>The centre (tenant) the actor acts in; null when no centre applies.</summary>
    public abstract Guid? CentreId { get; }
}

/// <summary>The application itself: background jobs, the seed command, CLI tools. May act in one centre or platform-wide (null).</summary>
public sealed record SystemActor(Guid? CentreId) : Actor
{
    public override Guid? CentreId { get; } = CentreId;
}

/// <summary>No authenticated caller (for example the login request itself). Never has a centre.</summary>
public sealed record AnonymousActor : Actor
{
    public override Guid? CentreId => null;
}

/// <summary>
/// A signed-in staff member. <see cref="CentreId"/> is null right after login, before a centre is selected;
/// <see cref="Role"/> is the actor's role in that centre and is null whenever <see cref="CentreId"/> is.
/// </summary>
public sealed record StaffActor(Guid UserId, Guid? CentreId, StaffRole? Role) : Actor
{
    public override Guid? CentreId { get; } = CentreId;
}
