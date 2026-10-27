# `frontend/src/api/apiFetch.test.ts`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/api/apiFetch.test.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 219 lines and 6,637 bytes. Source hash: `202c6bb647d1be67c2a01d9ccf87ea7a8cfdbbc451f893bffb8b22e79f61277a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { http, HttpResponse } from "msw";`
- Line 2: `import { beforeEach, describe, expect, it, vi } from "vitest";`
- Line 3: `import problem400 from "./fixtures/problem-400.json";`
- Line 4: `import problem404 from "./fixtures/problem-404.json";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { ApiRequestError, apiFetch, toApiError } from "./apiFetch";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

The excerpt stops at line 90; use the original file for the remaining 129 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
