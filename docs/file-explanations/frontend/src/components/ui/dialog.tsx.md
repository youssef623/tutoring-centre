# `frontend/src/components/ui/dialog.tsx`

## Purpose

Declares `Dialog`, `DialogTrigger`, `DialogPortal`, `DialogClose`, `DialogOverlay`, `DialogContent`, `DialogHeader`, `DialogFooter`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/dialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 163 lines and 4,195 bytes. Source hash: `8913b2887b5e8ecda71ab795e88cbab36e8601bd2a10b3ef580325cfa0538925`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import * as React from "react"`
- Line 2: `import { Dialog as DialogPrimitive } from "@base-ui/react/dialog"`
- Line 3: `import { cn } from "cn"`
- Line 5: `import { Button } from "@/components/ui/button"`
- Line 6: `import { XIcon } from "lucide-react"`
- Line 7: `import { useTranslation } from "react-i18next"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/components/ui/dialog.tsx#L9): `function Dialog({ ...props }: DialogPrimitive.Root.Props) {`
- [Line 13](../../../../../../frontend/src/components/ui/dialog.tsx#L13): `function DialogTrigger({ ...props }: DialogPrimitive.Trigger.Props) {`
- [Line 17](../../../../../../frontend/src/components/ui/dialog.tsx#L17): `function DialogPortal({ ...props }: DialogPrimitive.Portal.Props) {`
- [Line 21](../../../../../../frontend/src/components/ui/dialog.tsx#L21): `function DialogClose({ ...props }: DialogPrimitive.Close.Props) {`
- [Line 25](../../../../../../frontend/src/components/ui/dialog.tsx#L25): `function DialogOverlay({`
- [Line 41](../../../../../../frontend/src/components/ui/dialog.tsx#L41): `function DialogContent({`
- [Line 84](../../../../../../frontend/src/components/ui/dialog.tsx#L84): `function DialogHeader({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 94](../../../../../../frontend/src/components/ui/dialog.tsx#L94): `function DialogFooter({`
- [Line 123](../../../../../../frontend/src/components/ui/dialog.tsx#L123): `function DialogTitle({ className, ...props }: DialogPrimitive.Title.Props) {`
- [Line 136](../../../../../../frontend/src/components/ui/dialog.tsx#L136): `function DialogDescription({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:11](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L11)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:12](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L12)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:13](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L13)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:14](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L14)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:15](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L15)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:16](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L16)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:9](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L9)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:10](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L10)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:11](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L11)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:12](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L12)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:13](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L13)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:14](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L14)
- [frontend/src/features/enrollments/TransferDialog.tsx:11](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L11)
- [frontend/src/features/enrollments/TransferDialog.tsx:12](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L12)
- [frontend/src/features/enrollments/TransferDialog.tsx:13](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L13)
- [frontend/src/features/enrollments/TransferDialog.tsx:14](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L14)
- [frontend/src/features/enrollments/TransferDialog.tsx:15](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L15)
- [frontend/src/features/enrollments/TransferDialog.tsx:16](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L16)
- [frontend/src/features/groups/CreateGroupDialog.tsx:19](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L19)
- [frontend/src/features/groups/CreateGroupDialog.tsx:20](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L20)
- 77 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import * as React from "react"
import { Dialog as DialogPrimitive } from "@base-ui/react/dialog"
import { cn } from "cn"

import { Button } from "@/components/ui/button"
import { XIcon } from "lucide-react"
import { useTranslation } from "react-i18next"

function Dialog({ ...props }: DialogPrimitive.Root.Props) {
  return <DialogPrimitive.Root data-slot="dialog" {...props} />
}

function DialogTrigger({ ...props }: DialogPrimitive.Trigger.Props) {
  return <DialogPrimitive.Trigger data-slot="dialog-trigger" {...props} />
}

function DialogPortal({ ...props }: DialogPrimitive.Portal.Props) {
  return <DialogPrimitive.Portal data-slot="dialog-portal" {...props} />
}

function DialogClose({ ...props }: DialogPrimitive.Close.Props) {
  return <DialogPrimitive.Close data-slot="dialog-close" {...props} />
}

function DialogOverlay({
  className,
  ...props
}: DialogPrimitive.Backdrop.Props) {
  return (
    <DialogPrimitive.Backdrop
      data-slot="dialog-overlay"
      className={cn(
        "fixed inset-0 isolate z-50 bg-black/10 duration-100 supports-backdrop-filter:backdrop-blur-xs data-open:animate-in data-open:fade-in-0 data-closed:animate-out data-closed:fade-out-0",
        className
      )}
      {...props}
    />
  )
}

function DialogContent({
  className,
  children,
  showCloseButton = true,
  ...props
}: DialogPrimitive.Popup.Props & {
  showCloseButton?: boolean
}) {
  const { t } = useTranslation("common")

  return (
    <DialogPortal>
      <DialogOverlay />
      <DialogPrimitive.Popup
        data-slot="dialog-content"
        className={cn(
          "fixed top-1/2 left-1/2 z-50 grid w-full max-w-[calc(100%-2rem)] -translate-x-1/2 -translate-y-1/2 gap-4 rounded-xl bg-popover p-4 text-sm text-popover-foreground ring-1 ring-foreground/10 duration-100 outline-none sm:max-w-sm data-open:animate-in data-open:fade-in-0 data-open:zoom-in-95 data-closed:animate-out data-closed:fade-out-0 data-closed:zoom-out-95",
          className
        )}
        {...props}
      >
        {children}
        {showCloseButton && (
          <DialogPrimitive.Close
            data-slot="dialog-close"
            render={
              <Button
                variant="ghost"
                className="absolute top-2 end-2"
                size="icon-sm"
              />
            }
          >
            <XIcon
            />
            <span className="sr-only">{t("close")}</span>
          </DialogPrimitive.Close>
        )}
      </DialogPrimitive.Popup>
    </DialogPortal>
  )
}

function DialogHeader({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="dialog-header"
      className={cn("flex flex-col gap-2", className)}
      {...props}
    />
```

The excerpt stops at line 90; use the original file for the remaining 73 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
