import { useQuery } from "@tanstack/react-query";
import { fetchReadiness } from "./api";

/** Server state for the status page: cached under ["health", "ready"] and polled every 15 seconds. */
export function useReadiness() {
  return useQuery({
    queryKey: ["health", "ready"],
    queryFn: fetchReadiness,
    refetchInterval: 15_000,
  });
}
