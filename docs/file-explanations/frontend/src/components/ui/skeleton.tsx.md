# frontend/src/components/ui/skeleton.tsx

## Purpose

Pulsing placeholder used while loading.

## Where It Fits

frontend/src/components/ui. Used by `StatusCard`'s pending state.

## Walkthrough

`Skeleton` -> `<div data-slot="skeleton" className={cn("animate-pulse rounded-md bg-muted", className)} {...props} />`.

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Utility classes.

#### How it works here

Component.

#### Why it matters here

Loading placeholder.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/StatusCard.tsx`](../../features/status/StatusCard.tsx.md)
