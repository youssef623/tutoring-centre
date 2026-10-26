import { useQueryClient } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { showApiError } from "@/api/showApiError";
import {
  getListStaffQueryKey,
  useChangeStaffRole,
  StaffRole,
  type StaffMemberDto,
} from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

export function ChangeRoleDialog({
  member,
  onOpenChange,
}: {
  member: StaffMemberDto | null;
  onOpenChange: (open: boolean) => void;
}) {
  const { t } = useTranslation("staff");
  const { t: tAuth } = useTranslation("auth");
  const queryClient = useQueryClient();
  const mutation = useChangeStaffRole();
  const [role, setRole] = useState<StaffMemberDto["role"]>(StaffRole.teacher);

  // Adjusting state during rendering (not inside an effect) when the target member changes, per
  // https://react.dev/learn/you-might-not-need-an-effect#adjusting-some-state-when-a-prop-changes — this
  // component stays mounted for the dialog's whole lifetime (only `open` toggles), so a plain lazy initial
  // state would only ever see the very first member.
  const [lastMembershipId, setLastMembershipId] = useState<string | null>(null);
  if (member !== null && member.membershipId !== lastMembershipId) {
    setLastMembershipId(member.membershipId);
    setRole(member.role);
  }

  const invalidate = () => queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });

  const handleSave = async () => {
    if (member === null || role === null) {
      return;
    }

    try {
      await mutation.mutateAsync({ membershipId: member.membershipId, data: { role, version: member.version } });
      await invalidate();
      toast.success(t("toasts.roleChanged"));
      onOpenChange(false);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);

      if (apiError.code === "concurrency.stale") {
        await invalidate();
        onOpenChange(false);
      }
    }
  };

  const isUnchanged = member !== null && role === member.role;

  return (
    <Dialog open={member !== null} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle dir="auto">
            {member !== null && t("dialogs.changeRoleTitle", { name: member.displayName })}
          </DialogTitle>
          <DialogDescription>{t("form.changeRoleConsequence")}</DialogDescription>
        </DialogHeader>
        <div className="space-y-1.5 py-2">
          <Label htmlFor="change-role-select">{t("form.roleLabel")}</Label>
          <Select
            value={role ?? undefined}
            onValueChange={(value) => {
              setRole(value);
            }}
            disabled={mutation.isPending}
          >
            <SelectTrigger id="change-role-select" className="w-full">
              <SelectValue>{(value: StaffMemberDto["role"]) => (value === null ? "" : tAuth(`roles.${value}`))}</SelectValue>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={StaffRole.teacher}>{tAuth("roles.teacher")}</SelectItem>
              <SelectItem value={StaffRole.secretary}>{tAuth("roles.secretary")}</SelectItem>
              <SelectItem value={StaffRole.owner}>{tAuth("roles.owner")}</SelectItem>
            </SelectContent>
          </Select>
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
            disabled={isUnchanged || mutation.isPending}
            onClick={() => {
              void handleSave();
            }}
          >
            {mutation.isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
            {t("form.save")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
