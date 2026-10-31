import { useId } from "react";
import { cn } from "cn";

/**
 * Hessa's open book: rising pages meet at a shared spine, suggesting discovery and progress.
 * Self-contained SVG with a locally-scoped gradient id so multiple instances never collide.
 */
export function HessaMark({ className }: { className?: string }) {
  const gradientId = useId();
  return (
    <svg viewBox="0 0 48 48" role="img" aria-hidden="true" className={cn("size-10", className)}>
      <defs>
        <linearGradient
          id={gradientId}
          x1="0"
          y1="0"
          x2="48"
          y2="48"
          gradientUnits="userSpaceOnUse"
        >
          <stop offset="0" stopColor="#B69AFF" />
          <stop offset="1" stopColor="#8B5CF4" />
        </linearGradient>
      </defs>
      <g fill={`url(#${gradientId})`}>
        <path d="M23 38C17 33 10 31 3 32v-6c8 0 15 5 20 12Z" />
        <path d="M23 36C17 28 12 24 6 23l2-6c8 3 12 10 15 19Z" />
        <path d="M23 33C20 24 16 18 12 15l5-5c5 6 6 14 6 23Z" />
        <path d="M24 31 21 6l7 2-4 23Z" />
        <path d="M26 34c1-10 5-19 10-23l4 5c-6 4-10 10-14 18Z" />
        <path d="M26 37c5-10 10-15 16-17l3 6c-8 1-13 5-19 11Z" />
        <path d="M25 39c7-6 13-8 20-8v6c-8-2-14-1-20 2Z" />
      </g>
    </svg>
  );
}

/**
 * Full lockup: mark + the Hessa wordmark. Arabic (حصة) is shown alongside by default so the
 * bilingual brand reads the same in LTR and RTL.
 */
export function HessaWordmark({
  className,
  showArabic = true,
}: {
  className?: string;
  showArabic?: boolean;
}) {
  return (
    <span className={cn("inline-flex items-center gap-2.5", className)}>
      <HessaMark className="size-9" />
      <span className="brand-type flex items-baseline gap-2 leading-none">
        <span className="font-heading text-xl font-semibold tracking-tight">Hessa</span>
        {showArabic && (
          <span dir="rtl" className="brand-arabic text-lg font-medium text-muted-foreground">
            حصة
          </span>
        )}
      </span>
    </span>
  );
}
