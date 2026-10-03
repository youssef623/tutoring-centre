# frontend/components.json

## Purpose
Configuration for the **shadcn/ui CLI** (`npx shadcn add <component>`). It tells the CLI which style to generate, where the CSS lives, which icon library to use, and the import aliases for generated files.

## Where it fits
Frontend tooling. The app doesn't read it at runtime. It produced `src/components/ui/*` and the theme tokens in `src/index.css`.

## Walkthrough
- **Line 3:** `"style": "base-nova"`, the shadcn style built on **Base UI** primitives (`@base-ui/react`, used by `button.tsx`).
- **Lines 4–5:** `rsc: false` (no React Server Components), `tsx: true`.
- **Lines 6–12:** Tailwind v4 mode (`config: ""`), CSS at `src/index.css`, `baseColor: neutral`, CSS variables on.
- **Line 13:** `iconLibrary: lucide`.
- **Line 14:** `"rtl": false`.
- **Lines 15–21, aliases:**
  - `@/components`
  - `@/lib/utils`
  - `@/components/ui`
  - `@/lib`
  - `@/hooks` (`hooks` doesn't exist)
- **Lines 22–24:** menu styling and no extra registries.

## Concepts used
- **Copy-in component library:** shadcn writes component source into your repo instead of shipping a package.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **`"rtl": false` conflicts with the Arabic/RTL goal** (`frontend/README.md:13,40`). Generated components use physical classes.
- **The `utils` alias is ignored in practice.** It points to `@/lib/utils`, but the generated components import `cn` directly from the `cn` package.

## Related files
- [src/index.css](src/index.css.md)
- [button.tsx](src/components/ui/button.tsx.md)
- [lib/utils.ts](src/lib/utils.ts.md)
