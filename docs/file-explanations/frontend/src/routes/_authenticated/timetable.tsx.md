# `frontend/src/routes/_authenticated/timetable.tsx`

## Purpose

Source comment (intent): The week's Saturday; any date is moved back to its Saturday, and no date means the centre's current week.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/routes/_authenticated/timetable.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 47 lines and 1,718 bytes. Source hash: `e532a0a291a8bfff287f558184f45d838bf02f47f0faf9c8c557f8a7b179b5e6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- Line 2: `import { TimetablePage, type TimetableSearchPatch } from "@/features/schedule/TimetablePage";`
- Line 3: `import { isIsoDate, startOfWeek } from "@/features/schedule/week";`
- Line 4: `import { Permissions } from "@/features/session/permissions";`
- Line 5: `import { RouteGuard } from "@/features/session/RouteGuard";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L7): `export interface TimetableSearch {`
- [Line 14](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L14): `export const Route = createFileRoute("/_authenticated/timetable")({`
- [Line 24](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L24): `function TimetableRoute() {`
- [Line 33](../../../../../../frontend/src/routes/_authenticated/timetable.tsx#L33): `function TimetableRouteContent() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { TimetablePage, type TimetableSearchPatch } from "@/features/schedule/TimetablePage";
import { isIsoDate, startOfWeek } from "@/features/schedule/week";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";

export interface TimetableSearch {
  /** The week's Saturday; any date is moved back to its Saturday, and no date means the centre's current week. */
  week?: string;
  group?: string;
  teacher?: string;
}

export const Route = createFileRoute("/_authenticated/timetable")({
  validateSearch: (search: Record<string, unknown>): TimetableSearch => ({
    ...(isIsoDate(search.week) ? { week: startOfWeek(search.week) } : {}),
    ...(typeof search.group === "string" && search.group !== "" ? { group: search.group } : {}),
    ...(typeof search.teacher === "string" && search.teacher !== "" ? { teacher: search.teacher } : {}),
  }),
  staticData: { layout: "wide" },
  component: TimetableRoute,
});

function TimetableRoute() {
  return (
    <RouteGuard permission={Permissions.ScheduleView}>
      <TimetableRouteContent />
    </RouteGuard>
  );
}

/** The URL holds the week and both filters, so a week can be shared, bookmarked and reached with Back. */
function TimetableRouteContent() {
  const { week, group, teacher } = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });

  return (
    <TimetablePage
      week={week}
      groupId={group}
      teacherId={teacher}
      onSearchChange={(patch: TimetableSearchPatch) => {
        void navigate({ search: (previous) => ({ ...previous, ...patch }) });
      }}
    />
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
