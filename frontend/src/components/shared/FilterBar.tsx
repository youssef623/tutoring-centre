import type { ReactNode } from "react";

/** The bordered bar that holds a list's filters, the same on every list page; a search landmark named `label`. */
export function FilterBar({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div
      role="search"
      aria-label={label}
      className="flex flex-wrap items-end gap-x-4 gap-y-3 rounded-xl border border-border bg-card p-4"
    >
      {children}
    </div>
  );
}

/** A switch and its label as one filter item, aligned with the selects and inputs beside it. */
export function FilterToggle({ children }: { children: ReactNode }) {
  return <div className="flex min-h-8 items-center gap-2">{children}</div>;
}
