import type { LucideIcon } from "lucide-react";

/**
 * One summary figure above a list ("Active teachers: 4"). The value is a plain number rendered left-to-right
 * with Western digits, the app's convention in both languages, so it never reorders inside Arabic text.
 */
export function StatTile({
  icon: Icon,
  label,
  value,
}: {
  icon: LucideIcon;
  label: string;
  value: number;
}) {
  // Phones fit three tiles in a row: no icon, a smaller figure, and a label that may wrap to two lines.
  return (
    <div className="stat-tile flex items-center gap-3 rounded-xl border border-border bg-card p-4 sm:p-5">
      <span className="hidden size-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary sm:flex">
        <Icon className="size-5" aria-hidden="true" />
      </span>
      <div className="min-w-0">
        <p className="text-xs text-muted-foreground sm:text-sm">{label}</p>
        <p className="mt-1 text-3xl leading-tight font-semibold tracking-tight tabular-nums">
          <bdi>{String(value)}</bdi>
        </p>
      </div>
    </div>
  );
}
