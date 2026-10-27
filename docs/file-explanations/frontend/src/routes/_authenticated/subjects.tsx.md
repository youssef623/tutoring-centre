# `frontend/src/routes/_authenticated/subjects.tsx`

## Purpose

Declares `SubjectsSearch`, `SubjectsPage`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/subjects.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 80 lines and 2,583 bytes. Source hash: `e080f06d8aa083563e719df5d03038a94775f9e07efcbb494ee82e605908e088`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { Plus } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { SubjectFormDialog, type SubjectDialogState } from "@/features/subjects/SubjectFormDialog";`
- Line 6: `import { SubjectsList } from "@/features/subjects/SubjectsList";`
- Line 7: `import { Button } from "@/components/ui/button";`
- Line 8: `import { Label } from "@/components/ui/label";`
- Line 9: `import { Switch } from "@/components/ui/switch";`
- Line 10: `import { Can } from "@/features/session/Can";`
- Line 11: `import { Permissions } from "@/features/session/permissions";`
- Line 12: `import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";`
- Line 13: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L15): `export interface SubjectsSearch {`
- [Line 19](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L19): `export const Route = createFileRoute("/_authenticated/subjects")({`
- [Line 26](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L26): `function SubjectsPage() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/subjects.test.tsx:33](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { SubjectFormDialog, type SubjectDialogState } from "@/features/subjects/SubjectFormDialog";
import { SubjectsList } from "@/features/subjects/SubjectsList";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface SubjectsSearch {
  archived: boolean;
}

export const Route = createFileRoute("/_authenticated/subjects")({
  validateSearch: (search: Record<string, unknown>): SubjectsSearch => ({
    archived: search.archived === true || search.archived === "true",
  }),
  component: SubjectsPage,
});

function SubjectsPage() {
  const { t } = useTranslation("subjects");
  const { archived } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [dialogState, setDialogState] = useState<SubjectDialogState>(null);

  const setArchived = (next: boolean) => {
    void navigate({ search: { archived: next }, replace: true });
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.SubjectsManage}>
            <Button
              type="button"
              onClick={() => {
                setDialogState({ mode: "create" });
              }}
            >
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("page.filtersLabel")}>
        <FilterToggle>
          <Switch id="show-archived" checked={archived} onCheckedChange={setArchived} />
          <Label htmlFor="show-archived">{t("page.showArchived")}</Label>
        </FilterToggle>
      </FilterBar>

      <SubjectsList
        includeArchived={archived}
        onRename={(subject) => {
          setDialogState({ mode: "rename", subject });
        }}
      />

      <SubjectFormDialog
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
