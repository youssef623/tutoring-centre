# `frontend/src/features/enrollments/SeatMeter.tsx`

## Purpose

Source comment (intent): At or above this share of seats taken (and not yet full) a group is "almost full".

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/SeatMeter.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 56 lines and 2,315 bytes. Source hash: `7f960e8735c165d71e81824a3ee715bf721038a40f7d2a7b5403cc363f7c067f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { Badge } from "@/components/ui/badge";`
- Line 3: `import { cn } from "cn";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/features/enrollments/SeatMeter.tsx#L15): `export function SeatMeter({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/dashboard/DashboardOverview.tsx:12](../../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L12)
- [frontend/src/features/enrollments/GroupChoiceList.tsx:4](../../../../../../frontend/src/features/enrollments/GroupChoiceList.tsx#L4)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:14](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L14)
- [frontend/src/features/enrollments/SeatMeter.test.tsx:3](../../../../../../frontend/src/features/enrollments/SeatMeter.test.tsx#L3)
- [frontend/src/features/groups/GroupsList.tsx:21](../../../../../../frontend/src/features/groups/GroupsList.tsx#L21)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:46](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L46)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";
import { cn } from "cn";

/** At or above this share of seats taken (and not yet full) a group is "almost full". */
const AlmostFullShare = 0.8;

/**
 * Seats as "enrolled / capacity" plus a thin bar, with a badge once few or no seats are left: amber "Almost full"
 * from 80%, red "Full" at capacity. The badge's words carry the state; the bar's colour only repeats it. Both
 * numbers come from the server and are only displayed: the count is advisory, and the server's 409 decides who
 * gets the last seat. The number pair is isolated as LTR so "12 / 20" never reverses inside an Arabic row; the
 * bar is decorative (the text carries the value), so it is hidden from assistive tech.
 */
export function SeatMeter({
  enrolled,
  capacity,
  size = "compact",
}: {
  enrolled: number;
  capacity: number;
  size?: "compact" | "full";
}) {
  const { t } = useTranslation("enrollments");
  const isFull = capacity > 0 && enrolled >= capacity;
  const isAlmostFull = !isFull && capacity > 0 && enrolled / capacity >= AlmostFullShare;
  const percent = capacity > 0 ? Math.min(100, Math.round((enrolled / capacity) * 100)) : 0;

  return (
    <div className={size === "full" ? "w-full space-y-2" : "w-40 space-y-1.5"}>
      <div className="flex items-center gap-2">
        <span
          className={size === "full" ? "text-2xl font-semibold tabular-nums" : "text-sm tabular-nums"}
          dir="ltr"
          aria-label={t("seats.label", {
            enrolledText: String(enrolled),
            capacity,
            capacityText: String(capacity),
          })}
        >
          {`${String(enrolled)} / ${String(capacity)}`}
        </span>
        {isFull && <Badge variant="destructive">{t("seats.full")}</Badge>}
        {isAlmostFull && (
          <Badge className="bg-warning/20 text-warning-foreground dark:text-warning">{t("seats.almostFull")}</Badge>
        )}
      </div>
      <div className="h-1.5 w-full overflow-hidden rounded-full bg-muted" aria-hidden="true">
        <div
          className={cn("h-full rounded-full", isFull ? "bg-destructive" : isAlmostFull ? "bg-warning" : "bg-primary")}
          style={{ inlineSize: `${String(percent)}%` }}
        />
      </div>
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
