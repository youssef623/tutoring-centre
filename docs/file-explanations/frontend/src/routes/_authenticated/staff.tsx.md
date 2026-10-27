# `frontend/src/routes/_authenticated/staff.tsx`

## Purpose

Declares `StaffPage`, `StaffPageContent`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/staff.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 69 lines and 2,045 bytes. Source hash: `4fb854d67093acc97062575373e2d2bdf4a265377eca1989cd0721641dc7302e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute } from "@tanstack/react-router";`
- Line 2: `import { Plus } from "lucide-react";`
- Line 3: `import { useState } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { ChangeRoleDialog } from "@/features/staff/ChangeRoleDialog";`
- Line 6: `import { StaffFormDialog } from "@/features/staff/StaffFormDialog";`
- Line 7: `import { StaffList } from "@/features/staff/StaffList";`
- Line 8: `import type { StaffMemberDto } from "@/api/generated/tutoring-centre";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { Can } from "@/features/session/Can";`
- Line 11: `import { RouteGuard } from "@/features/session/RouteGuard";`
- Line 12: `import { Permissions } from "@/features/session/permissions";`
- Line 13: `import { PageHeader } from "@/components/shared/PageHeader";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L15): `export const Route = createFileRoute("/_authenticated/staff")({`
- [Line 19](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L19): `function StaffPage() {`
- [Line 27](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L27): `function StaffPageContent() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/staff.test.tsx:45](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L45)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { ChangeRoleDialog } from "@/features/staff/ChangeRoleDialog";
import { StaffFormDialog } from "@/features/staff/StaffFormDialog";
import { StaffList } from "@/features/staff/StaffList";
import type { StaffMemberDto } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { Can } from "@/features/session/Can";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";
import { PageHeader } from "@/components/shared/PageHeader";

export const Route = createFileRoute("/_authenticated/staff")({
  component: StaffPage,
});

function StaffPage() {
  return (
    <RouteGuard permission={Permissions.StaffView}>
      <StaffPageContent />
    </RouteGuard>
  );
}

function StaffPageContent() {
  const { t } = useTranslation("staff");
  const [createOpen, setCreateOpen] = useState(false);
  const [roleTarget, setRoleTarget] = useState<StaffMemberDto | null>(null);

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("page.title")}
        description={t("page.subtitle")}
        actions={
          <Can permission={Permissions.StaffManage}>
            <Button
              type="button"
              onClick={() => {
                setCreateOpen(true);
              }}
            >
              <Plus className="size-4" aria-hidden="true" />
              {t("actions.create")}
            </Button>
          </Can>
        }
      />

      <StaffList
        onChangeRole={(member) => {
          setRoleTarget(member);
        }}
      />

      <StaffFormDialog open={createOpen} onOpenChange={setCreateOpen} />
      <ChangeRoleDialog
        member={roleTarget}
        onOpenChange={(open) => {
          if (!open) {
            setRoleTarget(null);
          }
        }}
      />
    </section>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
