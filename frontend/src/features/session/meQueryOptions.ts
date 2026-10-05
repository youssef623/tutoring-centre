import { queryOptions } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import { getGetMeQueryKey, getMe, type MeDto } from "@/api/generated/tutoring-centre";

/**
 * GET /api/me as "the current session, or none" rather than a request that can fail: a 401 means
 * signed out and resolves to null, so a component reading this query never has to treat "no session"
 * as an error state. Any other failure (network, 500) still throws and is a real query error.
 */
export const meQueryOptions = queryOptions({
  queryKey: getGetMeQueryKey(),
  queryFn: async ({ signal }): Promise<MeDto | null> => {
    try {
      return await getMe({ signal });
    } catch (error) {
      if (asApiError(error).status === 401) {
        return null;
      }

      throw error;
    }
  },
  retry: false,
});
