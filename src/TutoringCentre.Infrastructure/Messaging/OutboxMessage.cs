namespace TutoringCentre.Infrastructure.Messaging;

/// <summary>Platform persistence metadata, not a tenant entity. Payloads contain identifiers and simple values only.</summary>
public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public Guid CentreId { get; init; }
    public required string Type { get; init; }
    public required string Payload { get; init; }
    public string? CorrelationId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public DateTimeOffset AvailableAt { get; set; }
    public int Attempts { get; set; }
    public string? ClaimedBy { get; set; }
    public DateTimeOffset? ClaimedUntil { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string? Outcome { get; set; }
    public string? LastError { get; set; }
    public DateTimeOffset? DeadLetteredAt { get; set; }
}
