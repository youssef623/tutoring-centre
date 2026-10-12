import { useQueryClient } from "@tanstack/react-query";
import { Link, createFileRoute } from "@tanstack/react-router";
import { SearchX } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getGetStudentQueryKey,
  getListStudentsQueryKey,
  useArchiveStudent,
  useGetStudent,
  useRestoreStudent,
  type StudentDetailDto,
} from "@/api/generated/tutoring-centre";
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
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { ParentsCard } from "@/features/students/ParentsCard";
import { StudentEditDialog } from "@/features/students/StudentEditDialog";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { RouteGuard } from "@/features/session/RouteGuard";
import { useCan } from "@/features/session/useCan";

export const Route = createFileRoute("/_authenticated/students/$studentId")({
  component: StudentDetailPage,
});

function StudentDetailPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <StudentDetailPageContent />
    </RouteGuard>
  );
}

function StudentDetailPageContent() {
  const { studentId } = Route.useParams();
  const { t } = useTranslation("students");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetStudent(studentId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="max-w-md space-y-3">
          <Skeleton className="h-8 w-48" />
          <Skeleton className="h-32 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.kind === "notFound") {
      return <StudentNotFound />;
    }

    return (
      <section className="space-y-4">
        <Alert variant="destructive">
          <AlertTitle>{t("detailError.title")}</AlertTitle>
          <AlertDescription className="flex flex-col gap-2">
            <p>{messageFor(apiError)}</p>
            {apiError.correlationId !== undefined && (
              <p>{t("error.reference", { correlationId: apiError.correlationId })}</p>
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
      </section>
    );
  }

  return <StudentDetailContent student={data} />;
}

/** Also what a foreign student id (another centre's) renders: the read service never reveals that it exists. */
function StudentNotFound() {
  const { t } = useTranslation("students");

  return (
    <div className="flex flex-col items-center gap-3 py-16 text-center">
      <SearchX className="size-10 text-muted-foreground" aria-hidden="true" />
      <h1 className="text-xl font-semibold tracking-tight">{t("notFound.title")}</h1>
      <p className="max-w-sm text-sm text-muted-foreground">{t("notFound.description")}</p>
      <Button render={<Link to="/students" />}>{t("notFound.backToList")}</Button>
    </div>
  );
}

function StudentDetailContent({ student }: { student: StudentDetailDto }) {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const canManage = useCan(Permissions.StudentsManage);
  const archiveMutation = useArchiveStudent();
  const restoreMutation = useRestoreStudent();
  const [archiveConfirmOpen, setArchiveConfirmOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);

  const isArchived = student.status === "archived";

  const invalidate = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() }),
    ]);

  const handleArchiveConfirm = async () => {
    try {
      await archiveMutation.mutateAsync({ id: student.id, data: { version: student.version } });
      await invalidate();
      toast.success(t("toasts.archived"));
      setArchiveConfirmOpen(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setArchiveConfirmOpen(false);
    }
  };

  const handleRestore = async () => {
    try {
      await restoreMutation.mutateAsync({ id: student.id, data: { version: student.version } });
      await invalidate();
      toast.success(t("toasts.restored"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  };

  return (
    <section className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight" dir="auto">
          {student.fullName}
        </h1>
        {canManage && (
          <div className="flex items-center gap-2">
            {isArchived ? (
              <Button
                type="button"
                variant="outline"
                disabled={restoreMutation.isPending}
                onClick={() => {
                  void handleRestore();
                }}
              >
                {t("actions.restore")}
              </Button>
            ) : (
              <>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setEditOpen(true);
                  }}
                >
                  {t("actions.edit")}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setArchiveConfirmOpen(true);
                  }}
                >
                  {t("actions.archive")}
                </Button>
              </>
            )}
          </div>
        )}
      </div>

      {isArchived && (
        <Alert>
          <AlertTitle>{t("archivedBanner.title")}</AlertTitle>
          <AlertDescription>{t("archivedBanner.description")}</AlertDescription>
        </Alert>
      )}

      <div className="max-w-md space-y-3 rounded-xl border border-border p-4">
        <div className="flex items-center justify-between gap-4">
          <span className="text-sm text-muted-foreground">{t("columns.status")}</span>
          <StatusBadge status={student.status} />
        </div>
        <div className="flex items-center justify-between gap-4">
          <span className="text-sm text-muted-foreground">{t("detail.phoneLabel")}</span>
          {student.phone === null ? <span>—</span> : <PhoneText value={student.phone} />}
        </div>
        <div className="flex items-center justify-between gap-4">
          <span className="text-sm text-muted-foreground">{t("columns.schoolYear")}</span>
          <span dir="auto">{student.schoolYear ?? "—"}</span>
        </div>
      </div>

      <ParentsCard student={student} canManage={canManage && !isArchived} />

      {canManage && <StudentEditDialog student={student} open={editOpen} onOpenChange={setEditOpen} />}

      <AlertDialog open={archiveConfirmOpen} onOpenChange={setArchiveConfirmOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle dir="auto">{t("confirmations.archiveTitle", { name: student.fullName })}</AlertDialogTitle>
            <AlertDialogDescription>{t("confirmations.archiveBody")}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{t("form.cancel")}</AlertDialogCancel>
            <AlertDialogAction
              disabled={archiveMutation.isPending}
              onClick={() => {
                void handleArchiveConfirm();
              }}
            >
              {t("confirmations.archiveConfirm")}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </section>
  );
}
