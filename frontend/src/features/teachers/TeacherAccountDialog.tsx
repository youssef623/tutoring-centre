import { useQueryClient } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { Link2, Link2Off, Loader2, UserPlus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getListStaffQueryKey,
  getListTeachersQueryKey,
  useListStaff,
  useSetTeacherAccount,
  type TeacherDto,
} from "@/api/generated/tutoring-centre";
import { eligibleStaffForTeacher } from "@/features/teachers/eligibleStaff";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

/** The two refusals that are about the chosen account itself, shown under the select rather than as a toast. */
const AccountFieldCodes = new Set(["teacher.user_not_eligible", "teacher.account_taken"]);

/**
 * Links a teacher record to a staff account, or removes the link. Reuses the Staff module's own endpoint
 * (`GET /api/staff`, `staff.view`) for the picker: both that permission and `teachers.manage` are owner-only,
 * so the emails it shows reach exactly the people who can use this dialog. The list is narrowed on the client
 * to active Teacher members not already linked to another teacher — a convenience; the server still decides
 * eligibility and its refusal is shown under the select. Mounted only while open, so the staff list (and its
 * emails) is never requested otherwise.
 */
export function TeacherAccountDialog({
  teacher,
  linkedUserIds,
  onOpenChange,
}: {
  teacher: TeacherDto;
  linkedUserIds: ReadonlySet<string>;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("teachers");
  const queryClient = useQueryClient();
  const staffQuery = useListStaff();
  const mutation = useSetTeacherAccount();
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [fieldError, setFieldError] = useState<string | null>(null);

  const eligible = eligibleStaffForTeacher(staffQuery.data?.items ?? [], teacher.userId, linkedUserIds);
  const selected = eligible.find((member) => member.userId === selectedUserId);

  const send = async (userId: string | null) => {
    setFieldError(null);
    try {
      await mutation.mutateAsync({ id: teacher.id, data: { userId, version: teacher.version } });
      await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
      toast.success(t(userId === null ? "toasts.accountUnlinked" : "toasts.accountLinked"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);

      if (AccountFieldCodes.has(apiError.code)) {
        setFieldError(messageFor(apiError));
        setSelectedUserId(null);
        await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });
        return;
      }

      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });
      onOpenChange(false);
    }
  };

  return (
    <Dialog
      open
      onOpenChange={(next) => {
        if (!next) {
          onOpenChange(false);
        }
      }}
    >
      <DialogContent>
        <DialogHeader>
          <DialogTitle dir="auto">{t("account.title", { name: teacher.fullName })}</DialogTitle>
          <DialogDescription>{t("account.effect")}</DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <div className="flex items-center justify-between gap-3 rounded-lg border border-border bg-muted/40 p-3">
            <div className="flex min-w-0 items-center gap-2.5">
              {teacher.userId === null ? (
                <Link2Off className="size-4 shrink-0 text-muted-foreground" aria-hidden="true" />
              ) : (
                <Link2 className="size-4 shrink-0 text-primary" aria-hidden="true" />
              )}
              <div className="min-w-0">
                <p className="text-xs text-muted-foreground">{t("account.currentLabel")}</p>
                <p className="truncate text-sm font-medium" dir="auto">
                  {teacher.userDisplayName ?? t("account.none")}
                </p>
              </div>
            </div>
            {teacher.userId !== null && (
              <Button
                type="button"
                variant="outline"
                size="sm"
                disabled={mutation.isPending}
                onClick={() => {
                  void send(null);
                }}
              >
                {t("account.remove")}
              </Button>
            )}
          </div>

          {staffQuery.isPending ? (
            <div role="status" aria-label={t("account.loading")} className="space-y-2">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-9 w-full" />
            </div>
          ) : staffQuery.error ? (
            <Alert variant="destructive">
              <AlertDescription className="flex flex-col gap-2">
                <p>{messageFor(asApiError(staffQuery.error))}</p>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  className="self-start"
                  onClick={() => {
                    void staffQuery.refetch();
                  }}
                >
                  {t("account.retry")}
                </Button>
              </AlertDescription>
            </Alert>
          ) : eligible.length === 0 ? (
            <div className="flex flex-col items-center gap-2 rounded-lg border border-dashed border-border p-5 text-center">
              <UserPlus className="size-5 text-muted-foreground" aria-hidden="true" />
              <p className="text-sm font-medium">{t("account.noEligibleTitle")}</p>
              <p className="text-sm text-muted-foreground">{t("account.noEligibleDescription")}</p>
              <Link to="/staff" className={buttonVariants({ variant: "outline", size: "sm" })}>
                {t("account.goToStaff")}
              </Link>
              {fieldError !== null && (
                <p role="alert" className="text-sm text-destructive">
                  {fieldError}
                </p>
              )}
            </div>
          ) : (
            <div className="space-y-1.5">
              <Label htmlFor="teacher-account-user">{t("account.selectLabel")}</Label>
              <Select
                value={selectedUserId}
                onValueChange={(value) => {
                  setSelectedUserId(value);
                  setFieldError(null);
                }}
                disabled={mutation.isPending}
              >
                <SelectTrigger
                  id="teacher-account-user"
                  className="w-full"
                  aria-invalid={fieldError !== null}
                  aria-describedby={fieldError !== null ? "teacher-account-user-error" : undefined}
                >
                  <SelectValue placeholder={t("account.selectPlaceholder")}>
                    {selected === undefined ? null : <span dir="auto">{selected.displayName}</span>}
                  </SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {eligible.map((member) => (
                    <SelectItem key={member.userId} value={member.userId}>
                      <span className="flex flex-col items-start">
                        <span dir="auto">{member.displayName}</span>
                        <span className="text-xs text-muted-foreground" dir="ltr">
                          {member.email}
                        </span>
                      </span>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {fieldError !== null && (
                <p id="teacher-account-user-error" role="alert" className="text-sm text-destructive">
                  {fieldError}
                </p>
              )}
            </div>
          )}
        </div>

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              onOpenChange(false);
            }}
          >
            {t("form.cancel")}
          </Button>
          <Button
            type="button"
            disabled={selectedUserId === null || mutation.isPending}
            onClick={() => {
              if (selectedUserId !== null) {
                void send(selectedUserId);
              }
            }}
          >
            {mutation.isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
            {t("account.link")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
