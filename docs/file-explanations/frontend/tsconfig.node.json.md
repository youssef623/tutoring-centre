# frontend/tsconfig.node.json

## Purpose

TypeScript settings for Node-side config (`vite.config.ts`, `orval.config.ts`, `playwright.config.ts` and the `e2e` folder).

## Where It Fits

Frontend config.

## Walkthrough

`target es2023`, `lib [ES2023]`, `types ["node"]` (for `node:url`), `module nodenext`, same strictness subset (`noUnusedLocals`, `erasableSyntaxOnly`...), `include: ["vite.config.ts", "orval.config.ts", "playwright.config.ts", "e2e"]` - the tool configs and the Playwright specs are Node-side code, type-checked with these settings (`types ["node"]`).

## Concepts Used

### Strict TypeScript and type-aware linting

#### What it means

TypeScript checks types at build time; `strict` and extra flags such as `noUncheckedIndexedAccess` make unsafe patterns compile errors. Type-aware ESLint rules use the compiler's type information to catch more bugs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Separate Node config.

#### How it works here

Whole file.

#### Why it matters here

Browser DOM types are not available in config code.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/tsconfig.json`](tsconfig.json.md)
- [`frontend/vite.config.ts`](vite.config.ts.md)
- [`frontend/orval.config.ts`](orval.config.ts.md)
- [`frontend/playwright.config.ts`](playwright.config.ts.md)
- [`frontend/e2e/auth.spec.ts`](e2e/auth.spec.ts.md)
