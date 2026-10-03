# frontend/tsconfig.node.json

## Purpose
TypeScript settings for Node-side config code. It only includes `vite.config.ts` and type-checks it against Node types with `module: nodenext`.

## Where it fits
Frontend tooling, referenced by `tsconfig.json`.

## Walkthrough
- **Lines 4–6:** ES2023, `types: ["node"]` (for `node:url` in the Vite config).
- **Line 10:** `module: nodenext`.
- **Lines 11–14:** `allowImportingTsExtensions`, `verbatimModuleSyntax`, `moduleDetection: force`, `noEmit`.
- **Lines 17–20:** unused-code and erasable-syntax checks.
- **Line 22:** `include: ["vite.config.ts"]`.

## Concepts used
- **Separate type environments** for browser and Node code.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **No `strict: true`** here, unlike `tsconfig.app.json`. The Vite config is type-checked less strictly.
- **`eslint.config.js` isn't included** in either tsconfig. It's plain JS, so this is fine.

## Related files
- [vite.config.ts](vite.config.ts.md)
- [tsconfig.json](tsconfig.json.md)
