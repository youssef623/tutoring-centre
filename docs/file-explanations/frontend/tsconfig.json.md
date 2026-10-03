# frontend/tsconfig.json

## Purpose

Solution-style TypeScript config that references the app and node configs.

## Where It Fits

Frontend root. `tsc -b` follows its references; ESLint's project service uses it to locate configs.

## Walkthrough

`files: []` (compiles nothing itself), `references` to `tsconfig.app.json` and `tsconfig.node.json`, `compilerOptions.paths` `@/*` -> `./src/*` (repeated so tools reading only the root config, such as the shadcn CLI, resolve the alias).

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Project references.

#### How it works here

`references`.

#### Why it matters here

Separate settings for browser code and Node tooling.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/tsconfig.app.json`](tsconfig.app.json.md)
- [`frontend/tsconfig.node.json`](tsconfig.node.json.md)
