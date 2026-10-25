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
