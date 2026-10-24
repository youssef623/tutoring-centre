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
