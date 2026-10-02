import { Outlet, createRootRoute } from "@tanstack/react-router";
import { Toaster } from "@/components/ui/sonner";

export const Route = createRootRoute({
  component: RootLayout,
});

function RootLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <main className="mx-auto w-full max-w-3xl p-6">
        <Outlet />
      </main>
      {/* One toaster for the whole app; Day 12's error utilities show API failures here. */}
      <Toaster richColors />
    </div>
  );
}
