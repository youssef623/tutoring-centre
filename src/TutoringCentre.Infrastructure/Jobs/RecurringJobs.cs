using Hangfire;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>Called only by the enabled job host. IDs are stable upsert keys.</summary>
public static class RecurringJobs
{
    public static void Register(IRecurringJobManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);
        manager.AddOrUpdate<ExtendSchedulesFanOutJob>("schedules.extend-nightly",
            job => job.ExecuteAsync(null, CancellationToken.None), "17 2 * * *",
            new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo") });
        manager.AddOrUpdate<CreateChargesFanOutJob>("billing.monthly-charges",
            job => job.ExecuteAsync(null, CancellationToken.None), "30 0 1 * *",
            new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo") });
        manager.AddOrUpdate<OutboxMaintenanceJob>("outbox.maintenance",
            job => job.ExecuteAsync(CancellationToken.None), "11 3 * * *",
            new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo") });
    }
}
