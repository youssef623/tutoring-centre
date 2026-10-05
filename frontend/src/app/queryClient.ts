import { MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import { getLoginMutationKey } from "@/api/generated/tutoring-centre";

/** Set once from main.tsx, after the router exists, so a session-expiry redirect can go through it. */
type LoginRedirect = (redirectTarget: string) => void;
let loginRedirect: LoginRedirect | null = null;

export function setLoginRedirect(fn: LoginRedirect): void {
  loginRedirect = fn;
}

const loginMutationKey = getLoginMutationKey();

function isLoginMutation(mutationKey: readonly unknown[] | undefined): boolean {
  return mutationKey !== undefined && JSON.stringify(mutationKey) === JSON.stringify(loginMutationKey);
}

/**
 * Builds a query client wired the same way the app's own instance is: a session can end server-side at any
 * time (Day 16 revocation, rate limiting aside), so any request — not just the one on page load — can come
 * back unauthenticated. The login mutation and the "me" query handle a 401 themselves (the session query by
 * resolving it to `null`, never throwing) and are excluded.
 *
 * A factory, not just the one exported instance below, so tests can get a fresh client with the real
 * session-expiry wiring instead of a reimplementation of it.
 */
export function createAppQueryClient(): QueryClient {
  // Holds the client once built, so the error handlers below (defined before it exists) can reach it.
  // A property on a const ref, not a reassigned `let`: the handlers never run until after it is set.
  const self: { client: QueryClient | null } = { client: null };

  function handleSessionExpiry(error: unknown): void {
    if (asApiError(error).kind !== "unauthenticated") {
      return;
    }

    if (window.location.pathname === "/login") {
      return;
    }

    self.client?.clear();
    loginRedirect?.(window.location.pathname + window.location.search);
  }

  const client = new QueryClient({
    defaultOptions: {
      queries: {
        retry: 1, // one retry hides a blip without delaying a real failure for long
        staleTime: 30_000, // data is fresh for 30 s, so remounting a screen doesn't refetch immediately
      },
    },
    queryCache: new QueryCache({
      onError: (error) => {
        handleSessionExpiry(error);
      },
    }),
    mutationCache: new MutationCache({
      onError: (error, _variables, _context, mutation) => {
        if (isLoginMutation(mutation.options.mutationKey)) {
          return;
        }

        handleSessionExpiry(error);
      },
    }),
  });

  self.client = client;
  return client;
}

/** Shared defaults for every query in the app. */
export const queryClient = createAppQueryClient();
