using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>Logs execution lifecycle without arguments, exception messages or serialized jobs.</summary>
public sealed partial class JobLoggingFilter(ILogger<JobLoggingFilter> logger) : JobFilterAttribute, IServerFilter
{
    public void OnPerforming(PerformingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Started(logger, context.BackgroundJob.Id, context.BackgroundJob.Job.Type.FullName);
    }

    public void OnPerformed(PerformedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Exception is null) Succeeded(logger, context.BackgroundJob.Id, context.BackgroundJob.Job.Type.FullName);
        else Failed(logger, context.BackgroundJob.Id, context.BackgroundJob.Job.Type.FullName);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Job started {JobId} {JobType}")]
    private static partial void Started(ILogger logger, string jobId, string? jobType);
    [LoggerMessage(Level = LogLevel.Information, Message = "Job succeeded {JobId} {JobType}")]
    private static partial void Succeeded(ILogger logger, string jobId, string? jobType);
    [LoggerMessage(Level = LogLevel.Error, Message = "Job failed {JobId} {JobType}")]
    private static partial void Failed(ILogger logger, string jobId, string? jobType);
}
