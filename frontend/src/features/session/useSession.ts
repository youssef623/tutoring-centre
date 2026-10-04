import { useQuery } from "@tanstack/react-query";
import { meQueryOptions } from "./meQueryOptions";

/** The signed-in session: `me` is the current user's profile, or null while signed out. */
export function useSession() {
  const { data, isPending, isError, error } = useQuery(meQueryOptions);

  return {
    me: data ?? null,
    isLoading: isPending,
    isError,
    error,
  };
}
