namespace TutoringCentre.Application.Common.Security;

public interface IPortalScopeResolver
{
    Task<IReadOnlySet<Guid>> ResolveAsync(IPortalActor actor, CancellationToken ct);
}
