# frontend/src/components/ui/skeleton.tsx

## Purpose
The shadcn `Skeleton`: a pulsing muted block used as a loading placeholder.

## Where it fits
UI primitives. Used by `StatusCard.tsx` in the `isPending` state.

## Walkthrough
- **Lines 3–11:** `<div data-slot="skeleton" className={cn("animate-pulse rounded-md bg-muted", className)} {...props} />`.

## Concepts used
- **Skeleton loading UI.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **`React.ComponentProps` without importing `React`.** It relies on the global `React` namespace types from `@types/react`. `tsc` passes, so this is accepted.
- **Not Prettier-formatted.**

## Related files
- [StatusCard.tsx](../../features/status/StatusCard.tsx.md)
