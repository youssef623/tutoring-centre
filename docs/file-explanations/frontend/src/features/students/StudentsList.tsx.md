# `frontend/src/features/students/StudentsList.tsx`

## Purpose

Declares `StudentsList`, `StudentRow`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/StudentsList.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 165 lines and 5,633 bytes. Source hash: `d3570c1182320dd666085a11ff5da357405ac205bf74a17643dad74f2d624629`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useInfiniteQuery } from "@tanstack/react-query";`
- Line 2: `import { Link } from "@tanstack/react-router";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { asApiError } from "@/api/apiFetch";`
- Line 5: `import { messageFor } from "@/api/errorMessages";`
- Line 6: `import {`
- Line 12: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 13: `import { Button } from "@/components/ui/button";`
- Line 14: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 15: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 16: `import {`
- Line 24: `import { LoadMoreButton } from "@/components/shared/LoadMoreButton";`
- Line 25: `import { PhoneText } from "@/components/shared/PhoneText";`
- Line 26: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 27: `import { Permissions } from "@/features/session/permissions";`
- Line 28: `import { useCan } from "@/features/session/useCan";`
- Line 29: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 33](../../../../../../frontend/src/features/students/StudentsList.tsx#L33): `export function StudentsList({ search, includeArchived }: { search: string; includeArchived: boolean }) {`
- [Line 140](../../../../../../frontend/src/features/students/StudentsList.tsx#L140): `function StudentRow({ student }: { student: StudentListItemDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:12](../../../../../onboarding-work/enrich_frontend.py#L12)
- [frontend/src/routes/_authenticated/students.index.tsx:8](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L8)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:443](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L443)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useInfiniteQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import {
  getListStudentsQueryKey,
  listStudents,
  type ListStudentsParams,
  type StudentListItemDto,
} from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Skeleton } from "@/components/ui/skeleton";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { LoadMoreButton } from "@/components/shared/LoadMoreButton";
import { PhoneText } from "@/components/shared/PhoneText";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

const PageSize = 25;

export function StudentsList({ search, includeArchived }: { search: string; includeArchived: boolean }) {
  const { t } = useTranslation("students");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.StudentsManage);

  const params: ListStudentsParams = {
    ...(search === "" ? {} : { search }),
    includeArchived,
    pageSize: PageSize,
  };

  const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({
    queryKey: getListStudentsQueryKey(params),
    queryFn: ({ pageParam, signal }) => listStudents({ ...params, cursor: pageParam }, { signal }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  });

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
          {apiError.correlationId !== undefined && <p>{t("error.reference", { correlationId: isolateLtr(apiError.correlationId) })}</p>}
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

  const items = data.pages.flatMap((page) => page.items);

  if (items.length === 0) {
    if (search === "") {
      return (
```

The excerpt stops at line 90; use the original file for the remaining 75 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
