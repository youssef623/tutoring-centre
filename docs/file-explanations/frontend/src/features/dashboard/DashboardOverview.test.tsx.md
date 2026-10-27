# `frontend/src/features/dashboard/DashboardOverview.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 3,021 bytes. Source hash: `758a29b3d02705c6ef45688d51ff00b982e9f7d0703fd655030ce45c2f659d87`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { screen, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { screen, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

describe("Dashboard overview", () => {
  let groupCalls = 0;

  beforeEach(() => {
    groupCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => {
        groupCalls += 1;
        return HttpResponse.json({
          items: [
            group({ id: "g-1", name: "Physics A", capacity: 20, enrolledCount: 4 }),
            group({ id: "g-2", name: "Physics B", capacity: 10, enrolledCount: 9 }),
            group({ id: "g-3", name: "Physics C", capacity: 2, enrolledCount: 2 }),
          ],
        });
      }),
    );
  });

  it("sums the server's seat counts into tiles", async () => {
    renderRouter("/");

    const tile = async (label: string) => (await screen.findByText(label, {}, { timeout: 5000 })).parentElement as HTMLElement;
    expect(within(await tile("Active groups")).getByText("3")).toBeInTheDocument();
    expect(within(await tile("Seats taken")).getByText("15")).toBeInTheDocument();
    expect(within(await tile("Open seats")).getByText("17")).toBeInTheDocument();
    expect(within(await tile("Full groups")).getByText("1")).toBeInTheDocument();
  });

  it("lists the groups filling up, fullest first, linking to their roster", async () => {
    renderRouter("/");

    const section = await screen.findByRole("region", { name: "Groups filling up" }, { timeout: 5000 });
    const links = within(section).getAllByRole("link").filter((link) => link.getAttribute("href")?.includes("tab=roster"));
    expect(links.map((link) => link.textContent)).toEqual(["Physics C", "Physics B"]);
    expect(within(section).queryByText("Physics A")).not.toBeInTheDocument();
    expect(within(section).getByText("Full")).toBeInTheDocument();
    expect(within(section).getByText("Almost full")).toBeInTheDocument();
  });

  it("shows only the shortcuts the session may use", async () => {
    renderRouter("/");

    expect(await screen.findByRole("link", { name: "Register student" }, { timeout: 5000 })).toHaveAttribute("href", "/students/new");
    expect(screen.getAllByRole("link", { name: "Groups" }).length).toBeGreaterThan(0);
  });

  it("without groups.view sends no groups request and shows no seat overview", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(me("teacher", ["subjects.view"]))));

    renderRouter("/");

    expect(await screen.findByRole("heading", { name: "Welcome, Nile teacher" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByText("Groups filling up")).not.toBeInTheDocument();
    expect(screen.queryByRole("link", { name: "Register student" })).not.toBeInTheDocument();
    expect(groupCalls).toBe(0);
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
