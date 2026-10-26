import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { PageHeader } from "@/components/shared/PageHeader";
import { GroupsOverview, QuickActions } from "@/features/dashboard/DashboardOverview";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useSession } from "@/features/session/useSession";

export const Route = createFileRoute("/_authenticated/")({
  component: DashboardPage,
});

/** The landing page after sign-in: shortcuts, then the centre's seats at a glance. Each part follows permissions. */
function DashboardPage() {
  const { t } = useTranslation("shell");
  const { me } = useSession();

  if (me === null) {
    return null;
  }

  const activeCentre = me.memberships.find((membership) => membership.centreId === me.activeCentreId);

  return (
    <section className="space-y-6">
      <PageHeader
        title={t("dashboard.welcome", { name: me.displayName })}
        {...(activeCentre === undefined
          ? {}
          : { description: t("dashboard.activeCentre", { centreName: activeCentre.centreName }) })}
      />
      <QuickActions />
      <Can permission={Permissions.GroupsView}>
        <GroupsOverview />
      </Can>
    </section>
  );
}
