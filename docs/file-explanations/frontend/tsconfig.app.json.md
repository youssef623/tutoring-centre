# frontend/tsconfig.app.json

## Purpose
TypeScript settings for the application source (`include: ["src"]`): ES2023 + DOM, bundler resolution, React JSX, no emit (Vite does the transpiling), strict checks, and the `@/*` alias.

## Where it fits
Frontend tooling. Referenced by `tsconfig.json`. Covers every file under `src/`, including the tests.

## Walkthrough
- **Line 3:** `tsBuildInfoFile` in `node_modules/.tmp` (incremental cache).
- **Lines 4–7:** `target`/`lib` ES2023 + DOM, `module: esnext`, `types: ["vite/client"]` (types for `import.meta.env` and asset imports).
- **Lines 8–9:** `allowArbitraryExtensions`, `skipLibCheck`.
- **Lines 12–17:**
  - `moduleResolution: bundler`;
  - `allowImportingTsExtensions`;
  - `verbatimModuleSyntax`: type-only imports must use `import type`, which is visible across the code;
  - `moduleDetection: force`;
  - `noEmit`;
  - `jsx: react-jsx`.
- **Lines 18–20:** `@/*` → `./src/*`.
- **Lines 23–28:**
  - `strict`;
  - `noUncheckedIndexedAccess`: indexing returns `T | undefined`, which is why `messageFor` uses `??` chains;
  - `noUnusedLocals` and `noUnusedParameters`;
  - `erasableSyntaxOnly`: bans enums, namespaces and parameter properties, so TS can be stripped without transformation;
  - `noFallthroughCasesInSwitch`.

## Concepts used
- **Bundler-mode TypeScript:** type-check only; the bundler transpiles.
- **Erasable-syntax-only TS:** compatible with type-stripping tools.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **No vitest types.** `types` lists only `vite/client`, so test files import `describe/it/expect` explicitly from `vitest`. That's consistent with globals being off.

## Related files
- [tsconfig.json](tsconfig.json.md)
- [tsconfig.node.json](tsconfig.node.json.md)
