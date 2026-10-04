import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/select-centre")({
  component: SelectCentrePage,
});

// Placeholder; the real picker lands in Task 17.4.
function SelectCentrePage() {
  return <section>Select centre</section>;
}
