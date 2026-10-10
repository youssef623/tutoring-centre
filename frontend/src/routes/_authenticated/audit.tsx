import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/audit")({
  component: AuditPage,
});

function AuditPage() {
  return (
    <RouteGuard permission={Permissions.AuditView}>
      <AuditPageContent />
    </RouteGuard>
  );
}

function AuditPageContent() {
  const { t } = useTranslation("audit");

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
    </section>
  );
}
