import { createFileRoute } from "@tanstack/react-router";
import { StatusCard } from "@/features/status/StatusCard";
import { SystemInfoCard } from "@/features/status/SystemInfoCard";

export const Route = createFileRoute("/")({
  component: IndexPage,
});

function IndexPage() {
  return (
    <section className="space-y-4">
      <h1 className="text-2xl font-semibold">System status</h1>
      <StatusCard />
      <SystemInfoCard />
    </section>
  );
}
