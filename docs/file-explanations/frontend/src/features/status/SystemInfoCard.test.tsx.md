# `frontend/src/features/status/SystemInfoCard.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/status/SystemInfoCard.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 1,623 bytes. Source hash: `f374ae55744227c216b0dcff18bd5bea43bebe4e5aa462761b6276c74c871ad6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { screen } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderWithQueryClient } from "@/test/render";`
- Line 6: `import { SystemInfoCard } from "./SystemInfoCard";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { screen } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { SystemInfoCard } from "./SystemInfoCard";

describe("SystemInfoCard", () => {
  it("shows version, latest migration and the up-to-date badge on success", async () => {
    // Arrange (default MSW handler)

    // Act
    renderWithQueryClient(<SystemInfoCard />);

    // Assert
    expect(await screen.findByText("1.0.0")).toBeInTheDocument();
    expect(screen.getByText("20261012_InitialPlatform")).toBeInTheDocument();
    expect(screen.getByText("Database schema up to date")).toBeInTheDocument();
  });

  it("shows the translated error message and the correlation reference on a 500 Problem Details response", async () => {
    // Arrange
    server.use(
      http.get("*/api/system/info", () =>
        HttpResponse.json(
          {
            title: "An unexpected error occurred.",
            status: 500,
            code: "server.unexpected",
            traceId: "trace-1",
            correlationId: "corr-500",
          },
          { status: 500 },
        ),
      ),
    );

    // Act
    renderWithQueryClient(<SystemInfoCard />);

    // Assert
    expect(await screen.findByText("Something went wrong. Please try again.")).toBeInTheDocument();
    // The id is wrapped in a left-to-right isolate so it never reorders against an Arabic label (isolateLtr).
    expect(screen.getByText("Reference: \u2066corr-500\u2069")).toBeInTheDocument();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
