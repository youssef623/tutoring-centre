using TutoringCentre.Application.Identity;

namespace TutoringCentre.Application.Tests.Fakes;

public sealed class FakeOperatorDirectory : IOperatorDirectory
{
    public bool IsOperator { get; set; }
    public Task<bool> IsOperatorAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult(IsOperator);
}
