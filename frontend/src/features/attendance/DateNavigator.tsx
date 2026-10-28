import { ChevronLeft, ChevronRight } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { addDays, formatCalendarDate } from "@/features/schedule/week";

/**
 * Previous day, today, next day, and the day's date for the current language. `date` is the day the server
 * answered for (its own today when the URL has none), so the arrows step from the centre's calendar; until the
 * first answer arrives there is nothing to step from and the arrows wait. "Today" removes the date from the URL
 * rather than writing one, so the server keeps deciding what today is.
 */
export function DateNavigator({
  date,
  isToday,
  onDateChange,
}: {
  date: string | undefined;
  isToday: boolean;
  onDateChange: (date: string | undefined) => void;
}) {
  const { t, i18n } = useTranslation("attendance");
  const label = date === undefined ? undefined : formatCalendarDate(date, i18n.language, { dateStyle: "full" });

  return (
    <nav aria-label={t("sessions.dateNav.label")} className="flex flex-wrap items-center gap-2">
      <div className="flex items-center gap-1">
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={date === undefined}
          aria-label={t("sessions.dateNav.previous")}
          onClick={() => {
            if (date !== undefined) {
              onDateChange(addDays(date, -1));
            }
          }}
        >
          <ChevronLeft className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
        <Button
          type="button"
          variant="outline"
          disabled={isToday}
          onClick={() => {
            onDateChange(undefined);
          }}
        >
          {t("sessions.dateNav.today")}
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon"
          disabled={date === undefined}
          aria-label={t("sessions.dateNav.next")}
          onClick={() => {
            if (date !== undefined) {
              onDateChange(addDays(date, 1));
            }
          }}
        >
          <ChevronRight className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
      </div>
      <p aria-live="polite" className="text-sm font-medium">
        {label}
      </p>
    </nav>
  );
}
