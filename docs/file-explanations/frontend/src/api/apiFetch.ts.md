# `frontend/src/api/apiFetch.ts`

## Purpose

Source comment (intent): What apiFetch throws. It extends Error (so lint's only-throw-error is satisfied) and implements ApiError, so components and messageFor treat it as the plain ApiError interface.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/apiFetch.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 179 lines and 5,489 bytes. Source hash: `cba03bf420ed415871f9e997f05151d9dec7622b17f62d44884e8d0b376e27a5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { getCsrfToken, refreshCsrfToken } from "./csrf";`
- Line 2: `import type { ApiError, ErrorKind } from "./errors";`
- Line 3: `import { isProblemDetails } from "./problemDetails";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../frontend/src/api/apiFetch.ts#L14): `export class ApiRequestError extends Error implements ApiError {`
- [Line 37](../../../../../frontend/src/api/apiFetch.ts#L37): `function kindFromStatus(status: number): ErrorKind {`
- [Line 57](../../../../../frontend/src/api/apiFetch.ts#L57): `export function toApiError(body: unknown, status: number): ApiError {`
- [Line 78](../../../../../frontend/src/api/apiFetch.ts#L78): `export function asApiError(error: unknown): ApiError {`
- [Line 91](../../../../../frontend/src/api/apiFetch.ts#L91): `async function readBody(response: Response): Promise<unknown> {`
- [Line 104](../../../../../frontend/src/api/apiFetch.ts#L104): `function isCsrfError(error: unknown): boolean {`
- [Line 108](../../../../../frontend/src/api/apiFetch.ts#L108): `async function buildHeaders(init: RequestInit | undefined, attachCsrfToken: boolean): Promise<Headers> {`
- [Line 123](../../../../../frontend/src/api/apiFetch.ts#L123): `async function sendOnce<T>(url: string, init: RequestInit | undefined, headers: Headers): Promise<T> {`
- [Line 151](../../../../../frontend/src/api/apiFetch.ts#L151): `* The only function that talks to the API. Network failure → ApiError(network.unreachable, status 0);`
- [Line 158](../../../../../frontend/src/api/apiFetch.ts#L158): `export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/assemble.py:28](../../../../onboarding-work/assemble.py#L28)
- [docs/onboarding-work/enrich_frontend.py:8](../../../../onboarding-work/enrich_frontend.py#L8)
- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../onboarding-work/extend_backend_purposes.py#L8)
- [docs/onboarding-work/file_guides.py:57](../../../../onboarding-work/file_guides.py#L57)
- [docs/onboarding-work/read_frontend.py:9](../../../../onboarding-work/read_frontend.py#L9)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [frontend/e2e/schedule.spec.ts:126](../../../../../frontend/e2e/schedule.spec.ts#L126)
- [frontend/orval.config.ts:14](../../../../../frontend/orval.config.ts#L14)
- [frontend/src/api/apiFetch.test.ts:6](../../../../../frontend/src/api/apiFetch.test.ts#L6)
- [frontend/src/api/generated/tutoring-centre.ts:26](../../../../../frontend/src/api/generated/tutoring-centre.ts#L26)
- [frontend/src/api/showApiError.ts:5](../../../../../frontend/src/api/showApiError.ts#L5)
- [frontend/src/app/queryClient.ts:2](../../../../../frontend/src/app/queryClient.ts#L2)
- [frontend/src/components/shared/FilterBar.tsx:3](../../../../../frontend/src/components/shared/FilterBar.tsx#L3)
- [frontend/src/components/shared/useParentLookup.ts:2](../../../../../frontend/src/components/shared/useParentLookup.ts#L2)
- [frontend/src/features/audit/AuditLog.tsx:5](../../../../../frontend/src/features/audit/AuditLog.tsx#L5)
- [frontend/src/features/dashboard/DashboardOverview.tsx:4](../../../../../frontend/src/features/dashboard/DashboardOverview.tsx#L4)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:5](../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L5)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:4](../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L4)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:5](../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L5)
- [frontend/src/features/enrollments/StudentPicker.tsx:5](../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L5)
- 46 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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

```

The excerpt stops at line 90; use the original file for the remaining 89 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
