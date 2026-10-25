import { useTranslation } from "react-i18next";
import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { Badge } from "@/components/ui/badge";
import { SessionCard } from "@/features/schedule/SessionCard";
import { formatCalendarDate, weekDates, weekdayOf } from "@/features/schedule/week";
import { cn } from "cn";

/**
 * The week, Saturday first: seven columns from `lg` up, one day after another below it. Columns follow the
 * document's direction, so in Arabic Saturday is the rightmost. Sessions are placed by the server's `localDate`
 * (the centre's calendar), never by the browser-local date of `startsAt`, and keep the server's order (by start).
 * The grid has no role or permission logic: a teacher's grid is the same grid with fewer sessions.
 */
export function TimetableGrid({
  weekStart,
  today,
  timeZoneId,
  sessions,
  cancelActionFor,
}: {
  weekStart: string;
  today: string | null;
  timeZoneId: string;
  sessions: TimetableSessionDto[];
  cancelActionFor?: (session: TimetableSessionDto) => (() => void) | undefined;
}) {
  const { t, i18n } = useTranslation("schedule");
  const byDate = new Map<string, TimetableSessionDto[]>();
  for (const session of sessions) {
    byDate.set(session.localDate, [...(byDate.get(session.localDate) ?? []), session]);
  }

  return (
    <div className="grid gap-3 lg:grid-cols-7 lg:gap-2">
      {weekDates(weekStart).map((date) => {
        const daySessions = byDate.get(date) ?? [];
        const isToday = date === today;
        const headingId = `timetable-day-${date}`;

        return (
          <section
            key={date}
            aria-labelledby={headingId}
            data-date={date}
            className={cn(
              "flex min-w-0 flex-col gap-2 rounded-xl border border-border bg-muted/30 p-2.5",
              isToday && "border-primary/60 bg-primary/5 ring-1 ring-primary/30",
            )}
          >
            <header className="flex items-baseline justify-between gap-2 lg:flex-col lg:items-start lg:gap-0.5">
              <h2 id={headingId} className="flex flex-wrap items-baseline gap-x-2 text-sm font-semibold">
                <span>{t(`weekdays.${weekdayOf(date)}`)}</span>
                <span className="text-xs font-normal text-muted-foreground">
                  {formatCalendarDate(date, i18n.language, { day: "numeric", month: "short" })}
                </span>
              </h2>
              <div className="flex items-center gap-2">
                {isToday && <Badge variant="default">{t("timetable.today")}</Badge>}
                <span className="text-xs text-muted-foreground lg:sr-only">
                  {daySessions.length === 0 ? t("timetable.noSessionsDay") : t("timetable.sessionCount", { count: daySessions.length })}
                </span>
              </div>
            </header>

            {daySessions.length === 0 ? (
              <p className="hidden py-4 text-center text-xs text-muted-foreground lg:block">{t("timetable.noSessionsDay")}</p>
            ) : (
              <div className="grid gap-2 sm:grid-cols-2 lg:grid-cols-1">
                {daySessions.map((session) => (
                  <SessionCard
                    key={session.id}
                    session={session}
                    timeZoneId={timeZoneId}
                    onCancel={cancelActionFor?.(session)}
                  />
                ))}
              </div>
            )}
          </section>
        );
      })}
    </div>
  );
}
