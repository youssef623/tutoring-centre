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
];
