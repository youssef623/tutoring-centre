namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// The two least-privilege PostgreSQL roles bootstrapped outside migrations (db/bootstrap-roles.sh). Named once so
/// grant migrations and the readiness check can't drift from what was actually created.
/// </summary>
public static class DatabaseRoles
{
    /// <summary>Owns the schemas and tables; migrates. Never used by the running app.</summary>
    public const string Owner = "tutoring_owner";

    /// <summary>The app's runtime login: no DDL rights beyond what later migrations grant it explicitly.</summary>
    public const string App = "tutoring_app";
}
