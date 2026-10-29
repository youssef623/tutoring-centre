namespace TutoringCentre.Application.Common.Security;

/// <summary>Trusted non-HTTP work in a fresh scope with an explicit system actor and correlation.</summary>
public interface ISystemScopeRunner
{
    Task<TResult> RunAsync<TResult>(Guid? centreId, string correlationId,
        Func<IServiceProvider, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);
}
