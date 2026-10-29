using Hangfire.PostgreSql;
using Hangfire.PostgreSql.Factories;

namespace TutoringCentre.Infrastructure.Jobs;

/// <summary>One storage definition for the owner installer and runtime clients. Runtime never prepares schemas.</summary>
public static class JobsStorage
{
    public static PostgreSqlStorage Create(string connectionString, bool prepareSchema)
    {
        var options = new PostgreSqlStorageOptions
        {
            SchemaName = "hangfire",
            PrepareSchemaIfNecessary = prepareSchema,
            QueuePollInterval = TimeSpan.FromSeconds(5),
            AllowDegradedModeWithoutStorage = false,
        };
        return new PostgreSqlStorage(new NpgsqlConnectionFactory(connectionString, options), options);
    }
}
