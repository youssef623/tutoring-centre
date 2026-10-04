import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, renderHook, type RenderHookResult, type RenderResult } from "@testing-library/react";
import type { ReactElement, ReactNode } from "react";

function newTestQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
}

/** Renders `ui` inside a fresh QueryClient so tests never share cache; retries off so failures surface immediately. */
export function renderWithQueryClient(ui: ReactElement): RenderResult {
  const queryClient = newTestQueryClient();

  return render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>);
}

/** Same as {@link renderWithQueryClient}, for hooks instead of components. */
export function renderHookWithQueryClient<TResult, TProps>(
  hook: (props: TProps) => TResult,
): RenderHookResult<TResult, TProps> {
  const queryClient = newTestQueryClient();
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );

  return renderHook(hook, { wrapper });
}
