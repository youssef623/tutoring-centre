using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;

/// <summary>
/// Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all
/// answer group.not_found, the same as reading the group itself, so the roster never confirms that the group exists.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetGroupRosterHandler(ITeacherScopeProvider scopeProvider, IEnrollmentReadService readService)
    : IQueryHandler<GetGroupRosterQuery, GroupRosterDto>
{
    public async Task<Result<GroupRosterDto>> HandleAsync(GetGroupRosterQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var roster = await readService.GetRosterAsync(query.GroupId, scope, cancellationToken);
        return roster is null ? Result<GroupRosterDto>.Failure(GroupErrors.NotFound()) : Result<GroupRosterDto>.Success(roster);
    }
}
