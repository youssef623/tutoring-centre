# Browser journeys executed during the onboarding audit

Date: 27 October 2026. These are results observed during this audit, not historical CI claims.

## Isolation and setup

The checked ports 7197, 5245, 5173 and 54329 had no listener before setup. A new disposable PostgreSQL 17 container, `tutoring-onboarding-e2e-20261027`, published only `127.0.0.1:54329`. It used database `tutoring_onboarding_e2e` and the repository's `db/bootstrap-roles.sh` initialization mount to create the distinct owner and runtime roles. All administrator, owner, application and staff passwords were throwaway values supplied through process environment; none was read from the user's development database or reproduced here.

The runtime and migration connection strings explicitly selected port 54329 and this disposable database. The seed command ran in Development so the empty `giza-new` setup centre was included:

```text
dotnet run --no-build --configuration Release --project src/TutoringCentre.Api --launch-profile https -- seed
```

The restricted attempt failed with socket error 10013. The same isolated setup rerun with approved unrestricted access completed with exit code 0. The user's local development database was not selected or modified.

## Executed command and result

From `frontend/`:

```text
npm run test:e2e -- --workers=2 --reporter=line
```

The process used `CI=true`, the isolated runtime/owner connection strings, matching `Seed__Password` and `SEED_PASSWORD`, and `Auth__LoginRateLimit__PermitLimit=100`. The API's default log minimum was raised to Warning only for this process to avoid capturing verbose query logs. Playwright's existing configuration started its own HTTPS API and Vite servers. `CI=true` disabled reuse of existing servers. No repository source or Playwright configuration was edited.

Observed final runner result:

```text
19 passed (1.6m)
```

Overall exit code: **0**. One Chromium project, two workers. The configured CI retry allowance remained in effect, but the output reported no failed or retried case. This exercised authentication, centre selection, RTL, enrollment, scheduling, staff access, students/parents, subjects, teachers/groups, first-login password change, the full empty-centre setup journey and role-change audit history.

Non-failing output included Node's warning that `NO_COLOR` was ignored because `FORCE_COLOR` was set. No production container-image, Caddy or deployed smoke run is claimed by this check: these journeys use the repository's HTTPS development API plus Vite proxy, with a real isolated database and browser.

## Cleanup and scope

Playwright stopped its own web servers. The disposable container was stopped; because it was started with `--rm`, Docker removed it. A subsequent listener check found no listener on 7197, 5245, 5173 or 54329. The checked generated contract/client/route-tree paths had no Git changes.

The existing two-centre subject journey navigates with `page.goto('/subjects')` after switching centres. That reloads the document and recreates the in-memory query client. Passing this journey therefore does not prove that an uninterrupted SPA navigation clears tenant-specific cache entries; the separate source-derived cache finding needs its own regression without that reload.
