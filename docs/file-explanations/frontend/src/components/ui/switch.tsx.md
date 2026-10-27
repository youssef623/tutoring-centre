# `frontend/src/components/ui/switch.tsx`

## Purpose

Declares `Switch`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/switch.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,787 bytes. Source hash: `367ad27cfe2f4ce5648fd3941410d3c3504876ee23e4360a220f1e6cd05f5c2d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Switch as SwitchPrimitive } from "@base-ui/react/switch"`
- Line 2: `import { cn } from "cn"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../frontend/src/components/ui/switch.tsx#L4): `function Switch({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/e2e/auth.spec.ts:44](../../../../../../frontend/e2e/auth.spec.ts#L44)
- [frontend/e2e/subjects.spec.ts:96](../../../../../../frontend/e2e/subjects.spec.ts#L96)
- [frontend/e2e/teachers-groups.spec.ts:104](../../../../../../frontend/e2e/teachers-groups.spec.ts#L104)
- [frontend/src/routes/_authenticated.test.tsx:72](../../../../../../frontend/src/routes/_authenticated.test.tsx#L72)
- [frontend/src/routes/_authenticated/groups.index.tsx:7](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L7)
- [frontend/src/routes/_authenticated/students.index.tsx:7](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L7)
- [frontend/src/routes/_authenticated/subjects.tsx:9](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L9)
- [frontend/src/routes/_authenticated/teachers.tsx:7](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Switch as SwitchPrimitive } from "@base-ui/react/switch"
import { cn } from "cn"

function Switch({
  className,
  size = "default",
  ...props
}: SwitchPrimitive.Root.Props & {
  size?: "sm" | "default"
}) {
  return (
    <SwitchPrimitive.Root
      data-slot="switch"
      data-size={size}
      className={cn(
        "peer group/switch relative inline-flex shrink-0 items-center rounded-full border border-transparent transition-all outline-none group-has-[:focus-visible]/field-label:border-transparent group-has-[:focus-visible]/field-label:ring-0 after:absolute after:-inset-x-3 after:-inset-y-2 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 data-[size=default]:h-[18.4px] data-[size=default]:w-[32px] data-[size=sm]:h-[14px] data-[size=sm]:w-[24px] dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 data-checked:bg-primary data-unchecked:bg-input dark:data-unchecked:bg-input/80 data-disabled:cursor-not-allowed data-disabled:opacity-50",
        className
      )}
      {...props}
    >
      <SwitchPrimitive.Thumb
        data-slot="switch-thumb"
        className="pointer-events-none block rounded-full bg-background ring-0 transition-transform group-data-[size=default]/switch:size-4 group-data-[size=sm]/switch:size-3 group-data-[size=default]/switch:data-checked:translate-x-[calc(100%-2px)] group-data-[size=sm]/switch:data-checked:translate-x-[calc(100%-2px)] dark:data-checked:bg-primary-foreground group-data-[size=default]/switch:data-unchecked:translate-x-0 group-data-[size=sm]/switch:data-unchecked:translate-x-0 dark:data-unchecked:bg-foreground"
      />
    </SwitchPrimitive.Root>
  )
}

export { Switch }
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
