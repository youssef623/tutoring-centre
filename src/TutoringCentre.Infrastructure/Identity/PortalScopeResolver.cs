using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>Scoped to one request; only a subject is cached in the session, never this student set.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class PortalScopeResolver(ICurrentActor currentActor, IPortalDirectory directory) : IPortalScopeResolver
{
    private IReadOnlySet<Guid>? _cached;

    public async Task<IReadOnlySet<Guid>> ResolveAsync(IPortalActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(actor);
        if (currentActor.Actor is not IPortalActor current || current.UserId != actor.UserId
            || current.CentreId != actor.CentreId || current.SubjectKind != actor.SubjectKind || current.SubjectId != actor.SubjectId)
            return FrozenSet<Guid>.Empty;

        if (_cached is not null)
            return _cached;
        _cached = actor.SubjectKind switch
        {
            PortalSubjectKind.Parent => (await directory.GetChildrenAsync(actor.SubjectId, ct)).Select(child => child.Id).ToFrozenSet(),
            PortalSubjectKind.Student => new[] { actor.SubjectId }.ToFrozenSet(),
            _ => FrozenSet<Guid>.Empty,
        };
        return _cached;
    }
}
