# `frontend/src/features/audit/AuditLog.tsx`

## Purpose

Declares `AuditFilters`, `AuditLog`, `AuditRow`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/audit/AuditLog.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 189 lines and 6,545 bytes. Source hash: `caab8bc4929f73b074079460ff23d5626fcaa29cbfc0a068e578a2a074c26529`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useInfiniteQuery } from "@tanstack/react-query";`
- Line 2: `import { Copy } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { getAuditLog, getGetAuditLogQueryKey, type AuditEntryDto, type GetAuditLogParams } from "@/api/generated/tutoring-centre";`
- Line 8: `import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";`
- Line 9: `import { Badge } from "@/components/ui/badge";`
- Line 10: `import { Button } from "@/components/ui/button";`
- Line 11: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 12: `import { formatAuditValue, type AuditEntityType } from "@/features/audit/auditFormatting";`
- Line 13: `import { isolateLtr } from "@/lib/bidi";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/features/audit/AuditLog.tsx#L15): `export interface AuditFilters {`
- [Line 22](../../../../../../frontend/src/features/audit/AuditLog.tsx#L22): `export function AuditLog({ filters }: { filters: AuditFilters }) {`
- [Line 109](../../../../../../frontend/src/features/audit/AuditLog.tsx#L109): `function AuditRow({ item }: { item: AuditEntryDto }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/audit.tsx:3](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useInfiniteQuery } from "@tanstack/react-query";
import { Copy } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { getAuditLog, getGetAuditLogQueryKey, type AuditEntryDto, type GetAuditLogParams } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { formatAuditValue, type AuditEntityType } from "@/features/audit/auditFormatting";
import { isolateLtr } from "@/lib/bidi";

export interface AuditFilters {
  entityType?: AuditEntityType;
  from?: string;
  to?: string;
  pageSize: number;
}

export function AuditLog({ filters }: { filters: AuditFilters }) {
  const { t } = useTranslation("audit");
  const { t: tCommon } = useTranslation("common");

  const params: GetAuditLogParams = {
    pageSize: filters.pageSize,
    ...(filters.entityType === undefined ? {} : { entityType: filters.entityType }),
    ...(filters.from === undefined || filters.from === "" ? {} : { from: filters.from }),
    ...(filters.to === undefined || filters.to === "" ? {} : { to: filters.to }),
  };

  const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({
    queryKey: getGetAuditLogQueryKey(params),
    queryFn: ({ pageParam, signal }) => getAuditLog({ ...params, cursor: pageParam }, { signal }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  });

  if (isPending) {
    return (
      <div role="status" aria-label={t("loading.label")} className="space-y-2">
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
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
    );
  }

  const items = data.pages.flatMap((page) => page.items);

  if (items.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-border py-10 text-center">
        <p className="text-sm font-medium">{t("emptyState.title")}</p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      {items.map((item) => (
        <AuditRow key={item.id} item={item} />
```

The excerpt stops at line 90; use the original file for the remaining 99 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
