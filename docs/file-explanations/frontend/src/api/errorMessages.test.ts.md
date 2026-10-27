# `frontend/src/api/errorMessages.test.ts`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/api/errorMessages.test.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 94 lines and 2,296 bytes. Source hash: `ef8b9cd38daacd8ec70c9080a58ce5ef8aa5c077cb1e031ba3470fa537d2c124`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { afterEach, describe, expect, it } from "vitest";`
- Line 2: `import i18next from "@/i18n";`
- Line 3: `import { messageFor } from "./errorMessages";`
- Line 4: `import type { ApiError, ErrorKind } from "./errors";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { afterEach, describe, expect, it } from "vitest";
import i18next from "@/i18n";
import { messageFor } from "./errorMessages";
import type { ApiError, ErrorKind } from "./errors";

describe("messageFor", () => {
  afterEach(async () => {
    // Leave i18next in English so other test files are unaffected.
    await i18next.changeLanguage("en");
  });

  it("returns the specific dictionary message for a known code", () => {
    // Arrange
    const error: ApiError = {
      kind: "validation",
      code: "centre.slug_invalid",
      message: "x",
      status: 400,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe(
      "The web address may only contain lowercase letters, digits and single hyphens.",
    );
  });

  it("falls back to the kind's message for an unknown code", () => {
    // Arrange
    const error: ApiError = {
      kind: "conflict",
      code: "centre.something_new",
      message: "x",
      status: 409,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("This conflicts with existing data.");
  });

  it("falls back to the generic message for an unknown code and unknown kind", () => {
    // Arrange
    const error: ApiError = {
      kind: "bogus" as ErrorKind,
      code: "totally.unknown",
      message: "x",
      status: 500,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("Something went wrong. Please try again.");
  });

  it("returns the English message for tenant.not_selected", () => {
    // Arrange
    const error: ApiError = {
      kind: "forbidden",
      code: "tenant.not_selected",
      message: "x",
      status: 403,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("Please select a centre to continue.");
  });

  it("returns the Arabic message for tenant.not_selected", async () => {
    // Arrange
    await i18next.changeLanguage("ar");
    const error: ApiError = {
      kind: "forbidden",
      code: "tenant.not_selected",
      message: "x",
      status: 403,
    };

    // Act
    const message = messageFor(error);

```

The excerpt stops at line 90; use the original file for the remaining 4 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
