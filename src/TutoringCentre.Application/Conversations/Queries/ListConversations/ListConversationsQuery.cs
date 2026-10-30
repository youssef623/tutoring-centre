using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Queries.ListConversations;

public sealed record ListConversationsQuery : IQuery<ConversationPage>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsView;
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ListConversationsHandler(IConversationReadService reads) : IQueryHandler<ListConversationsQuery, ConversationPage>
{
    public async Task<Result<ConversationPage>> HandleAsync(ListConversationsQuery query, CancellationToken cancellationToken) =>
        Result<ConversationPage>.Success(await reads.ListAsync(cancellationToken));
}
