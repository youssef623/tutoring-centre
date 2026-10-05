# frontend/src/components/ui/button.tsx

## Purpose

Button component: Base UI's button primitive with project variants.

## Where It Fits

frontend/src/components/ui. Used by `StatusCard`.

## Walkthrough

`Button({ className, variant = "default", size = "default", ...props })` renders `<ButtonPrimitive data-slot="button" className={cn(buttonVariants({variant,size,className}))} {...props} />`. Props type: `ButtonPrimitive.Props & VariantProps<typeof buttonVariants>`. Rest props (including `onClick`) pass through.

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Props spreading.

#### How it works here

`{...props}`.

#### Why it matters here

Wrapper keeps the primitive's full API.

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Variants.

#### How it works here

`buttonVariants`.

#### Why it matters here

Typed variant props.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/components/ui/button-variants.ts`](button-variants.ts.md)
- [`frontend/src/features/status/StatusCard.tsx`](../../features/status/StatusCard.tsx.md)
