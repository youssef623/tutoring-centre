using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Common.Cqrs;

/// <summary>Executes one query. Runs inside a read-only transaction and never saves (dispatcher, Day 7).</summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
