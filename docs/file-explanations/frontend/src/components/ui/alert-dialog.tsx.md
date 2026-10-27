# `frontend/src/components/ui/alert-dialog.tsx`

## Purpose

Declares `AlertDialog`, `AlertDialogTrigger`, `AlertDialogPortal`, `AlertDialogOverlay`, `AlertDialogContent`, `AlertDialogHeader`, `AlertDialogFooter`, `AlertDialogMedia`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/alert-dialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 185 lines and 5,249 bytes. Source hash: `f679acf47722af0435957a886e2f70bfbdd1e9e8438f08e4285e8c53f2237997`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { AlertDialog as AlertDialogPrimitive } from "@base-ui/react/alert-dialog"`
- Line 3: `import { cn } from "cn"`
- Line 5: `import { Button } from "@/components/ui/button"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L7): `function AlertDialog({ ...props }: AlertDialogPrimitive.Root.Props) {`
- [Line 11](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L11): `function AlertDialogTrigger({ ...props }: AlertDialogPrimitive.Trigger.Props) {`
- [Line 17](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L17): `function AlertDialogPortal({ ...props }: AlertDialogPrimitive.Portal.Props) {`
- [Line 23](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L23): `function AlertDialogOverlay({`
- [Line 39](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L39): `function AlertDialogContent({`
- [Line 62](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L62): `function AlertDialogHeader({`
- [Line 78](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L78): `function AlertDialogFooter({`
- [Line 94](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L94): `function AlertDialogMedia({`
- [Line 110](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L110): `function AlertDialogTitle({`
- [Line 126](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L126): `function AlertDialogDescription({`
- [Line 142](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L142): `function AlertDialogAction({`
- [Line 155](../../../../../../frontend/src/components/ui/alert-dialog.tsx#L155): `function AlertDialogCancel({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:4](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L4)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:5](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L5)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:6](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L6)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:7](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L7)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:8](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L8)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:9](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L9)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:10](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L10)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:11](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L11)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:5](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L5)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:6](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L6)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:7](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L7)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:8](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L8)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:9](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L9)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:10](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L10)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:11](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L11)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:12](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L12)
- [frontend/src/features/staff/StaffList.tsx:18](../../../../../../frontend/src/features/staff/StaffList.tsx#L18)
- [frontend/src/features/staff/StaffList.tsx:19](../../../../../../frontend/src/features/staff/StaffList.tsx#L19)
- [frontend/src/features/staff/StaffList.tsx:20](../../../../../../frontend/src/features/staff/StaffList.tsx#L20)
- [frontend/src/features/staff/StaffList.tsx:21](../../../../../../frontend/src/features/staff/StaffList.tsx#L21)
- 44 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { AlertDialog as AlertDialogPrimitive } from "@base-ui/react/alert-dialog"
import { cn } from "cn"

import { Button } from "@/components/ui/button"

function AlertDialog({ ...props }: AlertDialogPrimitive.Root.Props) {
  return <AlertDialogPrimitive.Root data-slot="alert-dialog" {...props} />
}

function AlertDialogTrigger({ ...props }: AlertDialogPrimitive.Trigger.Props) {
  return (
    <AlertDialogPrimitive.Trigger data-slot="alert-dialog-trigger" {...props} />
  )
}

function AlertDialogPortal({ ...props }: AlertDialogPrimitive.Portal.Props) {
  return (
    <AlertDialogPrimitive.Portal data-slot="alert-dialog-portal" {...props} />
  )
}

function AlertDialogOverlay({
  className,
  ...props
}: AlertDialogPrimitive.Backdrop.Props) {
  return (
    <AlertDialogPrimitive.Backdrop
      data-slot="alert-dialog-overlay"
      className={cn(
        "fixed inset-0 isolate z-50 bg-black/10 duration-100 supports-backdrop-filter:backdrop-blur-xs data-open:animate-in data-open:fade-in-0 data-closed:animate-out data-closed:fade-out-0",
        className
      )}
      {...props}
    />
  )
}

function AlertDialogContent({
  className,
  size = "default",
  ...props
}: AlertDialogPrimitive.Popup.Props & {
  size?: "default" | "sm"
}) {
  return (
    <AlertDialogPortal>
      <AlertDialogOverlay />
      <AlertDialogPrimitive.Popup
        data-slot="alert-dialog-content"
        data-size={size}
        className={cn(
          "group/alert-dialog-content fixed top-1/2 left-1/2 z-50 grid w-full -translate-x-1/2 -translate-y-1/2 gap-4 rounded-xl bg-popover p-4 text-popover-foreground ring-1 ring-foreground/10 duration-100 outline-none data-[size=default]:max-w-xs data-[size=sm]:max-w-xs data-[size=default]:sm:max-w-sm data-open:animate-in data-open:fade-in-0 data-open:zoom-in-95 data-closed:animate-out data-closed:fade-out-0 data-closed:zoom-out-95",
          className
        )}
        {...props}
      />
    </AlertDialogPortal>
  )
}

function AlertDialogHeader({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="alert-dialog-header"
      className={cn(
        "grid grid-rows-[auto_1fr] place-items-center gap-1.5 text-center has-data-[slot=alert-dialog-media]:grid-rows-[auto_auto_1fr] has-data-[slot=alert-dialog-media]:gap-x-4 sm:group-data-[size=default]/alert-dialog-content:place-items-start sm:group-data-[size=default]/alert-dialog-content:text-left sm:group-data-[size=default]/alert-dialog-content:has-data-[slot=alert-dialog-media]:grid-rows-[auto_1fr]",
        className
      )}
      {...props}
    />
  )
}

function AlertDialogFooter({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="alert-dialog-footer"
      className={cn(
        "-mx-4 -mb-4 flex flex-col-reverse gap-2 rounded-b-xl border-t bg-muted/50 p-4 group-data-[size=sm]/alert-dialog-content:grid group-data-[size=sm]/alert-dialog-content:grid-cols-2 sm:flex-row sm:justify-end",
        className
      )}
      {...props}
    />
```

The excerpt stops at line 90; use the original file for the remaining 95 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
