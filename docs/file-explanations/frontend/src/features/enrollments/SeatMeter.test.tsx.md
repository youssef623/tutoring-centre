# `frontend/src/features/enrollments/SeatMeter.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/enrollments/SeatMeter.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 998 bytes. Source hash: `5427089c001cf2ded5a3fd2530142dc87cff4cc96f831bddc22eea29eb75026c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { render, screen } from "@testing-library/react";`
- Line 2: `import { describe, expect, it } from "vitest";`
- Line 3: `import { SeatMeter } from "@/features/enrollments/SeatMeter";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { SeatMeter } from "@/features/enrollments/SeatMeter";

describe("SeatMeter", () => {
  it("below 80% shows only the seat pair", () => {
    render(<SeatMeter enrolled={7} capacity={10} />);

    expect(screen.getByText("7 / 10")).toHaveAttribute("dir", "ltr");
    expect(screen.queryByText("Almost full")).not.toBeInTheDocument();
    expect(screen.queryByText("Full")).not.toBeInTheDocument();
  });

  it("from 80% says Almost full in words", () => {
    render(<SeatMeter enrolled={8} capacity={10} />);

    expect(screen.getByText("Almost full")).toBeInTheDocument();
    expect(screen.queryByText("Full")).not.toBeInTheDocument();
  });

  it("at capacity says Full instead", () => {
    render(<SeatMeter enrolled={10} capacity={10} />);

    expect(screen.getByText("Full")).toBeInTheDocument();
    expect(screen.queryByText("Almost full")).not.toBeInTheDocument();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
