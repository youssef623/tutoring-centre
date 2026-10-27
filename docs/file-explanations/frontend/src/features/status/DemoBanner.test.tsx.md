# `frontend/src/features/status/DemoBanner.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/status/DemoBanner.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,274 bytes. Source hash: `e7d93c46e77a9c93ac6407cdd19f392cb75026cd7ad0ff7321e476979a47261d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderWithQueryClient } from "@/test/render";`
- Line 6: `import { DemoBanner } from "./DemoBanner";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { DemoBanner } from "./DemoBanner";

describe("DemoBanner", () => {
  it("renders nothing while loading and nothing once demoMode resolves to false", async () => {
    renderWithQueryClient(<DemoBanner />);

    // Null in both states (loading, then settled-false): confirm it stays null after the query settles.
    expect(screen.queryByRole("status")).not.toBeInTheDocument();
    await waitFor(() => {
      expect(screen.queryByRole("status")).not.toBeInTheDocument();
    });
  });

  it("renders the message when demoMode is true", async () => {
    server.use(
      http.get("*/api/system/info", () =>
        HttpResponse.json({
          applicationVersion: "1.0.0",
          latestMigration: "20261012_InitialPlatform",
          databaseUpToDate: true,
          demoMode: true,
        }),
      ),
    );

    renderWithQueryClient(<DemoBanner />);

    expect(
      await screen.findByText("Demo environment — data may be reset. Do not enter real personal data."),
    ).toBeInTheDocument();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
