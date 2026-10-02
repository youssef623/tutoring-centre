import { http, HttpResponse } from "msw";

/** Default network behaviour for tests: a healthy API. Tests override per case with server.use(...). */
export const handlers = [
  http.get("/health/ready", () => new HttpResponse("Healthy", { status: 200 })),
];
