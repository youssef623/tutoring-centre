import { ChevronLeft, ChevronRight } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { addDays, formatCalendarRange } from "@/features/schedule/week";

/**
 * Previous week, this week, next week, and the week's dates for the current language. The arrows point the way
 * the page reads (mirrored in Arabic), and the label is announced when the week changes.
 */
export function WeekNavigator({
  weekStart,
  currentWeekStart,
  onWeekChange,
}: {
  weekStart: string;
  currentWeekStart: string | null;
  onWeekChange: (weekStart: string) => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const range = formatCalendarRange(weekStart, addDays(weekStart, 6), i18n.language);

  return (
    <nav aria-label={t("timetable.weekLabel", { range })} className="flex flex-wrap items-center gap-2">
      <div className="flex items-center gap-1">
        <Button
          type="button"
          variant="outline"
          size="icon-sm"
          aria-label={t("timetable.previousWeek")}
          onClick={() => {
            onWeekChange(addDays(weekStart, -7));
          }}
        >
          <ChevronLeft className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={currentWeekStart === null || currentWeekStart === weekStart}
          onClick={() => {
            if (currentWeekStart !== null) {
              onWeekChange(currentWeekStart);
            }
          }}
        >
          {t("timetable.thisWeek")}
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon-sm"
          aria-label={t("timetable.nextWeek")}
          onClick={() => {
            onWeekChange(addDays(weekStart, 7));
          }}
        >
          <ChevronRight className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
      </div>
      <p aria-live="polite" className="text-sm font-medium">
        {range}
      </p>
    </nav>
  );
}
