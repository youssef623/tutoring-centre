using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Evaluation.Models;

public sealed class ReplayLanguageModel(Cassette cassette) : ILanguageModel
{
    public List<string> Errors { get; } = [];
    public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "Cancelled.")));
        var key = CassetteKey.Create(AssistantPrompt.Version, request);
        var entry = cassette.Entries.FirstOrDefault(e => e.RequestHash == key);
        if (cassette.PromptVersion != AssistantPrompt.Version || entry is null) { Errors.Add("eval.cassette_missing"); return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("eval.cassette_missing", cassette.CaseId + ": eval.cassette_missing"))); }
        return Task.FromResult(Result<ModelResponse>.Success(entry.Response));
    }
}
