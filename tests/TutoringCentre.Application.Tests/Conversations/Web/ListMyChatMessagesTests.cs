using TutoringCentre.Application.Conversations.Web;
using TutoringCentre.Application.Conversations.Web.Queries.ListMyChatMessages;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Conversations.Web;

public sealed class ListMyChatMessagesTests
{
    [Fact]
    public async Task Handler_PassesOnlyPagingToActorScopedPort()
    {
        var reads = new Reads();
        var actor = new CurrentActorContext();
        actor.Set(new ParentActor(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        var handler = new ListMyChatMessagesHandler(reads, actor);
        var result = await handler.HandleAsync(new(null, 25), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(25, reads.PageSize);
        Assert.Empty(result.Value.Items);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData(null, 101)]
    [InlineData("invalid", 50)]
    public async Task InvalidPaging_IsRejected(string? cursor, int pageSize) =>
        Assert.False((await new ListMyChatMessagesValidator().ValidateAsync(new ListMyChatMessagesQuery(cursor, pageSize))).IsValid);

    [Fact]
    public void Cursor_RoundTripsTimestampAndTieBreaker()
    {
        var at = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();
        Assert.True(ChatCursor.TryParse(ChatCursor.Format(at, id), out var actualAt, out var actualId));
        Assert.Equal(at, actualAt);
        Assert.Equal(id, actualId);
    }

    private sealed class Reads : IWebChatReadService
    {
        public int PageSize { get; private set; }
        public Task<ChatMessagesPage> ListAsync(string? cursor, int pageSize, CancellationToken cancellationToken)
        {
            PageSize = pageSize;
            return Task.FromResult(new ChatMessagesPage([], null));
        }
    }
}
