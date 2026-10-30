using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Queries.GetConversationMessages;

public sealed record GetConversationMessagesQuery(Guid ConversationId, string? Cursor, int PageSize = 50)
    : IQuery<ConversationMessagesPage>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsView;
}

internal sealed class GetConversationMessagesValidator : AbstractValidator<GetConversationMessagesQuery>
{
    public GetConversationMessagesValidator()
    {
        RuleFor(query => query.ConversationId).NotEmpty();
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Cursor).Must(cursor => cursor is null || ChatCursor.TryParse(cursor, out _, out _));
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetConversationMessagesHandler(IConversationReadService reads) : IQueryHandler<GetConversationMessagesQuery, ConversationMessagesPage>
{
    public async Task<Result<ConversationMessagesPage>> HandleAsync(GetConversationMessagesQuery query, CancellationToken cancellationToken)
    {
        var page = await reads.GetMessagesAsync(query.ConversationId, query.Cursor, query.PageSize, cancellationToken);
        return page is null ? Result<ConversationMessagesPage>.Failure(Error.NotFound("conversations.not_found", "Conversation not found."))
            : Result<ConversationMessagesPage>.Success(page);
    }
}
