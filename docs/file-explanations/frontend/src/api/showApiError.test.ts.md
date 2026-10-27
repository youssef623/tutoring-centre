# `frontend/src/api/showApiError.test.ts`

## Purpose

Test/support source. Read its assertions and fixture setup together; test source is not evidence that the tests were executed.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/api/showApiError.test.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 2,110 bytes. Source hash: `2ba5af69315ded67a26e578904298f7fc510632f44ce1f08df849e52c2359245`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { beforeEach, describe, expect, it, vi } from "vitest";`
- Line 2: `import type { ApiError } from "./errors";`
- Line 3: `import { applyFieldErrors, showApiError } from "./showApiError";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { ApiError } from "./errors";
import { applyFieldErrors, showApiError } from "./showApiError";

const toastError = vi.hoisted(() => vi.fn());
vi.mock("sonner", () => ({ toast: { error: toastError } }));

describe("showApiError", () => {
  beforeEach(() => {
    toastError.mockClear();
  });

  it("shows the translated message with a reference when a correlation id exists", () => {
    // Arrange
    const error: ApiError = {
      kind: "conflict",
      code: "centre.something_new",
      message: "x",
      status: 409,
      correlationId: "corr-1",
    };

    // Act
    showApiError(error);

    // Assert
    expect(toastError).toHaveBeenCalledWith("This conflicts with existing data.", {
      description: "Reference: corr-1",
    });
  });

  it("shows no description when there is no correlation id", () => {
    // Arrange
    const error: ApiError = { kind: "unexpected", code: "x", message: "x", status: 500 };

    // Act
    showApiError(error);

    // Assert
    expect(toastError).toHaveBeenCalledWith("Something went wrong. Please try again.", {});
  });
});

describe("applyFieldErrors", () => {
  it("applies the first message of known fields only", () => {
    // Arrange
    const error: ApiError = {
      kind: "validation",
      code: "validation.failed",
      message: "x",
      status: 400,
      fieldErrors: { name: ["Name is required.", "Second"], other: ["Ignored"] },
    };
    const setError = vi.fn();

    // Act
    applyFieldErrors(error, setError, ["name", "email"] as const);

    // Assert
    expect(setError).toHaveBeenCalledTimes(1);
    expect(setError).toHaveBeenCalledWith("name", { type: "server", message: "Name is required." });
  });

  it("does nothing when the error has no field errors", () => {
    // Arrange
    const error: ApiError = { kind: "forbidden", code: "x", message: "x", status: 403 };
    const setError = vi.fn();

    // Act
    applyFieldErrors(error, setError, ["name"] as const);

    // Assert
    expect(setError).not.toHaveBeenCalled();
  });
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
