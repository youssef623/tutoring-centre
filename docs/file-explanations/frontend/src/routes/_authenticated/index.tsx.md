# `frontend/src/routes/_authenticated/index.tsx`

## Purpose

Source comment (intent): The landing page after sign-in: shortcuts, then the centre's seats at a glance. Each part follows permissions.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/index.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,307 bytes. Source hash: `0b29c6599bf4f0531b2b0aa20d9a3c27418af78e7d9b75998343a3a451c9135f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute } from "@tanstack/react-router";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { PageHeader } from "@/components/shared/PageHeader";`
- Line 4: `import { GroupsOverview, QuickActions } from "@/features/dashboard/DashboardOverview";`
- Line 5: `import { Can } from "@/features/session/Can";`
- Line 6: `import { Permissions } from "@/features/session/permissions";`
- Line 7: `import { useSession } from "@/features/session/useSession";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/routes/_authenticated/index.tsx#L9): `export const Route = createFileRoute("/_authenticated/")({`
- [Line 14](../../../../../../frontend/src/routes/_authenticated/index.tsx#L14): `function DashboardPage() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { PageHeader } from "@/components/shared/PageHeader";
import { GroupsOverview, QuickActions } from "@/features/dashboard/DashboardOverview";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useSession } from "@/features/session/useSession";

export const Route = createFileRoute("/_authenticated/")({
  component: DashboardPage,
});

/** The landing page after sign-in: shortcuts, then the centre's seats at a glance. Each part follows permissions. */
function DashboardPage() {
  const { t } = useTranslation("shell");
  const { me } = useSession();

  if (me === null) {
    return null;
  }

  const activeCentre = me.memberships.find((membership) => membership.centreId === me.activeCentreId);

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("dashboard.welcome", { name: me.displayName })}
        {...(activeCentre === undefined
          ? {}
          : { description: t("dashboard.activeCentre", { centreName: activeCentre.centreName }) })}
      />
      <QuickActions />
      <Can permission={Permissions.GroupsView}>
        <GroupsOverview />
      </Can>
    </section>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
