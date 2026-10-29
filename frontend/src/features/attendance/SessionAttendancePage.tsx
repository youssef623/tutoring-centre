import { Link, useBlocker } from "@tanstack/react-router";
import { ArrowLeft, CalendarDays, CheckCheck, Clock, Loader2, Presentation, SearchX, TriangleAlert, Users, UsersRound } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetSessionAttendance, type AttendanceStudentDto, type SessionAttendanceDto } from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Skeleton } from "@/components/ui/skeleton";
import { formatTime, formatTimeRange } from "@/features/schedule/centreTime";
import { formatCalendarDate } from "@/features/schedule/week";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { windowOpensAt } from "./attendanceWindow";
import { isolateLtr } from "./bidi";
import { CorrectionDialog } from "./CorrectionDialog";
import { changedMarks, useAttendanceDraft } from "./useAttendanceDraft";
import { useSaveAttendance } from "./useSaveAttendance";
import { RosterRow } from "./RosterRow";
import { WindowBadge } from "./WindowBadge";

/**
 * The register of one session: one row per expected student, a draft of the teacher's taps, and Save.
 *
 * Two kinds of state, kept apart. The records are the server's, read through the query cache and changed only
 * by refetching it. The taps are a draft in this component's state; Save sends only the rows where the draft
 * differs from the record, and the draft is dropped when the server has accepted them. Whether marking is
 * possible is the server's `window` (and the session's own status), never a comparison with the browser's clock.
 */
export function SessionAttendancePage({ sessionId }: { sessionId: string }) {
  const { t } = useTranslation("attendance");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetSessionAttendance(sessionId);
  const canMark = useCan(Permissions.AttendanceMark);
  const { draft, setStatus, markUnrecordedPresent, discard } = useAttendanceDraft();
  const { save, isSaving, rowErrors, windowClosed } = useSaveAttendance(sessionId, discard);
  // The row as it was when Correct was pressed, so the version sent is the one the user saw.
  const [correcting, setCorrecting] = useState<AttendanceStudentDto | null>(null);

  const unsavedCount = data === undefined ? 0 : changedMarks(data.items, draft).length;
  const hasUnsaved = unsavedCount > 0;
  const blocker = useBlocker({
    shouldBlockFn: () => hasUnsaved && !isSaving,
    enableBeforeUnload: () => hasUnsaved,
    withResolver: true,
  });

  if (isPending) {
    return (
      <div role="status" aria-label={t("roster.loading.label")} className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-28 w-full" />
        <Skeleton className="h-28 w-full" />
        <Skeleton className="h-28 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.status === 404) {
      return (
        <EmptyState
          icon={SearchX}
          title={t("roster.notFound.title")}
          description={t("roster.notFound.description")}
          action={
            <Link to="/attendance" className={buttonVariants({ variant: "outline" })}>
              {t("roster.notFound.back")}
            </Link>
          }
        />
      );
    }

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("roster.error.title")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>
              {t("roster.error.reference")} <bdi>{apiError.correlationId}</bdi>
            </p>
          )}
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="self-start"
            onClick={() => {
              void refetch();
            }}
          >
            {tCommon("retry")}
          </Button>
        </AlertDescription>
      </Alert>
    );
  }

  const { session } = data;
  const isCancelled = session.status === "cancelled";
  const markingOpen = canMark && !isCancelled && data.window === "open" && !windowClosed;
  const rowErrorList = Object.entries(rowErrors);

  return (
    <section className="space-y-6 pb-24">
      <SessionHeader data={data} />
      {markingOpen && <Link to="/attendance/sessions/$sessionId/scan" params={{ sessionId }} className={buttonVariants()}>{t("scanner:scanCards")}</Link>}

      <WindowNotice
        window={data.window}
        isCancelled={isCancelled}
        canMark={canMark}
        windowClosed={windowClosed}
        startsAt={session.startsAt}
        timeZoneId={data.timeZoneId}
      />

      {rowErrorList.length > 0 && (
        <Alert variant="destructive" role="alert">
          <TriangleAlert aria-hidden="true" />
          <AlertTitle>{t("roster.rowErrors.title")}</AlertTitle>
          <AlertDescription>
            <ul className="list-disc ps-4">
              {rowErrorList.map(([studentId, rowError]) => (
                <li key={studentId}>
                  <bdi>{rowError.studentName}</bdi>: {rowError.message}
                </li>
              ))}
            </ul>
          </AlertDescription>
        </Alert>
      )}

      {data.items.length === 0 ? (
        <EmptyState icon={UsersRound} title={t("roster.empty.title")} description={t("roster.empty.description")} />
      ) : (
        <>
          {markingOpen && (
            <Button
              type="button"
              variant="outline"
              className="h-11 px-4"
              onClick={() => {
                markUnrecordedPresent(data.items);
              }}
            >
              <CheckCheck className="size-4" aria-hidden="true" />
              {t("roster.markAllPresent")}
            </Button>
          )}

          <ul className="space-y-3">
            {data.items.map((item) => (
              <RosterRow
                key={item.studentId}
                item={item}
                draftStatus={draft[item.studentId]}
                timeZoneId={data.timeZoneId}
                markingDisabled={!markingOpen || isSaving}
                rowError={rowErrors[item.studentId]?.message}
                onChange={(status) => {
                  setStatus(item.studentId, status);
                }}
                onCorrect={() => {
                  setCorrecting(item);
                }}
              />
            ))}
          </ul>
        </>
      )}

      {markingOpen && data.items.length > 0 && (
        <div className="fixed inset-x-0 bottom-0 z-10 border-t border-border bg-background/95 px-4 py-3 backdrop-blur">
          <div className="mx-auto flex max-w-5xl flex-wrap items-center justify-between gap-3">
            <p aria-live="polite" className="text-sm font-medium">
              {t("roster.unsavedCount", { count: unsavedCount })}
            </p>
            <div className="flex items-center gap-2">
              <Button type="button" variant="outline" className="h-11 px-4" disabled={!hasUnsaved || isSaving} onClick={discard}>
                {t("roster.discard")}
              </Button>
              <Button
                type="button"
                className="h-11 px-6"
                disabled={!hasUnsaved || isSaving}
                onClick={() => {
                  void save((items) => changedMarks(items, draft));
                }}
              >
                {isSaving && <Loader2 className="size-4 animate-spin" aria-hidden="true" />}
                {isSaving ? t("roster.saving") : t("roster.save")}
              </Button>
            </div>
          </div>
        </div>
      )}

      <CorrectionDialog
        student={correcting}
        sessionId={sessionId}
        onOpenChange={(open) => {
          if (!open) {
            setCorrecting(null);
          }
        }}
      />

      <AlertDialog
        open={blocker.status === "blocked"}
        onOpenChange={(open) => {
          if (!open) {
            blocker.reset?.();
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{t("roster.leave.title")}</AlertDialogTitle>
            <AlertDialogDescription>{t("roster.leave.description", { count: unsavedCount })}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{t("roster.leave.stay")}</AlertDialogCancel>
            <AlertDialogAction
              variant="destructive"
              onClick={() => {
                blocker.proceed?.();
              }}
            >
              {t("roster.leave.leave")}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </section>
  );
}

/** The group, subject, teacher, date and time range in the centre's zone, and the server's window state. */
function SessionHeader({ data }: { data: SessionAttendanceDto }) {
  const { t, i18n } = useTranslation("attendance");
  const { session } = data;
  const timeRange = formatTimeRange(session.startsAt, session.endsAt, data.timeZoneId, i18n.language);

  return (
    <header className="space-y-3">
      <Link
        to="/attendance"
        search={{ date: session.localDate }}
        className="inline-flex items-center gap-1.5 text-sm text-muted-foreground underline-offset-4 hover:text-foreground hover:underline"
      >
        <ArrowLeft className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        {t("roster.back")}
      </Link>

      <div className="flex flex-wrap items-start justify-between gap-x-4 gap-y-2">
        <div className="min-w-0 space-y-1">
          <h1 className="break-words text-2xl font-semibold tracking-tight">
            <bdi>{session.groupName}</bdi>
          </h1>
          <p className="break-words text-muted-foreground">
            <bdi>{session.subjectName}</bdi>
          </p>
        </div>
        {session.status === "cancelled" ? (
          <Badge variant="outline">{t("sessions.cancelled")}</Badge>
        ) : (
          <WindowBadge window={data.window} startsAt={session.startsAt} timeZoneId={data.timeZoneId} />
        )}
      </div>

      <div className="flex flex-wrap items-center gap-x-5 gap-y-1 text-sm text-muted-foreground">
        <p className="flex items-center gap-1.5">
          <Presentation className="size-4 shrink-0" aria-hidden="true" />
          <bdi>{session.teacherName}</bdi>
        </p>
        <p className="flex items-center gap-1.5">
          <CalendarDays className="size-4 shrink-0" aria-hidden="true" />
          {formatCalendarDate(session.localDate, i18n.language, { dateStyle: "medium" })}
        </p>
        <p className="flex items-center gap-1.5">
          <Users className="size-4 shrink-0" aria-hidden="true" />
          {t("roster.studentCount", { count: data.items.length })}
        </p>
        <p className="flex items-center gap-1.5">
          <Clock className="size-4 shrink-0" aria-hidden="true" />
          <time dateTime={session.startsAt} dir="ltr" className="tabular-nums">
            {timeRange}
          </time>
        </p>
      </div>
    </header>
  );
}

/** Says why the toggles are off whenever they are, so a read-only register is never a silent one. */
function WindowNotice({
  window,
  isCancelled,
  canMark,
  windowClosed,
  startsAt,
  timeZoneId,
}: {
  window: string;
  isCancelled: boolean;
  canMark: boolean;
  windowClosed: boolean;
  startsAt: string;
  timeZoneId: string;
}) {
  const { t, i18n } = useTranslation("attendance");

  const reason = isCancelled
    ? t("roster.notice.cancelled")
    : window === "closed" || windowClosed
      ? t("roster.notice.closed")
      : window === "not_open"
        ? t("roster.notice.notOpen", { time: isolateLtr(formatTime(windowOpensAt(startsAt), timeZoneId, i18n.language)) })
        : !canMark
          ? t("roster.notice.viewOnly")
          : undefined;

  if (reason === undefined) {
    return null;
  }

  return (
    <Alert>
      <Clock aria-hidden="true" />
      <AlertDescription>{reason}</AlertDescription>
    </Alert>
  );
}
