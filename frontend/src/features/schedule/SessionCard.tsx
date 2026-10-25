import { Link } from "@tanstack/react-router";
import { MapPin, MoreVertical, Presentation, XCircle } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { formatTimeRange } from "@/features/schedule/centreTime";
import { cn } from "cn";

/**
 * One session: its time range in the centre's zone (left to right in both languages), the group (a link to its
 * Schedule tab), subject, teacher and room. A cancelled session is never hidden: it stays in place, struck
 * through and labelled "Cancelled" in words. `onCancel` is passed only for a future scheduled session, and only
 * inside `schedule.manage`; the card itself makes no permission or role decision.
 */
export function SessionCard({
  session,
  timeZoneId,
  onCancel,
}: {
  session: TimetableSessionDto;
  timeZoneId: string;
  onCancel?: () => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const isCancelled = session.status === "cancelled";
  const timeRange = formatTimeRange(session.startsAt, session.endsAt, timeZoneId, i18n.language);

  return (
    <article
      aria-label={`${session.groupName}, ${timeRange}`}
      data-status={session.status}
      className={cn(
        "space-y-1.5 rounded-lg border border-border bg-card p-2.5 text-sm shadow-xs",
        isCancelled && "border-dashed bg-muted/40 text-muted-foreground shadow-none",
      )}
    >
      <div className="flex items-start justify-between gap-1">
        <time
          dateTime={session.startsAt}
          dir="ltr"
          className={cn("font-medium tabular-nums", isCancelled && "line-through")}
        >
          {timeRange}
        </time>
        {onCancel !== undefined && (
          <DropdownMenu>
            <DropdownMenuTrigger
              render={
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  className="-me-1 -mt-0.5 shrink-0"
                  aria-label={t("actions.openSessionMenu", { group: session.groupName, time: timeRange })}
                />
              }
            >
              <MoreVertical className="size-4" aria-hidden="true" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-auto">
              <DropdownMenuItem variant="destructive" onClick={onCancel}>
                <XCircle className="size-4" aria-hidden="true" />
                {t("actions.cancelSession")}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>

      <Link
        to="/groups/$groupId"
        params={{ groupId: session.groupId }}
        search={{ tab: "schedule" }}
        className={cn("block font-semibold break-words hover:underline", isCancelled && "line-through")}
      >
        <bdi>{session.groupName}</bdi>
      </Link>

      <p className="break-words text-xs text-muted-foreground">
        <bdi>{session.subjectName}</bdi>
      </p>

      <p className="flex items-center gap-1 text-xs text-muted-foreground">
        <Presentation className="size-3.5 shrink-0" aria-hidden="true" />
        <bdi className="min-w-0 break-words">{session.teacherName}</bdi>
      </p>

      {session.room !== null && (
        <p className="flex items-center gap-1 text-xs text-muted-foreground">
          <MapPin className="size-3.5 shrink-0" aria-hidden="true" />
          <span className="min-w-0 break-words">{t("timetable.room", { room: session.room })}</span>
        </p>
      )}

      {isCancelled && <Badge variant="outline">{t("timetable.cancelled")}</Badge>}
    </article>
  );
}
