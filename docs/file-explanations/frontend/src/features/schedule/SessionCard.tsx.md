# `frontend/src/features/schedule/SessionCard.tsx`

## Purpose

Source comment (intent): One session: its time range in the centre's zone (left to right in both languages), the group (a link to its Schedule tab), subject, teacher and room. A cancelled session is never hidden: it stays in place, struck through and labelled "Cancelled" in words. `onCancel` is passed only for a future scheduled session, and only inside `schedule.manage`; the card itself makes no permission or role decision.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/SessionCard.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 100 lines and 3,910 bytes. Source hash: `45a5726548153e2d0a5e9caaf6c3318ef5a5731cde267c820d516605c48ecbd1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Link } from "@tanstack/react-router";`
- Line 2: `import { MapPin, MoreVertical, Presentation, XCircle } from "lucide-react";`
- Line 3: `import { useTranslation } from "react-i18next";`
- Line 4: `import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";`
- Line 5: `import { Badge } from "@/components/ui/badge";`
- Line 6: `import { Button } from "@/components/ui/button";`
- Line 7: `import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";`
- Line 8: `import { formatTimeRange } from "@/features/schedule/centreTime";`
- Line 9: `import { cn } from "cn";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../../frontend/src/features/schedule/SessionCard.tsx#L17): `export function SessionCard({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/schedule/TimetableGrid.tsx:4](../../../../../../frontend/src/features/schedule/TimetableGrid.tsx#L4)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Link } from "@tanstack/react-router";
import { MapPin, MoreVertical, Presentation, XCircle } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { TimetableSessionDto } from "@/api/generated/tutoring-centre";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { formatTimeRange } from "@/features/schedule/centreTime";
import { cn } from "cn";

/**
 * One session: its time range in the centre's zone (left to right in both languages), the group (a link to its
 * Schedule tab), subject, teacher and room. A cancelled session is never hidden: it stays in place, struck
 * through and labelled "Cancelled" in words. `onCancel` is passed only for a future scheduled session, and only
 * inside `schedule.manage`; the card itself makes no permission or role decision.
 */
export function SessionCard({
  session,
  timeZoneId,
  onCancel,
}: {
  session: TimetableSessionDto;
  timeZoneId: string;
  onCancel?: () => void;
}) {
  const { t, i18n } = useTranslation("schedule");
  const isCancelled = session.status === "cancelled";
  const timeRange = formatTimeRange(session.startsAt, session.endsAt, timeZoneId, i18n.language);

  return (
    <article
      aria-label={`${session.groupName}, ${timeRange}`}
      data-status={session.status}
      className={cn(
        "space-y-1.5 rounded-lg border border-border bg-card p-2.5 text-sm shadow-xs",
        isCancelled && "border-dashed bg-muted/40 text-muted-foreground shadow-none",
      )}
    >
      <div className="flex items-start justify-between gap-1">
        <time
          dateTime={session.startsAt}
          dir="ltr"
          className={cn("font-medium tabular-nums", isCancelled && "line-through")}
        >
          {timeRange}
        </time>
        {onCancel !== undefined && (
          <DropdownMenu>
            <DropdownMenuTrigger
              render={
                <Button
                  type="button"
                  variant="ghost"
                  size="icon-xs"
                  className="-me-1 -mt-0.5 shrink-0"
                  aria-label={t("actions.openSessionMenu", { group: session.groupName, time: timeRange })}
                />
              }
            >
              <MoreVertical className="size-4" aria-hidden="true" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-auto">
              <DropdownMenuItem variant="destructive" onClick={onCancel}>
                <XCircle className="size-4" aria-hidden="true" />
                {t("actions.cancelSession")}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>

      <Link
        to="/groups/$groupId"
        params={{ groupId: session.groupId }}
        search={{ tab: "schedule" }}
        className={cn("block font-semibold break-words hover:underline", isCancelled && "line-through")}
      >
        <bdi>{session.groupName}</bdi>
      </Link>

      <p className="break-words text-xs text-muted-foreground">
        <bdi>{session.subjectName}</bdi>
      </p>

      <p className="flex items-center gap-1 text-xs text-muted-foreground">
        <Presentation className="size-3.5 shrink-0" aria-hidden="true" />
        <bdi className="min-w-0 break-words">{session.teacherName}</bdi>
      </p>

      {session.room !== null && (
```

The excerpt stops at line 90; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
