import type { ReactNode } from "react";

/**
 * Every page's heading: the title, one line saying what the page is for, and the page's primary action at the
 * end of the same row (wrapping under the title on a phone). The action arrives already wrapped in `Can`.
 */
export function PageHeader({
  title,
  description,
  actions,
}: {
  title: string;
  description?: string;
  actions?: ReactNode;
}) {
  return (
    <div className="page-heading flex flex-wrap items-end justify-between gap-x-4 gap-y-4">
      <div className="min-w-0 space-y-1">
        <h1 className="font-heading text-3xl leading-tight font-semibold tracking-tight lg:text-4xl">
          {title}
        </h1>
        {description !== undefined && (
          <p className="text-sm text-muted-foreground">{description}</p>
        )}
      </div>
      {actions !== undefined && <div className="flex flex-wrap items-center gap-2">{actions}</div>}
    </div>
  );
}
