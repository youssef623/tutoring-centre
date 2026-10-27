# `frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx`

## Purpose

Source comment (intent): Base UI selects commit a click only after a pointerdown on the same option (see test/selectOption.ts).

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 169 lines and 6,783 bytes. Source hash: `3bd07ff753652469a90f4b9ae737a97797a48118a2a65200d4455db85874c940`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, Now, secretaryMe } from "@/features/schedule/scheduleTestData";`
- Line 6: `import { problemDetails } from "@/test/msw/problemDetails";`
- Line 7: `import { server } from "@/test/msw/server";`
- Line 8: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L13): `async function choose(dialog: HTMLElement, label: string, option: string) {`
- [Line 20](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L20): `async function openDialog() {`
- [Line 27](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L27): `async function fillSaturdayEvening(dialog: HTMLElement) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, secretaryMe } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rulesUrl = "/api/groups/g-1/schedule-rules";

/** Base UI selects commit a click only after a pointerdown on the same option (see test/selectOption.ts). */
async function choose(dialog: HTMLElement, label: string, option: string) {
  fireEvent.click(within(dialog).getByRole("combobox", { name: label }));
  const item = await screen.findByRole("option", { name: option });
  fireEvent.pointerDown(item);
  fireEvent.click(item);
}

async function openDialog() {
  renderRouter("/groups/g-1?tab=schedule");
  fireEvent.click(await screen.findByRole("button", { name: "Add weekly session" }, { timeout: 5000 }));
  return screen.findByRole("dialog", { name: "Add weekly session" });
}

/** Saturday 6:00 PM, 1 hour 30 minutes, Room 2: what the office types for the most common weekly session. */
async function fillSaturdayEvening(dialog: HTMLElement) {
  await choose(dialog, "Day", "Saturday");
  await choose(dialog, "Hour", "6");
  await choose(dialog, "Minute", "00");
  await choose(dialog, "AM or PM", "PM");
  await choose(dialog, "Length", "1 hour 30 minutes");
  fireEvent.change(within(dialog).getByRole("textbox", { name: "Room (optional)" }), { target: { value: "Room 2" } });
}

describe("AddScheduleRuleDialog", () => {
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
        return HttpResponse.json({ items: [] });
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("the start date defaults to today in the centre's zone, not the browser's", async () => {
    // 23:30 UTC on 19 April is already 01:30 on 20 April in Cairo.
    vi.setSystemTime(new Date("2027-04-19T23:30:00Z"));

    const dialog = await openDialog();

    expect(within(dialog).getByLabelText("Starts on")).toHaveValue("2027-04-20");
    expect(within(dialog).getByText("Times are in the centre's time zone (Africa/Cairo).")).toBeInTheDocument();
  });

  it("sends exactly the five contract fields as local values, then toasts the count and refreshes the rules", async () => {
    let body: unknown;
    server.use(
      http.post(rulesUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "r-new", sessionsCreated: 4 }, { status: 201 });
      }),
    );

    const dialog = await openDialog();
    await fillSaturdayEvening(dialog);
    const before = rulesCalls;
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));

    expect(await screen.findByText("4 sessions scheduled")).toBeInTheDocument();
    expect(body).toStrictEqual({
      dayOfWeek: "saturday",
      startTime: "18:00",
      durationMinutes: 90,
      room: "Room 2",
      effectiveFrom: "2027-04-20",
    });
    await waitFor(() => {
```

The excerpt stops at line 90; use the original file for the remaining 79 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
