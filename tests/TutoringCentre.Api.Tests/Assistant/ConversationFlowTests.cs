using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure;
using TutoringCentre.Infrastructure.Messaging;
namespace TutoringCentre.Api.Tests.Assistant;
[Collection(ApiCollection.Name)]
public sealed class ConversationFlowTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        factory.Time.UseRealTime();
        await factory.ResetAsync(); Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        await factory.ExecuteAsync($"insert into conversations.channel_accounts (id,centre_id,provider,external_account_id,status,created_at) values ('{Guid.NewGuid()}','{centre}','web','web:nile-centre','active',now())");
    }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }
    [Theory]
    [InlineData("identifier", "guard.identifier")]
    [InlineData("attendance", "guard.ungrounded")]
    public async Task UnsafeCandidatesAreReplacedBeforeStorage(string scenario, string reason)
    {
        using var actor = await PortalAsync("parent");
        var candidate = scenario == "identifier" ? "child_999 is ready" : "Ahmed attended today";
        await using var services = new ServiceCollection().AddLogging().AddApplication()
            .AddInfrastructure(factory.Services.GetRequiredService<IConfiguration>())
            .AddSingleton(factory.Services.GetRequiredService<TutoringCentre.Application.Platform.IDemoModeProvider>())
            .AddSingleton<ILanguageModel>(new Model(candidate))
            .AddSingleton(factory.Services.GetRequiredService<IClock>()).BuildServiceProvider();
        using var sent = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = "Hello" });
        Assert.Equal(HttpStatusCode.Created, sent.StatusCode);
        var relay = services.GetRequiredService<OutboxRelayService>();
        for(var stage = 0; stage < 3; stage++) await relay.ProcessBatchAsync();
        Assert.Equal(reason, await factory.ScalarAsync<string>("select guard_reason from assistant.agent_runs"));
        Assert.Equal(1L, await factory.ScalarAsync<long>("select count(*) from assistant.agent_runs"));
        Assert.Equal("Sorry, I cannot reply right now. Please try again soon.", await factory.ScalarAsync<string>("select text from conversations.outbound_messages"));
        Assert.Equal(0L, await factory.ScalarAsync<long>($"select count(*) from conversations.outbound_messages where text='{candidate}'"));
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var conversation = await factory.ScalarAsync<Guid>("select id from conversations.conversations");
        using var timeline = await owner.Client.GetAsync(new Uri($"/api/conversations/{conversation}/timeline", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, timeline.StatusCode);
        Assert.Contains(reason, await timeline.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }
    [Fact]
    public async Task TwoMessagesHaveOneRunAndOneOrderedReplyEach()
    {
        using var actor = await PortalAsync("parent");
        await using var services = new ServiceCollection().AddLogging().AddApplication()
            .AddInfrastructure(factory.Services.GetRequiredService<IConfiguration>())
            .AddSingleton(factory.Services.GetRequiredService<TutoringCentre.Application.Platform.IDemoModeProvider>())
            .AddSingleton<ILanguageModel>(new EchoModel())
            .AddSingleton(factory.Services.GetRequiredService<IClock>()).BuildServiceProvider();
        var now = DateTimeOffset.UtcNow;
        factory.Time.FixAt(now);
        using var first = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = "First question" });
        factory.Time.FixAt(now.AddMilliseconds(100));
        using var second = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = "Second question" });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode); Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        var relay = services.GetRequiredService<OutboxRelayService>();
        for (var i = 0; i < 5; i++)
        {
            await relay.ProcessBatchAsync();
            factory.Time.FixAt(now.AddSeconds((i + 1) * 6));
        }
        Assert.Equal(2L, await factory.ScalarAsync<long>("select count(*) from assistant.agent_runs"));
        Assert.Equal(2L, await factory.ScalarAsync<long>("select count(distinct reply_to_inbound_id) from conversations.outbound_messages"));
        Assert.Equal("First question|Second question", await factory.ScalarAsync<string>("select string_agg(text, '|' order by created_at,id) from conversations.outbound_messages"));
        factory.Time.UseRealTime();
    }
    [Fact]
    public async Task DeactivatedParentHasFailedRunAndNoReply()
    {
        using var actor = await PortalAsync("parent");
        using var sent = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = "Hello" });
        await factory.ExecuteAsync("update identity.memberships set status='inactive' where role='parent'");
        await using var services = new ServiceCollection().AddLogging().AddApplication()
            .AddInfrastructure(factory.Services.GetRequiredService<IConfiguration>())
            .AddSingleton(factory.Services.GetRequiredService<TutoringCentre.Application.Platform.IDemoModeProvider>())
            .AddSingleton<ILanguageModel>(new EchoModel())
            .AddSingleton(factory.Services.GetRequiredService<IClock>()).BuildServiceProvider();
        var relay = services.GetRequiredService<OutboxRelayService>();
        for(var i = 0; i < 3; i++) await relay.ProcessBatchAsync();
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from conversations.outbound_messages"));
        Assert.Equal("assistant.account_inactive", await factory.ScalarAsync<string>("select failure_code from assistant.agent_runs"));
    }
    [Fact]
    public async Task GetAttendanceToolUsesMarkedFamilyFactsThroughRealRelay()
    {
        factory.Time.FixAt(AttendanceHttp.BeforeSession);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var attendance = await AttendanceHttp.Arrange(owner.Client);
        factory.Time.FixAt(AttendanceHttp.InSession);
        using var marked = await AttendanceHttp.Put(owner.Client, $"/api/sessions/{attendance.SessionId}/attendance", AttendanceHttp.Marks(attendance));
        Assert.Equal(HttpStatusCode.OK, marked.StatusCode);
        var parent = await factory.ScalarAsync<Guid>($"select parent_id from academics.student_parents where student_id='{attendance.Students[0]}' limit 1");
        using var actor = await PortalAsync("parent", parent);
        var model = new AttendanceModel();
        await using var services = new ServiceCollection().AddLogging().AddApplication()
            .AddInfrastructure(factory.Services.GetRequiredService<IConfiguration>())
            .AddSingleton(factory.Services.GetRequiredService<TutoringCentre.Application.Platform.IDemoModeProvider>())
            .AddSingleton<ILanguageModel>(model).AddSingleton(factory.Services.GetRequiredService<IClock>()).BuildServiceProvider();
        using var sent = await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = "Did my child attend today?" });
        Assert.Equal(HttpStatusCode.Created, sent.StatusCode);
        var relay = services.GetRequiredService<OutboxRelayService>();
        for(var i = 0; i < 3; i++) await relay.ProcessBatchAsync();
        Assert.Equal(1, model.ToolRequests);
        Assert.Equal("allowed", await factory.ScalarAsync<string>("select decision from assistant.tool_calls"));
        Assert.Equal("1 record", await factory.ScalarAsync<string>("select result_summary from assistant.tool_calls"));
        var reply = await factory.ScalarAsync<string>("select text from conversations.outbound_messages");
        Assert.EndsWith("present.", reply, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"[0-9a-fA-F]{8}-[0-9a-fA-F-]{27,}", reply);
        Assert.Equal(1L, await factory.ScalarAsync<long>("select count(*) from assistant.agent_runs where status='completed' and guard_reason is null"));
    }
    private sealed class AttendanceModel : ILanguageModel
    {
        public int ToolRequests { get; private set; }
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken ct)
        {
            var last = request.Messages[^1];
            if(last.Role == ModelRole.Tool)
            {
                using var facts = System.Text.Json.JsonDocument.Parse(last.Text!);
                var child = facts.RootElement.GetProperty("child").GetString();
                var status = facts.RootElement.GetProperty("records")[0].GetProperty("status").GetString();
                return Task.FromResult(Result<ModelResponse>.Success(new($"{child}: {status}.", [], new(10,10), ModelStopReason.Completed)));
            }
            ToolRequests++;
            return Task.FromResult(Result<ModelResponse>.Success(new(null,
                [new("attendance", "get_attendance", System.Text.Json.JsonSerializer.SerializeToElement(new { childHandle = "child_1" }))], new(10,10), ModelStopReason.ToolCalls)));
        }
    }
    private sealed class EchoModel : ILanguageModel
    {
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken ct) => Task.FromResult(Result<ModelResponse>.Success(new(request.Messages.Last(message => message.Role == ModelRole.User).Text, [], new(10, 10), ModelStopReason.Completed)));
    }
    private sealed class Model(string candidate) : ILanguageModel
    {
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken) => Task.FromResult(Result<ModelResponse>.Success(new(candidate, [], new(10, 10), ModelStopReason.Completed)));
    }
    private async Task<SignedInClient> PortalAsync(string role, Guid? selectedSubject = null)
    {
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
        var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
        var subject = await factory.ScalarAsync<Guid>($"select id from academics.{(role == "parent" ? "parents" : "students")} where centre_id='{centre}' order by id limit 1");
        subject = selectedSubject ?? subject;
        await factory.ExecuteAsync($"""
            update identity.memberships set role='{role}' where centre_id='{centre}' and user_id='{user}';
            insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
            values ('{Guid.NewGuid()}','{centre}','{user}','{role}',{(role == "parent" ? $"'{subject}'" : "null")},{(role == "student" ? $"'{subject}'" : "null")},now(),'{owner}');
            """);
        return await SignInHelper.SignInAsync(factory, "secretary@nile.test");
    }

}
