# Deployed smoke test

Proves a running instance is fit to serve: health, security headers, the deployed commit, that `/api` requires
auth, and that the demo owner can sign in, see Subjects, and sign out. Creates no data; takes about a minute.

Separate from `frontend/e2e`: this project never starts a local server, only points at one already running.

## Running it

```sh
npm install
npx playwright install --with-deps chromium  # once per machine
SMOKE_BASE_URL=https://localhost:8443 SMOKE_EXPECTED_SHA=local SMOKE_PASSWORD=<the demo owner's password> \
  SMOKE_EXPECT_HSTS=false npm test
```

- `SMOKE_BASE_URL` — the instance to check. Against the local production stack (Day 34), that's
  `https://localhost:8443`.
- `SMOKE_EXPECTED_SHA` — the commit SHA `/api/system/info` should report. The local stack's default build
  (`COMMIT_SHA` unset) reports `local`; a CI or deployed build passes the real Git SHA.
- `SMOKE_PASSWORD` — the demo Nile owner's (`owner@nile.test`) password. Never printed, logged, or captured —
  traces, screenshots and video are off in `playwright.config.ts` for exactly this reason.
- `SMOKE_EXPECT_HSTS` — defaults to `true`. Set to `false` only against the local production stack: ASP.NET
  Core's `UseHsts()` excludes the `localhost`/`127.0.0.1`/`[::1]` hosts by default, and Caddy's local site
  address (`Caddyfile`) is literally `localhost:8443`, so the header is correctly absent there. Any real
  hostname — a deployed instance included — gets the header and must leave this at its default.

## In the deployment pipeline

`deploy.yml` (not wired against real Azure in this slice — no subscription provisioned yet; see
`docs/architecture/deployment.md`) would run this as its smoke step against the newly deployed revision, with
`SMOKE_EXPECTED_SHA` set to the commit it just deployed and `SMOKE_PASSWORD` read from a GitHub environment
secret, and roll back to the previous revision if any check fails.
