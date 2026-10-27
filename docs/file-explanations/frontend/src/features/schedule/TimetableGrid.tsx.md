# `frontend/src/features/schedule/TimetableGrid.tsx`

## Purpose

Source comment (intent): The week, Saturday first: seven columns from `lg` up, one day after another below it. Columns follow the document's direction, so in Arabic Saturday is the rightmost. Sessions are placed by the server's `localDate` (the centre's calendar), never by the browser-local date of `startsAt`, and keep the server's order (by start). The grid has no role or permission logic: a teacher's grid is the same grid with fewer sessions.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 84 lines and 3,508 bytes. Source hash: `3388350b55a5154ffea1e8b0bf56ff138ee53b8b9bbfe433ad7d9b3d9ada5b35`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";`
- Line 3: `import { Badge } from "@/components/ui/badge";`
- Line 4: `import { SessionCard } from "@/features/schedule/SessionCard";`
- Line 5: `import { formatCalendarDate, weekDates, weekdayOf } from "@/features/schedule/week";`
- Line 6: `import { cn } from "cn";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L14): `export function TimetableGrid({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetablePage.tsx:18](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
