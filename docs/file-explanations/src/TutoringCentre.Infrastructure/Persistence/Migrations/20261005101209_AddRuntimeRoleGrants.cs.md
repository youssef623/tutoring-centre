# `src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs`

## Purpose

Source summary (intent, not independent execution proof): Gives tutoring_app exactly the access it needs on the Month 1 tables: no CREATE, no TRUNCATE, no REFERENCES, no ALL. Centres and memberships are never hard-deleted (Membership.Deactivate), so neither gets DELETE; the other Identity tables do, since Identity itself removes claims/logins/tokens outright.

## Where it fits

Category: **migration**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 61 lines and 3,124 bytes. Source hash: `3e48e6f95ba2381ced1da2e86f557ba70917899c32e44eab6298e24f83da4e65`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261005101209_AddRuntimeRoleGrants.cs#L12): `public partial class AddRuntimeRoleGrants : Migration`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
