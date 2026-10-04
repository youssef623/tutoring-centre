import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_authenticated/")({
  component: DashboardPage,
});

// Placeholder; the real dashboard shell and content land in Task 17.6.
function DashboardPage() {
  return <section>Dashboard</section>;
}
