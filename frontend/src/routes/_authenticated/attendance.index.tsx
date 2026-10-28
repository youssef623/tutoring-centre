import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { AttendanceSessionsPage } from "@/features/attendance/AttendanceSessionsPage";
import { isIsoDate } from "@/features/schedule/week";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";

export interface AttendanceSearch {
  /** The day shown; no date means the centre's today, which the server decides. */
  date?: string;
}

export const Route = createFileRoute("/_authenticated/attendance/")({
  validateSearch: (search: Record<string, unknown>): AttendanceSearch => ({
    ...(isIsoDate(search.date) ? { date: search.date } : {}),
  }),
  component: AttendanceRoute,
});

function AttendanceRoute() {
  return (
    <RouteGuard permission={Permissions.AttendanceView}>
      <AttendanceRouteContent />
    </RouteGuard>
  );
}

/** The URL holds the day, so it can be shared, bookmarked and reached with Back. */
function AttendanceRouteContent() {
  const search = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  // The router passes unvalidated keys through from its parents, so a bad value is re-checked here.
  const date = isIsoDate(search.date) ? search.date : undefined;

  return (
    <AttendanceSessionsPage
      date={date}
      onDateChange={(next) => {
        void navigate({ search: (previous) => ({ ...previous, date: next }) });
      }}
    />
  );
}
