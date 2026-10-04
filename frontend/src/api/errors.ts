/**
 * Mirrors the backend's ErrorKind (src/TutoringCentre.Domain/Common/ErrorKind.cs), plus "unexpected"
 * for failures with no business meaning (500s, network errors, unparseable responses).
 */
export type ErrorKind =
  | "validation"
  | "notFound"
  | "conflict"
  | "rule"
  | "forbidden"
  | "unauthenticated"
  | "unexpected";

/** The single error shape every failed API call is turned into (produced by the fetch wrapper from Day 12). */
export interface ApiError {
  kind: ErrorKind;
  /** Stable machine-readable code, e.g. "centre.slug_invalid"; the UI translates it. */
  code: string;
  /** Developer-facing message; never shown to users verbatim. */
  message: string;
  /** HTTP status code of the response. */
  status: number;
  /** Validation messages per field name, when kind is "validation". */
  fieldErrors?: Record<string, string[]>;
  /** Correlation ID to quote when reporting a problem (from Day 11). */
  correlationId?: string;
}
