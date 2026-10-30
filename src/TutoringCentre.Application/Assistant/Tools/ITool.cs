using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Tools;
public interface ITool
{
    ToolDescriptor Descriptor { get; }
    Task<Result<ToolResult>> ExecuteAsync(ToolContext context, ToolArguments arguments, CancellationToken ct);
}
