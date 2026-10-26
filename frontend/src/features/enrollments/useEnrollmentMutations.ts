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
  };
}
