import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import type { StudentListItemDto } from "@/api/generated/tutoring-centre";
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
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { StudentPicker } from "@/features/enrollments/StudentPicker";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";

/**
 * Enrolls a student found by search into one group. The body is `{ studentId }` and nothing else. The roster and
 * the seat meter change only after the server answers (no optimistic row). A 409 or 422 is an ordinary outcome:
 * the dialog stays open with the reason, so the user can pick someone else or cancel; a 404 closes it.
 */
export function EnrollStudentDialog({
  group,
  open,
  onOpenChange,
  enrolledStudentIds,
}: {
  group: { id: string; name: string };
  open: boolean;
  onOpenChange: (open: boolean) => void;
  enrolledStudentIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { enroll, isEnrolling } = useEnrollmentMutations();
  const [selected, setSelected] = useState<StudentListItemDto | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);

  const close = () => {
    setSelected(null);
    setRefusal(null);
    onOpenChange(false);
  };

  const handleSubmit = async () => {
    if (selected === null) {
      return;
    }

    setRefusal(null);
    const outcome = await enroll({ groupId: group.id, studentId: selected.id });

    if (outcome.ok) {
      toast.success(t("toasts.enrolled", { student: selected.fullName, group: group.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      if (outcome.error.code === "enrollment.already_enrolled") {
        // The refetched roster now marks this student; keep nothing picked that can no longer be enrolled.
        setSelected(null);
      }
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
            <DialogTitle>{t("enrollDialog.title")}</DialogTitle>
            <DialogDescription>{t("enrollDialog.description", { group: group.name })}</DialogDescription>
          </DialogHeader>

          <StudentPicker
            selectedId={selected?.id ?? null}
            onSelect={(student) => {
              setSelected(student);
              setRefusal(null);
            }}
            unavailableIds={enrolledStudentIds}
          />

          {refusal !== null && (
            <Alert variant="destructive">
              <AlertDescription>{refusal}</AlertDescription>
            </Alert>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={close}>
              {t("form.cancel")}
            </Button>
            <Button type="submit" disabled={selected === null || isEnrolling}>
              {isEnrolling && <Loader2 className="animate-spin" aria-hidden="true" />}
              {t("enrollDialog.submit")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
