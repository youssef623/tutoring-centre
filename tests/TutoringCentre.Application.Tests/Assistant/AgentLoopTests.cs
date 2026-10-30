using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class AgentLoopTests
{
    private static readonly ContextOutput Context = new("system",[new(ModelRole.User,"hello")]);
    [Fact]
    public async Task NormalReplyAndSentenceCap()
    {
        var model = new StubLanguageModel(_=>Task.FromResult(Reply("Hello. Another sentence that is long.")));
        var result = await new AgentLoop(model,new AssistantOptions { MaxReplyCharacters=15 }).RunAsync(Context,CancellationToken.None);
        Assert.Equal("Hello.",result.Reply); Assert.Equal(1,result.Steps);
        Assert.Equal("assistant.reply_capped",result.GuardReason);
        Assert.Equal(10,result.Usage.InputTokens);
        Assert.Empty(model.LastRequest!.Tools);
    }
    [Theory]
    [InlineData(false,"assistant.call_timeout")]
    [InlineData(true,"assistant.total_timeout")]
    public async Task TimeoutBoundsEvenNonCooperativeModel(bool total,string code)
    {
        var model = new StubLanguageModel(_=>new TaskCompletionSource<Result<ModelResponse>>().Task);
        var options = new AssistantOptions { CallTimeout=TimeSpan.FromMilliseconds(total?1000:30), TotalRunTimeout=TimeSpan.FromMilliseconds(total?30:1000) };
        var result = await new AgentLoop(model,options).RunAsync(Context,CancellationToken.None);
        Assert.Equal(code,result.FailureCode); Assert.Null(result.Reply);
    }
    [Fact]
    public async Task TotalTimeoutPreservesUsageFromEarlierSteps()
    {
        var calls = 0;
        var model = new StubLanguageModel(_ => calls++ == 0
            ? Task.FromResult(Reply("continue", ModelStopReason.Other))
            : new TaskCompletionSource<Result<ModelResponse>>().Task);
        var outcome = await new AgentLoop(model, new AssistantOptions { TotalRunTimeout = TimeSpan.FromMilliseconds(30) }).RunAsync(Context, CancellationToken.None);
        Assert.Equal("assistant.total_timeout", outcome.FailureCode);
        Assert.Equal(2, outcome.Steps);
        Assert.Equal(new TokenUsage(10, 5), outcome.Usage);
        Assert.Null(outcome.Reply);
    }

    [Fact]
    public async Task CancellationPropagates()
    {
        var model = new StubLanguageModel(_=>new TaskCompletionSource<Result<ModelResponse>>().Task);
        using var cancelled = new CancellationTokenSource(TimeSpan.FromMilliseconds(30));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>new AgentLoop(model,new()).RunAsync(Context,cancelled.Token));
    }
    [Fact]
    public async Task ModelFailureHasNoReply()
    {
        var model = new StubLanguageModel(_=>Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("provider.error","Unavailable"))));
        Assert.Equal("assistant.model_error",(await new AgentLoop(model,new()).RunAsync(Context,CancellationToken.None)).FailureCode);
        var throwing = new StubLanguageModel(_=>throw new InvalidOperationException("private content"));
        Assert.Equal("assistant.model_error",(await new AgentLoop(throwing,new()).RunAsync(Context,CancellationToken.None)).FailureCode);
    }
    [Fact]
    public async Task ToolsStopAfterOneStep()
    {
        var model = new StubLanguageModel(_=>Task.FromResult(Reply(null,ModelStopReason.ToolCalls)));
        var result = await new AgentLoop(model,new()).RunAsync(Context,CancellationToken.None);
        Assert.Equal("assistant.tools_unavailable",result.FailureCode); Assert.Equal(1,model.Calls);
    }
    [Fact]
    public async Task TokensAccumulateAcrossStepsAndStopAtCeiling()
    {
        var model = new StubLanguageModel(_=>Task.FromResult(Reply("continue",ModelStopReason.Other)));
        var result = await new AgentLoop(model,new AssistantOptions { MaxTotalTokens=30 }).RunAsync(Context,CancellationToken.None);
        Assert.Equal("assistant.token_limit",result.FailureCode); Assert.Equal(2,model.Calls);
        Assert.Equal(15,model.LastRequest!.MaxOutputTokens);
    }
    [Theory]
    [InlineData(1)] [InlineData(3)]
    public async Task StepCapStopsIncompleteResponses(int max)
    {
        var model = new StubLanguageModel(_=>Task.FromResult(Reply("continue",ModelStopReason.Other)));
        var result = await new AgentLoop(model,new AssistantOptions { MaxSteps=max }).RunAsync(Context,CancellationToken.None);
        Assert.Equal("assistant.step_limit",result.FailureCode); Assert.Equal(max,model.Calls);
    }
    private static Result<ModelResponse> Reply(string? text, ModelStopReason reason=ModelStopReason.Completed) => Result<ModelResponse>.Success(new(text,[],new(10,5),reason));
    private sealed class StubLanguageModel(Func<CancellationToken,Task<Result<ModelResponse>>> generate) : ILanguageModel
    {
        public int Calls { get; private set; }
        public ModelRequest? LastRequest { get; private set; }
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request,CancellationToken cancellationToken)
        { Calls++; LastRequest=request; return generate(cancellationToken); }
    }
}
