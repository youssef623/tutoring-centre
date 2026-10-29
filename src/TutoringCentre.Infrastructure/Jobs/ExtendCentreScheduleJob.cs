using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>One centre, one command transaction. Business refusals complete; exceptions reach Hangfire.</summary>
public sealed partial class ExtendCentreScheduleJob(ISystemScopeRunner runner, ILogger<ExtendCentreScheduleJob> logger)
{
    // Hangfire injects PerformContext at execution; it is never serialized as job data.
    public async Task ExecuteAsync(Guid centreId, PerformContext? context, CancellationToken cancellationToken)
    {
        var result = await runner.RunAsync(centreId, "job:" + (context?.BackgroundJob.Id ?? "direct"), async (services, ct) =>
        {
            var zone = await services.GetRequiredService<ICentreTimeZoneProvider>().GetForCurrentCentreAsync(ct);
            var today = CentreCalendar.Today(services.GetRequiredService<IClock>(), zone);
            return await services.GetRequiredService<Dispatcher>().SendAsync<ExtendScheduleCommand, ExtendScheduleResult>(
                new ExtendScheduleCommand(today.AddDays(ScheduleHorizon.Days)), ct);
        }, cancellationToken);
        if (result.IsSuccess) Extended(logger, centreId, result.Value.SessionsCreated);
        else Refused(logger, centreId, result.Error!.Code);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Schedule extended {CentreId} {SessionsCreated}")]
    private static partial void Extended(ILogger logger, Guid centreId, int sessionsCreated);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Schedule extension refused {CentreId} {ErrorCode}")]
    private static partial void Refused(ILogger logger, Guid centreId, string errorCode);
}
