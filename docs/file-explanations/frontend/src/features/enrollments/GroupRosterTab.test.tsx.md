# `frontend/src/features/enrollments/GroupRosterTab.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 148 lines and 6,813 bytes. Source hash: `be5ea8e9378c460d20c7fef3e81270ad65fcc9b18bfe2082f82893c6cb120fbe`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, roster, rosterEntry, secretaryMe, teacherMe } from "@/features/enrollments/enrollmentTestData";`
- Line 6: `import { problemDetails } from "@/test/msw/problemDetails";`
- Line 7: `import { server } from "@/test/msw/server";`
- Line 8: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { group, roster, rosterEntry, secretaryMe, teacherMe } from "@/features/enrollments/enrollmentTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rosterUrl = "/api/groups/g-1/roster";

describe("GroupRosterTab", () => {
  let rosterCalls = 0;

  beforeEach(() => {
    rosterCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group())),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get(rosterUrl, () => {
        rosterCalls += 1;
        return HttpResponse.json(roster());
      }),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the roster with seats and linked names", async () => {
    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByRole("status", { name: "Loading the roster" }, { timeout: 5000 })).toBeInTheDocument();
    const table = await screen.findByRole("table");
    expect(within(table).getByRole("link", { name: "Ahmed Mahmoud" })).toHaveAttribute("href", "/students/st-1");
    expect(within(table).getByRole("columnheader", { name: "Enrolled on" })).toBeInTheDocument();
    expect(screen.getByText("2 / 20")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Enroll student" })).toBeEnabled();
    expect(screen.getByRole("button", { name: "Actions for Sara Ali" })).toBeInTheDocument();
  });

  it("an empty roster says so and offers the enroll button", async () => {
    server.use(http.get(rosterUrl, () => HttpResponse.json(roster([]))));

    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("No students enrolled yet", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Enroll student" })).toBeInTheDocument();
  });

  it("a failed load shows the error with Retry, and Retry loads the roster", async () => {
    let fail = true;
    server.use(
      http.get(rosterUrl, () =>
        fail ? problemDetails(500, "server.error", "Boom.") : HttpResponse.json(roster()),
      ),
    );

    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("The roster could not be loaded", {}, { timeout: 5000 })).toBeInTheDocument();
    fail = false;
    fireEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("Sara Ali")).toBeInTheDocument();
  });

  it("with enrollments.view only (the group's teacher) shows plain names and no actions", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));

    renderRouter("/groups/g-1?tab=roster");

    const table = await screen.findByRole("table", {}, { timeout: 5000 });
    expect(within(table).getByText("Ahmed Mahmoud")).toBeInTheDocument();
    expect(within(table).queryByRole("link")).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Enroll student" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
  });

  it("at capacity the enroll button is disabled with its reason, and Full is written out", async () => {
    server.use(http.get(rosterUrl, () => HttpResponse.json(roster(undefined, { capacity: 2 }))));

    renderRouter("/groups/g-1?tab=roster");

    const button = await screen.findByRole("button", { name: "Enroll student" }, { timeout: 5000 });
    expect(button).toHaveAttribute("aria-disabled", "true");
    expect(button).toHaveAccessibleDescription("This group is full. Withdraw a student or raise the capacity first.");
    expect(screen.getByText("Full")).toBeInTheDocument();
    fireEvent.click(button);
```

The excerpt stops at line 90; use the original file for the remaining 58 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
