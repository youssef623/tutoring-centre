# `frontend/src/components/shared/StatTile.tsx`

## Purpose

Source comment (intent): One summary figure above a list ("Active teachers: 4"). The value is a plain number rendered left-to-right with Western digits, the app's convention in both languages, so it never reorders inside Arabic text.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/StatTile.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,071 bytes. Source hash: `6ca2fa844d6eeca1809522de14dc86699a6fa1ef21565b957cd1f0c926183be4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { LucideIcon } from "lucide-react";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/components/shared/StatTile.tsx#L7): `export function StatTile({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: number }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/dashboard/DashboardOverview.tsx:7](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L7)
- [frontend/src/features/groups/GroupsList.tsx:16](../../../../../../frontend/src/features/groups/GroupsList.tsx#L16)
- [frontend/src/features/teachers/TeachersList.tsx:26](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L26)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { LucideIcon } from "lucide-react";

/**
 * One summary figure above a list ("Active teachers: 4"). The value is a plain number rendered left-to-right
 * with Western digits, the app's convention in both languages, so it never reorders inside Arabic text.
 */
export function StatTile({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: number }) {
  // Phones fit three tiles in a row: no icon, a smaller figure, and a label that may wrap to two lines.
  return (
    <div className="flex items-center gap-3 rounded-xl border border-border bg-card p-3 sm:p-4">
      <span className="hidden size-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary sm:flex">
        <Icon className="size-5" aria-hidden="true" />
      </span>
      <div className="min-w-0">
        <p className="line-clamp-2 text-xs text-muted-foreground sm:truncate sm:text-sm">{label}</p>
        <p className="text-xl font-semibold tabular-nums sm:text-2xl">
          <bdi>{String(value)}</bdi>
        </p>
      </div>
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
