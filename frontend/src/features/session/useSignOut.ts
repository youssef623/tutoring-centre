import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { refreshCsrfToken } from "@/api/csrf";
import { useLogout } from "@/api/generated/tutoring-centre";

/** Logs out, refreshes the CSRF token (bound to the now-signed-out user), clears every cached query, then returns to login. */
export function useSignOut(): () => Promise<void> {
  const logoutMutation = useLogout();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  return async () => {
    await logoutMutation.mutateAsync();
    await refreshCsrfToken();
    queryClient.clear();
    await navigate({ to: "/login" });
  };
}
