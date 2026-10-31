using System.Text.Json;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Evaluation.Models;

public sealed class ObservedLanguageModel(ILanguageModel inner) : ILanguageModel
{
    public List<string> Outputs { get; } = [];
    public List<ModelRequest> Requests { get; } = [];
    public List<string> Errors { get; } = [];
    public async Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        Outputs.AddRange(request.Messages.Where(m => m.Role == ModelRole.Tool).Select(m => m.Text ?? ""));
        var result = await inner.GenerateAsync(request, cancellationToken);
        if (result.IsSuccess) { Outputs.Add(JsonSerializer.Serialize(result.Value)); Outputs.Add(result.Value.Text ?? ""); foreach (var call in result.Value.ToolCalls) Outputs.Add(call.Arguments.ToString()); } else Errors.Add(result.Error!.Code);
        return result;
    }
}
