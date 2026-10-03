# frontend/eslint.config.js

## Purpose
ESLint flat configuration. It applies JS recommended, **typescript-eslint `strictTypeChecked`** (type-aware), React Hooks and React Refresh (Vite) rules to all TS/TSX files. It ignores build output and the generated route tree, and relaxes one rule for route files.

## Where it fits
Frontend tooling. Run by `npm run lint` locally and in CI.

## Walkthrough
- **Lines 1–6:** imports, including `defineConfig` and `globalIgnores` from `eslint/config`.
- **Line 10:** ignores `dist`, `coverage` and `src/routeTree.gen.ts`.
- **Lines 11–27, main block for `**/*.{ts,tsx}`:**
  - extends `js.configs.recommended`, `tseslint.configs.strictTypeChecked`, `reactHooks.configs.flat.recommended`, `reactRefresh.configs.vite`;
  - browser globals;
  - `parserOptions.projectService: true`, so type-aware rules find the right tsconfig per file.
- **Lines 28–35, route files:** `react-refresh/only-export-components` is set to `warn` with `allowExportNames: ["Route"]`. TanStack file routes export a `Route` object next to the component.

## Concepts used
- **Flat config.**
- **Type-aware linting:** rules that use the TypeScript type checker, e.g. no floating promises. That's why `StatusCard` writes `void readiness.refetch()`.
- **React Fast Refresh constraints:** a module should export only components so hot reload can keep their state.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **The override doesn't silence the route warnings.** `npm run lint` still reports **2 warnings** (`src/routes/__root.tsx:8:10`, `src/routes/index.tsx:8:10`) for the locally declared, non-exported page components (`RootLayout`, `IndexPage`). The `allowExportNames` option allows the `Route` export, but the plugin still flags the local component. Warnings don't fail CI.
- **The ignore list repeats `.prettierignore`.** Both list the same generated paths in two places.

## Related files
- [package.json](package.json.md)
- [routes/__root.tsx](src/routes/__root.tsx.md)
- [routes/index.tsx](src/routes/index.tsx.md)
