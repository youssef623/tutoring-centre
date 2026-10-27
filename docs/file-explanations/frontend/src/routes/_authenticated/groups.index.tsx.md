# `frontend/src/routes/_authenticated/groups.index.tsx`

## Purpose

Source comment (intent): One page for owners, secretaries and teachers: the API scopes the rows. The teacher filter is rendered only for a session with `teachers.view` — without it the select would call `GET /api/teachers` and get a 403 on every load — and the create button only with `groups.manage`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 138 lines and 5,056 bytes. Source hash: `cbf604abb79af603008905c77ff608c6ed332c0ce945e1817a8c1c1e7dd0a55f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { Plus } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { Button } from "@/components/ui/button";`
- Line 6: `import { Label } from "@/components/ui/label";`
- Line 7: `import { Switch } from "@/components/ui/switch";`
- Line 8: `import { CreateGroupDialog } from "@/features/groups/CreateGroupDialog";`
- Line 9: `import { GroupsList } from "@/features/groups/GroupsList";`
- Line 10: `import { Can } from "@/features/session/Can";`
- Line 11: `import { Permissions } from "@/features/session/permissions";`
- Line 12: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 13: `import { SubjectSelect } from "@/features/subjects/SubjectSelect";`
- Line 14: `import { TeacherSelect } from "@/features/teachers/TeacherSelect";`
- Line 15: `import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";`
- Line 16: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L18): `export interface GroupsSearch {`
- [Line 25](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L25): `export const Route = createFileRoute("/_authenticated/groups/")({`
- [Line 35](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L35): `function GroupsPage() {`
- [Line 48](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L48): `function GroupsPageContent() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/groups.index.test.tsx:63](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L63)

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
import { CreateGroupDialog } from "@/features/groups/CreateGroupDialog";
import { GroupsList } from "@/features/groups/GroupsList";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";
import { SubjectSelect } from "@/features/subjects/SubjectSelect";
import { TeacherSelect } from "@/features/teachers/TeacherSelect";
import { FilterBar, FilterToggle } from "@/components/shared/FilterBar";
import { PageHeader } from "@/components/shared/PageHeader";

export interface GroupsSearch {
  subject?: string;
  teacher?: string;
  archived?: boolean;
  freeSeats?: boolean;
}

export const Route = createFileRoute("/_authenticated/groups/")({
  validateSearch: (search: Record<string, unknown>): GroupsSearch => ({
    ...(typeof search.subject === "string" && search.subject !== "" ? { subject: search.subject } : {}),
    ...(typeof search.teacher === "string" && search.teacher !== "" ? { teacher: search.teacher } : {}),
    ...(search.archived === true || search.archived === "true" ? { archived: true } : {}),
    ...(search.freeSeats === true || search.freeSeats === "true" ? { freeSeats: true } : {}),
  }),
  component: GroupsPage,
});

function GroupsPage() {
  return (
    <RouteGuard permission={Permissions.GroupsView}>
      <GroupsPageContent />
    </RouteGuard>
  );
}

/**
 * One page for owners, secretaries and teachers: the API scopes the rows. The teacher filter is rendered only
 * for a session with `teachers.view` — without it the select would call `GET /api/teachers` and get a 403 on
 * every load — and the create button only with `groups.manage`.
 */
function GroupsPageContent() {
  const { t } = useTranslation("groups");
  const { t: tEnrollments } = useTranslation("enrollments");
  const { subject, teacher, archived = false, freeSeats = false } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const [createOpen, setCreateOpen] = useState(false);

  const updateSearch = (patch: Partial<GroupsSearch>) => {
    void navigate({ search: (previous) => ({ ...previous, ...patch }), replace: true });
  };

  const openCreate = () => {
    setCreateOpen(true);
  };

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.GroupsManage}>
            <Button type="button" onClick={openCreate}>
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <FilterBar label={t("filters.label")}>
        <div className="w-full space-y-1.5 sm:w-56">
          <Label htmlFor="groups-filter-subject">{t("filters.subject")}</Label>
          <SubjectSelect
            id="groups-filter-subject"
            value={subject ?? null}
            allLabel={t("filters.allSubjects")}
            onChange={(next) => {
              updateSearch({ subject: next ?? undefined });
            }}
          />
        </div>
        <Can permission={Permissions.TeachersView}>
```

The excerpt stops at line 90; use the original file for the remaining 48 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
