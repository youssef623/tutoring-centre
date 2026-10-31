using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Application.Common.Execution;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class ToolGatewayTests
{
    [Theory]
    [InlineData("fake_tool", "{\"child\":\"child_1\"}", "allowed", null)]
    [InlineData("unknown", "{}", "denied", "tool.unknown")]
    [InlineData("fake_tool", "{\"child\":\"child_1\",\"studentId\":\"secret\"}", "invalid", "tool.invalid_arguments")]
    [InlineData("error_tool", "{\"child\":\"child_1\"}", "denied", "tool.handle_unknown")]
    [InlineData("throw_tool", "{\"child\":\"child_1\"}", "failed", "tool.internal_error")]
    [InlineData("fake_tool", "{\"child\":\"child_9\"}", "denied", "tool.handle_unknown")]
    public async Task DecisionsWriteExactlyOneContentFreeRow(string name,string json,string decision,string? reason)
    {
        await using var fixture = new Harness();
        var result = await fixture.Gateway.ExecuteAsync(fixture.Context,1,0,name,json,CancellationToken.None);
        Assert.Equal(decision,result.Decision.ToString().ToLowerInvariant()); Assert.Equal(reason,result.ReasonCode);
        var row = Assert.Single(fixture.Log.Rows); Assert.Equal(result.Decision,row.Decision);
        if(decision=="allowed") Assert.Equal("handles=child_1",row.ArgumentSummary);
        Assert.DoesNotContain("secret",result.Text,StringComparison.Ordinal); Assert.DoesNotContain("secret",row.ArgumentSummary,StringComparison.Ordinal);
        Assert.DoesNotContain("private exception",result.Text,StringComparison.Ordinal);
    }
    [Fact] public async Task CapIsIndependentAndEveryDeniedAttemptIsLogged()
    {
        await using var fixture = new Harness();
        for(var i=0;i<8;i++) await fixture.Gateway.ExecuteAsync(fixture.Context,1,i,"fake_tool","{\"child\":\"child_1\"}",CancellationToken.None);
        Assert.Equal(8,fixture.Log.Rows.Count); Assert.Equal(2,fixture.Log.Rows.Count(x=>x.ReasonCode=="tool.call_cap"));
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public async Task LoopContinuesWithToolMessageAndHonorsStepCap(bool forever)
    {
        await using var fixture = new Harness();
        var model = new StubLanguageModel(forever);
        var loop = new AgentLoop(model,new AssistantOptions(),fixture.Gateway,fixture.Registry);
        var result = await loop.RunWithinBudgetAsync(new ContextOutput("system",[new(ModelRole.User,"hello")]),TimeSpan.FromSeconds(10),CancellationToken.None,fixture.Context);
        Assert.Equal(forever ? "assistant.step_limit" : null,result.FailureCode);
        Assert.Equal(forever ? 4 : 1,fixture.Log.Rows.Count);
        Assert.Contains(model.Last!.Messages,x=>x.Role==ModelRole.Tool);
        Assert.Equal(forever ? 4 : 2,result.Steps);
    }
    [Fact] public async Task OutputIsBounded()
    {
        await using var fixture = new Harness();
        var outcome=await fixture.Gateway.ExecuteAsync(fixture.Context,1,0,"large_tool","{\"child\":\"child_1\"}",CancellationToken.None);
        Assert.Equal(4000,outcome.Text.Length);
    }
    [Fact] public async Task RunCancellationBoundsAToolThatIgnoresItsToken()
    {
        await using var fixture = new Harness();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        var outcome = await fixture.Gateway.ExecuteAsync(fixture.Context,1,0,"slow_tool","{\"child\":\"child_1\"}",timeout.Token).WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(ToolDecision.Failed,outcome.Decision);
        Assert.Equal("tool.internal_error",outcome.ReasonCode);
        Assert.Single(fixture.Log.Rows);
    }
    internal sealed class Harness : IAsyncDisposable
    {
        private readonly ServiceProvider _provider;
        public Log Log { get; } = new();
        public ToolContext Context { get; } = new(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),HandleResolver.Mint([new(Guid.NewGuid(),"Ahmed")]));
        public ToolRegistry Registry { get; }
        public ToolGateway Gateway { get; }
        public Harness(ITool? attendance = null, AssistantOptions? options = null)
        {
            ITool[] tools=[new FakeTool("fake_tool"),new FakeTool("error_tool"),new FakeTool("throw_tool"),new FakeTool("large_tool"),new FakeTool("slow_tool")];
            Registry=new ToolRegistry(attendance is null ? tools : [attendance]);
            _provider=new ServiceCollection().AddSingleton(Registry).BuildServiceProvider();
            var current=new CurrentActorContext();current.Set(new SystemActor(Context.CentreId));
            Gateway=new ToolGateway(Registry,new Scope(_provider),new PortalActorFactory(new Directory(),current),new Conversations(),Log,options ?? new AssistantOptions());
        }
        public ValueTask DisposeAsync()=>_provider.DisposeAsync();
    }
    internal sealed class Log : IToolCallLog { public List<ToolCall> Rows { get; }=[]; public void Add(ToolCall evidence)=>Rows.Add(evidence); }
    private sealed class Scope(IServiceProvider services) : IActorScopeRunner { public Task<T> RunAsync<T>(ICurrentActor actor,Func<IServiceProvider,Task<T>> work)=>work(services); }
    private sealed class Directory : IPortalActorDirectory { public Task<bool> IsActiveAsync(Guid userId,Guid subjectId,bool isStudent,CancellationToken ct)=>Task.FromResult(true); }
    private sealed class Conversations : IAssistantConversationGateway
    {
        public Task<AssistantConversationInput?> LoadAsync(Guid inboundId,CancellationToken cancellationToken)=>Task.FromResult<AssistantConversationInput?>(new(Guid.NewGuid(),Guid.NewGuid(),null,true,"",Guid.NewGuid()));
        public Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(Guid inboundId,int count,CancellationToken cancellationToken)=>throw new NotSupportedException();
        public Task<Result> QueueReplyAsync(Guid inboundId,string reply,CancellationToken cancellationToken)=>throw new NotSupportedException();
    }
    private sealed class FakeTool(string name) : ITool
    {
        public ToolDescriptor Descriptor { get; }=new(name,"test",ToolArgumentsTests.Schema);
        public Task<Result<ToolResult>> ExecuteAsync(ToolContext context,ToolArguments arguments,CancellationToken ct)
        {
            if(name=="throw_tool") throw new InvalidOperationException("private exception");
            if(name=="slow_tool") return new TaskCompletionSource<Result<ToolResult>>().Task;
            var resolved=context.Handles.Resolve(arguments.GetString("child"));
            return Task.FromResult(name=="error_tool" || resolved.IsFailure ? Result<ToolResult>.Failure(HandleError.Unknown)
                : Result<ToolResult>.Success(new(name=="large_tool" ? new string('a',5000) : "ok")));
        }
    }
    private sealed class StubLanguageModel(bool forever) : ILanguageModel
    {
        private int _calls;
        public ModelRequest? Last { get; private set; }
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request,CancellationToken cancellationToken)
        {
            Last=request; _calls++;
            return Task.FromResult(Result<ModelResponse>.Success(forever || _calls==1 ? new(null,[new("call","fake_tool",JsonDocument.Parse("{\"child\":\"child_1\"}").RootElement.Clone())],new(1,1),ModelStopReason.ToolCalls)
                : new("Done",[],new(1,1),ModelStopReason.Completed)));
        }
    }
}
