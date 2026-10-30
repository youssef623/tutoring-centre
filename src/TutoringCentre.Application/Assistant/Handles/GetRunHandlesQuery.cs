using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Assistant.Handles;
public sealed record GetRunHandlesQuery : IQuery<HandleTable>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalChat;
}
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetRunHandlesHandler(ICurrentActor current, IPortalScopeResolver resolver, IPortalDirectory directory)
    : IQueryHandler<GetRunHandlesQuery, HandleTable>
{
    public async Task<Result<HandleTable>> HandleAsync(GetRunHandlesQuery query, CancellationToken cancellationToken)
    {
        if (current.Actor is not IPortalActor actor) return Result<HandleTable>.Failure(HandleError.Unknown);
        var allowed = await resolver.ResolveAsync(actor, cancellationToken);
        IReadOnlyList<PortalChild> children;
        if (current.Actor is StudentActor)
        {
            var student = await directory.GetStudentAsync(actor.SubjectId, cancellationToken);
            children = student is { IsActive: true } && allowed.Contains(student.Id) ? [new(student.Id, student.DisplayName)] : [];
        }
        else children = (await directory.GetChildrenAsync(actor.SubjectId, cancellationToken)).Where(x => allowed.Contains(x.Id)).ToArray();
        return Result<HandleTable>.Success(HandleResolver.Mint(children, current.Actor is StudentActor));
    }
}
