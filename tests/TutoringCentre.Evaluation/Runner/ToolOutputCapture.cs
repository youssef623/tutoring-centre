using System.Collections.Concurrent;
using System.Text.Json;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Evaluation.Runner;

public sealed class ToolOutputCapture
{
    private readonly ConcurrentQueue<string> _outputs = new();
    public string[] Read() => _outputs.ToArray();
    public void Clear() => _outputs.Clear();
    public void Add(object data) => _outputs.Enqueue(JsonSerializer.Serialize(data));
}

// Observes the real tool, including its final result when a cap prevents another model call.
internal sealed class CapturingTool(ITool inner, ToolOutputCapture outputs) : ITool
{
    public ToolDescriptor Descriptor => inner.Descriptor;
    public async Task<Result<ToolResult>> ExecuteAsync(ToolContext context, ToolArguments arguments, CancellationToken ct)
    {
        var result = await inner.ExecuteAsync(context, arguments, ct);
        if (result.IsSuccess) outputs.Add(result.Value.Data);
        return result;
    }
}
