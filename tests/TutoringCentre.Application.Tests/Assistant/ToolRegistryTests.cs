using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class ToolRegistryTests
{
    [Fact] public void DuplicateNamesFailRegistration() => Assert.Throws<ArgumentException>(() => new ToolRegistry([new FakeTool(), new FakeTool()]));
    [Fact] public void InvalidNameFailsRegistration() => Assert.Throws<ArgumentException>(() => new ToolRegistry([new FakeTool("studentId")]));
    internal sealed class FakeTool(string name = "fake_tool") : ITool
    {
        public ToolDescriptor Descriptor { get; } = new(name, "test", ToolArgumentsTests.Schema);
        public Task<Result<ToolResult>> ExecuteAsync(ToolContext context, ToolArguments arguments, CancellationToken ct) => Task.FromResult(Result<ToolResult>.Success(new(new { ok = true })));
    }
}
