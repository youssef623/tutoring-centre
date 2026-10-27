# `src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs`

## Purpose

Checks the actual PostgreSQL current_user for superuser, BYPASSRLS, or schema-owner identity and fails readiness with a generic description if any applies. It detects dangerous connection-string configuration independently of whether ordinary connectivity succeeds. Database exceptions flow to health-check handling.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,704 bytes. Source hash: `be8b5efa81e1fc744d9de976c86ebb0f78e29c4d9048f2a96e28715e8c6cd765`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.Diagnostics.HealthChecks;`
- Line 2: `using Npgsql;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs#L12): `public sealed class RuntimeRolePrivilegeHealthCheck(string connectionString) : IHealthCheck`
- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs#L17): `public async Task<HealthCheckResult> CheckHealthAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:25](../../../../onboarding-work/extend_backend_purposes.py#L25)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:79](../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L79)
- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:39](../../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L39)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
