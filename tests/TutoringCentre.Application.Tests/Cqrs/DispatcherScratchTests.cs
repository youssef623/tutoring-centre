using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Cqrs;

// Scratch test to step through the Dispatcher in the debugger. Delete once Day 8's proper
// Dispatcher tests (with real handlers and a fake IUnitOfWork fixture) replace it.
public sealed class DispatcherScratchTests
{
    private sealed record PingCommand : ICommand<Unit>;

    private sealed class PingCommandHandler : ICommandHandler<PingCommand, Unit>
    {
        public Task<Result<Unit>> HandleAsync(PingCommand command, CancellationToken cancellationToken) =>
            Task.FromResult(Result<Unit>.Success(Unit.Value));
    }

    private sealed class RecordingUnitOfWork : IUnitOfWork
    {
        public List<string> Calls { get; } = [];

        public Task BeginAsync(bool readOnly, CancellationToken ct)
        {
            Calls.Add(nameof(BeginAsync));
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            Calls.Add(nameof(SaveChangesAsync));
            return Task.CompletedTask;
        }

        public Task CommitAsync(CancellationToken ct)
        {
            Calls.Add(nameof(CommitAsync));
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken ct)
        {
            Calls.Add(nameof(RollbackAsync));
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Scratch_PingCommand_RunsBeginSaveCommitInOrder()
    {
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<PingCommand, Unit>, PingCommandHandler>();
        var provider = services.BuildServiceProvider();

        var unitOfWork = new RecordingUnitOfWork();
        var dispatcher = new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance);

        var result = await dispatcher.SendAsync<PingCommand, Unit>(new PingCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["BeginAsync", "SaveChangesAsync", "CommitAsync"], unitOfWork.Calls);
    }
}
