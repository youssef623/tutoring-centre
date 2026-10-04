import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
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

describe("toApiError", () => {
  const problem = { title: "Title", status: 400, detail: "Detail", code: "x.y" };

  it.each([
    [400, "validation"],
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
