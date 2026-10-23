import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";

/**
 * Seats as "enrolled / capacity" plus a thin bar, with a "Full" badge when no seat is left (meaningful from
 * Day 48, when enrolments exist). The number pair is isolated left-to-right so "12 / 20" never reverses inside
 * an Arabic row; the bar is decorative (the text carries the value), so it is hidden from assistive tech.
 */
export function SeatsMeter({
  enrolled,
  capacity,
  size = "sm",
}: {
  enrolled: number;
  capacity: number;
  size?: "sm" | "lg";
}) {
  const { t } = useTranslation("groups");
  const isFull = capacity > 0 && enrolled >= capacity;
  const percent = capacity > 0 ? Math.min(100, Math.round((enrolled / capacity) * 100)) : 0;

  return (
    <div className={size === "lg" ? "w-full space-y-2" : "w-32 space-y-1.5"}>
      <div className="flex items-center gap-2">
        <span
          className={size === "lg" ? "text-2xl font-semibold tabular-nums" : "text-sm tabular-nums"}
          dir="ltr"
          aria-label={t("seats.label", {
            enrolledText: String(enrolled),
            capacity,
            capacityText: String(capacity),
          })}
        >
          {`${String(enrolled)} / ${String(capacity)}`}
        </span>
        {isFull && <Badge variant="destructive">{t("seats.full")}</Badge>}
      </div>
      <div className="h-1.5 w-full overflow-hidden rounded-full bg-muted" aria-hidden="true">
        <div
          className={isFull ? "h-full rounded-full bg-destructive" : "h-full rounded-full bg-primary"}
          style={{ inlineSize: `${String(percent)}%` }}
        />
      </div>
    </div>
  );
}
