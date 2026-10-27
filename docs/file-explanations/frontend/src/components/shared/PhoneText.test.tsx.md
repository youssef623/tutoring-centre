# `frontend/src/components/shared/PhoneText.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/components/shared/PhoneText.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 803 bytes. Source hash: `908062cb0ebcc140d14aea566858bfcc2810de6bd7e4729440b90cd666bb626c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { render, screen } from "@testing-library/react";`
- Line 2: `import { describe, expect, it } from "vitest";`
- Line 3: `import { PhoneText } from "./PhoneText";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PhoneText } from "./PhoneText";

describe("PhoneText", () => {
  it("renders left-to-right even when the document is rtl", () => {
    document.documentElement.dir = "rtl";
    try {
      render(<PhoneText value="+201012345678" />);

      expect(screen.getByText("+201012345678")).toHaveAttribute("dir", "ltr");
    } finally {
      document.documentElement.dir = "ltr";
    }
  });

  it("renders a tel: link when asLink is set", () => {
    render(<PhoneText value="+201012345678" asLink />);

    const link = screen.getByRole("link", { name: "+201012345678" });
    expect(link).toHaveAttribute("href", "tel:+201012345678");
    expect(link).toHaveAttribute("dir", "ltr");
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
