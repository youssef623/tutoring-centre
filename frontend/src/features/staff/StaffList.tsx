import { useQueryClient } from "@tanstack/react-query";
import { MoreVertical } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import {
  getListStaffQueryKey,
  useDeactivateStaff,
  useListStaff,
  useReactivateStaff,
  type StaffMemberDto,
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
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { isolateLtr } from "@/lib/bidi";

export function StaffList({ onChangeRole }: { onChangeRole: (member: StaffMemberDto) => void }) {
  const { t } = useTranslation("staff");
  const { t: tAuth } = useTranslation("auth");
  const { t: tCommon } = useTranslation("common");
  const queryClient = useQueryClient();
  const { data, error, isPending, refetch } = useListStaff();
  const reactivateMutation = useReactivateStaff();
  const deactivateMutation = useDeactivateStaff();
  const [deactivateTarget, setDeactivateTarget] = useState<StaffMemberDto | null>(null);
  const [reactivatingId, setReactivatingId] = useState<string | null>(null);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });

  const handleDeactivateConfirm = async () => {
    if (deactivateTarget === null) {
      return;
    }

    try {
      await deactivateMutation.mutateAsync({
        membershipId: deactivateTarget.membershipId,
        data: { version: deactivateTarget.version },
      });
      await invalidate();
      toast.success(t("toasts.deactivated"));
      setDeactivateTarget(null);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setDeactivateTarget(null);
    }
  };

  const handleReactivate = async (member: StaffMemberDto) => {
    setReactivatingId(member.membershipId);
    try {
      await reactivateMutation.mutateAsync({ membershipId: member.membershipId, data: { version: member.version } });
      await invalidate();
      toast.success(t("toasts.reactivated"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    } finally {
      setReactivatingId(null);
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
            <p>{t("error.reference", { correlationId: isolateLtr(apiError.correlationId) })}</p>
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
      <DataTable className="sm:min-w-[640px]">
        <DataTableHead>
          <DataTableColumn>{t("columns.name")}</DataTableColumn>
          <DataTableColumn>{t("columns.email")}</DataTableColumn>
          <DataTableColumn>{t("columns.role")}</DataTableColumn>
          <DataTableColumn hidden>{t("columns.actions")}</DataTableColumn>
        </DataTableHead>
        <DataTableBody>
          {data.items.map((member) => {
            const isInactive = member.status === "inactive";

            return (
              <DataTableRow key={member.membershipId} muted={isInactive}>
                <DataTableCell kind="primary">
                  <span className="flex flex-wrap items-center gap-2">
                    <bdi>{member.displayName}</bdi>
                    {member.isCurrentUser && <Badge variant="secondary">{t("you")}</Badge>}
                    {isInactive && <Badge variant="outline">{t("status.inactive")}</Badge>}
                  </span>
                </DataTableCell>
                <DataTableCell label={t("columns.email")} className="[overflow-wrap:anywhere]">
                  <bdi>{member.email}</bdi>
                </DataTableCell>
                <DataTableCell label={t("columns.role")}>{tAuth(`roles.${String(member.role)}`)}</DataTableCell>
                <DataTableCell kind="actions">
                  {!member.isCurrentUser && (
                    <DropdownMenu>
                      <DropdownMenuTrigger
                        render={
                          <Button type="button" variant="ghost" size="icon-sm" aria-label={t("actions.openMenu")} />
                        }
                      >
                        <MoreVertical className="size-4" aria-hidden="true" />
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        {!isInactive && (
                          <DropdownMenuItem
                            onClick={() => {
                              onChangeRole(member);
                            }}
                          >
                            {t("actions.changeRole")}
                          </DropdownMenuItem>
                        )}
                        {!isInactive && (
                          <DropdownMenuItem
                            onClick={() => {
                              setDeactivateTarget(member);
                            }}
                          >
                            {t("actions.deactivate")}
                          </DropdownMenuItem>
                        )}
                        {isInactive && (
                          <DropdownMenuItem
                            disabled={reactivatingId === member.membershipId}
                            onClick={() => {
                              void handleReactivate(member);
                            }}
                          >
                            {t("actions.reactivate")}
                          </DropdownMenuItem>
                        )}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  )}
                </DataTableCell>
              </DataTableRow>
            );
          })}
        </DataTableBody>
      </DataTable>

      <AlertDialog
        open={deactivateTarget !== null}
        onOpenChange={(open) => {
          if (!open) {
            setDeactivateTarget(null);
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle dir="auto">
              {deactivateTarget !== null && t("confirmations.deactivateTitle", { name: deactivateTarget.displayName })}
            </AlertDialogTitle>
            <AlertDialogDescription dir="auto">
              {deactivateTarget !== null && t("confirmations.deactivateBody", { name: deactivateTarget.displayName })}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{t("form.cancel")}</AlertDialogCancel>
            <AlertDialogAction
              disabled={deactivateMutation.isPending}
              onClick={() => {
                void handleDeactivateConfirm();
              }}
            >
              {t("confirmations.deactivateConfirm")}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
