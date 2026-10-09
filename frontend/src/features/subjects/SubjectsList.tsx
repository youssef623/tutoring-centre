import { useQueryClient } from "@tanstack/react-query";
import { MoreVertical } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListSubjectsQueryKey,
  useArchiveSubject,
  useListSubjects,
  useRestoreSubject,
  type SubjectDto,
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
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";

export function SubjectsList({
  includeArchived,
  onRename,
}: {
  includeArchived: boolean;
  onRename: (subject: SubjectDto) => void;
}) {
  const { t } = useTranslation("subjects");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const { data, error, isPending, refetch } = useListSubjects({ includeArchived });
  const archiveMutation = useArchiveSubject();
  const restoreMutation = useRestoreSubject();
  const [archiveTarget, setArchiveTarget] = useState<SubjectDto | null>(null);
  const [restoringId, setRestoringId] = useState<string | null>(null);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });

  const handleArchiveConfirm = async () => {
    if (archiveTarget === null) {
      return;
    }

    try {
      await archiveMutation.mutateAsync({ id: archiveTarget.id, data: { version: archiveTarget.version } });
      await invalidate();
      toast.success(t("toasts.archived"));
      setArchiveTarget(null);
    } catch (thrown) {
      await invalidate();
      showApiError(asApiError(thrown));
      setArchiveTarget(null);
    }
  };

  const handleRestore = async (subject: SubjectDto) => {
    setRestoringId(subject.id);
    try {
      await restoreMutation.mutateAsync({ id: subject.id, data: { version: subject.version } });
      await invalidate();
      toast.success(t("toasts.restored"));
    } catch (thrown) {
      await invalidate();
      showApiError(asApiError(thrown));
    } finally {
      setRestoringId(null);
    }
  };

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-2">
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("error.title")}</AlertTitle>
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
    );
  }

  if (data.items.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-border py-10 text-center">
        <p className="text-sm font-medium">{t("emptyState.title")}</p>
        <p className="text-sm text-muted-foreground">{t("emptyState.description")}</p>
      </div>
    );
  }

  return (
    <>
      <div className="overflow-x-auto rounded-xl border border-border">
        <table className="w-full min-w-[480px] text-start text-sm">
          <thead className="bg-muted/50 text-muted-foreground">
            <tr>
              <th className="px-4 py-2 text-start font-medium">{t("columns.name")}</th>
              <th className="px-4 py-2 text-start font-medium">{t("columns.status")}</th>
              <th className="px-4 py-2 text-start font-medium">
                <span className="sr-only">{t("columns.actions")}</span>
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {data.items.map((subject) => {
              const isArchived = subject.status === "archived";

              return (
                <tr key={subject.id} className={isArchived ? "text-muted-foreground" : undefined}>
                  <td className="px-4 py-2" dir="auto">
                    {subject.name}
                  </td>
                  <td className="px-4 py-2">
                    <Badge variant={isArchived ? "outline" : "default"}>
                      {t(isArchived ? "status.archived" : "status.active")}
                    </Badge>
                  </td>
                  <td className="px-4 py-2 text-end">
                    <DropdownMenu>
                      <DropdownMenuTrigger
                        render={
                          <Button type="button" variant="ghost" size="icon-sm" aria-label={t("actions.openMenu")} />
                        }
                      >
                        <MoreVertical className="size-4" aria-hidden="true" />
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        {!isArchived && (
                          <DropdownMenuItem
                            onClick={() => {
                              onRename(subject);
                            }}
                          >
                            {t("actions.rename")}
                          </DropdownMenuItem>
                        )}
                        {!isArchived && (
                          <DropdownMenuItem
                            onClick={() => {
                              setArchiveTarget(subject);
                            }}
                          >
                            {t("actions.archive")}
                          </DropdownMenuItem>
                        )}
                        {isArchived && (
                          <DropdownMenuItem
                            disabled={restoringId === subject.id}
                            onClick={() => {
                              void handleRestore(subject);
                            }}
                          >
                            {t("actions.restore")}
                          </DropdownMenuItem>
                        )}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      <AlertDialog
        open={archiveTarget !== null}
        onOpenChange={(open) => {
          if (!open) {
            setArchiveTarget(null);
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle dir="auto">
              {archiveTarget !== null && t("confirmations.archiveTitle", { name: archiveTarget.name })}
            </AlertDialogTitle>
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
    </>
  );
}
