import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/settings")({
  component: SettingsPage,
});

function SettingsPage() {
  return (
    <RouteGuard permission={Permissions.CentreSettingsManage}>
      <SettingsPageContent />
    </RouteGuard>
  );
}

function SettingsPageContent() {
  const { t } = useTranslation("settings");

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
    </section>
  );
}
