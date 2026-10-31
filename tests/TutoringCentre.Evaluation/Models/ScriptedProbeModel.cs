using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Assistant;

namespace TutoringCentre.Evaluation.Models;

// Explicit synthetic probes reuse the existing provider; never a recording/replay fallback.
public sealed class ScriptedProbeModel(bool leak = false) : ILanguageModel
{
    private readonly ScriptedLanguageModel _inner = new(Path.Combine(AppContext.BaseDirectory,
        "fixtures", leak ? "probe-leak.json" : "probe-attendance.json"));

    public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken) =>
        _inner.GenerateAsync(request, cancellationToken);
}
