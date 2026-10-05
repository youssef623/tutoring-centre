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
            for rel in
                select c.relname, c.relkind
                from pg_class c
                join pg_namespace n on n.oid = c.relnamespace
                where n.nspname = target_schema
                  and c.relowner <> (select oid from pg_roles where rolname = 'tutoring_owner')
                  and c.relkind in ('r', 'p', 'v', 'm', 'S')
                  -- Exclude sequences linked to an identity/serial column: changing
                  -- the owning table's owner (below) cascades to these automatically,
                  -- and Postgres refuses to change their owner directly.
                  and not (c.relkind = 'S' and exists (
                      select 1 from pg_depend d
                      where d.objid = c.oid and d.deptype = 'a'
                  ))
            loop
                execute format(
                    'alter %s %I.%I owner to tutoring_owner',
                    case rel.relkind when 'S' then 'sequence' else 'table' end,
                    target_schema,
                    rel.relname
                );
            end loop;

            execute format('alter schema %I owner to tutoring_owner', target_schema);
        end if;
    end loop;
end
$$;
SQL

echo "tutoring_owner and tutoring_app roles are ready."
