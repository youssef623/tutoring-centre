# `frontend/src/features/session/useSession.test.ts`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/session/useSession.test.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 85 lines and 2,520 bytes. Source hash: `49960541a5d29aa32fe4620fbcb14639221453367f835be9236aaba78bc9e7a3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderHookWithQueryClient } from "@/test/render";`
- Line 6: `import { useSession } from "./useSession";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderHookWithQueryClient } from "@/test/render";
import { useSession } from "./useSession";

describe("useSession", () => {
  it("resolves to a signed-out session (not an error) on a 401 from /api/me", async () => {
    // Arrange (default MSW handler: 401)

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });
    expect(result.current.me).toBeNull();
    expect(result.current.isError).toBe(false);
  });

  it("resolves to the signed-in user's profile when /api/me succeeds", async () => {
    // Arrange
    server.use(
      http.get("/api/me", () =>
        HttpResponse.json({
          userId: "01a1071e-016b-77bc-8135-aaa275518b2a",
          displayName: "Nile Owner",
          email: "owner@nile.test",
          preferredLocale: "en",
          activeCentreId: "01a0febd-7bd7-7b2c-aea0-d975b64f6d06",
          activeRole: "owner",
          memberships: [
            {
              centreId: "01a0febd-7bd7-7b2c-aea0-d975b64f6d06",
              centreName: "Nile Tutoring Centre",
              centreSlug: "nile-centre",
              role: "owner",
            },
          ],
          permissions: [
            "audit.view",
            "centre.settings.manage",
            "staff.manage",
            "staff.view",
            "subjects.manage",
            "subjects.view",
          ],
        }),
      ),
    );

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });
    expect(result.current.me?.displayName).toBe("Nile Owner");
    expect(result.current.me?.activeRole).toBe("owner");
  });

  it("reports a real error for a non-401 failure, e.g. a 500", async () => {
    // Arrange
    server.use(
      http.get("/api/me", () =>
        HttpResponse.json(
          { title: "An unexpected error occurred.", status: 500, code: "server.unexpected" },
          { status: 500 },
        ),
      ),
    );

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isError).toBe(true);
    });
    expect(result.current.me).toBeNull();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
