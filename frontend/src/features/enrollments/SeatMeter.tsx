import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";
import { cn } from "cn";

/** At or above this share of seats taken (and not yet full) a group is "almost full". */
const AlmostFullShare = 0.8;

/**
 * Seats as "enrolled / capacity" plus a thin bar, with a badge once few or no seats are left: amber "Almost full"
 * from 80%, red "Full" at capacity. The badge's words carry the state; the bar's colour only repeats it. Both
 * numbers come from the server and are only displayed: the count is advisory, and the server's 409 decides who
 * gets the last seat. The number pair is isolated as LTR so "12 / 20" never reverses inside an Arabic row; the
 * bar is decorative (the text carries the value), so it is hidden from assistive tech.
 */
export function SeatMeter({
  enrolled,
  capacity,
  size = "compact",
}: {
  enrolled: number;
  capacity: number;
  size?: "compact" | "full";
}) {
  const { t } = useTranslation("enrollments");
  const isFull = capacity > 0 && enrolled >= capacity;
  const isAlmostFull = !isFull && capacity > 0 && enrolled / capacity >= AlmostFullShare;
  const percent = capacity > 0 ? Math.min(100, Math.round((enrolled / capacity) * 100)) : 0;

  return (
    <div className={size === "full" ? "w-full space-y-2" : "w-40 space-y-1.5"}>
      <div className="flex items-center gap-2">
        <span
          className={size === "full" ? "text-2xl font-semibold tabular-nums" : "text-sm tabular-nums"}
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
        {isAlmostFull && (
          <Badge className="bg-warning/20 text-warning-foreground dark:text-warning">{t("seats.almostFull")}</Badge>
        )}
      </div>
      <div className="h-1.5 w-full overflow-hidden rounded-full bg-muted" aria-hidden="true">
        <div
          className={cn("h-full rounded-full", isFull ? "bg-destructive" : isAlmostFull ? "bg-warning" : "bg-primary")}
          style={{ inlineSize: `${String(percent)}%` }}
        />
      </div>
    </div>
  );
}
