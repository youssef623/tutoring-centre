# `frontend/src/features/schedule/centreTime.test.ts`

## Purpose

Source comment (intent): The hour and minute an Arabic (or any) formatted time shows, read through Intl's own parts in the same zone.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/centreTime.test.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 3,368 bytes. Source hash: `1ab016e5a0eda5069321e545a45768c4632a27b69b498790443e01f641c8e23d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { describe, expect, it } from "vitest";`
- Line 2: `import { addMinutes, formatLocalTime, formatTime, formatTimeRange, todayIn } from "@/features/schedule/centreTime";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/features/schedule/centreTime.test.ts#L7): `function hourMinute(instant: string, locale: string): { hour: string; minute: string; dayPeriod: string } {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { describe, expect, it } from "vitest";
import { addMinutes, formatLocalTime, formatTime, formatTimeRange, todayIn } from "@/features/schedule/centreTime";

const Cairo = "Africa/Cairo";

/** The hour and minute an Arabic (or any) formatted time shows, read through Intl's own parts in the same zone. */
function hourMinute(instant: string, locale: string): { hour: string; minute: string; dayPeriod: string } {
  const parts = new Intl.DateTimeFormat(locale, { hour: "numeric", minute: "2-digit", timeZone: Cairo, numberingSystem: "latn" }).formatToParts(
    new Date(instant),
  );
  const part = (type: Intl.DateTimeFormatPartTypes) => parts.find((candidate) => candidate.type === type)?.value ?? "";
  return { hour: part("hour"), minute: part("minute"), dayPeriod: part("dayPeriod") };
}

describe("centreTime", () => {
  // Egypt moves its clocks forward on Friday 30 April 2027: the same Saturday 18:00 is 16:00Z before, 15:00Z after.
  it.each([
    ["2027-04-24T16:00:00Z", "before the spring change"],
    ["2027-05-01T15:00:00Z", "after the spring change"],
  ])("formats %s as 6:00 PM in Cairo (%s)", (instant) => {
    expect(formatTime(instant, Cairo, "en")).toBe("6:00 PM");
  });

  // And back on the last Thursday of October 2027: 15:00Z the Saturday before, 16:00Z the Saturday after.
  it.each([
    ["2027-10-23T15:00:00Z", "before the autumn change"],
    ["2027-10-30T16:00:00Z", "after the autumn change"],
  ])("formats %s as 6:00 PM in Cairo (%s)", (instant) => {
    expect(formatTime(instant, Cairo, "en")).toBe("6:00 PM");
  });

  it("formats in the zone it is given, not the runner's: the same instant reads 6 PM in Cairo and 4 PM in London", () => {
    expect(formatTime("2027-04-24T16:00:00Z", Cairo, "en")).toBe("6:00 PM");
    expect(formatTime("2027-04-24T16:00:00Z", "Europe/London", "en")).toBe("5:00 PM");
    expect(formatTime("2027-04-24T16:00:00Z", "UTC", "en")).toBe("4:00 PM");
  });

  it("in Arabic shows the same hour and minute, judged by value rather than by digit shapes", () => {
    const arabic = formatTime("2027-05-01T15:00:00Z", Cairo, "ar");
    const { hour, minute } = hourMinute("2027-05-01T15:00:00Z", "ar");

    expect(hour).toBe("6");
    expect(minute).toBe("00");
    // Whatever digits the environment's ICU uses, the formatted text is not the UTC hour.
    expect(arabic).not.toBe(formatTime("2027-05-01T15:00:00Z", "UTC", "ar"));
    expect(arabic).toBe(formatTime("2027-04-24T16:00:00Z", Cairo, "ar"));
  });

  it("formats a session's range in the centre's zone", () => {
    expect(formatTimeRange("2027-04-24T16:00:00Z", "2027-04-24T17:30:00Z", Cairo, "en")).toMatch(/^6:00\s?–\s?7:30\sPM$/u);
  });

  it("todayIn gives the centre's date, which can differ from UTC's around midnight", () => {
    // 22:30 UTC on 31 December is already 00:30 on 1 January in Cairo (UTC+2).
    expect(todayIn(Cairo, new Date("2026-12-31T22:30:00Z"))).toBe("2027-01-01");
    expect(todayIn("UTC", new Date("2026-12-31T22:30:00Z"))).toBe("2026-12-31");
    expect(todayIn(Cairo, new Date("2026-12-31T21:59:00Z"))).toBe("2026-12-31");
  });

  it("formats a rule's wall-clock time without any zone, and computes its end", () => {
    expect(formatLocalTime("18:05", "en")).toBe("6:05 PM");
    expect(addMinutes("18:00", 90)).toBe("19:30");
    expect(addMinutes("23:30", 60)).toBe("00:30");
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
