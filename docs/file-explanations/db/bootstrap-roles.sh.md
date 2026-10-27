# `db/bootstrap-roles.sh`

## Purpose

Current database/schema file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **database/schema**. [Open current file](../../../db/bootstrap-roles.sh). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 121 lines and 4,747 bytes. Source hash: `d6c8da097a8f8f531ca492d972fed12f7cf3bccddc25aa0eb9fb3776f0ec6869`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 2: Idempotent creation of the tutoring_owner and tutoring_app database roles.
- Line 4: tutoring_owner owns the application schemas and runs migrations.
- Line 5: tutoring_app is the runtime login used by the API: it cannot change schema,
- Line 6: is not a superuser, and cannot bypass row-level security.
- Line 8: Reusable by any caller that points standard libpq connection env vars
- Line 9: (PGHOST/PGPORT/PGUSER/PGPASSWORD/PGDATABASE, or the Postgres image's own
- Line 10: POSTGRES_USER/POSTGRES_DB/POSTGRES_PASSWORD) at a target database and runs
- Line 11: this script as a role that can create roles (a superuser, locally):
- Line 12: - the local Compose database, via docker-entrypoint-initdb.d on first start
- Line 13: - the integration test fixtures, against a Testcontainers Postgres instance
- Line 14: - a cloud administrator, against a managed Postgres instance
- Line 16: Required env vars: TUTORING_OWNER_PASSWORD, TUTORING_APP_PASSWORD.
- Line 17: Never pass passwords as command-line arguments (visible via `ps`) and never
- Line 18: write them to a file; this script only ever forwards them to psql as
- Line 19: variables, which substitutes them straight into the session.
- Line 25: psql has no implicit database to connect to; fall back to the Postgres
- Line 26: image's POSTGRES_USER/POSTGRES_DB when the standard PGUSER/PGDATABASE
- Line 27: aren't already set (e.g. by a cloud administrator or the test fixtures).

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```bash
#!/usr/bin/env bash
# Idempotent creation of the tutoring_owner and tutoring_app database roles.
#
# tutoring_owner owns the application schemas and runs migrations.
# tutoring_app is the runtime login used by the API: it cannot change schema,
# is not a superuser, and cannot bypass row-level security.
#
# Reusable by any caller that points standard libpq connection env vars
# (PGHOST/PGPORT/PGUSER/PGPASSWORD/PGDATABASE, or the Postgres image's own
# POSTGRES_USER/POSTGRES_DB/POSTGRES_PASSWORD) at a target database and runs
# this script as a role that can create roles (a superuser, locally):
#   - the local Compose database, via docker-entrypoint-initdb.d on first start
#   - the integration test fixtures, against a Testcontainers Postgres instance
#   - a cloud administrator, against a managed Postgres instance
#
# Required env vars: TUTORING_OWNER_PASSWORD, TUTORING_APP_PASSWORD.
# Never pass passwords as command-line arguments (visible via `ps`) and never
# write them to a file; this script only ever forwards them to psql as
# variables, which substitutes them straight into the session.
set -euo pipefail

: "${TUTORING_OWNER_PASSWORD:?TUTORING_OWNER_PASSWORD must be set}"
: "${TUTORING_APP_PASSWORD:?TUTORING_APP_PASSWORD must be set}"

# psql has no implicit database to connect to; fall back to the Postgres
# image's POSTGRES_USER/POSTGRES_DB when the standard PGUSER/PGDATABASE
# aren't already set (e.g. by a cloud administrator or the test fixtures).
export PGUSER="${PGUSER:-${POSTGRES_USER:-}}"
export PGDATABASE="${PGDATABASE:-${POSTGRES_DB:-}}"

psql -v ON_ERROR_STOP=1 \
     -v "owner_password=${TUTORING_OWNER_PASSWORD}" \
     -v "app_password=${TUTORING_APP_PASSWORD}" \
     --no-psqlrc <<'SQL'
-- Create each role only when absent, then always reset its attributes so a
-- second run converges to the same state instead of erroring.
do $$
begin
    if not exists (select 1 from pg_roles where rolname = 'tutoring_owner') then
        create role tutoring_owner;
    end if;
end
$$;

alter role tutoring_owner with
    login
    nosuperuser
    nocreatedb
    nocreaterole
    nobypassrls
    password :'owner_password';

do $$
begin
    if not exists (select 1 from pg_roles where rolname = 'tutoring_app') then
        create role tutoring_app;
    end if;
end
$$;

alter role tutoring_app with
    login
    nosuperuser
    nocreatedb
    nocreaterole
    nobypassrls
    password :'app_password';

-- tutoring_owner may create schemas/tables and migrate; tutoring_app only connects.
-- Neither role is ever made a member of the other, so tutoring_app can never
-- SET ROLE to tutoring_owner.
do $$
begin
    execute format('grant create, connect on database %I to tutoring_owner', current_database());
    execute format('grant connect on database %I to tutoring_app', current_database());
end
$$;

-- Move ownership of the platform and identity schemas (src/TutoringCentre.Infrastructure/
-- Persistence/Schemas.cs), and everything in them, from whoever currently owns them (the
-- migrating superuser) onto tutoring_owner. Scoped to just those schemas: REASSIGN OWNED BY
-- also touches the database object itself, which Postgres refuses while connected to it,
-- and which we don't want to move anyway.
do $$
declare
    rel record;
    target_schema text;
begin
    foreach target_schema in array array['platform', 'identity'] loop
        if exists (select 1 from pg_namespace where nspname = target_schema) then
```

The excerpt stops at line 90; use the original file for the remaining 31 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
