using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Portal;

[Collection(ApiCollection.Name)]
public sealed class PortalChatEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri Messages = new("/api/portal/chat/messages", UriKind.Relative);
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        await factory.ExecuteAsync($"""
            insert into conversations.channel_accounts (id,centre_id,provider,external_account_id,status,created_at)
            values ('{Guid.NewGuid()}','{centre}','web','web:nile-centre','active',now());
            """);
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("parent")]
    [InlineData("student")]
    public async Task Portal_SendListReplay_HasSafeContract(string role)
    {
        using var actor = await PortalAsync(role);
        var clientMessageId = Guid.NewGuid();
        using var first = await SendAsync(actor.Client, clientMessageId, "هو أحمد حضر النهارده؟");
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        using var firstBody = await JsonAsync(first);
        Assert.False(firstBody.RootElement.GetProperty("alreadyRecorded").GetBoolean());
        using var repeat = await SendAsync(actor.Client, clientMessageId, "repeat");
        Assert.Equal(HttpStatusCode.OK, repeat.StatusCode);
        using var repeatBody = await JsonAsync(repeat);
        Assert.True(repeatBody.RootElement.GetProperty("alreadyRecorded").GetBoolean());
        Assert.Equal(firstBody.RootElement.GetProperty("messageId").GetGuid(), repeatBody.RootElement.GetProperty("messageId").GetGuid());
        using var list = await actor.Client.GetAsync(Messages);
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal("no-store", list.Headers.CacheControl?.ToString());
        using var body = await JsonAsync(list);
        var item = Assert.Single(body.RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(["createdAt", "direction", "id", "status", "text"], item.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
        Assert.Equal("inbound", item.GetProperty("direction").GetString());
        Assert.Equal("هو أحمد حضر النهارده؟", item.GetProperty("text").GetString());
        Assert.Equal(1L, await factory.ScalarAsync<long>("select count(*) from conversations.inbound_messages"));

        var conversationId = await factory.ScalarAsync<Guid>("select id from conversations.conversations");
        var centreId = await factory.ScalarAsync<Guid>("select centre_id from conversations.conversations");
        await factory.ExecuteAsync($"""
            insert into conversations.outbound_messages (id,centre_id,conversation_id,text,status,failure_code,created_at)
            values ('{Guid.NewGuid()}','{centreId}','{conversationId}','stored failed reply','failed','delivery.failed',now());
            """);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var staffList = await owner.Client.GetAsync(new Uri("/api/conversations", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, staffList.StatusCode);
        using var staffListBody = await JsonAsync(staffList);
        Assert.Equal("web", Assert.Single(staffListBody.RootElement.GetProperty("items").EnumerateArray()).GetProperty("channel").GetString());
        using var thread = await owner.Client.GetAsync(new Uri($"/api/conversations/{conversationId}/messages", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, thread.StatusCode);
        using var threadBody = await JsonAsync(thread);
        var outbound = Assert.Single(threadBody.RootElement.GetProperty("items").EnumerateArray(), message => message.GetProperty("direction").GetString() == "outbound");
        Assert.Equal("failed", outbound.GetProperty("status").GetString());
        Assert.Equal("delivery.failed", outbound.GetProperty("failureCode").GetString());
        Assert.Equal(["channel", "correlationId", "createdAt", "direction", "failureCode", "id", "processedAt", "sentAt", "status", "text"],
            outbound.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task Staff_Matrix_DeniesBothPortalRoutesAndChecksStaffRoutes(string email, HttpStatusCode staffStatus)
    {
        using var actor = email == "teacher" ? await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre") : await SignInHelper.SignInAsync(factory, email);
        using var list = await actor.Client.GetAsync(Messages);
        using var send = await SendAsync(actor.Client, Guid.NewGuid(), "valid text");
        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, send.StatusCode);
        using var conversations = await actor.Client.GetAsync(new Uri("/api/conversations", UriKind.Relative));
        Assert.Equal(staffStatus, conversations.StatusCode);
        using var missing = await actor.Client.GetAsync(new Uri($"/api/conversations/{Guid.NewGuid()}/messages", UriKind.Relative));
        Assert.Equal(staffStatus == HttpStatusCode.OK ? HttpStatusCode.NotFound : staffStatus, missing.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task InvalidText_Is400(string? text)
    {
        using var actor = await PortalAsync("parent");
        using var response = await SendAsync(actor.Client, Guid.NewGuid(), text);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var overlong = await SendAsync(actor.Client, Guid.NewGuid(), new string('x', 1001));
        Assert.Equal(HttpStatusCode.BadRequest, overlong.StatusCode);
    }

    [Fact]
    public async Task TwentyFirstSend_Is429_AndReadsRemainAvailable()
    {
        using var actor = await PortalAsync("parent");
        for (var index = 0; index < 20; index++)
        {
            using var response = await SendAsync(actor.Client, Guid.NewGuid(), "rate test");
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        using var limited = await SendAsync(actor.Client, Guid.NewGuid(), "limited");
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        using var list = await actor.Client.GetAsync(Messages);
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
    }

    [Theory]
    [InlineData("parent")]
    [InlineData("student")]
    public async Task Portal_CannotAccessStaffRoutes(string role)
    {
        using var actor = await PortalAsync(role);
        using var list = await actor.Client.GetAsync(new Uri("/api/conversations", UriKind.Relative));
        using var thread = await actor.Client.GetAsync(new Uri($"/api/conversations/{Guid.NewGuid()}/messages", UriKind.Relative));
        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, thread.StatusCode);
    }

    private async Task<SignedInClient> PortalAsync(string role)
    {
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
        var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
        var subject = await factory.ScalarAsync<Guid>($"select id from academics.{(role == "parent" ? "parents" : "students")} where centre_id='{centre}' order by id limit 1");
        await factory.ExecuteAsync($"""
            update identity.memberships set role='{role}' where centre_id='{centre}' and user_id='{user}';
            insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
            values ('{Guid.NewGuid()}','{centre}','{user}','{role}',{(role == "parent" ? $"'{subject}'" : "null")},{(role == "student" ? $"'{subject}'" : "null")},now(),'{owner}');
            """);
        return await SignInHelper.SignInAsync(factory, "secretary@nile.test");
    }
    private static Task<HttpResponseMessage> SendAsync(HttpClient client, Guid clientMessageId, string? text) =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, Messages, new { clientMessageId, text });
    private static async Task<JsonDocument> JsonAsync(HttpResponseMessage response) => JsonDocument.Parse(await response.Content.ReadAsStringAsync());
}
