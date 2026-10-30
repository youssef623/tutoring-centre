using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public abstract class ConversationsTestBase(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    protected static readonly DateTimeOffset Now = new(2027, 3, 1, 8, 0, 0, TimeSpan.Zero);
    protected Guid NileAccountId { get; private set; }
    protected Guid MaadiAccountId { get; private set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        NileAccountId = (await RegisterAsync(NileCentreId, "100000000000001")).Value;
        MaadiAccountId = (await RegisterAsync(MaadiCentreId, "100000000000002")).Value;
    }

    protected Task<Result<Guid>> RegisterAsync(Guid centre, string externalId) =>
        SendAsync<RegisterChannelAccountCommand, Guid>(centre, new(ChannelProvider.Whatsapp, externalId, "+201000000091"));

    protected Task<Result<T>> SendAsync<TCommand, T>(Guid centre, TCommand command) where TCommand : ICommand<T> =>
        Fixture.Services.GetRequiredService<ISystemScopeRunner>().RunAsync(centre, "test:conversations",
            (services, ct) => services.GetRequiredService<Dispatcher>().SendAsync<TCommand, T>(command, ct));

    protected async Task<int> ExecuteAsync(string sql, string? connectionString = null, Guid? centre = null)
    {
        await using var connection = new NpgsqlConnection(connectionString ?? Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centre is not null)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.Value.ToString());
            await setting.ExecuteNonQueryAsync();
        }
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        var count = await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
        return count;
    }
}
