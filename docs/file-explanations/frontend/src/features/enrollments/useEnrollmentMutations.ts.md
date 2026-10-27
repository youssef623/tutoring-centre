# `frontend/src/features/enrollments/useEnrollmentMutations.ts`

## Purpose

Source comment (intent): The one definition of enroll, withdraw and transfer, shared by the roster tab and the student page. No optimistic updates: every operation waits for the server, then invalidates every query that shows any resource it touched — on success and on failure alike, because a refusal (a 409 for the last seat, a stale version) means what the browser shows is out of date. Callers get the outcome and decide how to present it.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 3,355 bytes. Source hash: `8482436e40f45ff406139c64d776eabaa2bc63e53328f3ab5d9e93dccaaa1e35`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { asApiError } from "@/api/apiFetch";`
- Line 3: `import type { ApiError } from "@/api/errors";`
- Line 4: `import {`
- Line 13: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts#L15): `export type EnrollmentOutcome = { ok: true } | { ok: false; error: ApiError };`
- [Line 23](../../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts#L23): `export function useEnrollmentMutations() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:15](../../../../../onboarding-work/enrich_frontend.py#L15)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:22](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L22)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:18](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L18)
- [frontend/src/features/enrollments/TransferDialog.tsx:21](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L21)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:14](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L14)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useQueryClient } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import type { ApiError } from "@/api/errors";
import {
  getGetGroupQueryKey,
  getGetGroupRosterQueryKey,
  getListGroupsQueryKey,
  getListStudentEnrollmentsQueryKey,
  useEnrollStudent,
  useTransferEnrollment,
  useWithdrawEnrollment,
} from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

export type EnrollmentOutcome = { ok: true } | { ok: false; error: ApiError };

/**
 * The one definition of enroll, withdraw and transfer, shared by the roster tab and the student page. No
 * optimistic updates: every operation waits for the server, then invalidates every query that shows any resource
 * it touched — on success and on failure alike, because a refusal (a 409 for the last seat, a stale version)
 * means what the browser shows is out of date. Callers get the outcome and decide how to present it.
 */
export function useEnrollmentMutations() {
  const queryClient = useQueryClient();
  const enrollMutation = useEnrollStudent();
  const withdrawMutation = useWithdrawEnrollment();
  const transferMutation = useTransferEnrollment();

  const invalidate = (groupIds: string[], studentId: string) =>
    Promise.all([
      ...groupIds.flatMap((groupId) => [
        queryClient.invalidateQueries({ queryKey: getGetGroupRosterQueryKey(groupId) }),
        queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(groupId) }),
      ]),
      queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),
      queryClient.invalidateQueries({ queryKey: getListStudentEnrollmentsQueryKey(studentId) }),
    ]);

  const run = async (request: () => Promise<unknown>, groupIds: string[], studentId: string): Promise<EnrollmentOutcome> => {
    try {
      await request();
      return { ok: true };
    } catch (thrown) {
      const error = asApiError(thrown);
      refetchMeOnPermissionDenied(error, queryClient);
      return { ok: false, error };
    } finally {
      await invalidate(groupIds, studentId);
    }
  };

  return {
    enroll: ({ groupId, studentId }: { groupId: string; studentId: string }) =>
      run(() => enrollMutation.mutateAsync({ groupId, data: { studentId } }), [groupId], studentId),

    withdraw: ({
      enrollmentId,
      version,
      groupId,
      studentId,
    }: {
      enrollmentId: string;
      version: number | string;
      groupId: string;
      studentId: string;
    }) => run(() => withdrawMutation.mutateAsync({ id: enrollmentId, data: { version } }), [groupId], studentId),

    // One request: the server ends the old enrollment and opens the new one atomically. Never withdraw-then-enroll.
    transfer: ({
      enrollmentId,
      version,
      sourceGroupId,
      targetGroupId,
      studentId,
    }: {
      enrollmentId: string;
      version: number | string;
      sourceGroupId: string;
      targetGroupId: string;
      studentId: string;
    }) =>
      run(
        () => transferMutation.mutateAsync({ id: enrollmentId, data: { targetGroupId, version } }),
        [sourceGroupId, targetGroupId],
        studentId,
      ),

    isEnrolling: enrollMutation.isPending,
    isWithdrawing: withdrawMutation.isPending,
    isTransferring: transferMutation.isPending,
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
