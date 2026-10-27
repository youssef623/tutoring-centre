# `frontend/src/routes/_authenticated/students.$studentId.tsx`

## Purpose

Source comment (intent): Also what a foreign student id (another centre's) renders: the read service never reveals that it exists.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 272 lines and 9,799 bytes. Source hash: `2474bb2ef26674b45869920de201e3428f2566717e34d126389eca930a040c21`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { Link, createFileRoute } from "@tanstack/react-router";`
- Line 3: `import { SearchX } from "lucide-react";`
- Line 4: `import { useState } from "react";`
- Line 5: `import { useTranslation } from "react-i18next";`
- Line 6: `import { toast } from "sonner";`
- Line 7: `import { asApiError } from "@/api/apiFetch";`
- Line 8: `import { messageFor } from "@/api/errorMessages";`
- Line 9: `import { showApiError } from "@/api/showApiError";`
- Line 10: `import {`
- Line 19: `import {`
- Line 29: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 30: `import { Button } from "@/components/ui/button";`
- Line 31: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 32: `import { PhoneText } from "@/components/shared/PhoneText";`
- Line 33: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 34: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 35: `import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";`
- Line 36: `import { StudentEnrollmentsCard } from "@/features/enrollments/StudentEnrollmentsCard";`
- Line 37: `import { ParentsCard } from "@/features/students/ParentsCard";`
- Line 38: `import { StudentEditDialog } from "@/features/students/StudentEditDialog";`
- Line 39: `import { Can } from "@/features/session/Can";`
- Line 40: `import { Permissions } from "@/features/session/permissions";`
- Line 41: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`
- Line 42: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 43: `import { useCan } from "@/features/session/useCan";`
- Line 44: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 46](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L46): `export const Route = createFileRoute("/_authenticated/students/$studentId")({`
- [Line 50](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L50): `function StudentDetailPage() {`
- [Line 58](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L58): `function StudentDetailPageContent() {`
- [Line 112](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L112): `function StudentNotFound() {`
- [Line 125](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L125): `function StudentDetailContent({ student }: { student: StudentDetailDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/students.$studentId.test.tsx:57](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L57)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useQueryClient } from "@tanstack/react-query";
import { Link, createFileRoute } from "@tanstack/react-router";
import { SearchX } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { showApiError } from "@/api/showApiError";
import {
  getGetStudentQueryKey,
  getListStudentEnrollmentsQueryKey,
  getListStudentsQueryKey,
  useArchiveStudent,
  useGetStudent,
  useRestoreStudent,
  type StudentDetailDto,
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
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { enrollmentMessageFor } from "@/features/enrollments/enrollmentMessages";
import { StudentEnrollmentsCard } from "@/features/enrollments/StudentEnrollmentsCard";
import { ParentsCard } from "@/features/students/ParentsCard";
import { StudentEditDialog } from "@/features/students/StudentEditDialog";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { RouteGuard } from "@/features/session/RouteGuard";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

export const Route = createFileRoute("/_authenticated/students/$studentId")({
  component: StudentDetailPage,
});

function StudentDetailPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <StudentDetailPageContent />
    </RouteGuard>
  );
}

function StudentDetailPageContent() {
  const { studentId } = Route.useParams();
  const { t } = useTranslation("students");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetStudent(studentId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="max-w-md space-y-3">
          <Skeleton className="h-8 w-48" />
          <Skeleton className="h-32 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.kind === "notFound") {
      return <StudentNotFound />;
    }

    return (
      <section className="space-y-4">
        <Alert variant="destructive">
          <AlertTitle>{t("detailError.title")}</AlertTitle>
          <AlertDescription className="flex flex-col gap-2">
            <p>{messageFor(apiError)}</p>
            {apiError.correlationId !== undefined && (
              <p>{t("error.reference", { correlationId: isolateLtr(apiError.correlationId) })}</p>
            )}
```

The excerpt stops at line 90; use the original file for the remaining 182 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
