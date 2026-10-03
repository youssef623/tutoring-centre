# frontend/src/lib/utils.ts

## Purpose
A one-line re-export, `export { cn } from "cn"`. It's the conventional shadcn location for the `cn` class-merging helper, which `components.json`'s `utils` alias points to.

## Where it fits
`src/lib/`. **Nothing imports it** (verified by grep). All UI components import `cn` directly from the `cn` package.

## Walkthrough
- **Line 1:** re-export.

## Concepts used
- **Barrel re-export.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Dead code.** Either remove it, or switch the components to `@/lib/utils` for a single indirection point.
- **Not Prettier-formatted:** missing semicolon.

## Related files
- [components.json](../../components.json.md)
- [alert.tsx](../components/ui/alert.tsx.md)
