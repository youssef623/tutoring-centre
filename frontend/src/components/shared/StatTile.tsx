import type { LucideIcon } from "lucide-react";

/**
 * One summary figure above a list ("Active teachers: 4"). The value is a plain number rendered left-to-right
 * with Western digits, the app's convention in both languages, so it never reorders inside Arabic text.
 */
export function StatTile({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: number }) {
  return (
    <div className="flex items-center gap-3 rounded-xl border border-border bg-card p-4">
      <span className="flex size-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
        <Icon className="size-5" aria-hidden="true" />
      </span>
      <div className="min-w-0">
        <p className="truncate text-sm text-muted-foreground">{label}</p>
        <p className="text-2xl font-semibold tabular-nums">
          <bdi>{String(value)}</bdi>
        </p>
      </div>
    </div>
  );
}
