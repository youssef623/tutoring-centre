# `frontend/src/components/ui/radio-group.tsx`

## Purpose

Declares `RadioGroup`, `RadioGroupItem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/radio-group.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,183 bytes. Source hash: `9e98ba25fc28e5763a6a48e69dfbb1848f6bcc4ebbb2afa17969171ac702334a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Radio as RadioPrimitive } from "@base-ui/react/radio"`
- Line 2: `import { RadioGroup as RadioGroupPrimitive } from "@base-ui/react/radio-group"`
- Line 3: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/components/ui/radio-group.tsx#L5): `function RadioGroup({ className, ...props }: RadioGroupPrimitive.Props) {`
- [Line 9](../../../../../../frontend/src/components/ui/radio-group.tsx#L9): `function RadioGroupItem({ className, ...props }: RadioPrimitive.Root.Props) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/ui/dropdown-menu.tsx:181](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L181)
- [frontend/src/features/enrollments/GroupChoiceList.tsx:3](../../../../../../frontend/src/features/enrollments/GroupChoiceList.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Radio as RadioPrimitive } from "@base-ui/react/radio"
import { RadioGroup as RadioGroupPrimitive } from "@base-ui/react/radio-group"
import { cn } from "cn"

function RadioGroup({ className, ...props }: RadioGroupPrimitive.Props) {
  return <RadioGroupPrimitive data-slot="radio-group" className={cn("grid w-full gap-2", className)} {...props} />
}

function RadioGroupItem({ className, ...props }: RadioPrimitive.Root.Props) {
  return (
    <RadioPrimitive.Root
      data-slot="radio-group-item"
      className={cn(
        "relative flex aspect-square size-4 shrink-0 items-center justify-center rounded-full border border-input outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 data-checked:border-primary data-checked:bg-primary data-disabled:cursor-not-allowed data-disabled:opacity-50",
        className
      )}
      {...props}
    >
      <RadioPrimitive.Indicator data-slot="radio-group-indicator" className="flex items-center justify-center">
        <span className="size-1.5 rounded-full bg-primary-foreground" />
      </RadioPrimitive.Indicator>
    </RadioPrimitive.Root>
  )
}

export { RadioGroup, RadioGroupItem }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
