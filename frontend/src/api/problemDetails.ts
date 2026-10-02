/** RFC 9457 Problem Details as produced by the API (Day 11), including the project's extensions. */
export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
  /** Stable machine-readable error code, e.g. "centre.slug_invalid". */
  code?: string;
  traceId?: string;
  correlationId?: string;
  /** Field name (camelCase) → messages; present on validation failures. */
  errors?: Record<string, string[]>;
}

/** True when `value` has the minimum Problem Details shape: a string `title` and a numeric `status`. */
export function isProblemDetails(value: unknown): value is ProblemDetails {
  return (
    typeof value === "object" &&
    value !== null &&
    "title" in value &&
    typeof value.title === "string" &&
    "status" in value &&
    typeof value.status === "number"
  );
}
