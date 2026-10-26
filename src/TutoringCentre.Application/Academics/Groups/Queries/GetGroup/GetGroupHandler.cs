using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Queries.GetGroup;

/// <summary>
/// Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all
/// answer group.not_found, never 403, so the response never confirms that the group exists.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetGroupHandler(ITeacherScopeProvider scopeProvider, IGroupReadService readService)
    : IQueryHandler<GetGroupQuery, GroupDto>
{
    public async Task<Result<GroupDto>> HandleAsync(GetGroupQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var group = await readService.GetAsync(query.GroupId, scope, cancellationToken);
        return group is null ? Result<GroupDto>.Failure(GroupErrors.NotFound()) : Result<GroupDto>.Success(group);
    }
}
