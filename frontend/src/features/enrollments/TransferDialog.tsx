import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetGroup, useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Skeleton } from "@/components/ui/skeleton";
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";
import type { EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";

/**
 * Moves a student to another group of the same subject in one request, `{ targetGroupId, version }`; the client
 * never withdraws and then enrolls. The list is advisory (other groups of this subject, minus this one and archived
 * ones, full ones disabled); the server re-checks everything, including the subject, and its refusals are shown
 * in the dialog after the lists have been refetched.
 */
export function TransferDialog({
  target,
  subjectId,
  onClose,
}: {
  target: EnrollmentTarget | null;
  subjectId?: string;
  onClose: () => void;
}) {
  const { t } = useTranslation("enrollments");
  const { transfer, isTransferring } = useEnrollmentMutations();
  const [targetGroupId, setTargetGroupId] = useState<string | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);

  const close = () => {
    setTargetGroupId(null);
    setRefusal(null);
    onClose();
  };

  const handleSubmit = async (groups: GroupDto[]) => {
    const destination = groups.find((group) => group.id === targetGroupId);
    if (target === null || destination === undefined) {
      return;
    }

    setRefusal(null);
    const outcome = await transfer({
      enrollmentId: target.enrollmentId,
      version: target.version,
      sourceGroupId: target.groupId,
      targetGroupId: destination.id,
      studentId: target.studentId,
    });

    if (outcome.ok) {
      toast.success(t("toasts.transferred", { student: target.studentName, group: destination.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      if (outcome.error.code === "group.full") {
        setTargetGroupId(null);
      }
    }
  };

  return (
    <Dialog
      open={target !== null}
      onOpenChange={(next) => {
        if (!next) {
          close();
        }
      }}
    >
      <DialogContent className="sm:max-w-lg">
        {target !== null && (
          <TransferForm
            target={target}
            subjectId={subjectId}
            targetGroupId={targetGroupId}
            onTargetChange={(groupId) => {
              setTargetGroupId(groupId);
              setRefusal(null);
            }}
            refusal={refusal}
            isTransferring={isTransferring}
            onCancel={close}
            onSubmit={(groups) => {
              void handleSubmit(groups);
            }}
          />
        )}
      </DialogContent>
    </Dialog>
  );
}

function TransferForm({
  target,
  subjectId: knownSubjectId,
  targetGroupId,
  onTargetChange,
  refusal,
  isTransferring,
  onCancel,
  onSubmit,
}: {
  target: EnrollmentTarget;
  subjectId: string | undefined;
  targetGroupId: string | null;
  onTargetChange: (groupId: string) => void;
  refusal: string | null;
  isTransferring: boolean;
  onCancel: () => void;
  onSubmit: (groups: GroupDto[]) => void;
}) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  // The student page knows only the subject's name, so there the subject comes from the source group.
  const sourceGroup = useGetGroup(target.groupId, { query: { enabled: knownSubjectId === undefined } });
  const subjectId = knownSubjectId ?? sourceGroup.data?.subjectId;
  const groupList = useListGroups({ subjectId }, { query: { enabled: subjectId !== undefined } });
  const error = sourceGroup.error ?? groupList.error;
  const groups = (groupList.data?.items ?? []).filter(
    (group) => group.id !== target.groupId && group.status === "active",
  );

  return (
    <form
      onSubmit={(event) => {
        event.preventDefault();
        onSubmit(groups);
      }}
      className="space-y-4"
    >
      <DialogHeader>
        <DialogTitle>{t("transferDialog.title", { student: target.studentName })}</DialogTitle>
        <DialogDescription>{t("transferDialog.description", { group: target.groupName })}</DialogDescription>
      </DialogHeader>

      {error ? (
        <div role="alert" className="flex flex-col items-start gap-2 rounded-lg border border-destructive/40 p-3 text-sm">
          <p>{messageFor(asApiError(error))}</p>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => {
              void (sourceGroup.error ? sourceGroup.refetch() : groupList.refetch());
            }}
          >
            {tCommon("retry")}
          </Button>
        </div>
      ) : groupList.isPending ? (
        <div role="status" aria-label={t("transferDialog.loadingLabel")} className="space-y-2">
          <Skeleton className="h-14 w-full" />
          <Skeleton className="h-14 w-full" />
        </div>
      ) : groups.length === 0 ? (
        <p className="rounded-lg border border-dashed border-border px-3 py-6 text-center text-sm text-muted-foreground">
          {t("transferDialog.empty")}
        </p>
      ) : (
        <GroupChoiceList
          groups={groups}
          value={targetGroupId}
          onChange={onTargetChange}
          label={t("transferDialog.groupsLabel")}
        />
      )}

      {refusal !== null && (
        <Alert variant="destructive">
          <AlertDescription>{refusal}</AlertDescription>
        </Alert>
      )}

      <DialogFooter>
        <Button type="button" variant="outline" onClick={onCancel}>
          {t("form.cancel")}
        </Button>
        <Button type="submit" disabled={targetGroupId === null || isTransferring}>
          {isTransferring && <Loader2 className="animate-spin" aria-hidden="true" />}
          {t("transferDialog.submit")}
        </Button>
      </DialogFooter>
    </form>
  );
}
