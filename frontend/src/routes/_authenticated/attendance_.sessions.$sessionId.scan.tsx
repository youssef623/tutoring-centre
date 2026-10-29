import { createFileRoute } from "@tanstack/react-router";
import { ScannerPage } from "@/features/scanner/ScannerPage";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/attendance_/sessions/$sessionId/scan")({ component: Scan });
function Scan() {
  const { sessionId } = Route.useParams();
  return <RouteGuard permission={Permissions.AttendanceMark}><ScannerPage key={sessionId} sessionId={sessionId} /></RouteGuard>;
}
