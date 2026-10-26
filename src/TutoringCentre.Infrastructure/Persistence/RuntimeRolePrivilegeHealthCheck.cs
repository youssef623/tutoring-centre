using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Fails readiness when the runtime connection reaches PostgreSQL as a role that could defeat row-level security:
/// a superuser, a role with BYPASSRLS, or the schema-owning role itself. None of the three is constrained by RLS,
/// so a connection string pointed at one of them (by misconfiguration, not code) would make tenant isolation
/// unenforceable without anything else noticing. A tripwire for a control that configuration alone can switch off.
/// </summary>
public sealed class RuntimeRolePrivilegeHealthCheck(string connectionString) : IHealthCheck
{
    // Generic on purpose: readiness is anonymous, so the body must give an attacker nothing to work with.
    private const string GenericDescription = "The runtime database connection does not have the expected least-privilege role.";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            "select rolsuper or rolbypassrls or rolname = @ownerRole from pg_roles where rolname = current_user;",
            connection);
        command.Parameters.AddWithValue("ownerRole", DatabaseRoles.Owner);

        var isPrivileged = (bool)(await command.ExecuteScalarAsync(cancellationToken))!;

        return isPrivileged ? HealthCheckResult.Unhealthy(GenericDescription) : HealthCheckResult.Healthy();
    }
}
