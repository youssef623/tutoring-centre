import { createFileRoute } from "@tanstack/react-router";
import { LanguageSwitcher } from "@/features/language/LanguageSwitcher";
import { StatusCard } from "@/features/status/StatusCard";
import { SystemInfoCard } from "@/features/status/SystemInfoCard";

export const Route = createFileRoute("/status")({
  component: StatusPage,
});

function StatusPage() {
  return (
    <main className="mx-auto w-full max-w-3xl p-6">
      <div className="mb-4 flex justify-end">
        <LanguageSwitcher />
      </div>
      <section className="space-y-4">
        <h1 className="text-2xl font-semibold">System status</h1>
        <StatusCard />
        <SystemInfoCard />
      </section>
    </main>
  );
}
