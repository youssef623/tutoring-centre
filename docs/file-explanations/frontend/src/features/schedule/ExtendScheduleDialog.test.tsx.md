# `frontend/src/features/schedule/ExtendScheduleDialog.test.tsx`

## Purpose

Declares `openDialog`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 3,852 bytes. Source hash: `c2ae84c6464a402d801352c218c59edcf9f9ace16e879a70a52a4746ec632272`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, Now, secretaryMe, teachers, timetable } from "@/features/schedule/scheduleTestData";`
- Line 6: `import { problemDetails } from "@/test/msw/problemDetails";`
- Line 7: `import { server } from "@/test/msw/server";`
- Line 8: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L10): `async function openDialog() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, secretaryMe, teachers, timetable } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

async function openDialog() {
  renderRouter("/timetable?week=2027-04-17");
  fireEvent.click(await screen.findByRole("button", { name: "Extend schedule" }, { timeout: 5000 }));
  return screen.findByRole("dialog", { name: "Extend the schedule" });
}

describe("ExtendScheduleDialog", () => {
  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get("/api/teachers", () => HttpResponse.json({ items: teachers })),
      http.get("/api/timetable", () => HttpResponse.json(timetable("2027-04-17"))),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("is bounded to today through 92 days ahead in the centre's zone, and sends the chosen date", async () => {
    let body: unknown;
    server.use(
      http.post("/api/schedule/extend", async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ sessionsCreated: 12 });
      }),
    );

    const dialog = await openDialog();
    const through = within(dialog).getByLabelText("Create sessions through");
    expect(through).toHaveAttribute("min", "2027-04-20");
    expect(through).toHaveAttribute("max", "2027-07-21");
    fireEvent.change(through, { target: { value: "2027-06-15" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await screen.findByText("12 sessions created")).toBeInTheDocument();
    expect(body).toStrictEqual({ through: "2027-06-15" });
  });

  it("reports a repeat that creates nothing as already up to date", async () => {
    server.use(http.post("/api/schedule/extend", () => HttpResponse.json({ sessionsCreated: 0 })));

    const dialog = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await screen.findByText("Already up to date")).toBeInTheDocument();
  });

  it("a date beyond 92 days is refused under the field before any request", async () => {
    let sent = false;
    server.use(
      http.post("/api/schedule/extend", () => {
        sent = true;
        return HttpResponse.json({ sessionsCreated: 0 });
      }),
    );

    const dialog = await openDialog();
    fireEvent.change(within(dialog).getByLabelText("Create sessions through"), { target: { value: "2027-07-22" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await within(dialog).findByRole("alert")).toHaveTextContent("Choose a date from today up to");
    expect(sent).toBe(false);
  });

  it("a double booking found while extending names its kind and advises a review, keeping the dialog open", async () => {
    server.use(http.post("/api/schedule/extend", () => problemDetails(409, "schedule.teacher_double_booked", "Double booked.")));

    const dialog = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(
      await within(dialog).findByText(
        "Two weekly sessions of one teacher now overlap. Review the teacher's weekly sessions and end one of them.",
      ),
    ).toBeInTheDocument();
    expect(screen.getByRole("dialog", { name: "Extend the schedule" })).toBeInTheDocument();
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
