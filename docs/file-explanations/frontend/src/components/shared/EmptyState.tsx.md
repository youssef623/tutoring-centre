# `frontend/src/components/shared/EmptyState.tsx`

## Purpose

Source comment (intent): A list's "nothing here" state: an icon, what is missing, what to do about it, and (optionally) the action that fixes it. The action is passed in already wrapped in `Can` by the caller, so a read-only session sees the explanation without a button it could not use.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/EmptyState.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,102 bytes. Source hash: `e2d92c18445d9205f8644317ea087887c6b3195b21f1ae43386b2142aa94a39a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { LucideIcon } from "lucide-react";`
- Line 2: `import type { ReactNode } from "react";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/components/shared/EmptyState.tsx#L9): `export function EmptyState({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/groups/GroupsList.tsx:15](../../../../../../frontend/src/features/groups/GroupsList.tsx#L15)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:7](../../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L7)
- [frontend/src/features/schedule/TimetablePage.tsx:7](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L7)
- [frontend/src/features/teachers/TeachersList.tsx:24](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { LucideIcon } from "lucide-react";
import type { ReactNode } from "react";

/**
 * A list's "nothing here" state: an icon, what is missing, what to do about it, and (optionally) the action
 * that fixes it. The action is passed in already wrapped in `Can` by the caller, so a read-only session sees
 * the explanation without a button it could not use.
 */
export function EmptyState({
  icon: Icon,
  title,
  description,
  action,
}: {
  icon: LucideIcon;
  title: string;
  description?: string;
  action?: ReactNode;
}) {
  return (
    <div className="flex flex-col items-center gap-3 rounded-xl border border-dashed border-border px-6 py-12 text-center">
      <span className="flex size-12 items-center justify-center rounded-full bg-muted text-muted-foreground">
        <Icon className="size-6" aria-hidden="true" />
      </span>
      <div className="space-y-1">
        <p className="text-sm font-semibold">{title}</p>
        {description !== undefined && <p className="max-w-sm text-sm text-muted-foreground">{description}</p>}
      </div>
      {action}
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
