# frontend/src/components/ui/button.tsx

## Purpose
The shadcn `Button`: Base UI's headless `Button` primitive, styled with `buttonVariants`.

## Where it fits
UI primitives. Used by `StatusCard.tsx` for the Retry buttons (`variant="outline" size="sm"`).

## Walkthrough
- **Line 1:** `import { Button as ButtonPrimitive } from "@base-ui/react/button"`.
- **Lines 6–19, `Button({ className, variant = "default", size = "default", ...props })`:**
  - The props type is `ButtonPrimitive.Props & VariantProps<typeof buttonVariants>`.
  - It renders `<ButtonPrimitive data-slot="button" className={cn(buttonVariants({ variant, size, className }))} {...props} />`.

## Concepts used
- **Headless UI primitives:** Base UI provides behaviour and accessibility; Tailwind provides the look.
- **Prop spreading** and variant props.

## Data and control flow
Props → primitive → `<button>`.

## Configuration and environment
None.

## Gotchas and issues
- **`className` goes into the CVA call.** It's passed through `buttonVariants({..., className})` rather than as a second `cn` argument. CVA appends it, and `cn` merges conflicts, so the result is the same.
- **Not Prettier-formatted.**

## Related files
- [button-variants.ts](button-variants.ts.md)
- [StatusCard.tsx](../../features/status/StatusCard.tsx.md)
