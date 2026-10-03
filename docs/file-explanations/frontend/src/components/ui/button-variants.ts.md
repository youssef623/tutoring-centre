# frontend/src/components/ui/button-variants.ts

## Purpose
The CVA definition of button styles: variants `default`, `outline`, `secondary`, `ghost`, `destructive` and `link`, and sizes `default`, `xs`, `sm`, `lg`, `icon`, `icon-xs`, `icon-sm` and `icon-lg`. It lives in its own file so `button.tsx` exports only a component, which keeps React Fast Refresh happy.

## Where it fits
UI primitives. Imported by `button.tsx`. It can be reused to style links as buttons.

## Walkthrough
- **Line 4, base classes:** inline-flex, rounded, focus ring (`focus-visible:ring-3`), a 1 px press translation, disabled opacity, `aria-invalid` styling, and SVG sizing.
- **Lines 7–18, variants:** colours driven by theme tokens (`bg-primary`, `bg-secondary`, `bg-destructive/10`, …). `secondary` hover uses `color-mix(in oklch, ...)`.
- **Lines 19–31, sizes:** heights 6–9 (24–36 px). Padding adjusts when an inline icon is at the start or end (`has-data-[icon=inline-start]`).
- **Lines 33–36:** defaults `variant: default`, `size: default`.

## Concepts used
- **CVA.**
- **Design tokens via CSS variables** (defined in `index.css`).
- **Fast-Refresh-friendly module split.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Physical padding:** `pr-*` and `pl-*` on the icon padding rules (lines 21–24) aren't RTL-safe.
- **Not Prettier-formatted.**

## Related files
- [button.tsx](button.tsx.md)
- [index.css](../../index.css.md)
