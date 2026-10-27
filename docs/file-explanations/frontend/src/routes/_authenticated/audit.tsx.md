# `frontend/src/routes/_authenticated/audit.tsx`

## Purpose

Declares `AuditSearch`, `AuditPage`, `AuditPageContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/audit.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 124 lines and 4,570 bytes. Source hash: `aff42a69a70ac86725297bc27790fc64b683ccfdae21ffac7c0ddefde869b8ef`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { AuditLog, type AuditFilters } from "@/features/audit/AuditLog";`
- Line 4: `import { AuditEntityTypes, type AuditEntityType } from "@/features/audit/auditFormatting";`
- Line 5: `import { Input } from "@/components/ui/input";`
- Line 6: `import { Label } from "@/components/ui/label";`
- Line 7: `import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";`
- Line 8: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 9: `import { Permissions } from "@/features/session/permissions";`
- Line 10: `import { FilterBar } from "@/components/shared/FilterBar";`
- Line 11: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L15): `export interface AuditSearch {`
- [Line 24](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L24): `export const Route = createFileRoute("/_authenticated/audit")({`
- [Line 41](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L41): `function AuditPage() {`
- [Line 49](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L49): `function AuditPageContent() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/audit.test.tsx:44](../../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { AuditLog, type AuditFilters } from "@/features/audit/AuditLog";
import { AuditEntityTypes, type AuditEntityType } from "@/features/audit/auditFormatting";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";
import { FilterBar } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

const DefaultPageSize = 50;

export interface AuditSearch {
  entityType?: AuditEntityType;
  from?: string;
  to?: string;
  pageSize?: number;
}

const KnownEntityTypes = new Set<string>(AuditEntityTypes);

export const Route = createFileRoute("/_authenticated/audit")({
  validateSearch: (search: Record<string, unknown>): AuditSearch => {
    const entityType = typeof search.entityType === "string" && KnownEntityTypes.has(search.entityType)
      ? (search.entityType as AuditSearch["entityType"])
      : undefined;
    const pageSize = typeof search.pageSize === "string" ? Number(search.pageSize) : undefined;

    return {
      ...(entityType === undefined ? {} : { entityType }),
      ...(typeof search.from === "string" && search.from !== "" ? { from: search.from } : {}),
      ...(typeof search.to === "string" && search.to !== "" ? { to: search.to } : {}),
      ...(pageSize !== undefined && Number.isFinite(pageSize) && pageSize > 0 ? { pageSize } : {}),
    };
  },
  component: AuditPage,
});

function AuditPage() {
  return (
    <RouteGuard permission={Permissions.AuditView}>
      <AuditPageContent />
    </RouteGuard>
  );
}

function AuditPageContent() {
  const { t } = useTranslation("audit");
  const search = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });

  const updateSearch = (patch: Partial<AuditSearch>) => {
    void navigate({ search: { ...search, ...patch }, replace: true });
  };

  const filters: AuditFilters = {
    ...(search.entityType === undefined ? {} : { entityType: search.entityType }),
    ...(search.from === undefined ? {} : { from: search.from }),
    ...(search.to === undefined ? {} : { to: search.to }),
    pageSize: search.pageSize ?? DefaultPageSize,
  };

  return (
    <section className="space-y-6">
      <PageHeader title={t("page.title")} description={t("page.subtitle")} />

      <FilterBar label={t("page.filtersLabel")}>
        <div className="space-y-1.5">
          <Label htmlFor="audit-entity-type">{t("filters.entityTypeLabel")}</Label>
          <Select
            value={search.entityType ?? "all"}
            onValueChange={(value) => {
              updateSearch({ entityType: value === "all" ? undefined : (value as AuditSearch["entityType"]) });
            }}
          >
            <SelectTrigger id="audit-entity-type" className="w-44">
              <SelectValue>
                {(value: string) => (value === "all" ? t("filters.entityTypeAll") : t(`entityType.${value}`))}
              </SelectValue>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">{t("filters.entityTypeAll")}</SelectItem>
              {AuditEntityTypes.map((entityType) => (
                <SelectItem key={entityType} value={entityType}>
                  {t(`entityType.${entityType}`)}
                </SelectItem>
              ))}
            </SelectContent>
```

The excerpt stops at line 90; use the original file for the remaining 34 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
