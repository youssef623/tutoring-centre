import { useId } from "react";
import { cn } from "cn";

/**
 * Hessa's six fanned pages converge at the lower-right corner, matching the brand reference.
 * Self-contained SVG with a locally-scoped gradient id so multiple instances never collide.
 */
export function HessaMark({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 128 144" role="img" aria-hidden="true" className={cn("size-10", className)}>
      <HessaPages />
    </svg>
  );
}

function HessaPages() {
  const gradientId = useId();
  return (
    <>
      <defs>
        <linearGradient
          id={gradientId}
          x1="20"
          y1="20"
          x2="108"
          y2="126"
          gradientUnits="userSpaceOnUse"
        >
          <stop offset="0" stopColor="#B69AFF" />
          <stop offset="0.58" stopColor="#8B5CF4" />
          <stop offset="1" stopColor="#6F3FE5" />
        </linearGradient>
      </defs>
      <g fill={`url(#${gradientId})`}>
        <path d="M0 80v20c35-4 72 9 103 23C69 97 35 78 0 80Z" />
        <path d="m13 60-5 13c41 10 73 31 97 51C72 90 43 67 13 60Z" />
        <path d="m25 40-7 16c41 19 72 48 90 69C80 85 51 55 25 40Z" />
        <path d="m44 27-13 8c38 31 66 71 79 91C88 83 63 43 44 27Z" />
        <path d="m73 13-15 7c25 34 45 83 53 105C102 77 89 38 73 13Z" />
        <path d="m97 0-14 12c17 47 25 87 30 114C110 75 105 34 97 0Z" />
      </g>
    </>
  );
}

/** Fixed SVG baselines keep the two names as close as in the supplied reference. */
export function HessaLoginLogo({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 304 144"
      direction="ltr"
      role="img"
      aria-label="Hessa حصة"
      className={cn("login-reference-logo", className)}
    >
      <HessaPages />
      <text
        x="142"
        y="58"
        fill="currentColor"
        fontFamily="Geist Variable, sans-serif"
        fontSize="52"
        fontWeight="700"
        letterSpacing="-2"
        textLength="152"
        lengthAdjust="spacingAndGlyphs"
      >
        Hessa
      </text>
      <text
        x="142"
        y="112"
        fill="currentColor"
        fontFamily="IBM Plex Sans Arabic, sans-serif"
        fontSize="60"
        fontWeight="600"
        direction="rtl"
        textAnchor="end"
        textLength="116"
        lengthAdjust="spacingAndGlyphs"
      >
        حصة
      </text>
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
    <span dir="ltr" className={cn("inline-flex items-center gap-2.5", className)}>
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
