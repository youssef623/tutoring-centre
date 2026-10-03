# frontend/src/components/ui/card.tsx

## Purpose

shadcn Card components (`Card`, `CardHeader`, `CardTitle`, `CardDescription`, `CardAction`, `CardContent`, `CardFooter`).

## Where It Fits

frontend/src/components/ui. `StatusCard` uses Card, CardHeader, CardTitle, CardDescription, CardContent.

## Walkthrough

Each part is a `div` with `data-slot` and classes composed with `cn`. `Card` accepts `size: "default" | "sm"` and sets `--card-spacing` via a CSS variable class; descendants use `px-(--card-spacing)`. Layout reacts to the presence of footer/image children through `has-data-[slot=card-footer]`/`has-[>img:first-child]` selectors.

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Slots and arbitrary variants.

#### How it works here

Class strings.

#### Why it matters here

Parts compose without wrapper props.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/status/StatusCard.tsx`](../../features/status/StatusCard.tsx.md)
