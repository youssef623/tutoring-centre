using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Domain.Tests.Assistant;
public sealed class AgentRunTests
{
    [Fact]
    public void CompletedRunCannotChangeAgain()
    {
        var now = DateTimeOffset.Parse("2026-10-30T12:00:00Z", System.Globalization.CultureInfo.InvariantCulture);
        var run = AgentRun.Start(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), now);
        Assert.Equal(AgentRunStatus.Running, run.Status);
        Assert.True(run.Complete(1, 10, 20, now.AddSeconds(1), "assistant.reply_capped").IsSuccess);
        Assert.Equal(AgentRunStatus.Completed, run.Status);
        Assert.NotNull(run.FinishedAt);
        Assert.Null(run.FailureCode);
        Assert.True(run.Fail("assistant.model_error", 2, 1, 1, now.AddSeconds(2)).IsFailure);
        Assert.Equal(10, run.InputTokens);
    }
    [Fact]
    public void FailureStoresOnlyCountsAndCodeAndRejectsInvalidData()
    {
        var now = DateTimeOffset.UnixEpoch;
        var run = AgentRun.Start(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), now);
        Assert.True(run.Complete(-1, 0, 0, now).IsFailure);
        Assert.True(run.Fail("message text!", 0, 0, 0, now).IsFailure);
        Assert.True(run.Complete(1, 0, 0, now.AddSeconds(-1)).IsFailure);
        Assert.True(run.Fail("assistant.model_error", 1, 20, 0, now.AddSeconds(1)).IsSuccess);
        Assert.Equal(AgentRunStatus.Failed, run.Status);
        Assert.Equal("assistant.model_error", run.FailureCode);
        Assert.True(run.Complete(1, 0, 0, now).IsFailure);
    }
}
