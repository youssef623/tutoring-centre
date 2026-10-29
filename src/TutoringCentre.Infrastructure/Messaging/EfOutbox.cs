using System.Text.Json;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Messaging;

internal sealed class EfOutbox(AppDbContext db, ICurrentActor actor, ICorrelationContext correlation, IClock clock) : IOutbox
{
    public void Enqueue<TMessage>(TMessage message) where TMessage : class, IOutboxMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        var centreId = actor.Actor.CentreId ?? throw new InvalidOperationException("An outbox message requires an acting centre.");
        var now = clock.UtcNow;
        db.Set<OutboxMessage>().Add(new OutboxMessage
        {
            Id = Guid.CreateVersion7(), CentreId = centreId, Type = TMessage.TypeName,
            Payload = JsonSerializer.Serialize(message, JsonSerializerOptions.Web),
            CorrelationId = correlation.CorrelationId, OccurredAt = now, AvailableAt = now,
        });
    }
}
