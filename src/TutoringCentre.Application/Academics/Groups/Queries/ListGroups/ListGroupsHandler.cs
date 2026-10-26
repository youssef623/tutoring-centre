using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Queries.ListGroups;

/// <summary>Resolves the actor's teacher scope and hands it to the read service, which applies it in SQL.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ListGroupsHandler(ITeacherScopeProvider scopeProvider, IGroupReadService readService)
    : IQueryHandler<ListGroupsQuery, IReadOnlyList<GroupDto>>
{
    public async Task<Result<IReadOnlyList<GroupDto>>> HandleAsync(ListGroupsQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var criteria = new GroupListCriteria(query.SubjectId, query.TeacherId, query.IncludeArchived);
        var groups = await readService.ListAsync(criteria, scope, cancellationToken);
        return Result<IReadOnlyList<GroupDto>>.Success(groups);
    }
}
