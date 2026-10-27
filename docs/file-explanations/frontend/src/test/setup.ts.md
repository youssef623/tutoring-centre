# `frontend/src/test/setup.ts`

## Purpose

Current test file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/test/setup.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 33 lines and 1,111 bytes. Source hash: `e369f3a5e27109193557c984e4364f64620bc5345b7a39ac03fa798e80197973`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import "@testing-library/jest-dom/vitest";`
- Line 2: `import { cleanup } from "@testing-library/react";`
- Line 3: `import { afterAll, afterEach, beforeAll } from "vitest";`
- Line 4: `import "@/i18n";`
- Line 5: `import { server } from "./msw/server";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import "@testing-library/jest-dom/vitest";
import { cleanup } from "@testing-library/react";
import { afterAll, afterEach, beforeAll } from "vitest";
import "@/i18n";
import { server } from "./msw/server";

// jsdom has no matchMedia; Sonner's Toaster (rendered on every route via __root.tsx) reads it for
// prefers-color-scheme. A minimal stub is enough — no test asserts on an actual media query match.
window.matchMedia = (query: string) =>
  ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => undefined,
    removeListener: () => undefined,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
    dispatchEvent: () => false,
  }) as MediaQueryList;

// Any request without a handler fails the test instead of silently hitting the network.
beforeAll(() => {
  server.listen({ onUnhandledRequest: "error" });
});

afterEach(() => {
  cleanup(); // unmount rendered components (Vitest globals are off, so Testing Library can't do it automatically)
  server.resetHandlers(); // drop per-test overrides
});

afterAll(() => {
  server.close();
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
