namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>Host switches and platform operator allowlist. Contains no connection secret.</summary>
public sealed class JobsOptions
{
    public bool RunServer { get; set; }
    public int WorkerCount { get; set; } = 2;
    public string DashboardPath { get; set; } = "/ops/jobs";
    public string[] OperatorEmails { get; set; } = [];
}
