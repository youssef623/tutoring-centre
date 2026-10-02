export type ReadinessStatus = {
  state: "healthy" | "unavailable";
  checkedAt: Date;
};

/**
 * Calls GET /health/ready (contract in frontend/README.md).
 * 200 → healthy; 503 → unavailable (API up, database down).
 * Any other status, or a network failure, throws: the API itself can't be trusted or reached.
 */
export async function fetchReadiness(): Promise<ReadinessStatus> {
  const response = await fetch("/health/ready");

  if (response.status === 200) {
    return { state: "healthy", checkedAt: new Date() };
  }

  if (response.status === 503) {
    return { state: "unavailable", checkedAt: new Date() };
  }

  throw new Error(`Unexpected readiness status ${String(response.status)}.`);
}
