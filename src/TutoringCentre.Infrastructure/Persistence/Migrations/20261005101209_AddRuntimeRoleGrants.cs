using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Gives tutoring_app exactly the access it needs on the Month 1 tables: no CREATE, no TRUNCATE, no
    /// REFERENCES, no ALL. Centres and memberships are never hard-deleted (Membership.Deactivate), so neither
    /// gets DELETE; the other Identity tables do, since Identity itself removes claims/logins/tokens outright.
    /// </summary>
    public partial class AddRuntimeRoleGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                do $$
                begin
                    if not exists (select 1 from pg_roles where rolname = '{DatabaseRoles.Owner}') then
                        raise exception 'Role % is missing; run db/bootstrap-roles.sh before applying this migration.', '{DatabaseRoles.Owner}';
                    end if;
                    if not exists (select 1 from pg_roles where rolname = '{DatabaseRoles.App}') then
                        raise exception 'Role % is missing; run db/bootstrap-roles.sh before applying this migration.', '{DatabaseRoles.App}';
                    end if;
                end
                $$;
                """);

            migrationBuilder.GrantRuntimeAccess(Schemas.Platform);
            migrationBuilder.GrantRuntimeAccess(Schemas.Identity);

            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Platform}.centres to {DatabaseRoles.App};");
            migrationBuilder.Sql($"grant select on {Schemas.Platform}.__ef_migrations_history to {DatabaseRoles.App};");

            migrationBuilder.Sql($"""
                grant select, insert, update, delete
                    on {Schemas.Identity}.users, {Schemas.Identity}.user_claims, {Schemas.Identity}.user_logins, {Schemas.Identity}.user_tokens
                    to {DatabaseRoles.App};
                """);
            migrationBuilder.Sql($"grant select, insert, update on {Schemas.Identity}.memberships to {DatabaseRoles.App};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Identity}.memberships from {DatabaseRoles.App};");
            migrationBuilder.Sql($"""
                revoke select, insert, update, delete
                    on {Schemas.Identity}.users, {Schemas.Identity}.user_claims, {Schemas.Identity}.user_logins, {Schemas.Identity}.user_tokens
                    from {DatabaseRoles.App};
                """);

            migrationBuilder.Sql($"revoke select on {Schemas.Platform}.__ef_migrations_history from {DatabaseRoles.App};");
            migrationBuilder.Sql($"revoke select, insert, update on {Schemas.Platform}.centres from {DatabaseRoles.App};");

            migrationBuilder.RevokeRuntimeAccess(Schemas.Identity);
            migrationBuilder.RevokeRuntimeAccess(Schemas.Platform);
        }
    }
}
