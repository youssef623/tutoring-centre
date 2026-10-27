# `frontend/src/features/status/StatusCard.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/status/StatusCard.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 47 lines and 1,779 bytes. Source hash: `7473ecdacdd8f95e52b6d7aa594f4397c632e3262b66ae2fbbc1697af6a1b522`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderWithQueryClient } from "@/test/render";`
- Line 6: `import { StatusCard } from "./StatusCard";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { StatusCard } from "./StatusCard";

const readinessUrl = "/health/ready";
const healthy = () => new HttpResponse("Healthy", { status: 200 });
const unavailable = () => new HttpResponse("Unhealthy", { status: 503 });

describe("StatusCard", () => {
  it("shows the healthy state when the API returns 200", async () => {
    // Arrange
    server.use(http.get(readinessUrl, healthy));

    // Act
    renderWithQueryClient(<StatusCard />);

    // Assert
    expect(await screen.findByText("API and database are reachable")).toBeInTheDocument();
  });

  it("shows the database-unavailable state with Retry when the API returns 503", async () => {
    server.use(http.get(readinessUrl, unavailable));

    renderWithQueryClient(<StatusCard />);

    expect(
      await screen.findByText("API is running but the database is unavailable"),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Retry" })).toBeInTheDocument();
    // 503 is an answer from the API, not a network failure.
    expect(screen.queryByText("Cannot reach the API")).not.toBeInTheDocument();
  });

  it("refetches and shows the healthy state after Retry", async () => {
    server.use(http.get(readinessUrl, unavailable));
    renderWithQueryClient(<StatusCard />);
    const retryButton = await screen.findByRole("button", { name: "Retry" });

    server.use(http.get(readinessUrl, healthy));
    fireEvent.click(retryButton);

    expect(await screen.findByText("API and database are reachable")).toBeInTheDocument();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
