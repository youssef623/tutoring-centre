# `frontend/src/features/schedule/CancelSessionDialog.tsx`

## Purpose

Source comment (intent): Confirms cancelling one session, naming its group, date and time (in the centre's zone). Cancel, never delete: the session stays on the timetable, marked. The body carries only the session's version. A session that has started, that someone else cancelled first, or a stale version is a toast after which the timetable refetches.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 86 lines and 2,796 bytes. Source hash: `302cb038964cf9b1a6c36fa8223e4b5bd2bf93531825a08a601e0ebdf487a09d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { toast } from "sonner";`
- Line 3: `import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";`
- Line 4: `import {`
- Line 14: `import { formatTime } from "@/features/schedule/centreTime";`
- Line 15: `import { scheduleMessageFor } from "@/features/schedule/scheduleMessages";`
- Line 16: `import { useScheduleMutations } from "@/features/schedule/useScheduleMutations";`
- Line 17: `import { formatCalendarDate } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L24): `export function CancelSessionDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetablePage.tsx:15](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
