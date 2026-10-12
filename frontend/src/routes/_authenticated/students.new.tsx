import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

/**
 * Placeholder (Day 42 builds the registration form). Routed and guarded now so the students list (Day 41) can
 * link to it with type-safe routing instead of a string that resolves to nothing.
 */
export const Route = createFileRoute("/_authenticated/students/new")({
  component: RegisterStudentPage,
});

function RegisterStudentPage() {
  const { t } = useTranslation("common");

  return (
    <RouteGuard permission={Permissions.StudentsManage}>
      <p className="text-sm text-muted-foreground">{t("comingSoon")}</p>
    </RouteGuard>
  );
}
