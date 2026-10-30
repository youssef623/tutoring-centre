using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Tests.Assistant;

public sealed class AgentTransactionBudgetTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ServerBudgetAlsoCancelsSaveOrCommitAndRollsBack(bool stallCommit)
    {
        using var services = new ServiceCollection()
            .AddSingleton<ICommandTimeoutPolicy>(new BudgetPolicy())
            .AddSingleton<ICommandHandler<BudgetCommand, Unit>>(new Handler())
            .BuildServiceProvider();
        var unit = new DelayedUnitOfWork(stallCommit);
        var dispatcher = new Dispatcher(services, unit, NullLogger<Dispatcher>.Instance);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            dispatcher.SendAsync<BudgetCommand, Unit>(new(), CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(3)));
        Assert.True(unit.BeginHasDeadline);
        Assert.True(unit.RolledBack);
        Assert.False(unit.RollbackCanBeCancelled);
    }

    private sealed record BudgetCommand : ICommand<Unit>;
    private sealed class BudgetPolicy : ICommandTimeoutPolicy
    {
        public TimeSpan? GetTimeout(object command) => command is BudgetCommand ? TimeSpan.FromMilliseconds(30) : null;
    }
    private sealed class Handler : ICommandHandler<BudgetCommand, Unit>
    {
        public Task<Result<Unit>> HandleAsync(BudgetCommand command, CancellationToken cancellationToken) => Task.FromResult(Result<Unit>.Success(Unit.Value));
    }
    private sealed class DelayedUnitOfWork(bool stallCommit) : IUnitOfWork
    {
        public bool BeginHasDeadline { get; private set; }
        public bool RolledBack { get; private set; }
        public bool RollbackCanBeCancelled { get; private set; }
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Mirrors the unit of work contract.")]
        public Task BeginAsync(bool readOnly, CancellationToken ct)
        {
            BeginHasDeadline = ct.CanBeCanceled;
            return Task.CompletedTask;
        }
        public async Task<Result> SaveChangesAsync(CancellationToken ct)
        {
            if (!stallCommit) await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            return Result.Success();
        }
        public Task CommitAsync(CancellationToken ct) => Task.Delay(Timeout.InfiniteTimeSpan, ct);
        public Task RollbackAsync(CancellationToken ct)
        {
            RolledBack = true;
            RollbackCanBeCancelled = ct.CanBeCanceled;
            return Task.CompletedTask;
        }
    }
}
