import { createFileRoute } from "@tanstack/react-router";
import { GroupCardsPage } from "@/features/cards/GroupCardsPage";
import { Permissions } from "@/features/session/permissions";
import { RouteGuard } from "@/features/session/RouteGuard";

export const Route = createFileRoute("/_authenticated/groups_/$groupId/cards")({ component: CardsPage });
function CardsPage() {
  const { groupId } = Route.useParams();
  return <RouteGuard permission={Permissions.AttendanceCardsManage}><GroupCardsPage key={groupId} groupId={groupId} /></RouteGuard>;
}
