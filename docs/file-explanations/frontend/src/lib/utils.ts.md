# frontend/src/lib/utils.ts

## Purpose

Re-exports the `cn` class-name helper under the conventional shadcn path.

## Where It Fits

frontend/src/lib. **Unused**: every `components/ui/*` file imports `cn` directly from `"cn"` instead of `@/lib/utils`.

## Walkthrough

`export { cn } from "cn"`. `components.json` declares `@/lib/utils` as the utils alias, which is why the file exists.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dead module (verified by search for `lib/utils` imports).

## Related Files

- [`frontend/components.json`](../../components.json.md)
- [`frontend/src/components/ui/button.tsx`](../components/ui/button.tsx.md)
