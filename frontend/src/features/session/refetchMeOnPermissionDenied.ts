import type { QueryClient } from "@tanstack/react-query";
import { getGetMeQueryKey } from "@/api/generated/tutoring-centre";
import type { ApiError } from "@/api/errors";

/** After a 403 auth.permission_denied, refetches /api/me so the UI's permission-based hiding catches up with a role that changed server-side mid-session. */
export function refetchMeOnPermissionDenied(error: ApiError, queryClient: QueryClient): void {
  if (error.code === "auth.permission_denied") {
    void queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() });
  }
}
