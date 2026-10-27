# `frontend/src/routes/_authenticated/students.index.tsx`

## Purpose

Declares `StudentsSearch`, `StudentsPage`, `StudentsPageContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.index.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 77 lines and 2,787 bytes. Source hash: `3bd4cefdae1049b7aea1600ede056ddf45542c5afe9a9e203f8a335f32875cc7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { Plus } from "lucide-react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { SearchInput } from "@/components/shared/SearchInput";`
- Line 5: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 6: `import { Label } from "@/components/ui/label";`
- Line 7: `import { Switch } from "@/components/ui/switch";`
- Line 8: `import { StudentsList } from "@/features/students/StudentsList";`
- Line 9: `import { Can } from "@/features/session/Can";`
- Line 10: `import { Permissions } from "@/features/session/permissions";`
- Line 11: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 12: `import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";`
- Line 13: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L15): `export interface StudentsSearch {`
- [Line 20](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L20): `export const Route = createFileRoute("/_authenticated/students/")({`
- [Line 28](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L28): `function StudentsPage() {`
- [Line 36](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L36): `function StudentsPageContent() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/students.index.test.tsx:44](../../../../../../frontend/src/routes/_authenticated/students.index.test.tsx#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useTranslation } from "react-i18next";
import { SearchInput } from "@/components/shared/SearchInput";
import { buttonVariants } from "@/components/ui/button-variants";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { StudentsList } from "@/features/students/StudentsList";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface StudentsSearch {
  q?: string;
  archived?: boolean;
}

export const Route = createFileRoute("/_authenticated/students/")({
  validateSearch: (search: Record<string, unknown>): StudentsSearch => ({
    ...(typeof search.q === "string" && search.q !== "" ? { q: search.q } : {}),
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
  }),
  component: StudentsPage,
});

function StudentsPage() {
  return (
    <RouteGuard permission={Permissions.StudentsView}>
      <StudentsPageContent />
    </RouteGuard>
  );
}

function StudentsPageContent() {
  const { t } = useTranslation("students");
  const { q = "", archived = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });

  const setQuery = (next: string) => {
    void navigate({ search: (previous) => ({ ...previous, q: next === "" ? undefined : next }), replace: true });
  };

  const setArchived = (next: boolean) => {
    void navigate({ search: (previous) => ({ ...previous, archived: next ? true : undefined }), replace: true });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.StudentsManage}>
            <Link to="/students/new" className={buttonVariants()}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.register")}
            </Link>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <div className="w-full sm:max-w-xs">
          <SearchInput value={q} onChange={setQuery} label={t("page.searchLabel")} />
        </div>
        <FilterToggle>
          <Switch id="show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <StudentsList search={q} includeArchived={archived} />
    </section>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
