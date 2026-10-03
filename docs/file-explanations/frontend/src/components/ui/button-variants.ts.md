# frontend/src/components/ui/button-variants.ts

## Purpose

The `cva` definition of Button variants and sizes, kept separate from the component.

## Where It Fits

frontend/src/components/ui. Imported by `button.tsx`.

## Walkthrough

`buttonVariants = cva(base, { variants: { variant: default|outline|secondary|ghost|destructive|link, size: default|xs|sm|lg|icon|icon-xs|icon-sm|icon-lg }, defaultVariants })`. Base classes include focus rings, `disabled:` and `aria-invalid:` states, and SVG sizing. Splitting it from `button.tsx` keeps the component file exporting only components (react-refresh rule).

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

`cva`.

#### How it works here

Whole file.

#### Why it matters here

Single source of Button styling.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/components/ui/button.tsx`](button.tsx.md)
