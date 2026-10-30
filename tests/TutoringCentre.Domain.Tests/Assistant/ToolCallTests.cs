using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Domain.Tests.Assistant;
public sealed class ToolCallTests
{
    [Fact] public void DenialNeedsCodeAndDurationCannotBeNegative()
    {
        Assert.Throws<ArgumentException>(() => Record(ToolDecision.Denied, null, 0));
        Assert.Throws<ArgumentException>(() => Record(ToolDecision.Allowed, null, -1));
        Assert.Equal(200, Record(ToolDecision.Denied, "tool.unknown", 0).ArgumentSummary.Length);
    }
    private static ToolCall Record(ToolDecision decision, string? code, int duration) => ToolCall.Record(Guid.NewGuid(), Guid.NewGuid(), 1, 0, "fake_tool", decision, code, new string('x', 300), null, duration, DateTimeOffset.UtcNow);
}
