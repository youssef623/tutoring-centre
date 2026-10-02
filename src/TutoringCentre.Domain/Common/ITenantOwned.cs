namespace TutoringCentre.Domain.Common;

/// <summary>
/// Marks an entity that belongs to exactly one centre (tenant). Infrastructure applies tenant filtering
/// to every implementer automatically (Month 2). Centre itself does not implement it: it is the tenant.
/// </summary>
public interface ITenantOwned
{
    Guid CentreId { get; }
}
