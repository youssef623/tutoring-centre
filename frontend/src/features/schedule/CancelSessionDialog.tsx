import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";
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
import { formatTime } from "@/features/schedule/centreTime";
import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";
import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";
import { formatCalendarDate } from "@/features/schedule/week";

/**
 * Confirms cancelling one session, naming its group, date and time (in the centre's zone). Cancel, never delete:
 * the session stays on the timetable, marked. The body carries only the session's version. A session that has
 * started, that someone else cancelled first, or a stale version is a toast after which the timetable refetches.
 */
export function CancelSessionDialog({
  session,
  timeZoneId,
  onClose,
}: {
  session: TimetableSessionDto | null;
  timeZoneId: string;
  onClose: () => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const { cancelSession, isCancelling } = useScheduleMutations();

  const handleConfirm = async () => {
    if (session === null) {
      return;
    }

    const outcome = await cancelSession({ sessionId: session.id, version: session.version });
    if (outcome.ok) {
      toast.success(t("toasts.cancelled"));
    } else {
      toast.error(scheduleMessageFor(outcome.error));
    }
    onClose();
  };

  return (
    <AlertDialog
      open={session !== null}
      onOpenChange={(open) => {
        if (!open) {
          onClose();
        }
      }}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            {session !== null &&
              t("cancelDialog.title", {
                group: session.groupName,
                date: formatCalendarDate(session.localDate, i18n.language, { weekday: "long", day: "numeric", month: "long" }),
                time: formatTime(session.startsAt, timeZoneId, i18n.language),
              })}
          </AlertDialogTitle>
          <AlertDialogDescription>{t("cancelDialog.body")}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>{t("cancelDialog.keep")}</AlertDialogCancel>
          <AlertDialogAction
            variant="destructive"
            disabled={isCancelling}
            onClick={() => {
              void handleConfirm();
            }}
          >
            {t("cancelDialog.confirm")}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
