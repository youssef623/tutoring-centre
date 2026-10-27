# `frontend/src/components/ui/tooltip.tsx`

## Purpose

Declares `TooltipProvider`, `Tooltip`, `TooltipTrigger`, `TooltipContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/tooltip.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 53 lines and 1,776 bytes. Source hash: `9de7b6ef1606fd4b9b3e977a4aef0433643b6a9199175021db5861d106e8e918`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { Tooltip as TooltipPrimitive } from "@base-ui/react/tooltip"`
- Line 3: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/components/ui/tooltip.tsx#L5): `function TooltipProvider({ delay = 0, ...props }: TooltipPrimitive.Provider.Props) {`
- [Line 9](../../../../../../frontend/src/components/ui/tooltip.tsx#L9): `function Tooltip({ ...props }: TooltipPrimitive.Root.Props) {`
- [Line 13](../../../../../../frontend/src/components/ui/tooltip.tsx#L13): `function TooltipTrigger({ ...props }: TooltipPrimitive.Trigger.Props) {`
- [Line 17](../../../../../../frontend/src/components/ui/tooltip.tsx#L17): `function TooltipContent({`
- [Line 53](../../../../../../frontend/src/components/ui/tooltip.tsx#L53): `export type TooltipContentProps = React.ComponentProps<typeof TooltipContent>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:10](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L10)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:11](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { Tooltip as TooltipPrimitive } from "@base-ui/react/tooltip"
import { cn } from "cn"

function TooltipProvider({ delay = 0, ...props }: TooltipPrimitive.Provider.Props) {
  return <TooltipPrimitive.Provider data-slot="tooltip-provider" delay={delay} {...props} />
}

function Tooltip({ ...props }: TooltipPrimitive.Root.Props) {
  return <TooltipPrimitive.Root data-slot="tooltip" {...props} />
}

function TooltipTrigger({ ...props }: TooltipPrimitive.Trigger.Props) {
  return <TooltipPrimitive.Trigger data-slot="tooltip-trigger" {...props} />
}

function TooltipContent({
  className,
  side = "top",
  sideOffset = 4,
  align = "center",
  alignOffset = 0,
  children,
  ...props
}: TooltipPrimitive.Popup.Props &
  Pick<TooltipPrimitive.Positioner.Props, "align" | "alignOffset" | "side" | "sideOffset">) {
  return (
    <TooltipPrimitive.Portal>
      <TooltipPrimitive.Positioner
        className="isolate z-50"
        align={align}
        alignOffset={alignOffset}
        side={side}
        sideOffset={sideOffset}
      >
        <TooltipPrimitive.Popup
          data-slot="tooltip-content"
          className={cn(
            "z-50 max-w-xs origin-(--transform-origin) rounded-md bg-foreground px-3 py-1.5 text-xs text-background shadow-md data-open:animate-in data-open:fade-in-0 data-open:zoom-in-95 data-closed:animate-out data-closed:fade-out-0 data-closed:zoom-out-95",
            className
          )}
          {...props}
        >
          {children}
        </TooltipPrimitive.Popup>
      </TooltipPrimitive.Positioner>
    </TooltipPrimitive.Portal>
  )
}

export { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger }

export type TooltipContentProps = React.ComponentProps<typeof TooltipContent>
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
