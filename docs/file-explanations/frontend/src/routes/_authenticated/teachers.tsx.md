# `frontend/src/routes/_authenticated/teachers.tsx`

## Purpose

Declares `TeachersSearch`, `TeachersPage`, `TeachersPageContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/teachers.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 89 lines and 2,889 bytes. Source hash: `fe36d8e68faa32a118660a887e3400f4796b6d30b38b84b4c4bc2b0d309ed36f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { Plus } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { Button } from "@/components/ui/button";`
- Line 6: `import { Label } from "@/components/ui/label";`
- Line 7: `import { Switch } from "@/components/ui/switch";`
- Line 8: `import { Can } from "@/features/session/Can";`
- Line 9: `import { Permissions } from "@/features/session/permissions";`
- Line 10: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 11: `import { TeacherFormDialog, type TeacherDialogState } from "@/features/teachers/TeacherFormDialog";`
- Line 12: `import { TeachersList } from "@/features/teachers/TeachersList";`
- Line 13: `import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";`
- Line 14: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L16): `export interface TeachersSearch {`
- [Line 20](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L20): `export const Route = createFileRoute("/_authenticated/teachers")({`
- [Line 27](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L27): `function TeachersPage() {`
- [Line 35](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L35): `function TeachersPageContent() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/teachers.test.tsx:77](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L77)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { TeacherFormDialog, type TeacherDialogState } from "@/features/teachers/TeacherFormDialog";
import { TeachersList } from "@/features/teachers/TeachersList";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface TeachersSearch {
  archived?: boolean;
}

export const Route = createFileRoute("/_authenticated/teachers")({
  validateSearch: (search: Record<string, unknown>): TeachersSearch => ({
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
  }),
  component: TeachersPage,
});

function TeachersPage() {
  return (
    <RouteGuard permission={Permissions.TeachersView}>
      <TeachersPageContent />
    </RouteGuard>
  );
}

function TeachersPageContent() {
  const { t } = useTranslation("teachers");
  const { archived = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [dialogState, setDialogState] = useState<TeacherDialogState>(null);

  const setArchived = (next: boolean) => {
    void navigate({ search: (previous) => ({ ...previous, archived: next ? true : undefined }), replace: true });
  };

  const openCreate = () => {
    setDialogState({ mode: "create" });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.TeachersManage}>
            <Button type="button" onClick={openCreate}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <FilterToggle>
          <Switch id="teachers-show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="teachers-show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <TeachersList
        includeArchived={archived}
        onCreate={openCreate}
        onEdit={(teacher) => {
          setDialogState({ mode: "edit", teacher });
        }}
      />

      <TeacherFormDialog
        state={dialogState}
        onOpenChange={(open) => {
          if (!open) {
            setDialogState(null);
          }
        }}
      />
    </section>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
