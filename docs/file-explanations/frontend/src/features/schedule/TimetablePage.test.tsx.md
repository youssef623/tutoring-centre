# `frontend/src/features/schedule/TimetablePage.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/TimetablePage.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 163 lines and 7,689 bytes. Source hash: `257c4098f876df0054bf33ba6f6171657b3dfcaf9e8853945471ed428bc12d33`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, Now, secretaryMe, session, teacherMe, teachers, timetable } from "@/features/schedule/scheduleTestData";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

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
import { group, Now, secretaryMe, session, teacherMe, teachers, timetable } from "@/features/schedule/scheduleTestData";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

describe("TimetablePage", () => {
  let requestedWeeks: string[] = [];
  let teachersCalls = 0;
  let sessions = [session()];
  let responseZone = "Africa/Cairo";

  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    requestedWeeks = [];
    teachersCalls = 0;
    sessions = [session()];
    responseZone = "Africa/Cairo";
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get("/api/teachers", () => {
        teachersCalls += 1;
        return HttpResponse.json({ items: teachers });
      }),
      http.get("/api/timetable", ({ request }) => {
        const weekStart = new URL(request.url).searchParams.get("weekStart") ?? "";
        requestedWeeks.push(weekStart);
        return HttpResponse.json(timetable(weekStart, weekStart === "2027-04-24" ? sessions : [], responseZone));
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("without a week in the URL opens the centre's current week, from Saturday", async () => {
    renderRouter("/timetable");

    expect(await screen.findByText("No sessions this week", {}, { timeout: 5000 })).toBeInTheDocument();
    // Tuesday 20 April in Cairo belongs to the week of Saturday 17 April.
    expect(requestedWeeks).toEqual(["2027-04-17"]);
    expect(screen.getByRole("button", { name: "This week" })).toBeDisabled();
  });

  it("places sessions by the server's localDate and formats times in the response's zone", async () => {
    // 22:30Z on Saturday is 00:30 on Sunday in Cairo: the server says Sunday, and so does the grid.
    sessions = [
      session(),
      session({ id: "s-2", groupName: "Late Group", localDate: "2027-04-25", startsAt: "2027-04-24T22:30:00Z", endsAt: "2027-04-24T23:30:00Z" }),
    ];

    renderRouter("/timetable?week=2027-04-24");

    const saturday = await screen.findByRole("region", { name: /^Saturday/ }, { timeout: 5000 });
    const sunday = screen.getByRole("region", { name: /^Sunday/ });
    expect(within(saturday).getByText("Physics A")).toBeInTheDocument();
    expect(within(saturday).queryByText("Late Group")).not.toBeInTheDocument();
    expect(within(sunday).getByText("Late Group")).toBeInTheDocument();
    expect(within(saturday).getByText(/^6:00\s?–\s?7:30\sPM$/u)).toHaveAttribute("dir", "ltr");
    expect(within(sunday).getByText(/^12:30\s?–\s?1:30\sAM$/u)).toBeInTheDocument();
  });

  it("uses the zone the response names, whatever the browser's or the membership's zone is", async () => {
    // 16:00Z is 8 PM in Dubai (UTC+4): only the response's timeZoneId explains that text.
    responseZone = "Asia/Dubai";

    renderRouter("/timetable?week=2027-04-24");

    const saturday = await screen.findByRole("region", { name: /^Saturday/ }, { timeout: 5000 });
    expect(within(saturday).getByText(/^8:00\s?–\s?9:30\sPM$/u)).toBeInTheDocument();
  });

  it("keeps a cancelled session visible, struck through and labelled", async () => {
    sessions = [session({ status: "cancelled" })];

    renderRouter("/timetable?week=2027-04-24");

    const card = await screen.findByRole("article", { name: /^Physics A/ }, { timeout: 5000 });
    expect(within(card).getByText("Cancelled")).toBeInTheDocument();
    expect(within(card).getByText(/6:00/u)).toHaveClass("line-through");
    // A cancelled session offers nothing to cancel.
    expect(within(card).queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
  });

```

The excerpt stops at line 90; use the original file for the remaining 73 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
