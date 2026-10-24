using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;

/// <summary>
/// Uniform not-found: an unknown ID, another centre's group and a group outside the actor's teacher scope all
/// answer group.not_found, the same as reading the group itself.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListGroupScheduleRulesHandler(ITeacherScopeProvider scopeProvider, IScheduleReadService readService)
    : IQueryHandler<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>
{
    public async Task<Result<IReadOnlyList<ScheduleRuleDto>>> HandleAsync(ListGroupScheduleRulesQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var rules = await readService.ListRulesAsync(query.GroupId, scope, cancellationToken);
        return rules is null
            ? Result<IReadOnlyList<ScheduleRuleDto>>.Failure(GroupErrors.NotFound())
            : Result<IReadOnlyList<ScheduleRuleDto>>.Success(rules);
    }
}
