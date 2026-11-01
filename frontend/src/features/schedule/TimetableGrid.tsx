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

  const minuteAt = (instant: string) => {
    const parts = new Intl.DateTimeFormat("en-GB", {
      timeZone: timeZoneId,
      hour: "2-digit",
      minute: "2-digit",
      hourCycle: "h23",
    }).formatToParts(new Date(instant));
    return (
      Number(parts.find((p) => p.type === "hour")?.value ?? 0) * 60 +
      Number(parts.find((p) => p.type === "minute")?.value ?? 0)
    );
  };
  const startHour = Math.min(
    8,
    ...sessions.map((item) => Math.floor(minuteAt(item.startsAt) / 60)),
  );
  const endHour = Math.max(
    20,
    ...sessions.map((item) =>
      Math.ceil(
        (minuteAt(item.startsAt) +
          (new Date(item.endsAt).getTime() - new Date(item.startsAt).getTime()) / 60000) /
          60,
      ),
    ),
  );
  return (
    <div className="timetable-grid grid gap-3 xl:grid-cols-7 xl:gap-2">
      <div className="timetable-axis" aria-hidden="true">
        <div className="timetable-axis-heading" />
        {Array.from({ length: endHour - startHour + 1 }, (_, i) => (
          <div key={i}>
            {new Intl.DateTimeFormat(i18n.language, { hour: "numeric", timeZone: "UTC" }).format(
              new Date(Date.UTC(2000, 0, 1, startHour + i)),
            )}
          </div>
        ))}
      </div>
      {weekDates(weekStart).map((date) => {
        const daySessions = byDate.get(date) ?? [];
        const clusters: TimetableSessionDto[][] = [];
        for (const session of [...daySessions].sort((a, b) =>
          a.startsAt.localeCompare(b.startsAt),
        )) {
          const previous = clusters[clusters.length - 1];
          if (
            previous &&
            new Date(session.startsAt).getTime() <
              Math.max(...previous.map((item) => new Date(item.endsAt).getTime()))
          )
            previous.push(session);
          else clusters.push([session]);
        }
        const isToday = date === today;
        const headingId = `timetable-day-${date}`;

        return (
          <section
            key={date}
            aria-labelledby={headingId}
            data-date={date}
            data-today={isToday}
            className={cn(
              "flex min-w-0 flex-col gap-2 rounded-xl border border-border bg-muted/30 p-2.5",
              isToday && "border-primary/60 bg-primary/5 ring-1 ring-primary/30",
            )}
          >
            <header className="flex items-baseline justify-between gap-2 lg:flex-col lg:items-start lg:gap-0.5">
              <h2
                id={headingId}
                className="flex flex-wrap items-baseline gap-x-2 text-sm font-semibold"
              >
                <span>{t(`weekdays.${weekdayOf(date)}`)}</span>
                <span className="text-xs font-normal text-muted-foreground">
                  {formatCalendarDate(date, i18n.language, { day: "numeric", month: "short" })}
                </span>
              </h2>
              <div className="flex items-center gap-2">
                {isToday && <Badge variant="default">{t("timetable.today")}</Badge>}
                <span className="text-xs text-muted-foreground lg:sr-only">
                  {daySessions.length === 0
                    ? t("timetable.noSessionsDay")
                    : t("timetable.sessionCount", { count: daySessions.length })}
                </span>
              </div>
            </header>

            {daySessions.length === 0 ? (
              <p className="hidden py-4 text-center text-xs text-muted-foreground lg:block">
                {t("timetable.noSessionsDay")}
              </p>
            ) : (
              <div className={`timetable-day-body timeline-hours-${String(endHour - startHour)}`}>
                {clusters.map((cluster) => {
                  const first = cluster[0];
                  if (!first) return null;
                  const duration = Math.max(
                    30,
                    Math.round(
                      (Math.max(...cluster.map((item) => new Date(item.endsAt).getTime())) -
                        new Date(first.startsAt).getTime()) /
                        60000,
                    ),
                  );
                  return (
                    <div
                      key={first.id}
                      className={`timetable-position ${positionClasses(Math.max(0, minuteAt(first.startsAt) - startHour * 60), duration)}`}
                    >
                      {cluster.length === 1 ? (
                        <SessionCard
                          session={first}
                          timeZoneId={timeZoneId}
                          onCancel={cancelActionFor?.(first)}
                        />
                      ) : (
                        <details className="timetable-cluster" name="timetable-session-stack">
                          <summary>
                            <time dateTime={first.startsAt}>
                              {new Intl.DateTimeFormat(i18n.language, {
                                hour: "numeric",
                                minute: "2-digit",
                                timeZone: timeZoneId,
                              }).format(new Date(first.startsAt))}
                            </time>
                            <strong>
                              {t("timetable.sessionCount", { count: cluster.length })}
                            </strong>
                            <span>
                              <bdi>{first.groupName}</bdi>
                            </span>
                          </summary>
                          <div className="timetable-cluster-list">
                            {cluster.map((session) => (
                              <SessionCard
                                key={session.id}
                                session={session}
                                timeZoneId={timeZoneId}
                                onCancel={cancelActionFor?.(session)}
                              />
                            ))}
                          </div>
                        </details>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </section>
        );
      })}
    </div>
  );
}

function positionClasses(offset: number, duration: number): string {
  const rounded = Math.round(duration);
  return `offset-hour-${String(Math.floor(offset / 60))} offset-minute-${String(offset % 60)} duration-hour-${String(Math.floor(rounded / 60))} duration-minute-${String(rounded % 60)}`;
}
