# `frontend/src/test/msw/problemDetails.ts`

## Purpose

Source comment (intent): A Problem Details response shaped like the API's own (ResultHttpExtensions + ProblemResult.Enrich): status, title by kind, the authored detail, the stable `code`, and the trace and correlation IDs.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/test/msw/problemDetails.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,498 bytes. Source hash: `58db34e9029e7e3e795c3a2517a56d461cefb04d1f7c06d51b58afc666c153aa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { HttpResponse } from "msw";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/test/msw/problemDetails.ts#L15): `export function problemDetails(status: number, code: string, detail: string) {`
- [Line 31](../../../../../../frontend/src/test/msw/problemDetails.ts#L31): `export const groupFullProblem = () => problemDetails(409, "group.full", "This group has no free seats.");`
- [Line 34](../../../../../../frontend/src/test/msw/problemDetails.ts#L34): `export const alreadyEnrolledProblem = () =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.ts:3](../../../../../../frontend/src/api/apiFetch.ts#L3)
- [frontend/src/api/problemDetails.test.ts:2](../../../../../../frontend/src/api/problemDetails.test.ts#L2)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:6](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L6)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:6](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L6)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:5](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L5)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:5](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L5)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:6](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L6)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:6](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L6)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:6](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L6)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { HttpResponse } from "msw";

const TitleByStatus: Record<number, string> = {
  400: "One or more validation errors occurred.",
  403: "You are not allowed to perform this action.",
  404: "The requested resource was not found.",
  409: "The request conflicts with the current state.",
  422: "The request breaks a business rule.",
};

/**
 * A Problem Details response shaped like the API's own (ResultHttpExtensions + ProblemResult.Enrich): status,
 * title by kind, the authored detail, the stable `code`, and the trace and correlation IDs.
 */
export function problemDetails(status: number, code: string, detail: string) {
  return HttpResponse.json(
    {
      type: `https://tools.ietf.org/html/rfc9110#section-15.5.${status === 422 ? "21" : String(status - 399)}`,
      title: TitleByStatus[status] ?? "Error",
      status,
      detail,
      code,
      traceId: "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01",
      correlationId: "test-correlation-id",
    },
    { status, headers: { "Content-Type": "application/problem+json" } },
  );
}

/** 409 when the last seat went to someone else between loading the page and submitting. */
export const groupFullProblem = () => problemDetails(409, "group.full", "This group has no free seats.");

/** 409 when the student already holds an active enrollment in the group. */
export const alreadyEnrolledProblem = () =>
  problemDetails(409, "enrollment.already_enrolled", "This student is already enrolled in this group.");
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
