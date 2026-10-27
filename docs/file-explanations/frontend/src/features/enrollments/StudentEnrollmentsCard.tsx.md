# `frontend/src/features/enrollments/StudentEnrollmentsCard.tsx`

## Purpose

Source comment (intent): A student's groups on their own page: active enrollments first with their actions, then ended ones, muted, with the end date and reason. The caller renders this only with `enrollments.view`, so without it no enrollment request is sent. Enroll, withdraw and transfer are the same components and mutation as the roster tab.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 199 lines and 7,703 bytes. Source hash: `1445e25e324f22b398ddc93ef84db81076a7a35ef98e3a6ce97b77c782ad9196`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { UserPlus } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { useListStudentEnrollments, type StudentEnrollmentDto } from "@/api/generated/tutoring-centre";`
- Line 8: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 11: `import { EnrollInGroupDialog } from "@/features/enrollments/EnrollInGroupDialog";`
- Line 12: `import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";`
- Line 13: `import { TransferDialog } from "@/features/enrollments/TransferDialog";`
- Line 14: `import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";`
- Line 15: `import { Permissions } from "@/features/session/permissions";`
- Line 16: `import { useCan } from "@/features/session/useCan";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L23): `export function StudentEnrollmentsCard({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:52](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L52)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:36](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { UserPlus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListStudentEnrollments, type StudentEnrollmentDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { EnrollInGroupDialog } from "@/features/enrollments/EnrollInGroupDialog";
import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";
import { TransferDialog } from "@/features/enrollments/TransferDialog";
import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";

/**
 * A student's groups on their own page: active enrollments first with their actions, then ended ones, muted, with
 * the end date and reason. The caller renders this only with `enrollments.view`, so without it no enrollment
 * request is sent. Enroll, withdraw and transfer are the same components and mutation as the roster tab.
 */
export function StudentEnrollmentsCard({
  student,
  canEnroll,
}: {
  student: { id: string; fullName: string };
  canEnroll: boolean;
}) {
  const { t, i18n } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.EnrollmentsManage);
  const linkGroups = useCan(Permissions.GroupsView);
  const { data, error, isPending, refetch } = useListStudentEnrollments(student.id);
  const [enrollOpen, setEnrollOpen] = useState(false);
  const [withdrawTarget, setWithdrawTarget] = useState<EnrollmentTarget | null>(null);
  const [transferTarget, setTransferTarget] = useState<EnrollmentTarget | null>(null);
  const items = data?.items ?? [];
  const active = items.filter((item) => item.status === "active");
  const ended = items.filter((item) => item.status === "ended");
  const activeGroupIds = new Set(active.map((item) => item.groupId));
  const dateFormat = new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium" });

  const toTarget = (item: StudentEnrollmentDto): EnrollmentTarget => ({
    enrollmentId: item.enrollmentId,
    version: item.version,
    studentId: student.id,
    studentName: student.fullName,
    groupId: item.groupId,
    groupName: item.groupName,
  });

  const groupName = (item: StudentEnrollmentDto) =>
    linkGroups ? (
      <Link
        to="/groups/$groupId"
        params={{ groupId: item.groupId }}
        search={{ tab: "roster" }}
        className="font-medium hover:underline"
        dir="auto"
      >
        {item.groupName}
      </Link>
    ) : (
      <span className="font-medium" dir="auto">
        {item.groupName}
      </span>
    );

  return (
    <section aria-labelledby="student-enrollments-title" className="space-y-3 rounded-xl border border-border p-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 id="student-enrollments-title" className="text-base font-semibold">
          {t("studentCard.title")}
        </h2>
        {canManage && canEnroll && (
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => {
              setEnrollOpen(true);
            }}
          >
            <UserPlus className="size-4" aria-hidden="true" />
            {t("studentCard.enroll")}
          </Button>
        )}
      </div>

```

The excerpt stops at line 90; use the original file for the remaining 109 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
