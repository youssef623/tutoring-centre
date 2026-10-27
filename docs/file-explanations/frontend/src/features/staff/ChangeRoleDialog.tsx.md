# `frontend/src/features/staff/ChangeRoleDialog.tsx`

## Purpose

Declares `ChangeRoleDialog`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 128 lines and 4,545 bytes. Source hash: `97692c5c56d34ed3603ef991cfe7430d9d0b7c0454ff1e7b246584f63b9b2edc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Loader2 } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { toast } from "sonner";`
- Line 6: `import { asApiError } from "@/api/apiFetch";`
- Line 7: `import { showApiError } from "@/api/showApiError";`
- Line 8: `import {`
- Line 14: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 15: `import { Button } from "@/components/ui/button";`
- Line 16: `import {`
- Line 24: `import { Label } from "@/components/ui/label";`
- Line 25: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 27](../../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L27): `export function ChangeRoleDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/staff.tsx:5](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 38 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
