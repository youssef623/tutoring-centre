# `frontend/src/features/schedule/GroupScheduleTab.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 5,623 bytes. Source hash: `f470976bee91cff98d7d1d5e1315d33b510d663233f49ebfa58b829167ee7799`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, Now, rule, secretaryMe, teacherMe } from "@/features/schedule/scheduleTestData";`
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
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, rule, secretaryMe, teacherMe } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rulesUrl = "/api/groups/g-1/schedule-rules";

describe("GroupScheduleTab", () => {
  let rulesCalls = 0;

  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    rulesCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group())),
      http.get(rulesUrl, () => {
        rulesCalls += 1;
        return HttpResponse.json({
          items: [rule(), rule({ id: "r-2", dayOfWeek: "monday", startTime: "16:00", durationMinutes: 60, room: null, effectiveTo: "2027-04-12" })],
        });
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("shows a loading state, then the rules Saturday first, times left to right and an ended rule labelled", async () => {
    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByRole("status", { name: "Loading weekly sessions" }, { timeout: 5000 })).toBeInTheDocument();
    const table = await screen.findByRole("table");
    const [saturday, monday] = within(table).getAllByRole("row").slice(1);
    if (saturday === undefined || monday === undefined) {
      throw new Error("Expected two rule rows.");
    }
    expect(within(saturday).getByText("Saturday")).toBeInTheDocument();
    expect(within(saturday).getByText(/6:00\s?–\s?7:30\sPM/u)).toHaveAttribute("dir", "ltr");
    expect(within(saturday).getByText("Active")).toBeInTheDocument();
    expect(within(monday).getByText("Monday")).toBeInTheDocument();
    expect(within(monday).getByText("No room")).toBeInTheDocument();
    // effectiveTo 12 April is before today (20 April in Cairo): ended, and offered no End action.
    expect(within(monday).getByText("Ended")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /^End Saturday at/ })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^End Monday at/ })).not.toBeInTheDocument();
  });

  it("an empty list says so and offers to add a weekly session", async () => {
    server.use(http.get(rulesUrl, () => HttpResponse.json({ items: [] })));

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByText("No weekly sessions yet", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add weekly session" })).toBeInTheDocument();
  });

  it("a failed load shows the error with Retry, and Retry loads the rules", async () => {
    let fail = true;
    server.use(
      http.get(rulesUrl, () => (fail ? problemDetails(500, "server.error", "Boom.") : HttpResponse.json({ items: [rule()] }))),
    );

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByText("Weekly sessions could not be loaded", {}, { timeout: 5000 })).toBeInTheDocument();
    fail = false;
    fireEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("Saturday")).toBeInTheDocument();
  });

  it("with schedule.view only (the group's teacher) the rules are read-only", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByRole("table", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add weekly session" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^End / })).not.toBeInTheDocument();
  });

  it("end asks for a last date not before the rule's start, then sends exactly { lastDate, version }", async () => {
    let body: unknown;
```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
