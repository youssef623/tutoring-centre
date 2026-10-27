# `frontend/src/components/shared/PageHeader.tsx`

## Purpose

Source comment (intent): Every page's heading: the title, one line saying what the page is for, and the page's primary action at the end of the same row (wrapping under the title on a phone). The action arrives already wrapped in `Can`.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/PageHeader.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 25 lines and 845 bytes. Source hash: `89f78d05a2b69a1ea43a5d95a2281109380b9093f67fb810289b7444132cbe66`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import type { ReactNode } from "react";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/components/shared/PageHeader.tsx#L7): `export function PageHeader({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetablePage.tsx:9](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L9)
- [frontend/src/routes/_authenticated/audit.tsx:11](../../../../../../frontend/src/routes/_authenticated/audit.tsx#L11)
- [frontend/src/routes/_authenticated/groups.index.tsx:16](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L16)
- [frontend/src/routes/_authenticated/index.tsx:3](../../../../../../frontend/src/routes/_authenticated/index.tsx#L3)
- [frontend/src/routes/_authenticated/settings.tsx:28](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L28)
- [frontend/src/routes/_authenticated/staff.tsx:13](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L13)
- [frontend/src/routes/_authenticated/students.index.tsx:13](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L13)
- [frontend/src/routes/_authenticated/subjects.tsx:13](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L13)
- [frontend/src/routes/_authenticated/teachers.tsx:14](../../../../../../frontend/src/routes/_authenticated/teachers.tsx#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import type { ReactNode } from "react";

/**
 * Every page's heading: the title, one line saying what the page is for, and the page's primary action at the
 * end of the same row (wrapping under the title on a phone). The action arrives already wrapped in `Can`.
 */
export function PageHeader({
  title,
  description,
  actions,
}: {
  title: string;
  description?: string;
  actions?: ReactNode;
}) {
  return (
    <div className="flex flex-wrap items-end justify-between gap-x-4 gap-y-3">
      <div className="min-w-0 space-y-1">
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        {description !== undefined && <p className="text-sm text-muted-foreground">{description}</p>}
      </div>
      {actions !== undefined && <div className="flex flex-wrap items-center gap-2">{actions}</div>}
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
