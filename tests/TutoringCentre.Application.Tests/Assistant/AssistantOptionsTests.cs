using System.ComponentModel.DataAnnotations;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Assistant;

public sealed class ScriptedContractExampleTests : ModelContractTests
{
    protected override ILanguageModel CreateModel() => new DeterministicModel();
    private sealed class DeterministicModel : ILanguageModel
    {
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(cancellationToken.IsCancellationRequested
                ? Result<ModelResponse>.Failure(Error.Rule("assistant.model_cancelled", "Cancelled."))
                : Result<ModelResponse>.Success(new("Hello", [], new(1, 1), ModelStopReason.Completed)));
    }
}

public sealed class AssistantOptionsTests
{
    [Theory]
    [InlineData(nameof(AssistantOptions.MaxSteps))]
    [InlineData(nameof(AssistantOptions.MaxTotalTokens))]
    [InlineData(nameof(AssistantOptions.MaxReplyCharacters))]
    [InlineData(nameof(AssistantOptions.HistoryMessages))]
    public void ZeroLimits_AreRejected(string property)
    {
        var options = new AssistantOptions();
        typeof(AssistantOptions).GetProperty(property)!.SetValue(options, 0);
        Assert.False(Validator.TryValidateObject(options, new ValidationContext(options), [], true));
    }
}
