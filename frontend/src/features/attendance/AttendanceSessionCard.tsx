import { Link } from "@tanstack/react-router";
import { ChevronRight, MapPin, Presentation } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { AttendanceSessionListItemDto } from "@/api/generated/tutoring-centre";
import { Badge } from "@/components/ui/badge";
import { formatTimeRange } from "@/features/schedule/centreTime";
import { cn } from "cn";
import { WindowBadge } from "./WindowBadge";

const cardClassName = "block rounded-lg border border-border bg-card p-4 text-sm shadow-xs";

/**
 * One session of the day: its time range in the centre's zone, the group, subject, teacher and room, the
 * server's window state and how many of the expected students have a record. Names, times and counts only. A
 * cancelled session stays in place, muted and labelled "Cancelled" in words, and is not a link: there is no
 * register to open for it.
 */
export function AttendanceSessionCard({
  session,
  timeZoneId,
}: {
  session: AttendanceSessionListItemDto;
  timeZoneId: string;
}) {
  const { t, i18n } = useTranslation("attendance");
  const isCancelled = session.status === "cancelled";
  const timeRange = formatTimeRange(session.startsAt, session.endsAt, timeZoneId, i18n.language);
  const recorded = String(Number(session.recordedCount));
  const expected = String(Number(session.expectedCount));

  const body = (
    <>
      <div className="flex items-start justify-between gap-3">
        <time
          dateTime={session.startsAt}
          dir="ltr"
          className={cn("font-semibold tabular-nums", isCancelled && "line-through")}
        >
          {timeRange}
        </time>
        {isCancelled ? (
          <Badge variant="outline">{t("sessions.cancelled")}</Badge>
        ) : (
          <WindowBadge window={session.window} startsAt={session.startsAt} timeZoneId={timeZoneId} />
        )}
      </div>

      <p className={cn("mt-2 break-words text-base font-semibold", isCancelled && "line-through")}>
        <bdi>{session.groupName}</bdi>
      </p>
      <p className="break-words text-muted-foreground">
        <bdi>{session.subjectName}</bdi>
      </p>

      <div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-1 text-muted-foreground">
        <p className="flex items-center gap-1">
          <Presentation className="size-4 shrink-0" aria-hidden="true" />
          <bdi className="min-w-0 break-words">{session.teacherName}</bdi>
        </p>
        {session.room !== null && (
          <p className="flex items-center gap-1">
            <MapPin className="size-4 shrink-0" aria-hidden="true" />
            <span className="min-w-0 break-words">{t("sessions.room", { room: session.room })}</span>
          </p>
        )}
      </div>

      {!isCancelled && (
        <p className="mt-3 flex items-center justify-between gap-2 font-medium">
          <span className="flex items-baseline gap-1.5" aria-label={t("sessions.recordedLabel", { recorded, expected })}>
            <bdi dir="ltr" className="tabular-nums">{`${recorded} / ${expected}`}</bdi>
            <span className="font-normal text-muted-foreground" aria-hidden="true">
              {t("sessions.recorded")}
            </span>
          </span>
          <ChevronRight className="size-4 text-muted-foreground rtl:-scale-x-100" aria-hidden="true" />
        </p>
      )}
    </>
  );

  if (isCancelled) {
    return (
      <li>
        <div data-status={session.status} className={cn(cardClassName, "border-dashed bg-muted/40 text-muted-foreground shadow-none")}>
          {body}
        </div>
      </li>
    );
  }

  return (
    <li>
      <Link
        to="/attendance/sessions/$sessionId"
        params={{ sessionId: session.sessionId }}
        data-status={session.status}
        className={cn(
          cardClassName,
          "outline-none transition-colors hover:bg-muted/40 focus-visible:ring-3 focus-visible:ring-ring/50",
        )}
      >
        {body}
      </Link>
    </li>
  );
}
