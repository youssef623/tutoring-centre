import { createFileRoute } from "@tanstack/react-router";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { StatusCard } from "@/features/status/StatusCard";
import { SystemInfoCard } from "@/features/status/SystemInfoCard";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { PageHeader } from "@/components/shared/PageHeader";
import { useTranslation } from "react-i18next";

export const Route = createFileRoute("/status")({
  component: StatusPage,
});

function StatusPage() {
  const { t } = useTranslation("status");
  return (
    <main className="mx-auto w-full max-w-3xl p-6">
      <div className="mb-10 flex items-center justify-between">
        <HessaWordmark />
        <LanguageSwitcher />
      </div>
      <section className="space-y-4">
        <PageHeader title={t("title")} />
        <StatusCard />
        <SystemInfoCard />
      </section>
    </main>
  );
}
