# `frontend/src/components/ui/dropdown-menu.tsx`

## Purpose

Declares `DropdownMenu`, `DropdownMenuPortal`, `DropdownMenuTrigger`, `DropdownMenuContent`, `DropdownMenuGroup`, `DropdownMenuLabel`, `DropdownMenuItem`, `DropdownMenuSub`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/dropdown-menu.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 267 lines and 8,726 bytes. Source hash: `ffd69006ed9c79e16ffefffbf8a9ebbf1c34ca38f40cb9939c946a71943127d7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `import * as React from "react"`
- Line 4: `import { Menu as MenuPrimitive } from "@base-ui/react/menu"`
- Line 5: `import { cn } from "cn"`
- Line 6: `import { ChevronRightIcon, CheckIcon } from "lucide-react"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L8): `function DropdownMenu({ ...props }: MenuPrimitive.Root.Props) {`
- [Line 12](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L12): `function DropdownMenuPortal({ ...props }: MenuPrimitive.Portal.Props) {`
- [Line 16](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L16): `function DropdownMenuTrigger({ ...props }: MenuPrimitive.Trigger.Props) {`
- [Line 20](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L20): `function DropdownMenuContent({`
- [Line 51](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L51): `function DropdownMenuGroup({ ...props }: MenuPrimitive.Group.Props) {`
- [Line 55](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L55): `function DropdownMenuLabel({`
- [Line 75](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L75): `function DropdownMenuItem({`
- [Line 98](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L98): `function DropdownMenuSub({ ...props }: MenuPrimitive.SubmenuRoot.Props) {`
- [Line 102](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L102): `function DropdownMenuSubTrigger({`
- [Line 126](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L126): `function DropdownMenuSubContent({`
- [Line 147](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L147): `function DropdownMenuCheckboxItem({`
- [Line 181](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L181): `function DropdownMenuRadioGroup({ ...props }: MenuPrimitive.RadioGroup.Props) {`
- [Line 190](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L190): `function DropdownMenuRadioItem({`
- [Line 222](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L222): `function DropdownMenuSeparator({`
- [Line 235](../../../../../../frontend/src/components/ui/dropdown-menu.tsx#L235): `function DropdownMenuShortcut({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:5](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L5)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:6](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L6)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:7](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L7)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:8](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L8)
- [frontend/src/features/schedule/SessionCard.tsx:7](../../../../../../frontend/src/features/schedule/SessionCard.tsx#L7)
- [frontend/src/features/staff/StaffList.tsx:31](../../../../../../frontend/src/features/staff/StaffList.tsx#L31)
- [frontend/src/features/staff/StaffList.tsx:32](../../../../../../frontend/src/features/staff/StaffList.tsx#L32)
- [frontend/src/features/staff/StaffList.tsx:33](../../../../../../frontend/src/features/staff/StaffList.tsx#L33)
- [frontend/src/features/staff/StaffList.tsx:34](../../../../../../frontend/src/features/staff/StaffList.tsx#L34)
- [frontend/src/features/students/ParentsCard.tsx:30](../../../../../../frontend/src/features/students/ParentsCard.tsx#L30)
- [frontend/src/features/students/ParentsCard.tsx:31](../../../../../../frontend/src/features/students/ParentsCard.tsx#L31)
- [frontend/src/features/students/ParentsCard.tsx:32](../../../../../../frontend/src/features/students/ParentsCard.tsx#L32)
- [frontend/src/features/students/ParentsCard.tsx:33](../../../../../../frontend/src/features/students/ParentsCard.tsx#L33)
- [frontend/src/features/subjects/SubjectsList.tsx:38](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L38)
- [frontend/src/features/subjects/SubjectsList.tsx:39](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L39)
- [frontend/src/features/subjects/SubjectsList.tsx:40](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L40)
- [frontend/src/features/subjects/SubjectsList.tsx:41](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L41)
- [frontend/src/features/teachers/TeachersList.tsx:42](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L42)
- [frontend/src/features/teachers/TeachersList.tsx:43](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L43)
- [frontend/src/features/teachers/TeachersList.tsx:44](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L44)
- 2 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
"use client"

import * as React from "react"
import { Menu as MenuPrimitive } from "@base-ui/react/menu"
import { cn } from "cn"
import { ChevronRightIcon, CheckIcon } from "lucide-react"

function DropdownMenu({ ...props }: MenuPrimitive.Root.Props) {
  return <MenuPrimitive.Root data-slot="dropdown-menu" {...props} />
}

function DropdownMenuPortal({ ...props }: MenuPrimitive.Portal.Props) {
  return <MenuPrimitive.Portal data-slot="dropdown-menu-portal" {...props} />
}

function DropdownMenuTrigger({ ...props }: MenuPrimitive.Trigger.Props) {
  return <MenuPrimitive.Trigger data-slot="dropdown-menu-trigger" {...props} />
}

function DropdownMenuContent({
  align = "start",
  alignOffset = 0,
  side = "bottom",
  sideOffset = 4,
  className,
  ...props
}: MenuPrimitive.Popup.Props &
  Pick<
    MenuPrimitive.Positioner.Props,
    "align" | "alignOffset" | "side" | "sideOffset"
  >) {
  return (
    <MenuPrimitive.Portal>
      <MenuPrimitive.Positioner
        className="isolate z-50 outline-none"
        align={align}
        alignOffset={alignOffset}
        side={side}
        sideOffset={sideOffset}
      >
        <MenuPrimitive.Popup
          data-slot="dropdown-menu-content"
          className={cn("z-50 max-h-(--available-height) w-(--anchor-width) min-w-32 origin-(--transform-origin) overflow-x-hidden overflow-y-auto rounded-lg bg-popover p-1 text-popover-foreground shadow-md ring-1 ring-foreground/10 duration-100 outline-none data-[side=bottom]:slide-in-from-top-2 data-[side=inline-end]:slide-in-from-left-2 data-[side=inline-start]:slide-in-from-right-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 data-open:animate-in data-open:fade-in-0 data-open:zoom-in-95 data-closed:animate-out data-closed:overflow-hidden data-closed:fade-out-0 data-closed:zoom-out-95", className )}
          {...props}
        />
      </MenuPrimitive.Positioner>
    </MenuPrimitive.Portal>
  )
}

function DropdownMenuGroup({ ...props }: MenuPrimitive.Group.Props) {
  return <MenuPrimitive.Group data-slot="dropdown-menu-group" {...props} />
}

function DropdownMenuLabel({
  className,
  inset,
  ...props
}: MenuPrimitive.GroupLabel.Props & {
  inset?: boolean
}) {
  return (
    <MenuPrimitive.GroupLabel
      data-slot="dropdown-menu-label"
      data-inset={inset}
      className={cn(
        "px-1.5 py-1 text-xs font-medium text-muted-foreground data-inset:pl-7",
        className
      )}
      {...props}
    />
  )
}

function DropdownMenuItem({
  className,
  inset,
  variant = "default",
  ...props
}: MenuPrimitive.Item.Props & {
  inset?: boolean
  variant?: "default" | "destructive"
}) {
  return (
    <MenuPrimitive.Item
      data-slot="dropdown-menu-item"
      data-inset={inset}
      data-variant={variant}
      className={cn(
        "group/dropdown-menu-item relative flex cursor-default items-center gap-1.5 rounded-md px-1.5 py-1 text-sm outline-hidden select-none focus:bg-accent focus:text-accent-foreground not-data-[variant=destructive]:focus:**:text-accent-foreground data-inset:pl-7 data-[variant=destructive]:text-destructive data-[variant=destructive]:focus:bg-destructive/10 data-[variant=destructive]:focus:text-destructive dark:data-[variant=destructive]:focus:bg-destructive/20 data-disabled:pointer-events-none data-disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4 data-[variant=destructive]:*:[svg]:text-destructive",
```

The excerpt stops at line 90; use the original file for the remaining 177 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
