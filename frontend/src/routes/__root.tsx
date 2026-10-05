import type { QueryClient } from "@tanstack/react-query";
import { Outlet, createRootRouteWithContext } from "@tanstack/react-router";
import { Toaster } from "@/components/ui/sonner";

/** Available to every route's `beforeLoad`/`loader` (Day 17: the guard reads the session through it). */
export interface RouterContext {
  queryClient: QueryClient;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
});

function RootLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <Outlet />
      {/* One toaster for the whole app; Day 12's error utilities show API failures here. */}
      <Toaster richColors />
    </div>
  );
}
