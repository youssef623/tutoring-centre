import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups } from "@/api/generated/tutoring-centre";
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
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";

/**
 * Enrolls one student from their own page: pick a subject, then one of its active groups with their seats. It
 * calls the same enroll mutation as the roster tab (body `{ studentId }`, same invalidation, same outcomes): a 409
 * keeps the dialog open with its reason, a 404 closes it.
 */
export function EnrollInGroupDialog({
  student,
  open,
  onOpenChange,
  activeGroupIds,
}: {
  student: { id: string; fullName: string };
  open: boolean;
  onOpenChange: (open: boolean) => void;
  activeGroupIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const { enroll, isEnrolling } = useEnrollmentMutations();
  const [subjectId, setSubjectId] = useState<string | null>(null);
  const [groupId, setGroupId] = useState<string | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);
  const groupList = useListGroups(
    { subjectId: subjectId ?? undefined },
    { query: { enabled: open && subjectId !== null } },
  );
  const groups = (groupList.data?.items ?? []).filter((group) => group.status === "active");

  const close = () => {
    setSubjectId(null);
    setGroupId(null);
    setRefusal(null);
    onOpenChange(false);
  };

  const handleSubmit = async () => {
    const group = groups.find((candidate) => candidate.id === groupId);
    if (group === undefined) {
      return;
    }

    setRefusal(null);
    const outcome = await enroll({ groupId: group.id, studentId: student.id });

    if (outcome.ok) {
      toast.success(t("toasts.enrolled", { student: student.fullName, group: group.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      setGroupId(null);
    }
  };

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        if (!next) {
          close();
        }
      }}
    >
      <DialogContent className="sm:max-w-lg">
        <form
          onSubmit={(event) => {
            event.preventDefault();
            void handleSubmit();
          }}
          className="space-y-4"
        >
          <DialogHeader>
            <DialogTitle>{t("groupPicker.title")}</DialogTitle>
            <DialogDescription>{t("groupPicker.description", { student: student.fullName })}</DialogDescription>
          </DialogHeader>

          <div className="space-y-1.5">
            <Label htmlFor="enroll-subject">{t("groupPicker.subjectLabel")}</Label>
            <SubjectSelect
              id="enroll-subject"
              value={subjectId}
              onChange={(next) => {
                setSubjectId(next);
                setGroupId(null);
                setRefusal(null);
              }}
            />
          </div>

          {subjectId !== null &&
            (groupList.error ? (
              <div role="alert" className="flex flex-col items-start gap-2 rounded-lg border border-destructive/40 p-3 text-sm">
                <p>{messageFor(asApiError(groupList.error))}</p>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => {
                    void groupList.refetch();
                  }}
                >
                  {tCommon("retry")}
                </Button>
              </div>
            ) : groupList.isPending ? (
              <div role="status" aria-label={t("groupPicker.loadingLabel")} className="space-y-2">
                <Skeleton className="h-14 w-full" />
                <Skeleton className="h-14 w-full" />
              </div>
            ) : groups.length === 0 ? (
              <p className="rounded-lg border border-dashed border-border px-3 py-6 text-center text-sm text-muted-foreground">
                {t("groupPicker.empty")}
              </p>
            ) : (
              <GroupChoiceList
                groups={groups}
                value={groupId}
                onChange={(next) => {
                  setGroupId(next);
                  setRefusal(null);
                }}
                label={t("groupPicker.groupsLabel")}
                unavailableIds={activeGroupIds}
              />
            ))}

          {refusal !== null && (
            <Alert variant="destructive">
              <AlertDescription>{refusal}</AlertDescription>
            </Alert>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={close}>
              {t("form.cancel")}
            </Button>
            <Button type="submit" disabled={groupId === null || isEnrolling}>
              {isEnrolling && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("enrollDialog.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
