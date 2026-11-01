import { useState } from "react";
import { useTranslation } from "react-i18next";
import { CalendarDays } from "lucide-react";
import { useGetTimetable, type TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { useCan } from "@/features/session/useCan";
import { Permissions } from "@/features/session/permissions";
import { EmptyState } from "@/components/shared/EmptyState";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { startOfWeek, formatCalendarDate } from "./week";
import { WeekNavigator } from "./WeekNavigator";
import { SessionCard } from "./SessionCard";
import { CancelSessionDialog } from "./CancelSessionDialog";

/** Reuses the existing timetable and cancellation workflow, scoped to this group. */
export function GroupSessionTimeline({
  groupId,
  today,
  timeZoneId,
}: {
  groupId: string;
  today: string;
  timeZoneId: string;
}) {
  const { t, i18n } = useTranslation("schedule");
  const [week, setWeek] = useState<string | null>(null);
  const [now] = useState(() => Date.now());
  const [cancelTarget, setCancelTarget] = useState<TimetableSessionDto | null>(null);
  const canManage = useCan(Permissions.ScheduleManage);
  const currentWeek = startOfWeek(today);
  const weekStart = week ?? currentWeek;
  const query = useGetTimetable({ weekStart, groupId });
  return (
    <section className="group-session-timeline">
      <header>
        <h2>{t("groupSessions")}</h2>
        <WeekNavigator
          weekStart={weekStart}
          currentWeekStart={currentWeek}
          onWeekChange={setWeek}
        />
      </header>
      {query.isPending && <Skeleton className="h-32 w-full" />}
      {query.isError && (
        <div role="alert">
          <p>{t("timetable.errorTitle")}</p>
          <Button
            variant="outline"
            onClick={() => {
              void query.refetch();
            }}
          >
            {t("common:retry")}
          </Button>
        </div>
      )}
      {query.data?.sessions.length === 0 && (
        <EmptyState icon={CalendarDays} title={t("timetable.emptyTitle")} />
      )}
      <ol>
        {query.data?.sessions.map((session) => (
          <li key={session.id}>
            <time dateTime={session.localDate}>
              {formatCalendarDate(session.localDate, i18n.language, {
                weekday: "short",
                day: "numeric",
                month: "short",
              })}
            </time>
            <SessionCard
              session={session}
              timeZoneId={timeZoneId}
              onCancel={
                canManage &&
                session.status !== "cancelled" &&
                new Date(session.startsAt).getTime() > now
                  ? () => {
                      setCancelTarget(session);
                    }
                  : undefined
              }
            />
          </li>
        ))}
      </ol>
      <CancelSessionDialog
        session={cancelTarget}
        timeZoneId={timeZoneId}
        onClose={() => {
          setCancelTarget(null);
        }}
      />
    </section>
  );
}
