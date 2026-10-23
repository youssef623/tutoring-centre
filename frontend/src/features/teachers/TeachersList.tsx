import { useQueryClient } from "@tanstack/react-query";
import { Link2, MoreVertical, Plus, UserCheck, Users, UsersRound } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListTeachersQueryKey,
  useArchiveTeacher,
  useListTeachers,
  useRestoreTeacher,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import { EmptyState } from "@/components/shared/EmptyState";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatTile } from "@/components/shared/StatTile";
import { StatusBadge } from "@/components/shared/StatusBadge";
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
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { useCan } from "@/features/session/useCan";
import { TeacherAccountDialog } from "@/features/teachers/TeacherAccountDialog";
import { initialsOf } from "@/lib/initials";

/**
 * The teacher directory: summary tiles, then the table. Every manage affordance (row menu, empty-state button)
 * sits behind `teachers.manage`, so a secretary — who holds `teachers.view` only — gets the same page read-only.
 */
export function TeachersList({
  includeArchived,
  onCreate,
  onEdit,
}: {
  includeArchived: boolean;
  onCreate: () => void;
  onEdit: (teacher: TeacherDto) => void;
}) {
  const { t } = useTranslation("teachers");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const canManage = useCan(Permissions.TeachersManage);
  const { data, error, isPending, refetch } = useListTeachers({ includeArchived });
  const archiveMutation = useArchiveTeacher();
  const restoreMutation = useRestoreTeacher();
  const [archiveTarget, setArchiveTarget] = useState<TeacherDto | null>(null);
  const [accountTarget, setAccountTarget] = useState<TeacherDto | null>(null);
  const [restoringId, setRestoringId] = useState<string | null>(null);

  const linkedUserIds = useMemo(
    () => new Set((data?.items ?? []).flatMap((teacher) => (teacher.userId === null ? [] : [teacher.userId]))),
    [data],
  );

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });

  const handleArchiveConfirm = async () => {
    if (archiveTarget === null) {
      return;
    }

    try {
      await archiveMutation.mutateAsync({ id: archiveTarget.id, data: { version: archiveTarget.version } });
      toast.success(t("toasts.archived"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      // teacher.has_active_groups has its own translated message: reassign or archive the groups first.
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    } finally {
      await invalidate();
      setArchiveTarget(null);
    }
  };

  const handleRestore = async (teacher: TeacherDto) => {
    setRestoringId(teacher.id);
    try {
      await restoreMutation.mutateAsync({ id: teacher.id, data: { version: teacher.version } });
      toast.success(t("toasts.restored"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    } finally {
      await invalidate();
      setRestoringId(null);
    }
  };

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-4">
        <div className="grid gap-3 sm:grid-cols-3">
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
          <Skeleton className="h-20 w-full" />
        </div>
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-12 w-full" />
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
            <p>
              {t("error.reference")} <bdi>{apiError.correlationId}</bdi>
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

  if (data.items.length === 0) {
    return (
      <EmptyState
        icon={UsersRound}
        title={t("emptyState.title")}
        description={t(canManage ? "emptyState.description" : "emptyState.readOnlyDescription")}
        action={
          <Can permission={Permissions.TeachersManage}>
            <Button type="button" onClick={onCreate}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />
    );
  }

  const active = data.items.filter((teacher) => teacher.status === "active");
  const linkedCount = active.filter((teacher) => teacher.userId !== null).length;
  const groupCount = active.reduce((sum, teacher) => sum + Number(teacher.activeGroupCount), 0);

  return (
    <div className="space-y-4">
      <div className="grid gap-3 sm:grid-cols-3">
        <StatTile icon={Users} label={t("stats.activeTeachers")} value={active.length} />
        <StatTile icon={UserCheck} label={t("stats.linkedAccounts")} value={linkedCount} />
        <StatTile icon={UsersRound} label={t("stats.activeGroups")} value={groupCount} />
      </div>

      <div className="overflow-x-auto rounded-xl border border-border bg-card">
        <table className="w-full min-w-[720px] text-sm">
          <thead className="bg-muted/50 text-muted-foreground">
            <tr>
              <th className="px-4 py-2.5 text-start font-medium">{t("columns.name")}</th>
              <th className="px-4 py-2.5 text-start font-medium">{t("columns.phone")}</th>
              <th className="px-4 py-2.5 text-start font-medium">{t("columns.account")}</th>
              <th className="px-4 py-2.5 text-start font-medium">{t("columns.activeGroups")}</th>
              <th className="px-4 py-2.5 text-start font-medium">{t("columns.status")}</th>
              {canManage && (
                <th className="px-4 py-2.5 text-end font-medium">
                  <span className="sr-only">{t("columns.actions")}</span>
                </th>
              )}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {data.items.map((teacher) => {
              const isArchived = teacher.status === "archived";
              const groups = Number(teacher.activeGroupCount);

              return (
                <tr key={teacher.id} className={isArchived ? "text-muted-foreground" : undefined}>
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-3">
                      <span
                        className="flex size-8 shrink-0 items-center justify-center rounded-full bg-primary/10 text-xs font-semibold text-primary"
                        aria-hidden="true"
                      >
                        {initialsOf(teacher.fullName)}
                      </span>
                      <span className="font-medium" dir="auto">
                        {teacher.fullName}
                      </span>
                    </div>
                  </td>
                  <td className="px-4 py-3">{teacher.phone === null ? "—" : <PhoneText value={teacher.phone} />}</td>
                  <td className="px-4 py-3">
                    {teacher.userDisplayName === null ? (
                      <span className="text-muted-foreground">{t("account.none")}</span>
                    ) : (
                      <Badge variant="secondary" className="gap-1">
                        <Link2 className="size-3" aria-hidden="true" />
                        <span dir="auto">{teacher.userDisplayName}</span>
                      </Badge>
                    )}
                  </td>
                  <td className="px-4 py-3">{t("columns.activeGroupsCount", { count: groups, countText: String(groups) })}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={teacher.status} />
                  </td>
                  {canManage && (
                    <td className="px-4 py-3 text-end">
                      <DropdownMenu>
                        <DropdownMenuTrigger
                          render={
                            <Button
                              type="button"
                              variant="ghost"
                              size="icon-sm"
                              aria-label={t("actions.openMenu", { name: teacher.fullName })}
                            />
                          }
                        >
                          <MoreVertical className="size-4" aria-hidden="true" />
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          {isArchived ? (
                            <DropdownMenuItem
                              disabled={restoringId === teacher.id}
                              onClick={() => {
                                void handleRestore(teacher);
                              }}
                            >
                              {t("actions.restore")}
                            </DropdownMenuItem>
                          ) : (
                            <>
                              <DropdownMenuItem
                                onClick={() => {
                                  onEdit(teacher);
                                }}
                              >
                                {t("actions.edit")}
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => {
                                  setAccountTarget(teacher);
                                }}
                              >
                                {t("actions.account")}
                              </DropdownMenuItem>
                              <DropdownMenuSeparator />
                              <DropdownMenuItem
                                variant="destructive"
                                onClick={() => {
                                  setArchiveTarget(teacher);
                                }}
                              >
                                {t("actions.archive")}
                              </DropdownMenuItem>
                            </>
                          )}
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </td>
                  )}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {accountTarget !== null && (
        <TeacherAccountDialog
          teacher={accountTarget}
          linkedUserIds={linkedUserIds}
          onOpenChange={(open) => {
            if (!open) {
              setAccountTarget(null);
            }
          }}
        />
      )}

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
              {archiveTarget !== null && t("confirmations.archiveTitle", { name: archiveTarget.fullName })}
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
    </div>
  );
}
