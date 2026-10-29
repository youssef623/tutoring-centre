using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Infrastructure.Messaging;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>Platform queue hygiene only; dead letters always require an operator.</summary>
public sealed partial class OutboxMaintenanceJob(OutboxStore store, IClock clock, ILogger<OutboxMaintenanceJob> logger)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var cutoff = clock.UtcNow.AddDays(-30);
        while (await store.PurgeProcessedAsync(cutoff, cancellationToken) == 1000)
            cancellationToken.ThrowIfCancellationRequested();
        var deadLetters = await store.CountDeadLetteredAsync(cancellationToken);
        if (deadLetters > 0) DeadLetters(logger, deadLetters);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Outbox dead letters require attention {DeadLetterCount}")]
    private static partial void DeadLetters(ILogger logger, long deadLetterCount);
}
