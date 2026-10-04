import { http, HttpResponse } from "msw";

/** Default network behaviour for tests: a healthy API. Tests override per case with server.use(...). */
export const handlers = [
  http.get("/health/ready", () => new HttpResponse("Healthy", { status: 200 })),
  http.get("/api/system/info", () =>
    HttpResponse.json({
      applicationVersion: "1.0.0",
      latestMigration: "20261012_InitialPlatform",
      databaseUpToDate: true,
    }),
  ),
  // No session by default; tests that need a signed-in user override this with server.use(...).
  http.get("/api/me", () =>
    HttpResponse.json(
      { title: "Unauthorized", status: 401 },
      { status: 401 },
    ),
  ),
  // Every non-GET request fetches this first (apiFetch's CSRF handling); a fixed token keeps that invisible
  // to tests that aren't about CSRF themselves.
  http.get("/api/auth/antiforgery", () => HttpResponse.json({ token: "test-csrf-token" })),
];
