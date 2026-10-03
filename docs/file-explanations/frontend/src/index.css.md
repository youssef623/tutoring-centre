# frontend/src/index.css

## Purpose

Global styles: Tailwind v4 imports, shadcn theme tokens (light and dark) and base layer rules.

## Where It Fits

frontend/src. Imported by `main.tsx`; configured by `components.json`.

## Walkthrough

`@import` of `tailwindcss`, `tw-animate-css`, `shadcn/tailwind.css`, the Geist variable font. `@custom-variant dark (&:is(.dark *))` defines how the `dark:` variant applies (a `.dark` class on an ancestor). `@theme inline { ... }` maps CSS variables (`--background`, `--primary`, ...) to Tailwind theme tokens and defines radius tokens derived from `--radius`. `:root` defines the light palette in `oklch(...)`, `.dark` the dark palette (nothing in the code toggles `.dark` yet). `@layer base`: all elements get `border-border outline-ring/50`; `body` gets `bg-background text-foreground`; `html` gets `font-sans` (Geist Variable).

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Theme tokens.

#### How it works here

`@theme inline`, `:root`, `.dark`.

#### Why it matters here

Components refer to semantic tokens (`bg-card`, `text-muted-foreground`) rather than raw colors.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dark theme tokens exist without a toggle; `next-themes` has no provider in the app.

## Related Files

- [`frontend/components.json`](../components.json.md)
- [`frontend/src/main.tsx`](main.tsx.md)
- [`frontend/src/components/ui/card.tsx`](components/ui/card.tsx.md)
