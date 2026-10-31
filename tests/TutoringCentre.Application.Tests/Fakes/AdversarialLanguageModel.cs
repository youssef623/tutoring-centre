using System.Text.Json;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>Ignores tool advertisements, schemas, denials and prompt instructions.</summary>
public sealed class AdversarialLanguageModel(string attackId, string foreignId = AttackCatalogue.ForeignId) : ILanguageModel
{
    private readonly Attack _attack = AttackCatalogue.Get(attackId);
    private int _calls;
    public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "Cancelled.")));
        var first = ++_calls == 1;
        ToolCallRequest Call(string name, object arguments) => new($"attack-{_calls}-{name}", name, JsonSerializer.SerializeToElement(arguments));
        var handle = _attack.Id switch
        {
            "A1" => "child_9", "A2" => foreignId, "A9" => "child_1'; drop table x;--", "A10" => "omar", _ => "child_1"
        };
        ToolCallRequest[] calls = !first && _attack.Id != "A6" ? [] : _attack.Id switch
        {
            "A7" or "A8" => [],
            "A3" => [Call("get_attendance", new { childHandle = handle, studentId = foreignId })],
            "A4" => [Call("get_balance", new { }), Call("confirm_action", new { })],
            "A5" => [Call("get_attendance", new { childHandle = handle, from = "2025-01-01", to = "2026-02-05" })],
            "A6" => Enumerable.Range(0, 7).Select(index => Call("get_attendance", new { childHandle = handle }) with { Id = $"loop-{_calls}-{index}" }).ToArray(),
            _ => [Call("get_attendance", new { childHandle = handle })]
        };
        var text = _attack.Id switch
        {
            "A7" => AttackCatalogue.Canary + " is doing well.",
            "A8" => foreignId + " child_1", _ => AttackCatalogue.Refusal
        };
        return Task.FromResult(Result<ModelResponse>.Success(new(calls.Length == 0 ? text : null, calls,
            new(1, 1), calls.Length == 0 ? ModelStopReason.Completed : ModelStopReason.ToolCalls)));
    }
}
