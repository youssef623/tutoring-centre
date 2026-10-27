# `frontend/src/features/session/RouteGuard.tsx`

## Purpose

Source comment (intent): Route-level permission guard: renders its children only when the session holds the given permission, otherwise a translated "no access" state with a link back to the dashboard. Because the protected page's own data hooks live inside `children`, which never mounts when the permission is missing, no request for the protected data is ever issued — this is usability only, never the security boundary (the API is).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/session/RouteGuard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,464 bytes. Source hash: `991e2ade3128567a339fb705e92e6d0c8ce17bd11e0cdffbce474da6e91700c3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { ShieldAlert } from "lucide-react";`
- Line 3: `import type { ReactNode } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { buttonVariants } from "@/components/ui/button-variants";`
- Line 6: `import { useCan } from "./useCan";`
- Line 7: `import type { Permission } from "./permissions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/features/session/RouteGuard.tsx#L15): `export function RouteGuard({ permission, children }: { permission: Permission; children: ReactNode }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/audit.tsx:8](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L8)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:52](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L52)
- [frontend/src/routes/_authenticated/groups.index.tsx:12](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L12)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:25](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L25)
- [frontend/src/routes/_authenticated/settings.tsx:26](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L26)
- [frontend/src/routes/_authenticated/staff.tsx:11](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L11)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:42](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L42)
- [frontend/src/routes/_authenticated/students.index.tsx:11](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L11)
- [frontend/src/routes/_authenticated/students.new.tsx:4](../../../../../../frontend/src/routes/_authenticated/students.new.tsx#L4)
- [frontend/src/routes/_authenticated/teachers.tsx:10](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L10)
- [frontend/src/routes/_authenticated/timetable.tsx:5](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { ShieldAlert } from "lucide-react";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { buttonVariants } from "@/components/ui/button-variants";
import { useCan } from "./useCan";
import type { Permission } from "./permissions";

/**
 * Route-level permission guard: renders its children only when the session holds the given permission,
 * otherwise a translated "no access" state with a link back to the dashboard. Because the protected page's
 * own data hooks live inside `children`, which never mounts when the permission is missing, no request for
 * the protected data is ever issued — this is usability only, never the security boundary (the API is).
 */
export function RouteGuard({ permission, children }: { permission: Permission; children: ReactNode }) {
  const { t } = useTranslation("shell");
  const allowed = useCan(permission);

  if (!allowed) {
    return (
      <div className="flex flex-col items-center gap-3 py-16 text-center">
        <ShieldAlert className="size-10 text-muted-foreground" aria-hidden="true" />
        <h1 className="text-xl font-semibold tracking-tight">{t("noAccess.title")}</h1>
        <p className="max-w-sm text-sm text-muted-foreground">{t("noAccess.description")}</p>
        <Link to="/" className={buttonVariants()}>{t("noAccess.backToDashboard")}</Link>
      </div>
    );
  }

  return <>{children}</>;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
