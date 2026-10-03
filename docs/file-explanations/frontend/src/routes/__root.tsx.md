# frontend/src/routes/__root.tsx

## Purpose
The TanStack Router **root route**: the app layout wrapping every page. It renders a centred `<main>` (max width 3xl, padding 6) containing the `<Outlet />` for the matched child route, plus a single app-wide `<Toaster richColors />`.

## Where it fits
Routing layer. The router plugin picks it up as the root of `routeTree.gen.ts`. Its child is `routes/index.tsx`. It mounts `components/ui/sonner.tsx`.

## Walkthrough
- **Lines 4–6:** `export const Route = createRootRoute({ component: RootLayout })`.
- **Lines 8–18, `RootLayout`:**
  - a `div` with `min-h-screen bg-background text-foreground`;
  - `<main className="mx-auto w-full max-w-3xl p-6"><Outlet/></main>`;
  - the toaster, with a comment that Day 12's error utilities will use it.

## Concepts used
- **File-based routing:** `__root.tsx` is the reserved name for the root.
- **Layout routes with `<Outlet>`.**

## Data and control flow
Router match → `RootLayout` → `Outlet` → page.

## Configuration and environment
None.

## Gotchas and issues
- **Lint warning.** `npm run lint` reports `react-refresh/only-export-components` at line 8:10 despite the route-file override in `eslint.config.js`.
- **No `notFoundComponent`.** Unknown paths get the router's default.

## Related files
- [routes/index.tsx](index.tsx.md)
- [sonner.tsx](../components/ui/sonner.tsx.md)
- [main.tsx](../main.tsx.md)
- [eslint.config.js](../../eslint.config.js.md)
