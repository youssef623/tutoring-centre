using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>Lists IDs only. Retried parents may enqueue duplicates; extension is idempotent.</summary>
public sealed partial class ExtendSchedulesFanOutJob(ISystemScopeRunner runner, IBackgroundJobClient client,
    ILogger<ExtendSchedulesFanOutJob> logger)
{
    public async Task ExecuteAsync(PerformContext? context, CancellationToken cancellationToken)
    {
        var count = await runner.RunAsync(null, "job:" + (context?.BackgroundJob.Id ?? "direct"), async (services, ct) =>
        {
            var ids = await services.GetRequiredService<ICentreDirectory>().ListCentreIdsAsync(ct);
            foreach (var id in ids)
            {
                ct.ThrowIfCancellationRequested();
                client.Enqueue<ExtendCentreScheduleJob>(job => job.ExecuteAsync(id, null, CancellationToken.None));
            }
            return ids.Count;
        }, cancellationToken);
        Enqueued(logger, count);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Schedule children enqueued {ChildrenEnqueued}")]
    private static partial void Enqueued(ILogger logger, int childrenEnqueued);
}
