# `frontend/src/components/shared/DataTable.tsx`

## Purpose

Source comment (intent): The one list table: a table from 640px up, stacked cards below it (the `.data-table` rules in index.css), so a phone never scrolls sideways. Explicit roles keep the table semantics when the phone layout changes `display`. `className` carries the table's own minimum width for the desktop layout, e.g. `sm:min-w-[720px]`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/DataTable.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 2,406 bytes. Source hash: `9eca60c204e1cf0fa8db17a9f7d4e95619c624737098e9e2a6360b6317f5ed0e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { ReactNode } from "react";`
- Line 2: `import { cn } from "cn";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/components/shared/DataTable.tsx#L9): `export function DataTable({ className, children }: { className?: string; children: ReactNode }) {`
- [Line 19](../../../../../../frontend/src/components/shared/DataTable.tsx#L19): `export function DataTableHead({ children }: { children: ReactNode }) {`
- [Line 27](../../../../../../frontend/src/components/shared/DataTable.tsx#L27): `export function DataTableColumn({ children, hidden = false }: { children: ReactNode; hidden?: boolean }) {`
- [Line 35](../../../../../../frontend/src/components/shared/DataTable.tsx#L35): `export function DataTableBody({ children }: { children: ReactNode }) {`
- [Line 44](../../../../../../frontend/src/components/shared/DataTable.tsx#L44): `export function DataTableRow({ muted = false, children }: { muted?: boolean; children: ReactNode }) {`
- [Line 56](../../../../../../frontend/src/components/shared/DataTable.tsx#L56): `export function DataTableCell({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:18](../../../../../onboarding-work/enrich_frontend.py#L18)
- [frontend/src/features/enrollments/RosterTable.tsx:6](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L6)
- [frontend/src/features/enrollments/RosterTable.tsx:7](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L7)
- [frontend/src/features/enrollments/RosterTable.tsx:8](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L8)
- [frontend/src/features/enrollments/RosterTable.tsx:9](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L9)
- [frontend/src/features/enrollments/RosterTable.tsx:10](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L10)
- [frontend/src/features/enrollments/RosterTable.tsx:11](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L11)
- [frontend/src/features/groups/GroupsList.tsx:8](../../../../../../frontend/src/features/groups/GroupsList.tsx#L8)
- [frontend/src/features/groups/GroupsList.tsx:9](../../../../../../frontend/src/features/groups/GroupsList.tsx#L9)
- [frontend/src/features/groups/GroupsList.tsx:10](../../../../../../frontend/src/features/groups/GroupsList.tsx#L10)
- [frontend/src/features/groups/GroupsList.tsx:11](../../../../../../frontend/src/features/groups/GroupsList.tsx#L11)
- [frontend/src/features/groups/GroupsList.tsx:12](../../../../../../frontend/src/features/groups/GroupsList.tsx#L12)
- [frontend/src/features/groups/GroupsList.tsx:13](../../../../../../frontend/src/features/groups/GroupsList.tsx#L13)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:5](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L5)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:6](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L6)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:7](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L7)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:8](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L8)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:9](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L9)
- [frontend/src/features/schedule/ScheduleRulesTable.tsx:10](../../../../../../frontend/src/features/schedule/ScheduleRulesTable.tsx#L10)
- [frontend/src/features/staff/StaffList.tsx:38](../../../../../../frontend/src/features/staff/StaffList.tsx#L38)
- 29 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { ReactNode } from "react";
import { cn } from "cn";

/**
 * The one list table: a table from 640px up, stacked cards below it (the `.data-table` rules in index.css), so a
 * phone never scrolls sideways. Explicit roles keep the table semantics when the phone layout changes `display`.
 * `className` carries the table's own minimum width for the desktop layout, e.g. `sm:min-w-[720px]`.
 */
export function DataTable({ className, children }: { className?: string; children: ReactNode }) {
  return (
    <div className="rounded-xl border border-border bg-card sm:overflow-x-auto">
      <table role="table" className={cn("data-table w-full text-start text-sm", className)}>
        {children}
      </table>
    </div>
  );
}

export function DataTableHead({ children }: { children: ReactNode }) {
  return (
    <thead role="rowgroup" className="bg-muted/50 text-muted-foreground">
      <tr role="row">{children}</tr>
    </thead>
  );
}

export function DataTableColumn({ children, hidden = false }: { children: ReactNode; hidden?: boolean }) {
  return (
    <th role="columnheader" scope="col" className="px-4 py-2.5 text-start font-medium">
      {hidden ? <span className="sr-only">{children}</span> : children}
    </th>
  );
}

export function DataTableBody({ children }: { children: ReactNode }) {
  return (
    <tbody role="rowgroup" className="divide-y divide-border">
      {children}
    </tbody>
  );
}

/** `muted` greys out an archived or inactive row; its badge, next to the name, says why in words. */
export function DataTableRow({ muted = false, children }: { muted?: boolean; children: ReactNode }) {
  return (
    <tr role="row" className={muted ? "text-muted-foreground" : undefined}>
      {children}
    </tr>
  );
}

/**
 * `label` is the column name shown before the value in the phone layout. `kind` places a cell in that layout:
 * "primary" (the row's name) and "actions" (the row menu) share the card's first line and need no label.
 */
export function DataTableCell({
  label,
  kind,
  className,
  children,
}: {
  label?: string;
  kind?: "primary" | "actions";
  className?: string;
  children?: ReactNode;
}) {
  return (
    <td
      role="cell"
      data-label={kind === undefined ? label : undefined}
      data-cell={kind}
      className={cn("px-4 py-3", kind === "actions" && "w-px text-end", className)}
    >
      {children}
    </td>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
