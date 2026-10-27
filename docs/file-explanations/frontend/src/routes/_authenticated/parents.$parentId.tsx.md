# `frontend/src/routes/_authenticated/parents.$parentId.tsx`

## Purpose

Source comment (intent): Also what a foreign parent id (another centre's) renders: the read service never reveals that it exists.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 188 lines and 6,608 bytes. Source hash: `ad5be6d9cd314fa8476570ea35282b8d6c0a0b276c6cd47724c3afca522fc54b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link, createFileRoute } from "@tanstack/react-router";`
- Line 2: `import { SearchX } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { useGetParent, type ParentChildDto, type ParentDetailDto } from "@/api/generated/tutoring-centre";`
- Line 8: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 11: `import { Badge } from "@/components/ui/badge";`
- Line 12: `import { PhoneText } from "@/components/shared/PhoneText";`
- Line 13: `import {`
- Line 21: `import { StatusBadge } from "@/components/shared/StatusBadge";`
- Line 22: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 23: `import { ParentEditDialog } from "@/features/parents/ParentEditDialog";`
- Line 24: `import { Permissions } from "@/features/session/permissions";`
- Line 25: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 26: `import { useCan } from "@/features/session/useCan";`
- Line 27: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 29](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L29): `export const Route = createFileRoute("/_authenticated/parents/$parentId")({`
- [Line 33](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L33): `function ParentDetailPage() {`
- [Line 41](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L41): `function ParentDetailPageContent() {`
- [Line 95](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L95): `function ParentNotFound() {`
- [Line 108](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L108): `function ParentDetailContent({ parent }: { parent: ParentDetailDto }) {`
- [Line 166](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L166): `function ChildRow({ child }: { child: ParentChildDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:44](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link, createFileRoute } from "@tanstack/react-router";
import { SearchX } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetParent, type ParentChildDto, type ParentDetailDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { Badge } from "@/components/ui/badge";
import { PhoneText } from "@/components/shared/PhoneText";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { ParentEditDialog } from "@/features/parents/ParentEditDialog";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

export const Route = createFileRoute("/_authenticated/parents/$parentId")({
  component: ParentDetailPage,
});

function ParentDetailPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <ParentDetailPageContent />
    </RouteGuard>
  );
}

function ParentDetailPageContent() {
  const { parentId } = Route.useParams();
  const { t } = useTranslation("parents");
  const { t: tCommon } = useTranslation("common");
  const { data, error, isPending, refetch } = useGetParent(parentId);

  if (isPending) {
    return (
      <section className="space-y-4">
        <div role="status" aria-label={t("loading.detailLabel")} className="max-w-md space-y-3">
          <Skeleton className="h-8 w-48" />
          <Skeleton className="h-24 w-full" />
        </div>
      </section>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    if (apiError.kind === "notFound") {
      return <ParentNotFound />;
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
      </section>
    );
  }

```

The excerpt stops at line 90; use the original file for the remaining 98 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
