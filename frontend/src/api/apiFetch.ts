import { getCsrfToken, refreshCsrfToken } from "./csrf";
import type { ApiError, ErrorKind } from "./errors";
import { isProblemDetails } from "./problemDetails";

const CsrfHeaderName = "X-XSRF-TOKEN";

// The token is bound to the signed-in user, so these three requests invalidate it when they succeed.
const SessionChangingPaths = new Set(["/api/auth/login", "/api/auth/logout", "/api/session/centre"]);

/**
 * What apiFetch throws. It extends Error (so lint's only-throw-error is satisfied) and implements ApiError,
 * so components and messageFor treat it as the plain ApiError interface.
 */
export class ApiRequestError extends Error implements ApiError {
  readonly kind: ErrorKind;
  readonly code: string;
  readonly status: number;
  readonly fieldErrors?: Record<string, string[]>;
  readonly correlationId?: string;

  constructor(error: ApiError) {
    super(error.message);
    this.name = "ApiRequestError";
    this.kind = error.kind;
    this.code = error.code;
    this.status = error.status;
    if (error.fieldErrors !== undefined) {
      this.fieldErrors = error.fieldErrors;
    }
    if (error.correlationId !== undefined) {
      this.correlationId = error.correlationId;
    }
  }
}

/** The backend's status table (ResultHttpExtensions) mirrored in exactly one place. */
function kindFromStatus(status: number): ErrorKind {
  switch (status) {
    case 400:
      return "validation";
    case 401:
      return "unauthenticated";
    case 403:
      return "forbidden";
    case 404:
      return "notFound";
    case 409:
      return "conflict";
    case 422:
      return "rule";
    default:
      return "unexpected";
  }
}

/** Converts a response body and status into an ApiError. Problem Details are understood; anything else is "unexpected". */
export function toApiError(body: unknown, status: number): ApiError {
  if (isProblemDetails(body)) {
    return {
      kind: kindFromStatus(status),
      code: body.code ?? `http.${String(status)}`,
      message: body.detail ?? body.title,
      status,
      ...(body.errors === undefined ? {} : { fieldErrors: body.errors }),
      ...(body.correlationId === undefined ? {} : { correlationId: body.correlationId }),
    };
  }

  return {
    kind: "unexpected",
    code: `http.${String(status)}`,
    message: "The server returned an unexpected response.",
    status,
  };
}

/** Components catch unknown values; this returns the ApiError inside them, or a generic one. */
export function asApiError(error: unknown): ApiError {
  if (error instanceof ApiRequestError) {
    return error;
  }

  return {
    kind: "unexpected",
    code: "client.unexpected",
    message: "Something went wrong.",
    status: 0,
  };
}

async function readBody(response: Response): Promise<unknown> {
  const text = await response.text();
  if (text.length === 0) {
    return undefined;
  }

  try {
    return JSON.parse(text) as unknown;
  } catch {
    return undefined;
  }
}

function isCsrfError(error: unknown): boolean {
  return error instanceof ApiRequestError && error.status === 403 && error.code === "auth.csrf_invalid";
}

async function buildHeaders(init: RequestInit | undefined, attachCsrfToken: boolean): Promise<Headers> {
  const headers = new Headers(init?.headers);
  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json");
  }
  if (init?.body !== undefined && init.body !== null && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }
  if (attachCsrfToken) {
    headers.set(CsrfHeaderName, await getCsrfToken());
  }

  return headers;
}

async function sendOnce<T>(url: string, init: RequestInit | undefined, headers: Headers): Promise<T> {
  let response: Response;
  try {
    response = await fetch(url, { credentials: "same-origin", ...init, headers });
  } catch {
    throw new ApiRequestError({
      kind: "unexpected",
      code: "network.unreachable",
      message: "Cannot reach the server.",
      status: 0,
    });
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const body = await readBody(response);

  if (response.ok) {
    // Success bodies are trusted to match the generated contract; the cast lives in one place.
    return body as T;
  }

  throw new ApiRequestError(toApiError(body, response.status));
}

/**
 * The only function that talks to the API. Network failure → ApiError(network.unreachable, status 0);
 * 204 → undefined; 2xx → parsed JSON; otherwise the body is mapped by toApiError and thrown as ApiRequestError.
 *
 * Every non-GET/HEAD request carries X-XSRF-TOKEN. A 403 auth.csrf_invalid refreshes the token and retries the
 * request exactly once (a stale token heals itself); a second failure is surfaced as-is, no further retry.
 * A successful login, logout or centre selection refreshes the token, since it is bound to the signed-in user.
 */
export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {
  const method = (init?.method ?? "GET").toUpperCase();
  const isUnsafe = method !== "GET" && method !== "HEAD";

  let result: T;
  try {
    result = await sendOnce<T>(url, init, await buildHeaders(init, isUnsafe));
  } catch (error) {
    if (!isUnsafe || !isCsrfError(error)) {
      throw error;
    }

    await refreshCsrfToken();
    result = await sendOnce<T>(url, init, await buildHeaders(init, isUnsafe));
  }

  if (isUnsafe && SessionChangingPaths.has(url)) {
    void refreshCsrfToken();
  }

  return result;
}
