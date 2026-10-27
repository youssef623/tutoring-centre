# `frontend/src/api/errors.ts`

## Purpose

Source comment (intent): Mirrors the backend's ErrorKind (src/TutoringCentre.Domain/Common/ErrorKind.cs), plus "unexpected" for failures with no business meaning (500s, network errors, unparseable responses).

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/errors.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 962 bytes. Source hash: `9e3bd85dca200025b513469312ea30c89a5de6b4a10e074ddf50fa9989161afd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../frontend/src/api/errors.ts#L5): `export type ErrorKind =`
- [Line 15](../../../../../frontend/src/api/errors.ts#L15): `export interface ApiError {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.test.ts:31](../../../../../frontend/src/api/apiFetch.test.ts#L31)
- [frontend/src/api/apiFetch.ts:2](../../../../../frontend/src/api/apiFetch.ts#L2)
- [frontend/src/api/errorMessages.test.ts:4](../../../../../frontend/src/api/errorMessages.test.ts#L4)
- [frontend/src/api/errorMessages.ts:2](../../../../../frontend/src/api/errorMessages.ts#L2)
- [frontend/src/api/fieldErrors.ts:3](../../../../../frontend/src/api/fieldErrors.ts#L3)
- [frontend/src/api/showApiError.test.ts:2](../../../../../frontend/src/api/showApiError.test.ts#L2)
- [frontend/src/api/showApiError.ts:3](../../../../../frontend/src/api/showApiError.ts#L3)
- [frontend/src/features/enrollments/enrollmentMessages.ts:3](../../../../../frontend/src/features/enrollments/enrollmentMessages.ts#L3)
- [frontend/src/features/enrollments/useEnrollmentMutations.ts:3](../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts#L3)
- [frontend/src/features/groups/GroupEditDialogs.tsx:9](../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L9)
- [frontend/src/features/groups/showGroupError.ts:4](../../../../../frontend/src/features/groups/showGroupError.ts#L4)
- [frontend/src/features/schedule/scheduleMessages.ts:3](../../../../../frontend/src/features/schedule/scheduleMessages.ts#L3)
- [frontend/src/features/schedule/useScheduleMutations.ts:3](../../../../../frontend/src/features/schedule/useScheduleMutations.ts#L3)
- [frontend/src/features/session/refetchMeOnPermissionDenied.ts:3](../../../../../frontend/src/features/session/refetchMeOnPermissionDenied.ts#L3)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
