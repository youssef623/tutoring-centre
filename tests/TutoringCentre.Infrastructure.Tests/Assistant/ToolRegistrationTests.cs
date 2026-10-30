using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Assistant;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
public sealed class ToolRegistrationTests
{
    [Fact] public async Task DuplicateNamesFailStartup()
    {
        var services=new ServiceCollection();
        services.AddScoped<ToolRegistry>();services.AddTool<FakeTool>();services.AddTool<FakeTool>();
        await using var provider=services.BuildServiceProvider();
        var startup=new ToolRegistrationValidator(provider.GetRequiredService<IServiceScopeFactory>());
        await Assert.ThrowsAsync<ArgumentException>(()=>startup.StartAsync(CancellationToken.None));
    }
    private sealed class FakeTool : ITool
    {
        public ToolDescriptor Descriptor { get; }=new("fake_tool","test",JsonDocument.Parse("""{"type":"object","properties":{},"additionalProperties":false}""").RootElement.Clone());
        public Task<Result<ToolResult>> ExecuteAsync(ToolContext context,ToolArguments arguments,CancellationToken ct)=>throw new NotSupportedException();
    }
}
