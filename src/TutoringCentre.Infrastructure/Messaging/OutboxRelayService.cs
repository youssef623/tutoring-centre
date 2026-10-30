using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Infrastructure.Messaging;

/// <summary>At-least-once delivery. Claims are committed before dispatch; acknowledgements follow command commit.</summary>
public sealed partial class OutboxRelayService(OutboxStore store, OutboxRegistry registry, ISystemScopeRunner runner,
    IClock clock, ILogger<OutboxRelayService> logger) : BackgroundService
{
    private readonly string _worker = Environment.MachineName[..Math.Min(Environment.MachineName.Length, 24)]
        + ":" + Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + Guid.NewGuid().ToString("N")[..16];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Started(logger);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var count = 0;
                try { count = await ProcessBatchAsync(stoppingToken); }
                catch (Exception exception) when (exception is not OperationCanceledException || !stoppingToken.IsCancellationRequested)
                {
                    PollFailed(logger, exception.GetType().Name);
                }
                if (count < OutboxOptions.BatchSize)
                    await Task.Delay(OutboxOptions.IdlePollInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Unfinished claims are deliberately left to expire; no acknowledgement precedes a commit.
        }
    }

    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken = default)
    {
        var rows = await store.ClaimAsync(_worker, clock.UtcNow, OutboxOptions.Lease, OutboxOptions.BatchSize, cancellationToken);
        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessAsync(row, cancellationToken);
        }
        return rows.Count;
    }

    private async Task ProcessAsync(ClaimedOutboxMessage row, CancellationToken cancellationToken)
    {
        var outcome = "abandoned";
        try
        {
            if (registry.IsDeferred(row.Type))
            {
                outcome = await store.ReleaseForRetryAsync(row.Id, _worker, clock.UtcNow.AddMinutes(5), "consumer_deferred", cancellationToken) == 0
                    ? "leaseLost" : "deferred";
                return;
            }
            if (!registry.TryResolve(row.Type, row.Payload, out var delivery))
            {
                outcome = await store.DeadLetterAsync(row.Id, _worker, clock.UtcNow, "unknown_type", cancellationToken) == 0
                    ? "leaseLost" : "deadLettered";
                return;
            }
            var correlation = string.IsNullOrWhiteSpace(row.CorrelationId) ? "outbox:" + row.Id.ToString("N") : row.CorrelationId;
            var result = await runner.RunAsync(row.CentreId, correlation, (services, ct) => delivery!.DispatchAsync(services, ct), cancellationToken);
            if (result.IsFailure && registry.IsRetryable(row.Type, result.Error!.Code))
            {
                var delay = TimeSpan.FromSeconds(Math.Min(5 * Math.Pow(2, row.Attempts - 1), 600));
                outcome = await store.ReleaseForRetryAsync(row.Id, _worker, clock.UtcNow.Add(delay), result.Error.Code, cancellationToken) == 0
                    ? "leaseLost" : "retry";
                return;
            }
            var changed = result.IsSuccess
                ? await store.MarkSucceededAsync(row.Id, _worker, clock.UtcNow, cancellationToken)
                : await store.MarkFailedAsync(row.Id, _worker, clock.UtcNow, result.Error!.Code, cancellationToken);
            outcome = changed == 0 ? "leaseLost" : result.IsSuccess ? "succeeded" : "failed";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            var errorType = exception.GetType().Name;
            if (row.Attempts >= 8 && !registry.IsDeferred(row.Type))
                outcome = await store.DeadLetterAsync(row.Id, _worker, clock.UtcNow, errorType, cancellationToken) == 0
                    ? "leaseLost" : "deadLettered";
            else
            {
                var delay = TimeSpan.FromSeconds(Math.Min(5 * Math.Pow(2, row.Attempts - 1), 600));
                outcome = await store.ReleaseForRetryAsync(row.Id, _worker, clock.UtcNow.Add(delay), errorType, cancellationToken) == 0
                    ? "leaseLost" : "retry";
            }
        }
        finally { Completed(logger, row.Type, outcome, row.Attempts, row.Id); }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox relay started")]
    private static partial void Started(ILogger logger);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Outbox polling failed {ErrorType}")]
    private static partial void PollFailed(ILogger logger, string errorType);
    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox message {Type} {Outcome} {Attempts} {OutboxId}")]
    private static partial void Completed(ILogger logger, string type, string outcome, int attempts, Guid outboxId);
}
