# frontend/components.json

## Purpose

Configuration for the shadcn/ui CLI that generated the files in `src/components/ui`.

## Where It Fits

Frontend config. Read only by `npx shadcn ...` commands; not used at runtime or by CI.

## Walkthrough

`style: base-nova` (a shadcn style built on Base UI); `rsc: false`, `tsx: true`; `tailwind.config: ""` (Tailwind v4 has no config file), `tailwind.css: src/index.css`, `baseColor: neutral`, `cssVariables: true`, `prefix: ""`; `iconLibrary: lucide`; `rtl: false`; `aliases` for components/utils/ui/lib/hooks (`@/components`, `@/lib/utils`, ...); `menuColor`, `menuAccent`; empty `registries`. `rtl: false` is notable now that Arabic RTL is implemented: the generated components do not use the CLI's RTL transformation; the project converted the few physical utilities by hand (for example in `alert.tsx`).

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

shadcn configuration.

#### How it works here

Whole file.

#### Why it matters here

Determines where generated components go and how they are styled.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/index.css`](src/index.css.md)
- [`frontend/src/lib/utils.ts`](src/lib/utils.ts.md)
- [`frontend/vite.config.ts`](vite.config.ts.md)
