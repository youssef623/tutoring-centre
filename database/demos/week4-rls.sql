-- Week 4 demo: row-level security on academics.subjects, shown with no application code involved.
--
-- Everything here — the two demo centres, the four demo subjects, the four results below — runs inside one
-- transaction that is rolled back at the end: no demo data is left behind, and the row count in
-- academics.subjects is the same before and after.
--
-- Run as the real PostgreSQL superuser (the one role FORCE ROW LEVEL SECURITY never applies to), which must
-- also be able to SET ROLE to tutoring_app: superusers can do this without membership. Connection details come
-- from standard libpq environment variables (PGHOST/PGPORT/PGUSER/PGPASSWORD/PGDATABASE) — never from this
-- file, never from a command-line argument. For the local Compose database:
--
--   PGPASSWORD="$POSTGRES_PASSWORD" psql -h localhost -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
--       -f database/demos/week4-rls.sql
--
-- Print the error from result 4 without aborting the script early.
\set ON_ERROR_STOP off

BEGIN;

-- Two throwaway demo centres, so this demo depends on nothing already in the database.
INSERT INTO platform.centres (id, name, slug, time_zone_id, default_locale, created_at)
VALUES (gen_random_uuid(), 'Demo Nile', 'week4-demo-nile-' || substr(gen_random_uuid()::text, 1, 8), 'Africa/Cairo', 'en', now())
RETURNING id AS nile_id \gset

INSERT INTO platform.centres (id, name, slug, time_zone_id, default_locale, created_at)
VALUES (gen_random_uuid(), 'Demo Maadi', 'week4-demo-maadi-' || substr(gen_random_uuid()::text, 1, 8), 'Africa/Cairo', 'en', now())
RETURNING id AS maadi_id \gset

-- Two demo subjects per centre, seeded as the superuser — bypassing every isolation layer, same as
-- PostgresFixture.SeedSubjectAsync in the test suite.
INSERT INTO academics.subjects (id, centre_id, name, normalized_name, status, created_at)
VALUES
    (gen_random_uuid(), :'nile_id', 'Mathematics', 'MATHEMATICS', 'active', now()),
    (gen_random_uuid(), :'nile_id', 'Chemistry', 'CHEMISTRY', 'active', now()),
    (gen_random_uuid(), :'maadi_id', 'Physics', 'PHYSICS', 'active', now()),
    (gen_random_uuid(), :'maadi_id', 'Biology', 'BIOLOGY', 'active', now());

-- From here on, act as the application's own runtime role — no superuser, no BYPASSRLS, exactly what the API
-- connects as. SET LOCAL reverts automatically when this transaction ends.
SET LOCAL ROLE tutoring_app;

\echo '--- Result 1: count as tutoring_app with no centre set (expect 0) ---'
SELECT count(*) FROM academics.subjects;

\echo '--- Result 2: count as tutoring_app with Nile set (expect 2) ---'
SELECT set_config('app.current_centre', :'nile_id', true);
SELECT count(*) FROM academics.subjects;

\echo '--- Result 3: count as tutoring_app with Maadi set (expect 2) ---'
SELECT set_config('app.current_centre', :'maadi_id', true);
SELECT count(*) FROM academics.subjects;

\echo '--- Result 4: cross-tenant insert while scoped to Nile (expect ERROR 42501, permission denied) ---'
SELECT set_config('app.current_centre', :'nile_id', true);
SAVEPOINT before_cross_tenant_insert;
INSERT INTO academics.subjects (id, centre_id, name, normalized_name, status, created_at)
VALUES (gen_random_uuid(), :'maadi_id', 'Smuggled', 'SMUGGLED', 'active', now());
ROLLBACK TO SAVEPOINT before_cross_tenant_insert;

-- The whole demo — centres, subjects, role change — never happened as far as the database is concerned.
ROLLBACK;
