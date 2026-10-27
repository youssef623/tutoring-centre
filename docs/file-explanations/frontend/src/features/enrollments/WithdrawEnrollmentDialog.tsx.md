# `frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx`

## Purpose

Source comment (intent): Confirms a withdrawal, naming the student and the group. Withdraw, never delete: the student leaves the roster and the ended enrollment stays in their history. The body carries only the row's version. A stale version or an enrollment someone else already ended is a toast, after which the invalidated lists show the current state.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 87 lines and 2,605 bytes. Source hash: `954ff7f523ac24e6058b649755fb91ae854876bcc78539101d90154aba045286`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { toast } from "sonner";`
- Line 3: `import {`
- Line 13: `import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";`
- Line 14: `import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L16): `export interface EnrollmentTarget {`
- [Line 30](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L30): `export function WithdrawEnrollmentDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:16](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L16)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:14](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L14)
- [frontend/src/features/enrollments/TransferDialog.tsx:22](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
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
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";

export interface EnrollmentTarget {
  enrollmentId: string;
  version: number | string;
  studentId: string;
  studentName: string;
  groupId: string;
  groupName: string;
}

/**
 * Confirms a withdrawal, naming the student and the group. Withdraw, never delete: the student leaves the roster
 * and the ended enrollment stays in their history. The body carries only the row's version. A stale version or an
 * enrollment someone else already ended is a toast, after which the invalidated lists show the current state.
 */
export function WithdrawEnrollmentDialog({
  target,
  onClose,
}: {
  target: EnrollmentTarget | null;
  onClose: () => void;
}) {
  const { t } = useTranslation("enrollments");
  const { withdraw, isWithdrawing } = useEnrollmentMutations();

  const handleConfirm = async () => {
    if (target === null) {
      return;
    }

    const outcome = await withdraw(target);
    if (outcome.ok) {
      toast.success(t("toasts.withdrawn", { student: target.studentName, group: target.groupName }));
    } else {
      toast.error(enrollmentMessageFor(outcome.error));
    }
    onClose();
  };

  return (
    <AlertDialog
      open={target !== null}
      onOpenChange={(open) => {
        if (!open) {
          onClose();
        }
      }}
    >
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            {target !== null && t("withdrawDialog.title", { student: target.studentName, group: target.groupName })}
          </AlertDialogTitle>
          <AlertDialogDescription>
            {target !== null && t("withdrawDialog.body", { student: target.studentName })}
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>{t("form.cancel")}</AlertDialogCancel>
          <AlertDialogAction
            variant="destructive"
            disabled={isWithdrawing}
            onClick={() => {
              void handleConfirm();
            }}
          >
            {t("withdrawDialog.confirm")}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
