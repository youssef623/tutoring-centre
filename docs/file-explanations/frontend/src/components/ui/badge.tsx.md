# frontend/src/components/ui/badge.tsx

## Purpose

shadcn Badge component: a small pill used to show roles (owner, teacher, assistant) and status text.

## Where It Fits

frontend/src/components/ui. Used by `AppShell.tsx` (active role), `routes/select-centre.tsx` (role per centre) and `SystemInfoCard.tsx` (schema status). Variants come from `badge-variants.ts`.

## Walkthrough

`Badge({ className, variant = "default", render, ...props })` (7-27): returns `useRender({ defaultTagName: "span", props: mergeProps(<{ className: cn(badgeVariants({ variant }), className) }>, props), render, state: { slot: "badge", variant } })`. `useRender` and `mergeProps` are Base UI utilities: the default element is a `span`, and a caller may pass `render` to change the element while keeping the styling. Exports `Badge` only (29).

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Variants plus Base UI render prop.

#### How it works here

Lines 7-27.

#### Why it matters here

The same styled component can render as another element without duplicating classes.

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Props merging.

#### How it works here

`mergeProps` at line 15.

#### Why it matters here

Merges the component's own props with the caller's (class names are combined, handlers are chained).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/components/ui/badge-variants.ts`](badge-variants.ts.md)
- [`frontend/src/features/shell/AppShell.tsx`](../../features/shell/AppShell.tsx.md)
- [`frontend/src/routes/select-centre.tsx`](../../routes/select-centre.tsx.md)
