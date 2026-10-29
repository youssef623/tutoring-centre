namespace TutoringCentre.Infrastructure.Messaging;

public sealed class OutboxOptions
{
    public bool RunRelay { get; set; }
    public const int BatchSize = 20;
    public static readonly TimeSpan Lease = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan IdlePollInterval = TimeSpan.FromSeconds(2);
}
