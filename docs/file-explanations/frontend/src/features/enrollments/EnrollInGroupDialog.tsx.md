# `frontend/src/features/enrollments/EnrollInGroupDialog.tsx`

## Purpose

Source comment (intent): Enrolls one student from their own page: pick a subject, then one of its active groups with their seats. It calls the same enroll mutation as the roster tab (body `{ studentId }`, same invalidation, same outcomes): a 409 keeps the dialog open with its reason, a 404 closes it.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 172 lines and 5,922 bytes. Source hash: `c4bcb9177fc1c706dffa0512f40f0a7439e323b584705b50d953210ddf4c29f0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { toast } from "sonner";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { useListGroups } from "@/api/generated/tutoring-centre";`
- Line 8: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import {`
- Line 18: `import { Label } from "@/components/ui/label";`
- Line 19: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 20: `import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";`
- Line 21: `import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";`
- Line 22: `import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";`
- Line 23: `import { SubjectSelect } from "@/features/subjects/SubjectSelect";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 30](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L30): `export function EnrollInGroupDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:11](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListGroups } from "@/api/generated/tutoring-centre";
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
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { GroupChoiceList } from "@/features/enrollments/GroupChoiceList";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";

/**
 * Enrolls one student from their own page: pick a subject, then one of its active groups with their seats. It
 * calls the same enroll mutation as the roster tab (body `{ studentId }`, same invalidation, same outcomes): a 409
 * keeps the dialog open with its reason, a 404 closes it.
 */
export function EnrollInGroupDialog({
  student,
  open,
  onOpenChange,
  activeGroupIds,
}: {
  student: { id: string; fullName: string };
  open: boolean;
  onOpenChange: (open: boolean) => void;
  activeGroupIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const { enroll, isEnrolling } = useEnrollmentMutations();
  const [subjectId, setSubjectId] = useState<string | null>(null);
  const [groupId, setGroupId] = useState<string | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);
  const groupList = useListGroups(
    { subjectId: subjectId ?? undefined },
    { query: { enabled: open && subjectId !== null } },
  );
  const groups = (groupList.data?.items ?? []).filter((group) => group.status === "active");

  const close = () => {
    setSubjectId(null);
    setGroupId(null);
    setRefusal(null);
    onOpenChange(false);
  };

  const handleSubmit = async () => {
    const group = groups.find((candidate) => candidate.id === groupId);
    if (group === undefined) {
      return;
    }

    setRefusal(null);
    const outcome = await enroll({ groupId: group.id, studentId: student.id });

    if (outcome.ok) {
      toast.success(t("toasts.enrolled", { student: student.fullName, group: group.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      setGroupId(null);
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
```

The excerpt stops at line 90; use the original file for the remaining 82 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
