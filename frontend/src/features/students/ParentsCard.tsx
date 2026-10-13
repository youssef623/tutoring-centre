import { useQueryClient } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { MoreVertical, Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { showApiError } from "@/api/showApiError";
import {
  getGetParentQueryKey,
  getGetStudentQueryKey,
  useMakeStudentParentPrimary,
  useUnlinkStudentParent,
  type StudentDetailDto,
  type StudentParentDto,
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
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { PhoneText } from "@/components/shared/PhoneText";
import { AddParentDialog } from "@/features/students/AddParentDialog";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

const MaxParents = 4;

/**
 * Renders the aggregate's rules as affordances only — no Unlink on the only or the primary parent, no
 * "Make primary" on the one already primary, no "Add parent" at four — while the server still enforces every
 * one of them. A direct request that bypasses these (browser console, another client) still gets 422; this
 * card only decides what's convenient to offer, never what's allowed.
 */
export function ParentsCard({ student, canManage }: { student: StudentDetailDto; canManage: boolean }) {
  const { t } = useTranslation("students");
  const queryClient = useQueryClient();
  const makePrimaryMutation = useMakeStudentParentPrimary();
  const unlinkMutation = useUnlinkStudentParent();
  const [addOpen, setAddOpen] = useState(false);
  const [unlinkTarget, setUnlinkTarget] = useState<StudentParentDto | null>(null);

  const sortedParents = [...student.parents].sort((a, b) => Number(b.isPrimary) - Number(a.isPrimary));
  const isOnlyParent = student.parents.length === 1;
  const canAddMore = student.parents.length < MaxParents;

  const invalidate = (parentId: string) =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),
      queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),
    ]);

  const handleMakePrimary = async (parent: StudentParentDto) => {
    try {
      await makePrimaryMutation.mutateAsync({
        id: student.id,
        parentId: parent.parentId,
        data: { version: student.version },
      });
      await invalidate(parent.parentId);
      toast.success(t("toasts.parentMadePrimary"));
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
    }
  };

  const handleUnlinkConfirm = async () => {
    if (unlinkTarget === null) {
      return;
    }

    try {
      await unlinkMutation.mutateAsync({
        id: student.id,
        parentId: unlinkTarget.parentId,
        data: { version: student.version },
      });
      await invalidate(unlinkTarget.parentId);
      toast.success(t("toasts.parentUnlinked"));
      setUnlinkTarget(null);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setUnlinkTarget(null);
    }
  };

  return (
    <div className="max-w-md space-y-3">
      <div className="flex items-center justify-between gap-3">
        <h2 className="text-lg font-semibold tracking-tight">{t("parentsCard.title")}</h2>
        {canManage && canAddMore && (
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => {
              setAddOpen(true);
            }}
          >
            <Plus className="size-4" aria-hidden="true" />
            {t("actions.addParent")}
          </Button>
        )}
      </div>

      <div className="divide-y divide-border rounded-xl border border-border">
        {sortedParents.map((parent) => (
          <div key={parent.parentId} className="flex items-center justify-between gap-3 px-4 py-3">
            <div className="min-w-0 space-y-0.5">
              <div className="flex items-center gap-2">
                <Link
                  to="/parents/$parentId"
                  params={{ parentId: parent.parentId }}
                  className="font-medium hover:underline"
                  dir="auto"
                >
                  {parent.fullName}
                </Link>
                {parent.isPrimary && <Badge variant="outline">{t("parentsCard.primaryBadge")}</Badge>}
              </div>
              <p className="text-sm text-muted-foreground" dir="auto">
                {t(`relationship.${parent.relationship}`)}
              </p>
            </div>
            <div className="flex shrink-0 items-center gap-2">
              <PhoneText value={parent.phone} />
              {canManage && (
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={
                      <Button type="button" variant="ghost" size="icon-sm" aria-label={t("parentsCard.openMenu")} />
                    }
                  >
                    <MoreVertical className="size-4" aria-hidden="true" />
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    {!parent.isPrimary && (
                      <DropdownMenuItem
                        onClick={() => {
                          void handleMakePrimary(parent);
                        }}
                      >
                        {t("actions.makePrimary")}
                      </DropdownMenuItem>
                    )}
                    {!parent.isPrimary && !isOnlyParent && (
                      <DropdownMenuItem
                        onClick={() => {
                          setUnlinkTarget(parent);
                        }}
                      >
                        {t("actions.unlink")}
                      </DropdownMenuItem>
                    )}
                  </DropdownMenuContent>
                </DropdownMenu>
              )}
            </div>
          </div>
        ))}
      </div>

      {canManage && <AddParentDialog student={student} open={addOpen} onOpenChange={setAddOpen} />}

      <AlertDialog
        open={unlinkTarget !== null}
        onOpenChange={(open) => {
          if (!open) {
            setUnlinkTarget(null);
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle dir="auto">
              {unlinkTarget !== null && t("confirmations.unlinkTitle", { name: unlinkTarget.fullName })}
            </AlertDialogTitle>
            <AlertDialogDescription>{t("confirmations.unlinkBody")}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{t("form.cancel")}</AlertDialogCancel>
            <AlertDialogAction
              disabled={unlinkMutation.isPending}
              onClick={() => {
                void handleUnlinkConfirm();
              }}
            >
              {t("confirmations.unlinkConfirm")}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
