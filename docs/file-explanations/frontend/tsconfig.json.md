# frontend/tsconfig.json

## Purpose
The root "solution-style" TypeScript config. It compiles no files itself (`"files": []`) and references two sub-projects: `tsconfig.app.json` (browser code) and `tsconfig.node.json` (the Vite config). It also declares the `@/*` path alias.

## Where it fits
Frontend tooling, used by `tsc -b` (the `build` and `typecheck` scripts), editors, and ESLint's `projectService`.

## Walkthrough
- **Line 2:** `files: []`.
- **Line 3:** `references` to the app and node configs.
- **Lines 4–8:** `paths: { "@/*": ["./src/*"] }`, so editors resolve `@/` imports. It's duplicated in `tsconfig.app.json`.

## Concepts used
- **TypeScript project references** and build mode (`tsc -b`).
- **Path aliases.** These must match Vite's `resolve.alias` (`vite.config.ts:18-22`).

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Three places define `@/*`:** here, `tsconfig.app.json` and `vite.config.ts`. They must be kept in sync.

## Related files
- [tsconfig.app.json](tsconfig.app.json.md)
- [tsconfig.node.json](tsconfig.node.json.md)
- [vite.config.ts](vite.config.ts.md)
