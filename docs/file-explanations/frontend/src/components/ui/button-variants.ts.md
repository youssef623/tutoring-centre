# `frontend/src/components/ui/button-variants.ts`

## Purpose

Current source file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/ui/button-variants.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 2,787 bytes. Source hash: `88403677cde11c0fcf576e4d9509d4379642b4afa17871af5ed21ee094397713`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { cva } from "class-variance-authority"`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../../../frontend/src/components/ui/button-variants.ts#L3): `export const buttonVariants = cva(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/ui/button.tsx:4](../../../../../../frontend/src/components/ui/button.tsx#L4)
- [frontend/src/features/dashboard/DashboardOverview.tsx:10](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L10)
- [frontend/src/features/session/RouteGuard.tsx:5](../../../../../../frontend/src/features/session/RouteGuard.tsx#L5)
- [frontend/src/features/students/RegisterStudentForm.tsx:21](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L21)
- [frontend/src/features/students/StudentsList.tsx:14](../../../../../../frontend/src/features/students/StudentsList.tsx#L14)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:20](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L20)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:41](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L41)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:10](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L10)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:31](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L31)
- [frontend/src/routes/_authenticated/students.index.tsx:5](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { cva } from "class-variance-authority"

export const buttonVariants = cva(
  "group/button inline-flex shrink-0 items-center justify-center rounded-lg border border-transparent bg-clip-padding text-sm font-medium whitespace-nowrap transition-all outline-none select-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 active:not-aria-[haspopup]:translate-y-px disabled:pointer-events-none disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/80",
        outline:
          "border-border bg-background hover:bg-muted hover:text-foreground aria-expanded:bg-muted aria-expanded:text-foreground dark:border-input dark:bg-input/30 dark:hover:bg-input/50",
        secondary:
          "bg-secondary text-secondary-foreground hover:bg-[color-mix(in_oklch,var(--secondary),var(--foreground)_5%)] aria-expanded:bg-secondary aria-expanded:text-secondary-foreground",
        ghost:
          "hover:bg-muted hover:text-foreground aria-expanded:bg-muted aria-expanded:text-foreground dark:hover:bg-muted/50",
        destructive:
          "bg-destructive/10 text-destructive hover:bg-destructive/20 focus-visible:border-destructive/40 focus-visible:ring-destructive/20 dark:bg-destructive/20 dark:hover:bg-destructive/30 dark:focus-visible:ring-destructive/40",
        link: "text-primary underline-offset-4 hover:underline",
      },
      size: {
        default:
          "h-8 gap-1.5 px-2.5 has-data-[icon=inline-end]:pr-2 has-data-[icon=inline-start]:pl-2",
        xs: "h-6 gap-1 rounded-[min(var(--radius-md),10px)] px-2 text-xs in-data-[slot=button-group]:rounded-lg has-data-[icon=inline-end]:pr-1.5 has-data-[icon=inline-start]:pl-1.5 [&_svg:not([class*='size-'])]:size-3",
        sm: "h-7 gap-1 rounded-[min(var(--radius-md),12px)] px-2.5 text-[0.8rem] in-data-[slot=button-group]:rounded-lg has-data-[icon=inline-end]:pr-1.5 has-data-[icon=inline-start]:pl-1.5 [&_svg:not([class*='size-'])]:size-3.5",
        lg: "h-9 gap-1.5 px-2.5 has-data-[icon=inline-end]:pr-2 has-data-[icon=inline-start]:pl-2",
        icon: "size-8",
        "icon-xs":
          "size-6 rounded-[min(var(--radius-md),10px)] in-data-[slot=button-group]:rounded-lg [&_svg:not([class*='size-'])]:size-3",
        "icon-sm":
          "size-7 rounded-[min(var(--radius-md),12px)] in-data-[slot=button-group]:rounded-lg",
        "icon-lg": "size-9",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
)
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
