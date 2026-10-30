using TutoringCentre.Application.Common.Security;
namespace TutoringCentre.Application.Common.Execution;
public interface IActorScopeRunner
{
    Task<T> RunAsync<T>(ICurrentActor actor, Func<IServiceProvider, Task<T>> work);
}
