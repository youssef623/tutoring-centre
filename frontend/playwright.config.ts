import { defineConfig, devices } from "@playwright/test";

/**
 * Deviation from the Day 18 plan's literal text, reported per its own "report the difference" rule
 * (Task 18.6): the plan calls for the API's plain-http launch profile. That breaks antiforgery —
 * `AntiforgeryOptions.Cookie.SecurePolicy = Always` makes `GetAndStoreTokens`/`ValidateRequestAsync`
 * hard-check `HttpContext.Request.IsHttps` on the request *the backend itself receives*, independent of
 * "localhost is a trustworthy origin" (that exception covers the browser accepting a Secure cookie from
 * http://localhost:5173, not whether Kestrel saw TLS). This is the same reason Day 16 moved dev to HTTPS
 * rather than weakening that policy. E2E therefore uses the same "https" profile as normal development;
 * the readiness probe below targets that profile's plain-http sibling binding purely to dodge the
 * self-signed certificate during the Node-side health check (the API itself is still reached over HTTPS).
 */
const apiHttpsTarget = "https://localhost:7197";
const apiReadinessUrl = "http://localhost:5245/health/ready";

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: true,
  retries: process.env.CI ? 1 : 0,
  reporter: "html",
  use: {
    baseURL: "http://localhost:5173",
    trace: "on-first-retry",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
  webServer: [
    {
      command: "dotnet run --project ../src/TutoringCentre.Api --launch-profile https",
      url: apiReadinessUrl,
      reuseExistingServer: !process.env.CI,
      timeout: 120_000,
      stdout: "ignore",
    },
    {
      command: "npm run dev",
      url: "http://localhost:5173",
      reuseExistingServer: !process.env.CI,
      timeout: 60_000,
      env: { API_TARGET: apiHttpsTarget },
      stdout: "ignore",
    },
  ],
});
