# frontend/src/components/ui/card.tsx

## Purpose
The shadcn **Card** family: `Card` (with `size` `default` or `sm`), `CardHeader`, `CardTitle`, `CardDescription`, `CardAction`, `CardContent` and `CardFooter`. These are layout `<div>`s with `data-slot` attributes and a `--card-spacing` CSS variable.

## Where it fits
UI primitives. `StatusCard.tsx` uses `Card`, `CardHeader`, `CardTitle`, `CardDescription` and `CardContent` for the healthy state.

## Walkthrough
- **Lines 4–20, `Card`:**
  - Sets `data-size`.
  - Its classes define `--card-spacing` (`--spacing(4)`, or `--spacing(3)` for `sm`).
  - It drops bottom padding when there's a footer and top padding when the first child is an image.
- **Lines 22–33, `CardHeader`:**
  - A container-query grid (`@container/card-header`).
  - Adds a column for `CardAction` and a row for `CardDescription`.
- **Lines 35–46:** `CardTitle` (heading font, `text-base`).
- **Lines 48–56:** `CardDescription` (muted).
- **Lines 58–69:** `CardAction`, top-right in the header grid.
- **Lines 71–79:** `CardContent` with horizontal padding.
- **Lines 81–92:** `CardFooter`, bordered and muted.

## Concepts used
- **Compound components.**
- **CSS custom properties** for spacing.
- **Container queries.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **`CardAction` placement:** it uses `justify-self-end`, which is logical and fine. Its grid placement (column 2) assumes LTR column order, but CSS grid follows `dir`, so it is fine in RTL too.
- **Not Prettier-formatted.**

## Related files
- [StatusCard.tsx](../../features/status/StatusCard.tsx.md)
