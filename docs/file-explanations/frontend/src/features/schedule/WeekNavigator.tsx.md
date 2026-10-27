# `frontend/src/features/schedule/WeekNavigator.tsx`

## Purpose

Source comment (intent): Previous week, this week, next week, and the week's dates for the current language. The arrows point the way the page reads (mirrored in Arabic), and the label is announced when the week changes.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/WeekNavigator.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 2,111 bytes. Source hash: `f7d48a6cafd1a55f9b374786ae6dc9074685c4736c7ba1f21b127ee5d9eaa2a4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { ChevronLeft, ChevronRight } from "lucide-react";`
- Line 2: `import { useTranslation } from "react-i18next";`
- Line 3: `import { Button } from "@/components/ui/button";`
- Line 4: `import { addDays, formatCalendarRange } from "@/features/schedule/week";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../../frontend/src/features/schedule/WeekNavigator.tsx#L10): `export function WeekNavigator({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetablePage.tsx:19](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { addDays, formatCalendarRange } from "@/features/schedule/week";

/**
 * Previous week, this week, next week, and the week's dates for the current language. The arrows point the way
 * the page reads (mirrored in Arabic), and the label is announced when the week changes.
 */
export function WeekNavigator({
  weekStart,
  currentWeekStart,
  onWeekChange,
}: {
  weekStart: string;
  currentWeekStart: string | null;
  onWeekChange: (weekStart: string) => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const range = formatCalendarRange(weekStart, addDays(weekStart, 6), i18n.language);

  return (
    <nav aria-label={t("timetable.weekLabel", { range })} className="flex flex-wrap items-center gap-2">
      <div className="flex items-center gap-1">
        <Button
          type="button"
          variant="outline"
          size="icon-sm"
          aria-label={t("timetable.previousWeek")}
          onClick={() => {
            onWeekChange(addDays(weekStart, -7));
          }}
        >
          <ChevronLeft className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
        <Button
          type="button"
          variant="outline"
          size="sm"
          disabled={currentWeekStart === null || currentWeekStart === weekStart}
          onClick={() => {
            if (currentWeekStart !== null) {
              onWeekChange(currentWeekStart);
            }
          }}
        >
          {t("timetable.thisWeek")}
        </Button>
        <Button
          type="button"
          variant="outline"
          size="icon-sm"
          aria-label={t("timetable.nextWeek")}
          onClick={() => {
            onWeekChange(addDays(weekStart, 7));
          }}
        >
          <ChevronRight className="size-4 rtl:-scale-x-100" aria-hidden="true" />
        </Button>
      </div>
      <p aria-live="polite" className="text-sm font-medium">
        {range}
      </p>
    </nav>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
