using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class RunAgentHandlerTests
{
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task SuccessOrFailureQueuesOneReplyAndFinishesRun(bool fail)
    {
        var gateway = new Gateway();
        var clock = new FakeClock();
        var runs = new Runs(gateway.Input.ConversationId,clock.UtcNow);
        var directory = new FakePortalDirectory();
        directory.Parents[gateway.Input.ParentId!.Value] = new(gateway.Input.ParentId.Value,"Family Parent",true);
        var actor = new FakeCurrentActor { Actor = new TutoringCentre.Application.Common.Security.SystemActor(runs.Centre) };
        var options = new AssistantOptions();
        var handler = new RunAgentHandler(gateway,runs,directory,new FakeCentreTimeZoneProvider(),clock,actor,new AgentLoop(new Model(fail),options),options,new PortalActorFactory(new ActorDirectory(),actor),new HandleScope(), new ConversationLock());
        var result = await handler.HandleAsync(new(runs.Inbound),CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.AlreadyRan);
        Assert.Single(gateway.Replies);
        Assert.Equal(fail ? AgentRunStatus.Failed : AgentRunStatus.Completed,runs.Run!.Status);
        Assert.Equal(fail ? "assistant.model_error" : null,runs.Run.FailureCode);
        Assert.NotNull(runs.Run.FinishedAt);
        Assert.Contains(fail ? "try again" : "soon",gateway.Replies[0],StringComparison.Ordinal);
        var repeated = await handler.HandleAsync(new(runs.Inbound),CancellationToken.None);
        Assert.True(repeated.Value.AlreadyRan);
        Assert.Single(gateway.Replies);
    }
    [Fact]
    public async Task NonSystemActorCannotClaim()
    {
        var gateway = new Gateway();
        var clock = new FakeClock();
        var runs = new Runs(gateway.Input.ConversationId,clock.UtcNow);
        var options = new AssistantOptions();
        var handler = new RunAgentHandler(gateway,runs,new FakePortalDirectory(),new FakeCentreTimeZoneProvider(),clock,
            new FakeCurrentActor(),new AgentLoop(new Model(false),options),options,new PortalActorFactory(new ActorDirectory(),new FakeCurrentActor()),new HandleScope(), new ConversationLock());
        Assert.True((await handler.HandleAsync(new(runs.Inbound),CancellationToken.None)).IsFailure);
        Assert.Null(runs.Run);
        Assert.Empty(gateway.Replies);
    }
    private sealed class ConversationLock : IConversationLock
    {
        public Task<bool> TryAcquireAsync(Guid centreId, Guid conversationId, CancellationToken ct) => Task.FromResult(true);
    }
    private sealed class Gateway : IAssistantConversationGateway
    {
        public AssistantConversationInput Input { get; } = new(Guid.NewGuid(),Guid.NewGuid(),null,true,"Hello",Guid.NewGuid());
        public List<string> Replies { get; } = [];
        public Task<AssistantConversationInput?> LoadAsync(Guid inboundId,CancellationToken cancellationToken) => Task.FromResult<AssistantConversationInput?>(Input);
        public Task<IReadOnlyList<AssistantHistoryEntry>> GetHistoryAsync(Guid inboundId,int count,CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<AssistantHistoryEntry>>([]);
        public Task<Result> QueueReplyAsync(Guid inboundId,string reply,CancellationToken cancellationToken) { Replies.Add(reply); return Task.FromResult(Result.Success()); }
    }
    private sealed class Runs(Guid conversation,DateTimeOffset now) : IAgentRunRepository
    {
        public Guid Centre { get; } = Guid.NewGuid();
        public Guid Inbound { get; } = Guid.NewGuid();
        public AgentRun? Run { get; private set; }
        public Task<AgentRun?> TryStartAsync(Guid conversationId,Guid inboundMessageId,DateTimeOffset at,CancellationToken cancellationToken)
        {
            if(Run is not null) return Task.FromResult<AgentRun?>(null);
            Run=AgentRun.Start(Centre,conversation,Inbound,now);
            return Task.FromResult<AgentRun?>(Run);
        }
    }
    private sealed class Model(bool fail) : ILanguageModel
    {
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request,CancellationToken cancellationToken) =>
            Task.FromResult(fail ? Result<ModelResponse>.Failure(Error.Rule("assistant.model_error","Unavailable"))
                : Result<ModelResponse>.Success(new("Hello. I can help soon.",[],new(10,5),ModelStopReason.Completed)));
    }
    private sealed class ActorDirectory : TutoringCentre.Application.Identity.Contracts.IPortalActorDirectory
    {
        public Task<bool> IsActiveAsync(Guid userId,Guid subjectId,bool isStudent,CancellationToken ct) => Task.FromResult(true);
    }
    private sealed class HandleScope : TutoringCentre.Application.Common.Execution.IActorScopeRunner
    {
        public Task<T> RunAsync<T>(TutoringCentre.Application.Common.Security.ICurrentActor actor,Func<IServiceProvider,Task<T>> work) =>
            Task.FromResult((T)(object)Result<TutoringCentre.Application.Assistant.Handles.HandleTable>.Success(TutoringCentre.Application.Assistant.Handles.HandleResolver.Mint([])));
    }
}
