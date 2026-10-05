import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider, createMemoryHistory, createRouter } from "@tanstack/react-router";
import { render } from "@testing-library/react";
import { createAppQueryClient, setLoginRedirect } from "@/app/queryClient";
import { routeTree } from "@/routeTree.gen";

/**
 * Renders the app's real route tree (not a copy built for tests) at `initialPath`, with a fresh query
 * client wired the same way main.tsx wires the real one — including the session-expiry redirect hook-up,
 * so a global 401 during a test navigates through this test's own router, not a stale one from an earlier test.
 */
export function renderRouter(initialPath: string) {
  const queryClient = createAppQueryClient();
  const history = createMemoryHistory({ initialEntries: [initialPath] });
  const router = createRouter({ routeTree, history, context: { queryClient } });

  setLoginRedirect((redirectTarget) => {
    void router.navigate({ to: "/login", search: { redirect: redirectTarget } });
  });

  const utils = render(
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>,
  );

  return { ...utils, router, queryClient };
}
