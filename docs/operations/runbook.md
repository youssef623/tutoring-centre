# Runbook

Commands that work when followed literally. Each section was either run for real today (2026-10-11, against
the local production stack — no Azure subscription is provisioned, see `docs/architecture/deployment.md`) or
is marked **not executed**, with the reason, rather than left to guesswork.

## 1. Deploy

**Not executed.** `deploy.yml` is not written in this slice: every step depends on a resource (registry,
jobs, container app, OIDC trust) that does not exist yet (Tasks 35.1–35.5). The commands below are the design
from `docs/architecture/deployment.md`'s six-step flow, to be proven true once that work lands.

**Normal path** — triggered automatically after CI goes green on a push to `main`:
1. `deploy.yml` runs in the `production` GitHub environment, authenticating to Azure via OIDC (no stored secret).
2. Build and push the image, tagged with the commit SHA.
3. Update and start the `migrate` job with that image and the owner connection; wait for success.
4. Update the container app's image; wait for the new revision to become ready.
5. Run `smoke/` (Task 35.8) against the new revision.
6. On smoke failure, activate the previous revision and fail the run.

**Manual trigger** — `workflow_dispatch`, same steps, used for the rollback and bad-revision drills:
```bash
gh workflow run deploy.yml --ref main
```

## 2. Roll back to the previous revision

**Design not executed against Azure** (no Container Apps resource exists). The equivalent was drilled today
against the local production stack, since the underlying question — does the system actually serve the older
SHA, then the current one again, on command? — is the same one either way.

**Azure (designed):**
```bash
az containerapp revision list --name <app> --resource-group <group> -o table
az containerapp ingress traffic set --name <app> --resource-group <group> \
  --revision-weight <previous-revision>=100
# confirm, then to return:
az containerapp ingress traffic set --name <app> --resource-group <group> \
  --revision-weight <current-revision>=100
```

**Local drill, run today, 2026-10-11 ~11:44 UTC** (corrected from the first draft, which assumed the stopped
container's Compose network alias `app` would simply transfer to a same-named replacement — it does not;
`--network-alias app` must be passed explicitly or Caddy's `reverse_proxy app:8080` gets a `502`):

```bash
# 1. Build an image from the previous commit without touching the working tree.
git archive <previous-sha> | docker build -t tutoring-centre:rollback-test \
  --build-arg COMMIT_SHA=<previous-sha> -f Dockerfile -

# 2. Stop the current revision, start the previous one under the same network alias Caddy expects.
docker stop tutoring-centre-app-1
docker run -d --name tutoring-centre-app-rollback --network tutoring-centre_prod --network-alias app \
  -e ASPNETCORE_ENVIRONMENT=Production -e Proxy__KnownNetworks__0=172.28.0.0/16 -e Demo__Enabled=true \
  -e "ConnectionStrings__Postgres=Host=postgres;Port=5432;Database=tutoring;Username=tutoring_app;Password=<TUTORING_APP_PASSWORD>" \
  tutoring-centre:rollback-test

# 3. Confirm the older SHA.
curl -sk https://localhost:8443/api/system/info
#   {"applicationVersion":"1.0.0+e45c80b", ...}   -- no "demoMode" key: that commit predates Task 35.7

# 4. Return to the current revision.
docker rm -f tutoring-centre-app-rollback
docker start tutoring-centre-app-1
curl -sk https://localhost:8443/api/system/info
#   {"applicationVersion":"1.0.0+aac9abe968d1...", ..., "demoMode":true}

docker rmi tutoring-centre:rollback-test   # clean up the throwaway image
```

**Observed:** older SHA (`e45c80b`) at 11:44:40 UTC; current SHA (`aac9abe968d1…`) restored at 11:45:10 UTC.
No data was changed — the drill only ever swapped which image served reads against the same database.

**When a rollback is safe.** Only while the schema the previous image expects is still compatible with what
the database actually has — i.e., no migration has run since that revision that the old code cannot tolerate
(a dropped or renamed column it still queries, a `NOT NULL` it doesn't populate). Code can go back one image;
the database cannot go back at all. This is also why the deploy flow migrates *before* updating the app
(§1, step 3): a rollback after a schema change failed to anticipate it is not a rollback, it is a second
incident.

## 3. Run the migrate job by hand

**Executed routinely throughout Day 36** against the local production stack:
```bash
docker compose -f compose.yaml -f compose.prod.yaml run --rm migrate
```
Idempotent — a database already up to date is a no-op (`MigrationRunner.ApplyMigrationsAsync`). Runs as
`tutoring_owner`; never the app's runtime role.

**Azure (designed, not executed):** `az containerapp job start --name <migrate-job> --resource-group <group>`.

## 4. Run the seed job on the demo

**Executed** (part of every `docker compose ... up --build` run this stack has done):
```bash
docker compose -f compose.yaml -f compose.prod.yaml run --rm seed
```
Gated: refuses outside Development/Testing unless `Demo:Enabled` is true (Task 34.7). Creates missing demo
accounts and subjects; **does not** change an existing account's password — see §6.

**Azure (designed, not executed):** `az containerapp job start --name <seed-job> --resource-group <group>`.

## 5. Rotate the two database role passwords

**Not executed** — out of this day's scope (Task 36.2 explicitly: documented, not run, unless asked). The
procedure, for when it is:
1. Generate two new passwords; never echo them to a terminal that logs, or write them to a file in the repo.
2. Update the `tutoring_owner` and `tutoring_app` PostgreSQL role passwords (`ALTER ROLE ... WITH PASSWORD ...`,
   run as the server administrator).
3. Update the corresponding Container Apps secrets (`ConnectionStrings__PostgresMigrations` on the jobs,
   `ConnectionStrings__Postgres` on the app and the jobs) to match.
4. Restart the app's revision so it picks up the new secret value; confirm `/health/ready` stays 200.
5. Confirm the old passwords no longer authenticate.

## 6. Rotate the demo password

**Executed**, on `owner@nile.test`, local production stack, 2026-10-11 11:34–11:42 UTC. No existing tool did
this: re-running the seed job only creates *missing* accounts (`DevelopmentIdentitySeeder.GetOrCreateUserAsync`
returns early for one that already exists, password untouched), and the self-service change-password flow
(Task 29/30) requires the current password, which an operator rotating a demo account does not have. A new
command, same trust level as migrate/seed (direct process execution against the database, never exposed over
HTTP), was added for exactly this:

```bash
dotnet run --project src/TutoringCentre.Api -- reset-password <email> <new-password>
```

Drill, run for real:
1. `dotnet run --project src/TutoringCentre.Api -- reset-password owner@nile.test <new-password>` → `Reset-password: password set for the requested account.`
2. Confirmed the **old** password now fails (`401 auth.invalid_credentials`) — but see the lockout note below.
3. Confirmed the **new** password signs in (`200`, full `MeDto`).
4. Ran `smoke/` with `SMOKE_PASSWORD=<new-password>` against the same stack: **5/5 passed.**
5. Rotated back to the original demo password afterward, so the documented demo credentials
   (`docs/architecture/deployment.md`) stay valid.

**One correction made from the actual run:** the first attempt to sign in with the *new* password, right
after rotation, also returned `401`. This was not the rotation failing — ASP.NET Core Identity's own
per-account lockout (`identity.users.access_failed_count`/`lockout_end`), tripped earlier today by the
live-verification rate-limit check (Task 36.1, item 6), blocks **any** password, correct or not, until it
expires. Confirmed directly:
```sql
SELECT email, access_failed_count, lockout_end, now() FROM identity.users WHERE email='owner@nile.test';
```
Waited out the lockout, then step 3 above succeeded. If this runbook is followed literally during an active
lockout, check `lockout_end` before concluding the rotation failed.

**Azure (designed):** the same command, run against the cloud database's connection string (never the app's
runtime secret) — `ConnectionStrings__Postgres` env var pointed at the Flexible Server, same `reset-password`
invocation, then update the `Seed:Password`/smoke-test secret to match.

## 7. Read logs and find a trace by correlation ID

**Executed repeatedly today.** Every response carries `X-Correlation-Id`; every log line for that request
carries the same value as `CorrelationId`:
```bash
docker logs tutoring-centre-app-1 2>&1 | grep <correlation-id>
```
Used throughout Task 36.1 to confirm a forged `X-Forwarded-For` never reached `ClientIp`, and to confirm a
role-change audit entry's `correlationId` matches the triggering request exactly.

**Trace lookup (OpenTelemetry) — not independently verified today.** `Otel:Endpoint` pointed at a local OTLP
viewer (e.g. the .NET Aspire Dashboard — see `docs/architecture/deployment.md`) would make the same
correlation ID a span attribute, searchable there. This machine was critically low on memory during this
session (a prior attempt was reaped — see Task 35.6's notes) and the check was not forced a second time.

## 8. When readiness fails

**Executed as a real drill, 2026-10-11 ~11:42 UTC** (stopping the database on purpose):
```bash
docker stop tutoring-centre-postgres-1
curl -sk https://localhost:8443/health/ready   # -> 503 Unhealthy, within one probe interval
```
**Observed:** both `/health/ready` health checks (`postgres` — basic connectivity, and
`postgres-runtime-role` — `RuntimeRolePrivilegeHealthCheck`, the tripwire below) went `Unhealthy` with
`57P01: terminating connection due to administrator command` in the app's log. One in-flight request
(`GET /api/system/info`) failed with a `500`; the client-visible body was the uniform
`{"title":"An unexpected error occurred.", ...}` — no stack trace, no connection string, no internals (the
same evidence recorded for Task 36.1, item 11). First steps:
1. `docker logs <app-container> --tail 50` — find the `HealthCheckError`/`HealthCheckEnd` lines; the
   `HealthCheckName` (`postgres` vs `postgres-runtime-role`) says which check failed.
2. If `postgres`: the database is unreachable — check it is running and the configured connection string is
   correct (`docker compose ps`, or in Azure, the Flexible Server's own status).
3. If `postgres-runtime-role` specifically, **and `postgres` is healthy**: this is the runtime-role tripwire
   (`RuntimeRolePrivilegeHealthCheck`) — the app's connection is a superuser, has `BYPASSRLS`, or is the owner
   role itself. This must never be bypassed or silenced; it means the one assumption every tenant-isolation
   layer depends on (Task 19) is false. Fix the connection string/role grant, never the health check.
4. Restore the dependency (`docker start tutoring-centre-postgres-1`, confirmed above); readiness recovers on
   its own, **no app restart needed** — confirmed `200` within 5 seconds of Postgres coming back, no action
   beyond restarting the database.

## 9. Add and remove a temporary administrator IP

**Not executed** — no Azure Flexible Server exists; nothing to open or close (Task 36.1's database check ran
directly against the local container, no firewall involved). Procedure for when one does:
```bash
az postgres flexible-server firewall-rule create --resource-group <group> --name <server> \
  --rule-name temp-admin --start-ip-address <my-ip> --end-ip-address <my-ip>
# ... do the work ...
az postgres flexible-server firewall-rule delete --resource-group <group> --name <server> \
  --rule-name temp-admin --yes
```
Always paired: never leave the rule in place after the task it was opened for.

## 10. Who may approve production deployments

**Designed, not yet enforced** — the mechanism (Task 35.3) is a GitHub `production` environment with the
repository owner as required reviewer, which has not been created (no Azure/OIDC trust exists to protect).
Until it is: no deployment pipeline runs against a real target at all, so the question does not yet arise in
practice. Once it does: the repository owner (`youssef623`) is the sole approver, enforced by GitHub's
environment protection rule, not by convention alone.
