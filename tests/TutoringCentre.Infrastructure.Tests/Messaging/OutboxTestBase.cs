using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Messaging;

public abstract class OutboxTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture), IAsyncLifetime
{
    private protected static readonly DateTimeOffset Now = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);
    private ServiceProvider? _services;
    private protected FakeTimeProvider Time { get; } = new(Now);
    private protected DeliveryState State { get; } = new();
    private protected MessageLog Log { get; } = new();
    private protected ServiceProvider Services => _services ??= Fixture.CreateServiceProvider(services =>
    {
        services.AddSingleton<TimeProvider>(Time);
        services.AddSingleton(State);
        services.AddSingleton<ILogger<OutboxRelayService>>(Log);
        services.AddOutboxTestCommands();
    });
    private protected OutboxStore Store => Services.GetRequiredService<OutboxStore>();
    private protected OutboxRelayService Relay => Services.GetRequiredService<OutboxRelayService>();

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_services is not null) await _services.DisposeAsync();
        await DisposeAsync();
    }

    private protected async Task<Guid> SeedAsync(Guid? centreId = null, string? type = null,
        Guid? messageId = null, DateTimeOffset? availableAt = null)
    {
        var id = Guid.CreateVersion7();
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            insert into jobs.outbox_messages (id, centre_id, type, payload, correlation_id, occurred_at, available_at)
            values (@id, @centre, @type, @payload::jsonb, 'test:delivery', @now, @available);
            """, connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("centre", centreId ?? NileCentreId);
        command.Parameters.AddWithValue("type", type ?? DeliveryMessage.TypeName);
        command.Parameters.AddWithValue("payload", JsonSerializer.Serialize(new DeliveryMessage(messageId ?? id), JsonSerializerOptions.Web));
        command.Parameters.AddWithValue("now", Time.GetUtcNow());
        command.Parameters.AddWithValue("available", availableAt ?? Time.GetUtcNow());
        await command.ExecuteNonQueryAsync();
        return id;
    }

    private protected sealed class MessageLog : ILogger<OutboxRelayService>
    {
        internal List<string> Entries { get; } = [];
        internal List<string[]> Fields { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add(formatter(state, exception));
            if (state is IEnumerable<KeyValuePair<string, object?>> fields)
                Fields.Add(fields.Select(field => field.Key).Where(key => key != "{OriginalFormat}").Order(StringComparer.Ordinal).ToArray());
        }
    }
}
