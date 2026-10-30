using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

internal sealed record DeliveryMessage(Guid MessageId) : IOutboxMessage
{
    public static string TypeName => "test.delivery";
}

internal sealed record EnqueueProbeCommand(Guid Id, bool Fail) : ICommand<Unit>, ITenantScoped;

internal sealed class EnqueueProbeHandler(AppDbContext db, IOutbox outbox, ICurrentActor actor) : ICommandHandler<EnqueueProbeCommand, Unit>
{
    public Task<Result<Unit>> HandleAsync(EnqueueProbeCommand command, CancellationToken cancellationToken)
    {
        db.Set<TenantProbe>().Add(new TenantProbe { Id = command.Id, CentreId = actor.Actor.CentreId!.Value, Label = "enqueued" });
        outbox.Enqueue(new DeliveryMessage(command.Id));
        return Task.FromResult(command.Fail
            ? Result<Unit>.Failure(Error.Rule("test.refused", "Expected test refusal."))
            : Result<Unit>.Success(Unit.Value));
    }
}

internal static class OutboxTestRegistration
{
    internal static void AddOutboxTestCommands(this IServiceCollection services)
    {
        services.TryAddSingleton<DeliveryState>();
        services.AddScoped<AppDbContext>(provider => provider.GetRequiredService<TenantProbeDbContext>());
        services.AddScoped<ICommandHandler<EnqueueProbeCommand, Unit>, EnqueueProbeHandler>();
        services.AddScoped<ICommandHandler<DeliverProbeCommand, Unit>, DeliverProbeHandler>();
        services.AddScoped<DeliveryScope>();
        services.AddOutboxMessage<DeliveryMessage, DeliverProbeCommand>(message => new DeliverProbeCommand(message.MessageId), "assistant.conversation_busy");
    }
}

internal sealed record DeliverProbeCommand(Guid MessageId) : ICommand<Unit>, ITenantScoped;
internal enum DeliveryMode { Succeed, Fail, Throw, Busy }
internal sealed class DeliveryScope { internal Guid Id { get; } = Guid.NewGuid(); }
internal sealed class DeliveryState
{
    internal DeliveryMode Mode { get; set; }
    internal int Calls { get; set; }
    internal List<(Guid Centre, string Setting, string? Correlation, Guid Scope)> Observations { get; } = [];
}

internal sealed class DeliverProbeHandler(AppDbContext db, ICurrentActor actor, ICorrelationContext correlation,
    DeliveryState state, DeliveryScope scope) : ICommandHandler<DeliverProbeCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(DeliverProbeCommand command, CancellationToken cancellationToken)
    {
        state.Calls++;
        await using var read = db.Database.GetDbConnection().CreateCommand();
        read.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
        read.CommandText = "select current_setting('app.current_centre', true)";
        var setting = (string)(await read.ExecuteScalarAsync(cancellationToken))!;
        var centre = actor.Actor.CentreId!.Value;
        state.Observations.Add((centre, setting, correlation.CorrelationId, scope.Id));
        if (state.Mode == DeliveryMode.Busy) return Result<Unit>.Failure(Error.Conflict("assistant.conversation_busy", "Conversation is busy."));
        if (state.Mode == DeliveryMode.Throw) throw new InvalidOperationException("Sensitive exception message must never be stored or logged.");
        if (state.Mode == DeliveryMode.Fail) return Result<Unit>.Failure(Error.Rule("test.business_failure", "Business message must never be stored."));
        if (!await db.Set<TenantProbe>().AnyAsync(probe => probe.Id == command.MessageId, cancellationToken))
            db.Set<TenantProbe>().Add(new TenantProbe { Id = command.MessageId, CentreId = centre, Label = "delivered" });
        return Result<Unit>.Success(Unit.Value);
    }
}
