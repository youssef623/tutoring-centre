namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>
/// Marks a command or query that operates on one centre's data. Tenant-scoped requests need an actor with
/// a centre; a request that runs before or outside a tenant (login, centre selection, platform-wide
/// queries) does not implement this.
/// </summary>
public interface ITenantScoped;
