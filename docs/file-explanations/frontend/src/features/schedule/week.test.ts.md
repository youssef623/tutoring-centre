# `frontend/src/features/schedule/week.test.ts`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/schedule/week.test.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,489 bytes. Source hash: `be889a3560e449436d617c7f447a529e8e8ba36ed957a439373a899c06d5d4a6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { describe, expect, it } from "vitest";`
- Line 2: `import { addDays, isIsoDate, startOfWeek, weekDates, weekdayOf } from "@/features/schedule/week";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { describe, expect, it } from "vitest";
import { addDays, isIsoDate, startOfWeek, weekDates, weekdayOf } from "@/features/schedule/week";

describe("week", () => {
  it("the start of the week for a Wednesday is the previous Saturday", () => {
    expect(startOfWeek("2027-04-28")).toBe("2027-04-24");
  });

  it("a Saturday is its own week start, and a Friday belongs to the Saturday six days before", () => {
    expect(startOfWeek("2027-04-24")).toBe("2027-04-24");
    expect(startOfWeek("2027-04-30")).toBe("2027-04-24");
  });

  it("adding 7 days across Cairo's 30 April clock change keeps the weekday and the date arithmetic exact", () => {
    expect(addDays("2027-04-24", 7)).toBe("2027-05-01");
    expect(weekdayOf("2027-05-01")).toBe("saturday");
    expect(addDays("2027-10-23", 7)).toBe("2027-10-30");
    expect(weekdayOf("2027-10-30")).toBe("saturday");
  });

  it("lists the seven days of a week from Saturday, across a month end", () => {
    expect(weekDates("2027-04-24")).toEqual([
      "2027-04-24",
      "2027-04-25",
      "2027-04-26",
      "2027-04-27",
      "2027-04-28",
      "2027-04-29",
      "2027-04-30",
    ]);
    expect(addDays("2027-03-01", -1)).toBe("2027-02-28");
  });

  it("accepts real calendar dates only", () => {
    expect(isIsoDate("2027-04-24")).toBe(true);
    expect(isIsoDate("2027-02-30")).toBe(false);
    expect(isIsoDate("24-04-2027")).toBe(false);
    expect(isIsoDate("2027-04-24T00:00:00Z")).toBe(false);
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
