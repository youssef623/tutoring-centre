namespace TutoringCentre.Application.Conversations.Web;

public interface IWebChatReadService
{
    Task<ChatMessagesPage> ListAsync(string? cursor, int pageSize, CancellationToken cancellationToken);
}
