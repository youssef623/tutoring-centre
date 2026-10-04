import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { useSession } from "@/features/session/useSession";
import { Card, CardContent } from "@/components/ui/card";

export const Route = createFileRoute("/_authenticated/")({
  component: DashboardPage,
});

// Placeholder; real dashboard content lands after Day 17.
function DashboardPage() {
  const { t } = useTranslation("shell");
  const { me } = useSession();

  if (me === null) {
    return null;
  }

  const activeCentre = me.memberships.find((membership) => membership.centreId === me.activeCentreId);

  return (
    <section className="space-y-4">
      <div className="space-y-1">
        <h1 className="text-2xl font-semibold tracking-tight" dir="auto">
          {t("dashboard.welcome", { name: me.displayName })}
        </h1>
        {activeCentre !== undefined && (
          <p className="text-sm text-muted-foreground" dir="auto">
            {t("dashboard.activeCentre", { centreName: activeCentre.centreName })}
          </p>
        )}
      </div>
      <Card className="border-dashed shadow-none">
        <CardContent className="py-10 text-center text-sm text-muted-foreground">
          {t("dashboard.placeholder")}
        </CardContent>
      </Card>
    </section>
  );
}
