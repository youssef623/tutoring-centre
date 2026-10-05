using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>
/// Lets `dotnet ef` create <see cref="AppDbContext"/> at design time, from ConnectionStrings:PostgresMigrations
/// (the tutoring_owner login) through the same <see cref="AppDbContextOptionsConfigurator"/> the runtime migration
/// path uses. EF Core tooling finds this automatically and skips building the full host for it. Tooling, like the
/// seed command alongside it: the one other place in Api allowed to reference Infrastructure directly (S7).
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    // Matches TutoringCentre.Api.csproj's UserSecretsId: a design-time factory bypasses WebApplication.CreateBuilder,
    // which is what would otherwise wire user-secrets up automatically in Development.
    private const string ApiUserSecretsId = "a0748b74-413e-4737-b25c-2a2e46b2d4e6";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(ApiUserSecretsId)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("PostgresMigrations");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:PostgresMigrations is required to create the design-time context. Set it via "
                + "user-secrets locally, or the ConnectionStrings__PostgresMigrations environment variable in CI.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        AppDbContextOptionsConfigurator.Configure(optionsBuilder, connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
