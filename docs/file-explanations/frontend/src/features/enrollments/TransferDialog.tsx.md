# `frontend/src/features/enrollments/TransferDialog.tsx`

## Purpose

Source comment (intent): Moves a student to another group of the same subject in one request, `{ targetGroupId, version }`; the client never withdraws and then enrolls. The list is advisory (other groups of this subject, minus this one and archived ones, full ones disabled); the server re-checks everything, including the subject, and its refusals are shown in the dialog after the lists have been refetched.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 203 lines and 6,700 bytes. Source hash: `ee3c0ed7abc6e8bfecfaaa78f1c3a3f7cd14e5cf4bb5b709d2e15f3ac8b21d2f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { toast } from "sonner";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { useGetGroup, useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";`
- Line 8: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import {`
- Line 18: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 19: `import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";`
- Line 20: `import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";`
- Line 21: `import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";`
- Line 22: `import type { EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L30): `export function TransferDialog({`
- [Line 111](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L111): `function TransferForm({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/GroupRosterTab.tsx:15](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L15)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:13](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L13)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:15](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetGroup, useListGroups, type GroupDto } from "@/api/generated/tutoring-centre";
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
import { Skeleton } from "@/components/ui/skeleton";
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";
import type { EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";

/**
 * Moves a student to another group of the same subject in one request, `{ targetGroupId, version }`; the client
 * never withdraws and then enrolls. The list is advisory (other groups of this subject, minus this one and archived
 * ones, full ones disabled); the server re-checks everything, including the subject, and its refusals are shown
 * in the dialog after the lists have been refetched.
 */
export function TransferDialog({
  target,
  subjectId,
  onClose,
}: {
  target: EnrollmentTarget | null;
  subjectId?: string;
  onClose: () => void;
}) {
  const { t } = useTranslation("enrollments");
  const { transfer, isTransferring } = useEnrollmentMutations();
  const [targetGroupId, setTargetGroupId] = useState<string | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);

  const close = () => {
    setTargetGroupId(null);
    setRefusal(null);
    onClose();
  };

  const handleSubmit = async (groups: GroupDto[]) => {
    const destination = groups.find((group) => group.id === targetGroupId);
    if (target === null || destination === undefined) {
      return;
    }

    setRefusal(null);
    const outcome = await transfer({
      enrollmentId: target.enrollmentId,
      version: target.version,
      sourceGroupId: target.groupId,
      targetGroupId: destination.id,
      studentId: target.studentId,
    });

    if (outcome.ok) {
      toast.success(t("toasts.transferred", { student: target.studentName, group: destination.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      if (outcome.error.code === "group.full") {
        setTargetGroupId(null);
      }
    }
  };

  return (
    <Dialog
      open={target !== null}
      onOpenChange={(next) => {
        if (!next) {
          close();
        }
      }}
    >
      <DialogContent className="sm:max-w-lg">
        {target !== null && (
          <TransferForm
```

The excerpt stops at line 90; use the original file for the remaining 113 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
