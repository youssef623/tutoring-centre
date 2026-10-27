# `frontend/src/components/ui/select.tsx`

## Purpose

Declares `SelectGroup`, `SelectValue`, `SelectTrigger`, `SelectContent`, `SelectLabel`, `SelectItem`, `SelectSeparator`, `SelectScrollUpButton`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/select.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 198 lines and 6,631 bytes. Source hash: `d3c275e0212325c0bba459800fb155839783d33d5c91d363b5b974973aa7e9e0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { Select as SelectPrimitive } from "@base-ui/react/select"`
- Line 3: `import { cn } from "cn"`
- Line 4: `import { ChevronDownIcon, CheckIcon, ChevronUpIcon } from "lucide-react"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/components/ui/select.tsx#L8): `function SelectGroup({ className, ...props }: SelectPrimitive.Group.Props) {`
- [Line 18](../../../../../../frontend/src/components/ui/select.tsx#L18): `function SelectValue({ className, ...props }: SelectPrimitive.Value.Props) {`
- [Line 28](../../../../../../frontend/src/components/ui/select.tsx#L28): `function SelectTrigger({`
- [Line 56](../../../../../../frontend/src/components/ui/select.tsx#L56): `function SelectContent({`
- [Line 95](../../../../../../frontend/src/components/ui/select.tsx#L95): `function SelectLabel({`
- [Line 108](../../../../../../frontend/src/components/ui/select.tsx#L108): `function SelectItem({`
- [Line 136](../../../../../../frontend/src/components/ui/select.tsx#L136): `function SelectSeparator({`
- [Line 149](../../../../../../frontend/src/components/ui/select.tsx#L149): `function SelectScrollUpButton({`
- [Line 168](../../../../../../frontend/src/components/ui/select.tsx#L168): `function SelectScrollDownButton({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:21](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L21)
- [frontend/src/features/schedule/TimetablePage.tsx:13](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L13)
- [frontend/src/features/staff/ChangeRoleDialog.tsx:25](../../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L25)
- [frontend/src/features/staff/StaffFormDialog.tsx:24](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L24)
- [frontend/src/features/students/AddParentDialog.tsx:34](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L34)
- [frontend/src/features/students/RegisterStudentForm.tsx:25](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L25)
- [frontend/src/features/subjects/SubjectSelect.tsx:4](../../../../../../frontend/src/features/subjects/SubjectSelect.tsx#L4)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:30](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L30)
- [frontend/src/features/teachers/TeacherSelect.tsx:4](../../../../../../frontend/src/features/teachers/TeacherSelect.tsx#L4)
- [frontend/src/routes/_authenticated/audit.tsx:7](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L7)
- [frontend/src/routes/_authenticated/settings.tsx:22](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { Select as SelectPrimitive } from "@base-ui/react/select"
import { cn } from "cn"
import { ChevronDownIcon, CheckIcon, ChevronUpIcon } from "lucide-react"

const Select = SelectPrimitive.Root

function SelectGroup({ className, ...props }: SelectPrimitive.Group.Props) {
  return (
    <SelectPrimitive.Group
      data-slot="select-group"
      className={cn("scroll-my-1 p-1", className)}
      {...props}
    />
  )
}

function SelectValue({ className, ...props }: SelectPrimitive.Value.Props) {
  return (
    <SelectPrimitive.Value
      data-slot="select-value"
      className={cn("flex flex-1 text-left", className)}
      {...props}
    />
  )
}

function SelectTrigger({
  className,
  size = "default",
  children,
  ...props
}: SelectPrimitive.Trigger.Props & {
  size?: "sm" | "default"
}) {
  return (
    <SelectPrimitive.Trigger
      data-slot="select-trigger"
      data-size={size}
      className={cn(
        "flex w-fit items-center justify-between gap-1.5 rounded-lg border border-input bg-transparent py-2 pr-2 pl-2.5 text-sm whitespace-nowrap transition-colors outline-none select-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 data-placeholder:text-muted-foreground data-[size=default]:h-8 data-[size=sm]:h-7 data-[size=sm]:rounded-[min(var(--radius-md),10px)] *:data-[slot=select-value]:line-clamp-1 *:data-[slot=select-value]:flex *:data-[slot=select-value]:items-center *:data-[slot=select-value]:gap-1.5 dark:bg-input/30 dark:hover:bg-input/50 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4",
        className
      )}
      {...props}
    >
      {children}
      <SelectPrimitive.Icon
        render={
          <ChevronDownIcon className="pointer-events-none size-4 text-muted-foreground" />
        }
      />
    </SelectPrimitive.Trigger>
  )
}

function SelectContent({
  className,
  children,
  side = "bottom",
  sideOffset = 4,
  align = "center",
  alignOffset = 0,
  alignItemWithTrigger = true,
  ...props
}: SelectPrimitive.Popup.Props &
  Pick<
    SelectPrimitive.Positioner.Props,
    "align" | "alignOffset" | "side" | "sideOffset" | "alignItemWithTrigger"
  >) {
  return (
    <SelectPrimitive.Portal>
      <SelectPrimitive.Positioner
        side={side}
        sideOffset={sideOffset}
        align={align}
        alignOffset={alignOffset}
        alignItemWithTrigger={alignItemWithTrigger}
        className="isolate z-50"
      >
        <SelectPrimitive.Popup
          data-slot="select-content"
          data-align-trigger={alignItemWithTrigger}
          className={cn("relative isolate z-50 max-h-(--available-height) w-(--anchor-width) min-w-36 origin-(--transform-origin) overflow-x-hidden overflow-y-auto rounded-lg bg-popover text-popover-foreground shadow-md ring-1 ring-foreground/10 duration-100 data-[align-trigger=true]:animate-none data-[side=bottom]:slide-in-from-top-2 data-[side=inline-end]:slide-in-from-left-2 data-[side=inline-start]:slide-in-from-right-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 data-open:animate-in data-open:fade-in-0 data-open:zoom-in-95 data-closed:animate-out data-closed:fade-out-0 data-closed:zoom-out-95", className )}
          {...props}
        >
          <SelectScrollUpButton />
          <SelectPrimitive.List>{children}</SelectPrimitive.List>
          <SelectScrollDownButton />
        </SelectPrimitive.Popup>
      </SelectPrimitive.Positioner>
```

The excerpt stops at line 90; use the original file for the remaining 108 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
