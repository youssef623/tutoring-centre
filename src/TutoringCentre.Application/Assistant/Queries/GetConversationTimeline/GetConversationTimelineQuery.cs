using TutoringCentre.Application.Conversations.Contracts;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Assistant.Queries.GetConversationTimeline;
public sealed record GetConversationTimelineQuery(Guid ConversationId, string? Cursor, int PageSize = 50)
    : IQuery<ConversationTimelinePage>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AssistantView;
}
internal sealed class GetConversationTimelineValidator : AbstractValidator<GetConversationTimelineQuery>
{
    public GetConversationTimelineValidator()
    {
        RuleFor(q => q.ConversationId).NotEmpty();
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
        RuleFor(q => q.Cursor).Must(cursor => cursor is null || ChatCursor.TryParse(cursor, out _, out _));
    }
}
internal sealed class GetConversationTimelineHandler(IAssistantReadService reads) : IQueryHandler<GetConversationTimelineQuery, ConversationTimelinePage>
{
    public async Task<Result<ConversationTimelinePage>> HandleAsync(GetConversationTimelineQuery query, CancellationToken cancellationToken)
    {
        var page = await reads.GetTimelineAsync(query.ConversationId, query.Cursor, query.PageSize, cancellationToken);
        return page is null ? Result<ConversationTimelinePage>.Failure(Error.NotFound("conversations.not_found", "Conversation not found."))
            : Result<ConversationTimelinePage>.Success(page);
    }
}
