# `frontend/src/features/enrollments/StudentPicker.tsx`

## Purpose

Source comment (intent): A picker built from the existing students list query: a small page and the debounced search term, so the server's normalized Arabic search works here with no new endpoint. The search lives in local state, not the URL. Students already on the roster are shown but marked and cannot be picked. Keyboard: Tab from the search box into the list, arrows and Home/End to move, Enter or Space to pick.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 159 lines and 6,077 bytes. Source hash: `cff5e47da96b090a66008d6fa3dd7bbf30aaf21ba1a6bffd813c881a40f90793`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { keepPreviousData } from "@tanstack/react-query";`
- Line 2: `import { Check } from "lucide-react";`
- Line 3: `import { useId, useState, type KeyboardEvent } from "react";`
- Line 4: `import { useTranslation } from "react-i18next";`
- Line 5: `import { asApiError } from "@/api/apiFetch";`
- Line 6: `import { messageFor } from "@/api/errorMessages";`
- Line 7: `import { useListStudents, type StudentListItemDto } from "@/api/generated/tutoring-centre";`
- Line 8: `import { SearchInput } from "@/components/shared/SearchInput";`
- Line 9: `import { Button } from "@/components/ui/button";`
- Line 10: `import { Skeleton } from "@/components/ui/skeleton";`
- Line 11: `import { cn } from "cn";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L21): `export function StudentPicker({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:17](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { keepPreviousData } from "@tanstack/react-query";
import { Check } from "lucide-react";
import { useId, useState, type KeyboardEvent } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListStudents, type StudentListItemDto } from "@/api/generated/tutoring-centre";
import { SearchInput } from "@/components/shared/SearchInput";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "cn";

const PickerPageSize = 10;

/**
 * A picker built from the existing students list query: a small page and the debounced search term, so the
 * server's normalized Arabic search works here with no new endpoint. The search lives in local state, not the
 * URL. Students already on the roster are shown but marked and cannot be picked. Keyboard: Tab from the search
 * box into the list, arrows and Home/End to move, Enter or Space to pick.
 */
export function StudentPicker({
  selectedId,
  onSelect,
  unavailableIds,
}: {
  selectedId: string | null;
  onSelect: (student: StudentListItemDto) => void;
  unavailableIds: ReadonlySet<string>;
}) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const listId = useId();
  const [search, setSearch] = useState("");
  const [activeIndex, setActiveIndex] = useState(0);
  const { data, error, isPending, refetch } = useListStudents(
    { search: search === "" ? undefined : search, pageSize: PickerPageSize },
    { query: { placeholderData: keepPreviousData } },
  );
  const items = data?.items ?? [];
  const active = items.at(Math.min(activeIndex, items.length - 1));

  const pick = (student: StudentListItemDto | undefined) => {
    if (student !== undefined && !unavailableIds.has(student.id)) {
      onSelect(student);
    }
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLUListElement>) => {
    const last = items.length - 1;
    const moves = new Map<string, number>([
      ["ArrowDown", Math.min(activeIndex + 1, last)],
      ["ArrowUp", Math.max(activeIndex - 1, 0)],
      ["Home", 0],
      ["End", last],
    ]);
    const next = moves.get(event.key);

    if (next !== undefined) {
      event.preventDefault();
      setActiveIndex(next);
      const option = items.at(next);
      const element = option === undefined ? null : document.getElementById(`${listId}-${option.id}`);
      // jsdom has no scrollIntoView; browsers do.
      if (element !== null && "scrollIntoView" in element) {
        element.scrollIntoView({ block: "nearest" });
      }
    } else if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      pick(active);
    }
  };

  return (
    <div className="space-y-2">
      <SearchInput
        value={search}
        onChange={(next) => {
          setSearch(next);
          setActiveIndex(0);
        }}
        label={t("picker.searchLabel")}
      />

      {isPending ? (
        <div role="status" aria-label={t("picker.loadingLabel")} className="space-y-1.5">
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-12 w-full" />
        </div>
      ) : error ? (
```

The excerpt stops at line 90; use the original file for the remaining 69 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
