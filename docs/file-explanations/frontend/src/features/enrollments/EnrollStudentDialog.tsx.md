# `frontend/src/features/enrollments/EnrollStudentDialog.tsx`

## Purpose

Source comment (intent): Enrolls a student found by search into one group. The body is `{ studentId }` and nothing else. The roster and the seat meter change only after the server answers (no optimistic row). A 409 or 422 is an ordinary outcome: the dialog stays open with the reason, so the user can pick someone else or cancel; a 404 closes it.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 3,851 bytes. Source hash: `f5fd3a16394c2faa1bc7aeeae24ae56989dba59427b568853e1aa79f3e6f46bf`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { toast } from "sonner";`
- Line 5: `import type { StudentListItemDto } from "@/api/generated/tutoring-centre";`
- Line 6: `import { Alert, AlertDescription } from "@/components/ui/alert";`
- Line 7: `import { Button } from "@/components/ui/button";`
- Line 8: `import {`
- Line 16: `import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";`
- Line 17: `import { StudentPicker } from "@/features/enrollments/StudentPicker";`
- Line 18: `import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L25): `export function EnrollStudentDialog({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:12](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L12)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:12](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import type { StudentListItemDto } from "@/api/generated/tutoring-centre";
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
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { StudentPicker } from "@/features/enrollments/StudentPicker";
import { useEnrollmentMutations } from "@/features/enrollments/useEnrollmentMutations";

/**
 * Enrolls a student found by search into one group. The body is `{ studentId }` and nothing else. The roster and
 * the seat meter change only after the server answers (no optimistic row). A 409 or 422 is an ordinary outcome:
 * the dialog stays open with the reason, so the user can pick someone else or cancel; a 404 closes it.
 */
export function EnrollStudentDialog({
  group,
  open,
  onOpenChange,
  enrolledStudentIds,
}: {
  group: { id: string; name: string };
  open: boolean;
  onOpenChange: (open: boolean) => void;
  enrolledStudentIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { enroll, isEnrolling } = useEnrollmentMutations();
  const [selected, setSelected] = useState<StudentListItemDto | null>(null);
  const [refusal, setRefusal] = useState<string | null>(null);

  const close = () => {
    setSelected(null);
    setRefusal(null);
    onOpenChange(false);
  };

  const handleSubmit = async () => {
    if (selected === null) {
      return;
    }

    setRefusal(null);
    const outcome = await enroll({ groupId: group.id, studentId: selected.id });

    if (outcome.ok) {
      toast.success(t("toasts.enrolled", { student: selected.fullName, group: group.name }));
      close();
    } else if (outcome.error.kind === "notFound") {
      toast.error(enrollmentMessageFor(outcome.error));
      close();
    } else {
      setRefusal(enrollmentMessageFor(outcome.error));
      if (outcome.error.code === "enrollment.already_enrolled") {
        // The refetched roster now marks this student; keep nothing picked that can no longer be enrolled.
        setSelected(null);
      }
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
        <form
          onSubmit={(event) => {
            event.preventDefault();
            void handleSubmit();
          }}
          className="space-y-4"
        >
          <DialogHeader>
            <DialogTitle>{t("enrollDialog.title")}</DialogTitle>
            <DialogDescription>{t("enrollDialog.description", { group: group.name })}</DialogDescription>
          </DialogHeader>
```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
