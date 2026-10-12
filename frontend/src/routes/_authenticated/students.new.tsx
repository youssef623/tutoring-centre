import { createFileRoute } from "@tanstack/react-router";
import { useTranslation } from "react-i18next";
import { RegisterStudentForm } from "@/features/students/RegisterStudentForm";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/students/new")({
  component: RegisterStudentPage,
});

function RegisterStudentPage() {
  return (
    <RouteGuard permission={Permissions.StudentsManage}>
      <RegisterStudentPageContent />
    </RouteGuard>
  );
}

function RegisterStudentPageContent() {
  const { t } = useTranslation("students");

  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold tracking-tight">{t("actions.register")}</h1>
      <RegisterStudentForm />
    </section>
  );
}
