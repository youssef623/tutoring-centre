# `frontend/src/components/shared/SearchInput.tsx`

## Purpose

Source comment (intent): A search box controlled by a URL search parameter (Day 41): the caller owns `value` (read from the route's search) and `onChange` (navigates with the new term). Typing updates the visible text immediately but only calls `onChange` after `debounceMs` of no further typing, so the server is never asked once per keystroke. Clearing is immediate — a deliberate action, not something to debounce.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/SearchInput.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 87 lines and 2,756 bytes. Source hash: `7bc082129353410fee64535b937ba53feba2752fda1708c85ce4d8b9e292c70c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { X } from "lucide-react";`
- Line 2: `import { useEffect, useRef, useState } from "react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import { Input } from "@/components/ui/input";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../frontend/src/components/shared/SearchInput.tsx#L14): `export function SearchInput({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/SearchInput.test.tsx:3](../../../../../../frontend/src/components/shared/SearchInput.test.tsx#L3)
- [frontend/src/features/enrollments/StudentPicker.tsx:8](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L8)
- [frontend/src/routes/_authenticated/students.index.tsx:4](../../../../../../frontend/src/routes/_authenticated/students.index.tsx#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { X } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Input } from "@/components/ui/input";

const DefaultDebounceMs = 300;

/**
 * A search box controlled by a URL search parameter (Day 41): the caller owns `value` (read from the route's
 * search) and `onChange` (navigates with the new term). Typing updates the visible text immediately but only
 * calls `onChange` after `debounceMs` of no further typing, so the server is never asked once per keystroke.
 * Clearing is immediate — a deliberate action, not something to debounce.
 */
export function SearchInput({
  value,
  onChange,
  label,
  debounceMs = DefaultDebounceMs,
}: {
  value: string;
  onChange: (value: string) => void;
  label?: string;
  debounceMs?: number;
}) {
  const { t } = useTranslation("common");
  const [draft, setDraft] = useState(value);
  const [syncedValue, setSyncedValue] = useState(value);
  const onChangeRef = useRef(onChange);
  useEffect(() => {
    onChangeRef.current = onChange;
  }, [onChange]);

  // An external reset (the URL changed from outside this component — a back-navigation, a "clear filters"
  // action elsewhere) must be reflected without waiting for the debounce. Adjusting state while rendering,
  // not in an effect: React re-renders immediately with the adjusted state before the browser paints, so
  // there is no flash of the stale draft the way a `useEffect` sync would produce.
  if (value !== syncedValue) {
    setSyncedValue(value);
    setDraft(value);
  }

  useEffect(() => {
    if (draft === value) {
      return;
    }

    const timeoutId = setTimeout(() => {
      onChangeRef.current(draft);
    }, debounceMs);

    return () => {
      clearTimeout(timeoutId);
    };
    // Only the draft's own changes should restart the timer; `value` and `debounceMs` are read, not watched.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [draft]);

  const handleClear = () => {
    setDraft("");
    onChange("");
  };

  return (
    <div className="relative">
      <Input
        type="search"
        aria-label={label ?? t("search.label")}
        placeholder={t("search.placeholder")}
        value={draft}
        onChange={(event) => {
          setDraft(event.target.value);
        }}
        className={draft === "" ? undefined : "pe-7"}
      />
      {draft !== "" && (
        <button
          type="button"
          aria-label={t("search.clear")}
          onClick={handleClear}
          className="absolute inset-y-0 end-1.5 flex items-center text-muted-foreground hover:text-foreground"
        >
          <X className="size-3.5" aria-hidden="true" />
        </button>
      )}
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
