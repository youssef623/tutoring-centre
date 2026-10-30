using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Assistant.Models;

public interface ILanguageModel
{
    Task<Result<ModelResponse>> GenerateAsync(ModelRequest request, CancellationToken cancellationToken);
}
