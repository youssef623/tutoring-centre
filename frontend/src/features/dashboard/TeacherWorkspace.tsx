import { useCan } from "@/features/session/useCan";
import { Permissions } from "@/features/session/permissions";
import { useState } from "react";
import { Link } from "@tanstack/react-router";
import { BookOpen, CalendarDays, ArrowRight } from "lucide-react";
import { useTranslation } from "react-i18next";
import { useListGroups, useGetTimetable } from "@/api/generated/tutoring-centre";
import { useCentreTimeZone } from "@/features/session/useCentreTimeZone";
import { todayIn, formatTimeRange } from "@/features/schedule/centreTime";
import { startOfWeek } from "@/features/schedule/week";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { EmptyState } from "@/components/shared/EmptyState";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";

/** Both endpoints enforce the teacher's own groups; no centre-wide totals are inferred. */
export function TeacherWorkspace() {
  const { t, i18n } = useTranslation("shell");
  const zone = useCentreTimeZone();
  const [now] = useState(() => Date.now());
  const groups = useListGroups();
  const canSchedule = useCan(Permissions.ScheduleView);
  const canAttendance = useCan(Permissions.AttendanceView);
  const weekStart = startOfWeek(zone ? todayIn(zone) : "2000-01-01");
  const timetable = useGetTimetable(
    { weekStart },
    { query: { enabled: zone !== null && canSchedule } },
  );
  const upcoming = (timetable.data?.sessions ?? []).filter(
    (item) => item.status !== "cancelled" && new Date(item.endsAt).getTime() > now,
  );
  const next = upcoming[0];
  return (
    <div className="teacher-workspace">
      <section className="teacher-groups">
        <h2>{t("teacher.groups")}</h2>
        {groups.isPending && <Skeleton className="h-36 w-full" />}
        {groups.isError && (
          <div role="alert">
            <p>{t("dashboard.errorTitle")}</p>
            <Button
              onClick={() => {
                void groups.refetch();
              }}
            >
              {t("teacher.retry")}
            </Button>
          </div>
        )}
        {groups.data?.items
          .filter((group) => group.status === "active")
          .map((group) => (
            <Link
              key={group.id}
              to="/groups/$groupId"
              params={{ groupId: group.id }}
              search={{ tab: "overview" }}
              className="teacher-group"
            >
              <BookOpen aria-hidden="true" />
              <div>
                <h3>
                  <bdi>{group.name}</bdi>
                </h3>
                <p>
                  <bdi>{group.subjectName}</bdi>
                </p>
              </div>
              <SeatMeter enrolled={Number(group.enrolledCount)} capacity={Number(group.capacity)} />
              <ArrowRight className="rtl:rotate-180" aria-hidden="true" />
            </Link>
          ))}
        {groups.data?.items.length === 0 && (
          <EmptyState icon={BookOpen} title={t("teacher.noGroups")} />
        )}
        <h2 className="mt-10">{t("teacher.upcoming")}</h2>
        <div className="teacher-upcoming">
          {upcoming.map((session) => (
            <Link
              key={session.id}
              to="/attendance/sessions/$sessionId"
              params={{ sessionId: session.id }}
            >
              <CalendarDays aria-hidden="true" />
              <time dateTime={session.startsAt}>{session.localDate}</time>
              <strong>
                <bdi>{session.groupName}</bdi>
              </strong>
              <span>
                {zone ? formatTimeRange(session.startsAt, session.endsAt, zone, i18n.language) : ""}
              </span>
            </Link>
          ))}
        </div>
        {timetable.isSuccess && upcoming.length === 0 && (
          <EmptyState icon={CalendarDays} title={t("teacher.noSessions")} />
        )}
        {timetable.isError && (
          <div role="alert">
            <p>{t("dashboard.errorTitle")}</p>
            <Button
              onClick={() => {
                void timetable.refetch();
              }}
            >
              {t("teacher.retry")}
            </Button>
          </div>
        )}
      </section>
      <aside className="teacher-next">
        <CalendarDays aria-hidden="true" />
        <h2>{t("teacher.next")}</h2>
        {next ? (
          <>
            <h3>
              <bdi>{next.groupName}</bdi>
            </h3>
            <p>{next.localDate}</p>
            <p>{zone ? formatTimeRange(next.startsAt, next.endsAt, zone, i18n.language) : ""}</p>
            {canAttendance && (
              <Link
                className={buttonVariants()}
                to="/attendance/sessions/$sessionId"
                params={{ sessionId: next.id }}
              >
                {t("teacher.openRegister")}
                <ArrowRight aria-hidden="true" className="rtl:rotate-180" />
              </Link>
            )}
          </>
        ) : (
          <p>{t("teacher.noSessions")}</p>
        )}
      </aside>
    </div>
  );
}
