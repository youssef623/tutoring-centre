namespace TutoringCentre.Application.Identity;

/// <summary>Platform operator allowlist. Independent of centre roles and selected membership.</summary>
public interface IOperatorDirectory
{
    Task<bool> IsOperatorAsync(Guid userId, CancellationToken cancellationToken = default);
}
