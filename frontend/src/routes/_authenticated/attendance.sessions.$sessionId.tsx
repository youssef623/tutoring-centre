import { createFileRoute } from "@tanstack/react-router";
import { SessionAttendancePage } from "@/features/attendance/SessionAttendancePage";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";

export const Route = createFileRoute("/_authenticated/attendance/sessions/$sessionId")({
  component: SessionAttendanceRoute,
});

function SessionAttendanceRoute() {
  return (
    <RouteGuard permission={Permissions.AttendanceView}>
      <SessionAttendanceRouteContent />
    </RouteGuard>
  );
}

function SessionAttendanceRouteContent() {
  const { sessionId } = Route.useParams();

  return <SessionAttendancePage sessionId={sessionId} />;
}
