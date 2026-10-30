import { createFileRoute } from "@tanstack/react-router";
import { ConversationsPage } from "@/features/conversations/ConversationsPage";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/conversations")({
  component: () => (
    <RouteGuard permission={Permissions.ConversationsView}>
      <ConversationsPage />
    </RouteGuard>
  ),
});
