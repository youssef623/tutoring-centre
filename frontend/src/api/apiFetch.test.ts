import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it, vi } from "vitest";
import problem400 from "./fixtures/problem-400.json";
import problem404 from "./fixtures/problem-404.json";
import { server } from "@/test/msw/server";
import { ApiRequestError, apiFetch, toApiError } from "./apiFetch";

describe("apiFetch", () => {
  it("returns the parsed body for a 200 response", async () => {
    // Arrange
    server.use(http.get("*/api/ping", () => HttpResponse.json({ message: "pong" })));

    // Act
    const result = await apiFetch<{ message: string }>("/api/ping");

    // Assert
    expect(result).toEqual({ message: "pong" });
  });

  it("returns undefined for a 204 response", async () => {
    // Arrange
    server.use(http.delete("*/api/ping", () => new HttpResponse(null, { status: 204 })));

    // Act
    const result = await apiFetch("/api/ping", { method: "DELETE" });

    // Assert
    expect(result).toBeUndefined();
  });

  it("maps the 404 Problem Details fixture to a notFound ApiError with code and correlation id", async () => {
    // Arrange
    server.use(http.get("*/api/missing", () => HttpResponse.json(problem404, { status: 404 })));

    // Act
    const act = apiFetch("/api/missing");

    // Assert
    await expect(act).rejects.toBeInstanceOf(ApiRequestError);
    await expect(act).rejects.toMatchObject({
      kind: "notFound",
      code: problem404.code,
      status: 404,
      correlationId: problem404.correlationId,
    });
  });

  it("maps the 400 Problem Details fixture to a validation ApiError", async () => {
    // Arrange
    server.use(http.post("*/api/things", () => HttpResponse.json(problem400, { status: 400 })));

    // Act
    const act = apiFetch("/api/things", { method: "POST", body: JSON.stringify({ name: "" }) });

    // Assert
    await expect(act).rejects.toMatchObject({
      kind: "validation",
      code: problem400.code,
      status: 400,
      correlationId: problem400.correlationId,
    });
  });

  it("maps a network failure to network.unreachable with status 0", async () => {
    // Arrange
    server.use(http.get("*/api/down", () => HttpResponse.error()));

    // Act
    const act = apiFetch("/api/down");

    // Assert
    await expect(act).rejects.toMatchObject({
      kind: "unexpected",
      code: "network.unreachable",
      status: 0,
    });
  });

  it("maps a non-JSON 502 response to an unexpected ApiError", async () => {
    // Arrange
    server.use(
      http.get(
        "*/api/bad-gateway",
        () =>
          new HttpResponse("<html>Bad gateway</html>", {
            status: 502,
            headers: { "Content-Type": "text/html" },
          }),
      ),
    );

    // Act
    const act = apiFetch("/api/bad-gateway");

    // Assert
    await expect(act).rejects.toMatchObject({ kind: "unexpected", code: "http.502", status: 502 });
  });
});

describe("apiFetch CSRF handling", () => {
  // The token cache (csrf.ts) is module-level state; a fresh module instance per test keeps call counts exact.
  beforeEach(() => {
    vi.resetModules();
  });

  it("attaches X-XSRF-TOKEN on a non-GET request", async () => {
    // Arrange
    let seenHeader: string | null = null;
    server.use(
      http.post("*/api/things", ({ request }) => {
        seenHeader = request.headers.get("X-XSRF-TOKEN");
        return HttpResponse.json({ ok: true });
      }),
    );
    const { apiFetch: freshApiFetch } = await import("./apiFetch");

    // Act
    await freshApiFetch("/api/things", { method: "POST" });

    // Assert
    expect(seenHeader).toBe("test-csrf-token");
  });

  it("does not attach X-XSRF-TOKEN on a GET request", async () => {
    // Arrange
    let sawHeader = false;
    server.use(
      http.get("*/api/things", ({ request }) => {
        sawHeader = request.headers.has("X-XSRF-TOKEN");
        return HttpResponse.json({ ok: true });
      }),
    );
    const { apiFetch: freshApiFetch } = await import("./apiFetch");

    // Act
    await freshApiFetch("/api/things");

    // Assert
    expect(sawHeader).toBe(false);
  });

  it("refreshes the token and retries exactly once after a 403 auth.csrf_invalid, then succeeds", async () => {
    // Arrange
    let antiforgeryCalls = 0;
    let postAttempts = 0;
    server.use(
      http.get("*/api/auth/antiforgery", () => {
        antiforgeryCalls += 1;
        return HttpResponse.json({ token: `token-${String(antiforgeryCalls)}` });
      }),
      http.post("*/api/things", ({ request }) => {
        postAttempts += 1;
        if (postAttempts === 1) {
          return HttpResponse.json({ title: "Forbidden", status: 403, code: "auth.csrf_invalid" }, { status: 403 });
        }
        return HttpResponse.json({ receivedToken: request.headers.get("X-XSRF-TOKEN") });
      }),
    );
    const { apiFetch: freshApiFetch } = await import("./apiFetch");

    // Act
    const result = await freshApiFetch<{ receivedToken: string }>("/api/things", { method: "POST" });

    // Assert
    expect(postAttempts).toBe(2);
    expect(antiforgeryCalls).toBe(2);
    expect(result.receivedToken).toBe("token-2");
  });

  it("surfaces the error when the retry also fails with auth.csrf_invalid, without retrying again", async () => {
    // Arrange
    let postAttempts = 0;
    server.use(
      http.post("*/api/things", () => {
        postAttempts += 1;
        return HttpResponse.json({ title: "Forbidden", status: 403, code: "auth.csrf_invalid" }, { status: 403 });
      }),
    );
    const { apiFetch: freshApiFetch } = await import("./apiFetch");

    // Act
    const act = freshApiFetch("/api/things", { method: "POST" });

    // Assert
    await expect(act).rejects.toMatchObject({ status: 403, code: "auth.csrf_invalid" });
    expect(postAttempts).toBe(2);
  });
});

describe("toApiError", () => {
  const problem = { title: "Title", status: 400, detail: "Detail", code: "x.y" };

  it.each([
    [400, "validation"],
    [401, "unauthenticated"],
    [403, "forbidden"],
    [404, "notFound"],
    [409, "conflict"],
    [422, "rule"],
    [500, "unexpected"],
  ] as const)("maps status %i to kind %s", (status, kind) => {
    // Act
    const error = toApiError({ ...problem, status }, status);

    // Assert
    expect(error.kind).toBe(kind);
    expect(error.code).toBe("x.y");
    expect(error.message).toBe("Detail");
  });

  it("falls back to http.<status> when the problem has no code", () => {
    // Act
    const error = toApiError({ title: "Title", status: 409 }, 409);

    // Assert
    expect(error.code).toBe("http.409");
    expect(error.message).toBe("Title");
  });
});
