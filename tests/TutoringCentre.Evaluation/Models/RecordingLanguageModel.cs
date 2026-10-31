using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Evaluation.Models;

public sealed class RecordingLanguageModel(ILanguageModel inner) : ILanguageModel
{
    public List<CassetteEntry> Entries { get; } = [];
    public async Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "Cancelled."));
        var result = await inner.GenerateAsync(request, cancellationToken);
        if (result.IsSuccess) Entries.Add(new(CassetteKey.Create(AssistantPrompt.Version, request), request, result.Value));
        return result;
    }
}
