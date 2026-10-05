# frontend/playwright.config.ts

## Purpose

Playwright configuration: where the E2E specs are, which browser, and how to start the API and the Vite dev server before the tests.

## Where It Fits

frontend root. Used by `npm run test:e2e` and the CI `e2e` job. Included by `tsconfig.node.json`. Starts `dotnet run` for the API and `npm run dev` for the SPA.

## Walkthrough

Header comment (3-13) explains a deliberate deviation from the plan: E2E uses the API's `https` launch profile, not plain http, because the antiforgery cookie is `SecurePolicy = Always`, which makes the *backend* check `Request.IsHttps`; the browser's 'localhost is trustworthy' rule does not help with that. Constants: `apiHttpsTarget = https://localhost:7197`, `apiReadinessUrl = http://localhost:5245/health/ready` (the plain-http sibling binding, used only so the Node-side readiness probe avoids the self-signed certificate).

`defineConfig`: `testDir ./e2e`; `fullyParallel: true`; `retries: process.env.CI ? 1 : 0`; `reporter: "html"`; `use.baseURL http://localhost:5173`, `trace: "on-first-retry"`; one project `chromium` (`devices["Desktop Chrome"]`). `webServer` has two entries: (1) `dotnet run --project ../src/TutoringCentre.Api --launch-profile https`, waits for `apiReadinessUrl`, `reuseExistingServer: !process.env.CI`, timeout 120 s, stdout ignored; (2) `npm run dev`, waits for `http://localhost:5173`, timeout 60 s, `env: { API_TARGET: apiHttpsTarget }` so `vite.config.ts` proxies to the HTTPS API.

## Concepts Used

### End-to-end testing with Playwright

#### What it means

An end-to-end test drives a real browser against a real running system (here the real API, real PostgreSQL and the Vite dev server) and clicks through a user journey. It is slow but catches wiring problems no unit test can.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#635-end-to-end-testing-with-playwright](../../PROJECT_OVERVIEW2.md#635-end-to-end-testing-with-playwright).)

#### Where it appears in this file

The whole config.

#### How it works here

`webServer` array at lines 27-43.

#### Why it matters here

Playwright starts and waits for both servers itself, so a single command runs the full stack.

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

`API_TARGET` hand-off.

#### How it works here

Line 40, consumed by `vite.config.ts` (`process.env.API_TARGET ?? "https://localhost:7197"`).

#### Why it matters here

Lets the same dev server proxy to a different API origin without editing code.

### Cookie authentication and server-side sessions

#### What it means

After a successful login the server must remember *who* the browser is on later requests (HTTP itself is stateless). Cookie authentication does that with one cookie that the browser attaches automatically. Here the cookie holds an **encrypted, tamper-proof ticket** (the user's claims); only the server can read or create it, and flags such as `HttpOnly`, `Secure`, `SameSite` and the `__Host-` name prefix limit where and how browsers send it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions](../../PROJECT_OVERVIEW2.md#624-cookie-authentication-and-server-side-sessions).)

#### Where it appears in this file

Why HTTPS even in tests.

#### How it works here

Header comment lines 3-13.

#### Why it matters here

`__Host-` cookies and `Secure` antiforgery cookies need TLS on the request the server sees; weakening the policy for tests would test different behaviour from production.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Env: `CI` (retries, no server reuse), `API_TARGET` (passed to Vite). Ports: API https 7197, API http 5245, Vite 5173.

## Gotchas and Issues

A comment in `vite.config.ts` says Playwright starts the API on its *plain-http* profile, but this file (the code that actually runs) uses the `https` profile and explains why; the vite comment is out of date. The readiness probe uses the http sibling binding to avoid the self-signed certificate.

## Related Files

- [`frontend/e2e/auth.spec.ts`](e2e/auth.spec.ts.md)
- [`frontend/vite.config.ts`](vite.config.ts.md)
- [`src/TutoringCentre.Api/Properties/launchSettings.json`](../src/TutoringCentre.Api/Properties/launchSettings.json.md)
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml.md)
