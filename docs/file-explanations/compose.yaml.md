# compose.yaml

## Purpose
Docker Compose definition for **local development infrastructure only**: one PostgreSQL 17 container with a persistent volume and a healthcheck. The API and frontend run on the host, not in Compose.

## Where it fits
Dev infrastructure. It reads `.env` (template `.env.example`). The API connects to it via `ConnectionStrings:Postgres` (`localhost:5432`, README step 3). The readiness endpoint reports whether it is up.

## Walkthrough
- **Line 3:** `image: postgres:17`.
- **Lines 4–7:** env vars from `.env`. `${POSTGRES_PASSWORD:?set POSTGRES_PASSWORD in .env}` makes Compose fail with that message if the password is unset or empty.
- **Lines 8–9:** `ports: "5432:5432"` publishes the port on the host.
- **Lines 10–11:** named volume `pgdata` mounted at `/var/lib/postgresql/data`, so data survives restarts.
- **Lines 12–16:** healthcheck `pg_isready -U $USER -d $DB` every 5 s, 5 s timeout, 5 retries. `docker compose ps` then shows `(healthy)`.
- **Lines 18–19:** declares the `pgdata` volume.

## Concepts used
- **Variable substitution with a required check** (`${VAR:?message}`).
- **Container healthchecks.**
- **Named volumes.**

## Data and control flow
`.env` → container env → Postgres initialises the DB and user on the first start with an empty volume.

## Configuration and environment
`POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`. See [.env.example](.env.example.md).

## Gotchas and issues
- `"5432:5432"` binds on all host interfaces. On a shared network the dev DB is reachable by others; `"127.0.0.1:5432:5432"` would be safer.
- It conflicts with any locally installed Postgres already using port 5432.
- `later.md:7` explicitly defers adding pgAdmin.

## Related files
- [.env.example](.env.example.md)
- [Infrastructure DependencyInjection.cs](src/TutoringCentre.Infrastructure/DependencyInjection.cs.md)
- [later.md](later.md.md)
