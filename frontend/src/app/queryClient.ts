import { QueryClient } from "@tanstack/react-query";

/** Shared defaults for every query in the app. */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1, // one retry hides a blip without delaying a real failure for long
      staleTime: 30_000, // data is fresh for 30 s, so remounting a screen doesn't refetch immediately
    },
  },
});
