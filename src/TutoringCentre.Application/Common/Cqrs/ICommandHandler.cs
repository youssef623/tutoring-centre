using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Executes one command. Always returns a Result: business failures are values, not exceptions.</summary>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
