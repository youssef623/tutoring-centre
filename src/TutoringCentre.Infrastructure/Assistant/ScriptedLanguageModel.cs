using System.Text.Json;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Assistant;

public sealed class ScriptedLanguageModel : ILanguageModel
{
    private readonly ModelResponse[] _responses;
    private readonly ScriptRule[] _rules;
    private int _position = -1;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    public ScriptedLanguageModel(string path)
    {
        using var script = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
        _rules = script.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object
            ? script.RootElement.GetProperty("rules").Deserialize<ScriptRule[]>(SerializerOptions) ?? [] : [];
        _responses = script.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array ? script.RootElement.Deserialize<ModelResponse[]>(SerializerOptions) ?? [] : [];
        /* legacy sequence remains supported */

        if (_responses.Length == 0 && _rules.Length == 0) throw new InvalidOperationException("The script must not be empty.");
    }

    private sealed record ScriptRule(string Contains, ModelResponse First, ModelResponse AfterTool);
    public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "The model call was cancelled.")));
        if (_rules.Length > 0)
        {
            var lastUser = request.Messages.LastOrDefault(message => message.Role == ModelRole.User)?.Text ?? "";
            var rule = _rules.FirstOrDefault(rule => lastUser.Contains(rule.Contains, StringComparison.OrdinalIgnoreCase));
            if (rule is null) return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("assistant.model_error", "No scripted rule matched.")));
            var afterTool = request.Messages.Count > 0 && request.Messages[^1].Role == ModelRole.Tool;
            return Task.FromResult(Result<ModelResponse>.Success(afterTool ? rule.AfterTool : rule.First));
        }
        var index = Interlocked.Increment(ref _position);
        return Task.FromResult(index < _responses.Length ? Result<ModelResponse>.Success(_responses[index])
            : Result<ModelResponse>.Failure(Error.Rule("assistant.model_error", "The script is exhausted.")));
    }
}
