import type { ReactNode } from "react";
import { cn } from "cn";

/**
 * The one list table: a table from 640px up, stacked cards below it (the `.data-table` rules in index.css), so a
 * phone never scrolls sideways. Explicit roles keep the table semantics when the phone layout changes `display`.
 * `className` carries the table's own minimum width for the desktop layout, e.g. `sm:min-w-[720px]`.
 */
export function DataTable({ className, children }: { className?: string; children: ReactNode }) {
  return (
    <div className="table-surface rounded-xl border border-border bg-card sm:overflow-x-auto">
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

export function DataTableColumn({
  children,
  hidden = false,
}: {
  children: ReactNode;
  hidden?: boolean;
}) {
  return (
    <th
      role="columnheader"
      scope="col"
      className="px-5 py-3.5 text-start text-xs font-semibold tracking-wide"
    >
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
export function DataTableRow({
  muted = false,
  children,
}: {
  muted?: boolean;
  children: ReactNode;
}) {
  return (
    <tr
      role="row"
      className={cn(
        "transition-colors hover:bg-muted/40 focus-within:bg-accent/40",
        muted && "text-muted-foreground",
      )}
    >
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
      className={cn("px-5 py-4", kind === "actions" && "w-px text-end", className)}
    >
      {children}
    </td>
  );
}
