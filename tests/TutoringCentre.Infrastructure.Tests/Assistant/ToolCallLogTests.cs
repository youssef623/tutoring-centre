using TutoringCentre.Application.Assistant.Queries.GetConversationTimeline;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Tests.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
public sealed class ToolCallLogTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Theory]
    [InlineData("fake_tool","{\"child\":\"child_1\"}",false,"allowed",null)]
    [InlineData("unknown","{}",false,"denied","tool.unknown")]
    [InlineData("fake_tool","{\"child\":\"child_1\",\"studentId\":\"raw\"}",false,"invalid","tool.arguments_invalid")]
    [InlineData("fake_tool","{\"child\":\"child_1\"}",true,"denied","tool.call_limit")]
    public async Task RealRunLogsEveryDecisionAndDatabaseProtectsEvidence(string name,string arguments,bool cap,string expected,string? code)
    {
        await using var services=Fixture.CreateServiceProvider(s=>{
            s.AddTool<FakeTool>();s.AddSingleton<ILanguageModel>(new StubLanguageModel(name,arguments,cap));
            if(cap)s.PostConfigure<AssistantOptions>(o=>o.MaxToolCalls=1);
        });
        var parent=await Fixture.SeedParentAsync(NileCentreId,"Parent","+201000000071");
        var child=await Fixture.SeedStudentAsync(NileCentreId,"Ahmed");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId,child,parent,true);
        var account=await Fixture.SendAsAsync<CreatePortalAccountCommand,CreatePortalAccountResult>(new SystemActor(NileCentreId,await Fixture.SeedUserAsync()),new(PortalSubjectKind.Parent,parent,"tool@nile.test",SupportedLocale.En));
        Assert.True(account.IsSuccess,account.Error?.Message);
        var channel=await SendAsync<RegisterChannelAccountCommand,Guid>(NileCentreId,new(ChannelProvider.Web,"web:nile-centre",null));Assert.True(channel.IsSuccess);
        var sent=await services.SendAsAsync<SendWebMessageCommand,SendWebMessageResult>(new ParentActor(account.Value.UserId,NileCentreId,parent),new(Guid.NewGuid(),"hello"));Assert.True(sent.IsSuccess);
        var result=await services.SendAsAsync<RunAgentCommand,RunAgentResult>(new SystemActor(NileCentreId),new(sent.Value.MessageId));
        Assert.True(result.IsSuccess,result.Error?.Message);
        Assert.Equal(cap?2L:1L,await Fixture.ScalarAsync<long>("select count(*) from assistant.tool_calls"));
        Assert.Equal(expected,await Fixture.ScalarAsync<string>("select decision from assistant.tool_calls order by ordinal desc limit 1"));
        if(code is not null)Assert.Equal(code,await Fixture.ScalarAsync<string>("select reason_code from assistant.tool_calls order by ordinal desc limit 1"));
        Assert.Equal("v2",await Fixture.ScalarAsync<string>("select prompt_version from assistant.agent_runs"));
        var conversation = await Fixture.ScalarAsync<Guid>("select conversation_id from assistant.agent_runs");
        var timelineItems = new List<AssistantTimelineItem>();
        string? cursor = null;
        do
        {
            var page = await services.QueryAsAsync<GetConversationTimelineQuery, ConversationTimelinePage>(new StaffActor(Guid.NewGuid(), NileCentreId, StaffRole.Owner), new(conversation, cursor, 1));
            Assert.True(page.IsSuccess);
            timelineItems.AddRange(page.Value.Items); cursor = page.Value.NextCursor;
        } while (cursor is not null);
        Assert.Equal(3, timelineItems.Count);
        Assert.Equal(3, timelineItems.Select(item => item.Id).Distinct().Count());
        Assert.Equal(timelineItems.OrderByDescending(item => item.At).ThenByDescending(item => item.Id), timelineItems);
        var activity = Assert.Single(timelineItems, item => item.Kind == "run");
        Assert.Equal(expected, activity.ToolCalls![^1].Decision);
        Assert.Equal(code, activity.ToolCalls[^1].ReasonCode);
        Assert.DoesNotContain("studentId", activity.ToolCalls[^1].ArgumentSummary, StringComparison.Ordinal);
        var foreignTimeline = await services.QueryAsAsync<GetConversationTimelineQuery, ConversationTimelinePage>(new SystemActor(MaadiCentreId), new(conversation, null));
        Assert.Equal("conversations.not_found", foreignTimeline.Error!.Code);
        foreach (var deniedActor in new Actor[] { new ParentActor(account.Value.UserId, NileCentreId, parent), new StaffActor(Guid.NewGuid(), NileCentreId, StaffRole.Teacher) })
        {
            var deniedTimeline = await services.QueryAsAsync<GetConversationTimelineQuery, ConversationTimelinePage>(deniedActor, new(conversation, null));
            Assert.Equal("auth.permission_denied", deniedTimeline.Error!.Code);
        }
        Assert.False(await Fixture.ScalarAsync<bool>("select has_table_privilege('tutoring_app','assistant.tool_calls','UPDATE') or has_table_privilege('tutoring_app','assistant.tool_calls','DELETE')"));
        Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='assistant.tool_calls'::regclass"));
        var update=await Assert.ThrowsAsync<PostgresException>(()=>ExecuteAsync("update assistant.tool_calls set result_summary='tampered'",Fixture.AppConnectionString,NileCentreId));
        Assert.Equal(PostgresErrorCodes.InsufficientPrivilege,update.SqlState);
        Assert.Equal("ck_tool_calls_reason",(await Assert.ThrowsAsync<PostgresException>(()=>ExecuteAsync("update assistant.tool_calls set decision='denied',reason_code=null"))).ConstraintName);
        Assert.Equal("ck_tool_calls_duration",(await Assert.ThrowsAsync<PostgresException>(()=>ExecuteAsync("update assistant.tool_calls set duration_ms=-1"))).ConstraintName);
        Assert.Equal(0L,await Fixture.ReadAsAsync(new SystemActor(MaadiCentreId),async p=>await p.GetRequiredService<TutoringCentre.Infrastructure.Persistence.AppDbContext>().Set<TutoringCentre.Domain.Assistant.ToolCall>().CountAsync()));
        Assert.Equal(0L,await Fixture.ScalarAsync<long>("select count(*) from assistant.tool_calls where argument_summary like '%Ahmed%' or result_summary like '%Ahmed%'"));
    }
    public sealed class FakeTool(Dispatcher dispatcher) : ITool
    {
        public ToolDescriptor Descriptor { get; }=new("fake_tool","A test tool",JsonDocument.Parse("""{"type":"object","additionalProperties":false,"properties":{"child":{"type":"string","maxLength":30}},"required":["child"]}""").RootElement.Clone());
        public async Task<Result<ToolResult>> ExecuteAsync(ToolContext context,ToolArguments arguments,CancellationToken ct)
        {
            var id=context.Handles.Resolve(arguments.GetString("child"));
            if(id.IsFailure)return Result<ToolResult>.Failure(id.Error!);
            var fresh=await dispatcher.QueryAsync<GetRunHandlesQuery,HandleTable>(new(),ct);
            return fresh.IsSuccess && fresh.Value.Children.Values.Any(x=>x.Id==id.Value)
                ? Result<ToolResult>.Success(new(new { count=1 })) : Result<ToolResult>.Failure(HandleError.Unknown);
        }
    }
    private sealed class StubLanguageModel(string name,string arguments,bool cap) : ILanguageModel
    {
        private int _calls;
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request,CancellationToken cancellationToken)
        {
            _calls++;
            var call=new ToolCallRequest("call",name,JsonDocument.Parse(arguments).RootElement.Clone());
            return Task.FromResult(Result<ModelResponse>.Success(_calls==1 ? new(null,cap?[call,call with { Id="second" }]:[call],new(1,1),ModelStopReason.ToolCalls)
                : new("Done",[],new(1,1),ModelStopReason.Completed)));
        }
    }
}
