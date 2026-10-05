import { useId } from "react";
import { cn } from "cn";

/**
 * The Hessa (حصة) brand mark: a graduation cap on a rounded teal tile.
 * Self-contained SVG with a locally-scoped gradient id so multiple instances never collide.
 */
export function HessaMark({ className }: { className?: string }) {
  const gradientId = useId();
  return (
    <svg
      viewBox="0 0 48 48"
      role="img"
      aria-hidden="true"
      className={cn("size-10", className)}
    >
      <defs>
        <linearGradient id={gradientId} x1="0" y1="0" x2="48" y2="48" gradientUnits="userSpaceOnUse">
          <stop offset="0" stopColor="var(--brand-grad-from, #14b8a6)" />
          <stop offset="1" stopColor="var(--brand-grad-to, #0f766e)" />
        </linearGradient>
      </defs>
      <rect width="48" height="48" rx="13" fill={`url(#${gradientId})`} />
      {/* mortarboard */}
      <path d="M24 12 L40 20 L24 28 L8 20 Z" fill="#fff" />
      <path
        d="M16 23.3 L24 27.3 L32 23.3 V30 C32 32.2 28.4 33.8 24 33.8 C19.6 33.8 16 32.2 16 30 Z"
        fill="#fff"
        opacity="0.9"
      />
      {/* tassel */}
      <path d="M40 20 V28.5" stroke="#fff" strokeWidth="1.6" strokeLinecap="round" />
      <circle cx="40" cy="30.2" r="1.9" fill="var(--brand-grad-accent, #f59e0b)" />
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
      <span className="flex items-baseline gap-1.5 leading-none">
        <span className="font-heading text-xl font-semibold tracking-tight">Hessa</span>
        {showArabic && (
          <span dir="rtl" className="text-lg font-medium text-muted-foreground">
            حصة
          </span>
        )}
      </span>
    </span>
  );
}
