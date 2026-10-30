using System.ComponentModel.DataAnnotations;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Assistant;

/// <summary>Reusable contract for deterministic and provider-backed models.</summary>
public abstract class ModelContractTests
{
    protected abstract ILanguageModel CreateModel();
    protected static ModelRequest Request => new("System", [new(ModelRole.User, "Hello")], [], 100);

    [Fact]
    public async Task SuccessOrFailure_IsRepresentedByResult()
    {
        var result = await CreateModel().GenerateAsync(Request, CancellationToken.None);
        if (result.IsSuccess)
        {
            Assert.Null(result.Error);
            Assert.NotNull(result.Value.ToolCalls);
            Assert.True(result.Value.Usage.InputTokens >= 0);
        }
        else
        {
            Assert.NotNull(result.Error);
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }
    }

    [Fact]
    public async Task CancelledCall_DoesNotReturnSuccessfulText()
    {
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        var result = await CreateModel().GenerateAsync(Request, cancelled.Token);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}
