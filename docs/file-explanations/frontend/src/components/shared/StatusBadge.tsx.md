# `frontend/src/components/shared/StatusBadge.tsx`

## Purpose

Source comment (intent): Active/archived as label plus colour, shared across every list that carries this same two-state lifecycle (Day 41; Weeks 8 and 9 reuse it). Month 2's `SubjectStatusBadge`, if one exists under that name, is left as it is — this is a new, generic component, not a replacement.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/StatusBadge.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 680 bytes. Source hash: `ad1b02b20871e50f084b004b777af063da722c4d2852325d060924f2b4941beb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { Badge } from "@/components/ui/badge";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/components/shared/StatusBadge.tsx#L9): `export function StatusBadge({ status }: { status: "active" | "archived" }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/StatusBadge.test.tsx:3](../../../../../../frontend/src/components/shared/StatusBadge.test.tsx#L3)
- [frontend/src/features/groups/GroupsList.tsx:17](../../../../../../frontend/src/features/groups/GroupsList.tsx#L17)
- [frontend/src/features/students/StudentsList.tsx:26](../../../../../../frontend/src/features/students/StudentsList.tsx#L26)
- [frontend/src/features/subjects/SubjectsList.tsx:35](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L35)
- [frontend/src/features/teachers/TeachersList.tsx:27](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L27)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:28](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L28)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:21](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L21)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:33](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";

/**
 * Active/archived as label plus colour, shared across every list that carries this same two-state lifecycle
 * (Day 41; Weeks 8 and 9 reuse it). Month 2's `SubjectStatusBadge`, if one exists under that name, is left as
 * it is — this is a new, generic component, not a replacement.
 */
export function StatusBadge({ status }: { status: "active" | "archived" }) {
  const { t } = useTranslation("common");
  const isArchived = status === "archived";

  return <Badge variant={isArchived ? "outline" : "default"}>{t(isArchived ? "status.archived" : "status.active")}</Badge>;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
