using System.Text.Json;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Assistant;

public sealed class ScriptedLanguageModel : ILanguageModel
{
    private readonly ModelResponse[] _responses;
    private int _position = -1;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    public ScriptedLanguageModel(string path)
    {
        _responses = JsonSerializer.Deserialize<ModelResponse[]>(File.ReadAllText(path), SerializerOptions)
            ?? throw new InvalidOperationException("The script must contain model responses.");
        if (_responses.Length == 0) throw new InvalidOperationException("The script must not be empty.");
    }

    public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "The model call was cancelled.")));
        var index = Interlocked.Increment(ref _position);
        return Task.FromResult(index < _responses.Length ? Result<ModelResponse>.Success(_responses[index])
            : Result<ModelResponse>.Failure(Error.Rule("assistant.model_error", "The script is exhausted.")));
    }
}
