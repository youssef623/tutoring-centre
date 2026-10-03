# frontend/src/routes/index.tsx

## Purpose
The `/` route: the "System status" page. It shows an `<h1>` and the `StatusCard`.

## Where it fits
Routing layer, a child of `__root.tsx`. It renders `features/status/StatusCard.tsx`.

## Walkthrough
- **Lines 4–6:** `export const Route = createFileRoute("/")({ component: IndexPage })`. The path string must match the file location; the plugin keeps it in sync.
- **Lines 8–15, `IndexPage`:** `<section className="space-y-4"><h1 ...>System status</h1><StatusCard/></section>`.

## Concepts used
- **`createFileRoute`.**
- **Thin route components** that delegate to feature components.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Lint warning.** The same `react-refresh` warning as `__root.tsx` appears at line 8:10.
- **Hard-coded English heading.**
- **Code-split.** With `autoCodeSplitting`, the component is lazy-loaded into a `routes-*.js` chunk.

## Related files
- [StatusCard.tsx](../features/status/StatusCard.tsx.md)
- [routes/__root.tsx](__root.tsx.md)
