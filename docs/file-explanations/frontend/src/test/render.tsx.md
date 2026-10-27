# `frontend/src/test/render.tsx`

## Purpose

Source comment (intent): Renders `ui` inside a fresh QueryClient so tests never share cache; retries off so failures surface immediately.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/test/render.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,139 bytes. Source hash: `836f18d4c1d6600a0238a4611b257de40947c9c5502f71316ead71ae5dbfd54e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { QueryClient, QueryClientProvider } from "@tanstack/react-query";`
- Line 2: `import { render, renderHook, type RenderHookResult, type RenderResult } from "@testing-library/react";`
- Line 3: `import type { ReactElement, ReactNode } from "react";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../frontend/src/test/render.tsx#L5): `function newTestQueryClient(): QueryClient {`
- [Line 12](../../../../../frontend/src/test/render.tsx#L12): `export function renderWithQueryClient(ui: ReactElement): RenderResult {`
- [Line 19](../../../../../frontend/src/test/render.tsx#L19): `export function renderHookWithQueryClient<TResult, TProps>(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/session/useSession.test.ts:5](../../../../../frontend/src/features/session/useSession.test.ts#L5)
- [frontend/src/features/status/DemoBanner.test.tsx:5](../../../../../frontend/src/features/status/DemoBanner.test.tsx#L5)
- [frontend/src/features/status/StatusCard.test.tsx:5](../../../../../frontend/src/features/status/StatusCard.test.tsx#L5)
- [frontend/src/features/status/SystemInfoCard.test.tsx:5](../../../../../frontend/src/features/status/SystemInfoCard.test.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, renderHook, type RenderHookResult, type RenderResult } from "@testing-library/react";
import type { ReactElement, ReactNode } from "react";

function newTestQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
}

/** Renders `ui` inside a fresh QueryClient so tests never share cache; retries off so failures surface immediately. */
export function renderWithQueryClient(ui: ReactElement): RenderResult {
  const queryClient = newTestQueryClient();

  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>);
}

/** Same as {@link renderWithQueryClient}, for hooks instead of components. */
export function renderHookWithQueryClient<TResult, TProps>(
  hook: (props: TProps) => TResult,
): RenderHookResult<TResult, TProps> {
  const queryClient = newTestQueryClient();
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );

  return renderHook(hook, { wrapper });
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
