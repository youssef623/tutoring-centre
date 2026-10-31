import type { LucideIcon } from "lucide-react";
import type { ReactNode } from "react";

/**
 * A list's "nothing here" state: an icon, what is missing, what to do about it, and (optionally) the action
 * that fixes it. The action is passed in already wrapped in `Can` by the caller, so a read-only session sees
 * the explanation without a button it could not use.
 */
export function EmptyState({
  icon: Icon,
  title,
  description,
  action,
}: {
  icon: LucideIcon;
  title: string;
  description?: string;
  action?: ReactNode;
}) {
  return (
    <div className="empty-state flex flex-col items-center gap-4 rounded-xl border border-dashed border-border bg-card px-6 py-14 text-center">
      <span className="flex size-14 items-center justify-center rounded-2xl bg-accent text-primary">
        <Icon className="size-6" aria-hidden="true" />
      </span>
      <div className="space-y-1">
        <p className="text-sm font-semibold">{title}</p>
        {description !== undefined && (
          <p className="max-w-sm text-sm text-muted-foreground">{description}</p>
        )}
      </div>
      {action}
    </div>
  );
}
