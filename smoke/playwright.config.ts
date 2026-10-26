import { defineConfig, devices } from "@playwright/test";

/**
 * Task 35.8: a smoke test asks one question — is this revision fit to serve? — against a real, already-running
 * instance. Unlike frontend/e2e, this project starts no local server and takes its target entirely from the
 * environment, so it can run unchanged against the local production stack, a deployed instance, or (one day)
 * the public URL.
 */
const baseURL = process.env.SMOKE_BASE_URL;
if (baseURL === undefined || baseURL === "") {
  throw new Error("SMOKE_BASE_URL must be set to the target instance's base URL (see README).");
}

export default defineConfig({
  testDir: "./tests",
  fullyParallel: false,
  retries: 0,
  timeout: 30_000,
  reporter: "list",
  use: {
    baseURL,
    // Signs in with a real (if throwaway) demo password: no trace, screenshot or video is ever recorded, so
    // none can retain it (Task 35.8's own constraint — read directly from `process.env`, never logged).
    trace: "off",
    screenshot: "off",
    video: "off",
    // The local production stack (Day 34) and early deployments use a self-signed or not-yet-trusted certificate.
    ignoreHTTPSErrors: true,
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
});
