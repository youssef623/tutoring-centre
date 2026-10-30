using TutoringCentre.Application.Conversations.Contracts;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;

public sealed record ListMyChatMessagesQuery(string? Cursor, int PageSize = 50) : IQuery<ChatMessagesPage>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.PortalChat;
}

internal sealed class ListMyChatMessagesValidator : AbstractValidator<ListMyChatMessagesQuery>
{
    public ListMyChatMessagesValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Cursor).Must(cursor => cursor is null || ChatCursor.TryParse(cursor, out _, out _));
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ListMyChatMessagesHandler(IWebChatReadService reads, ICurrentActor actor) : IQueryHandler<ListMyChatMessagesQuery, ChatMessagesPage>
{
    public async Task<Result<ChatMessagesPage>> HandleAsync(ListMyChatMessagesQuery query, CancellationToken cancellationToken) =>
        actor.Actor is IPortalActor
            ? Result<ChatMessagesPage>.Success(await reads.ListAsync(query.Cursor, query.PageSize, cancellationToken))
            : Result<ChatMessagesPage>.Failure(Error.Forbidden("chat.portal_actor_required", "A portal session is required."));
}
