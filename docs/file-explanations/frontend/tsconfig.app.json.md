# frontend/tsconfig.app.json

## Purpose

TypeScript settings for the application source under `src/`.

## Where It Fits

Frontend config used by `tsc -b` (`typecheck`, `build`) and the editor.

## Walkthrough

`target es2023`, `lib [ES2023, DOM]`, `module esnext`, `moduleResolution bundler`, `types ["vite/client"]`, `allowImportingTsExtensions`, `verbatimModuleSyntax` (type-only imports must use `import type`), `moduleDetection force`, `noEmit` (Vite does the bundling), `jsx react-jsx`, `paths @/*`. Strictness: `strict`, `noUncheckedIndexedAccess` (indexing yields `T | undefined`, which is why `errorMessages.ts` uses `??`), `noUnusedLocals`, `noUnusedParameters`, `erasableSyntaxOnly` (no enums/namespaces/parameter properties - code must be strippable), `noFallthroughCasesInSwitch`. `tsBuildInfoFile` under `node_modules/.tmp`. `include: ["src"]`.

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Compiler flags.

#### How it works here

Linting block.

#### Why it matters here

Compile-time safety net.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/tsconfig.json`](tsconfig.json.md)
- [`frontend/tsconfig.node.json`](tsconfig.node.json.md)
- [`frontend/package.json`](package.json.md)
