# `frontend/src/components/shared/LoadMoreButton.tsx`

## Purpose

Source comment (intent): The "load more" action for an infinite query over a cursor API (Day 41; Month 2's audit page has its own inline version).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/LoadMoreButton.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 13 lines and 505 bytes. Source hash: `3eb0af48dbc8eb8f20bdd97820e8eb899c66cd5aa094a96ebdb0a8bc22eae06d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { Button } from "@/components/ui/button";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/components/shared/LoadMoreButton.tsx#L5): `export function LoadMoreButton({ onClick, pending }: { onClick: () => void; pending?: boolean }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/LoadMoreButton.test.tsx:3](../../../../../../frontend/src/components/shared/LoadMoreButton.test.tsx#L3)
- [frontend/src/features/students/StudentsList.tsx:24](../../../../../../frontend/src/features/students/StudentsList.tsx#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";

/** The "load more" action for an infinite query over a cursor API (Day 41; Month 2's audit page has its own inline version). */
export function LoadMoreButton({ onClick, pending }: { onClick: () => void; pending?: boolean }) {
  const { t } = useTranslation("common");

  return (
    <Button type="button" variant="outline" disabled={pending} onClick={onClick}>
      {t("loadMore")}
    </Button>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
