import { TeacherWorkspace } from "@/features/dashboard/TeacherWorkspace";
import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { Link } from "@tanstack/react-router";
import { UserPlus } from "lucide-react";
import { buttonVariants } from "@/components/ui/button-variants";
import { GroupsOverview } from "@/features/dashboard/DashboardOverview";
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

  const [welcomeStart, welcomeEnd] = t("dashboard.welcome", { name: "__name__" }).split("__name__");
  return (
    <section className="dashboard space-y-8">
      <div className="dashboard-welcome">
        <h1>
          {welcomeStart}
          <span>{me.displayName}</span>
          {welcomeEnd}
        </h1>
        <Can permission={Permissions.StudentsManage}>
          <Link to="/students/new" className={buttonVariants({ size: "lg" })}>
            <UserPlus aria-hidden="true" />
            {t("dashboard.actions.registerStudent")}
          </Link>
        </Can>
      </div>
      <Can permission={Permissions.GroupsView}>
        {me.activeRole === "teacher" ? <TeacherWorkspace /> : <GroupsOverview />}
      </Can>
    </section>
  );
}
