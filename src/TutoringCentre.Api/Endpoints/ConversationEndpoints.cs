using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Application.Conversations.Queries.GetConversationMessages;
using TutoringCentre.Application.Conversations.Queries.ListConversations;

namespace TutoringCentre.Api.Endpoints;

public static class ConversationEndpoints
{
    public static RouteGroupBuilder MapConversationEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/conversations", async (HttpContext context, Dispatcher dispatcher, CancellationToken ct) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            var result = await dispatcher.QueryAsync<ListConversationsQuery, ConversationPage>(new(), ct);
            return result.ToHttpResult(value => Results.Ok(value));
        }).WithName("ListConversations").Produces<ConversationPage>().ProducesProblem(403);
        api.MapGet("/conversations/{conversationId:guid}/messages", async (Guid conversationId, string? cursor, int? pageSize, HttpContext context, Dispatcher dispatcher, CancellationToken ct) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            var result = await dispatcher.QueryAsync<GetConversationMessagesQuery, ConversationMessagesPage>(new(conversationId, cursor, pageSize ?? 50), ct);
            return result.ToHttpResult(value => Results.Ok(value));
        }).WithName("GetConversationMessages").Produces<ConversationMessagesPage>().ProducesValidationProblem().ProducesProblem(403).ProducesProblem(404);
        return api;
    }
}
