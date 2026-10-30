using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;

namespace TutoringCentre.Api.Endpoints;

public static class PortalChatEndpoints
{
    public static RouteGroupBuilder MapPortalChatEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/portal/chat/messages", ListAsync).WithName("ListMyChatMessages")
            .Produces<ChatMessagesPage>().ProducesValidationProblem().ProducesProblem(403);
        api.MapPost("/portal/chat/messages", SendAsync).WithName("SendPortalChatMessage")
            .RequireRateLimiting(ChatRateLimiting.PolicyName)
            .Produces<SendWebMessageResult>(201).Produces<SendWebMessageResult>().ProducesValidationProblem().ProducesProblem(403).ProducesProblem(429);
        return api;
    }

    private static async Task<IResult> ListAsync(string? cursor, int? pageSize, HttpContext context, Dispatcher dispatcher, CancellationToken ct)
    {
        context.Response.Headers.CacheControl = "no-store";
        var result = await dispatcher.QueryAsync<ListMyChatMessagesQuery, ChatMessagesPage>(new(cursor, pageSize ?? 50), ct);
        return result.ToHttpResult(value => Results.Ok(value));
    }

    private static async Task<IResult> SendAsync(SendPortalChatRequest request, HttpContext context, Dispatcher dispatcher, CancellationToken ct)
    {
        context.Response.Headers.CacheControl = "no-store";
        var result = await dispatcher.SendAsync<SendWebMessageCommand, SendWebMessageResult>(new(request.ClientMessageId, request.Text), ct);
        return result.ToHttpResult(value => value.AlreadyRecorded ? Results.Ok(value) : Results.Created("/api/portal/chat/messages", value));
    }
}

public sealed record SendPortalChatRequest(Guid ClientMessageId, string Text);
