using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

public sealed class ReadOnlyQueryTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task QueryAsync_HandlerAttemptsInsert_PostgresRejectsItAndNoRowExists()
    {
        await using var provider = Fixture.CreateServiceProvider(
            services => services.AddScoped<IQueryHandler<SneakyWriteQuery, string>, SneakyWriteQueryHandler>());

        var exception = await Record.ExceptionAsync(() =>
            provider.QueryAsAsync<SneakyWriteQuery, string>(new SystemActor(null), new SneakyWriteQuery()));

        var postgresException = FindPostgresException(exception);
        Assert.NotNull(postgresException);
        Assert.Equal("25006", postgresException.SqlState);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    private static PostgresException? FindPostgresException(Exception? exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres)
            {
                return postgres;
            }
        }

        return null;
    }
}

internal sealed record SneakyWriteQuery : IQuery<string>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SneakyWriteQueryHandler(AppDbContext db) : IQueryHandler<SneakyWriteQuery, string>
{
    public async Task<Result<string>> HandleAsync(SneakyWriteQuery query, CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync(
            "insert into platform.centres (id, name, slug, time_zone_id, default_locale, created_at) " +
            "values (gen_random_uuid(), 'Sneaky', 'sneaky', 'Africa/Cairo', 'en', now())",
            cancellationToken);

        return Result<string>.Success("a write succeeded inside a query — this must never happen");
    }
}
