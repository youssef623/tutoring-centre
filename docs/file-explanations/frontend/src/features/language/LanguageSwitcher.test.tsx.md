# `frontend/src/features/language/LanguageSwitcher.test.tsx`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/language/LanguageSwitcher.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,333 bytes. Source hash: `a4fee68319bee2b8a716a2728e7bd29a5995d01141e440031b4997fbceaa41ae`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, render, screen } from "@testing-library/react";`
- Line 2: `import { afterEach, describe, expect, it } from "vitest";`
- Line 3: `import i18n from "@/i18n";`
- Line 4: `import { LanguageSwitcher } from "./LanguageSwitcher";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { LanguageSwitcher } from "./LanguageSwitcher";

describe("LanguageSwitcher", () => {
  afterEach(async () => {
    // Leave the document and i18next in English so other test files are unaffected.
    await i18n.changeLanguage("en");
  });

  it("switching to Arabic sets dir=rtl on the document and persists the choice", async () => {
    // Arrange
    render(<LanguageSwitcher />);

    // Act
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "English" });

    // Assert
    expect(document.documentElement.dir).toBe("rtl");
    expect(document.documentElement.lang).toBe("ar");
    expect(localStorage.getItem("tcm.lang")).toBe("ar");
  });

  it("switching back to English sets dir=ltr", async () => {
    // Arrange
    render(<LanguageSwitcher />);
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "English" });

    // Act
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "العربية" });

    // Assert
    expect(document.documentElement.dir).toBe("ltr");
    expect(document.documentElement.lang).toBe("en");
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
