import { useQueryClient } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import type { ApiError } from "@/api/errors";
import {
  getGetTimetableQueryKey,
  getListGroupScheduleRulesQueryKey,
  useAddScheduleRule,
  useCancelSession,
  useEndScheduleRule,
  useExtendSchedule,
  type AddScheduleRuleRequest,
} from "@/api/generated/tutoring-centre";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";

export type ScheduleOutcome<T = undefined> = { ok: true; value: T } | { ok: false; error: ApiError };

/**
 * The one definition of the four schedule writes, shared by the group's Schedule tab and the timetable. No
 * optimistic updates: every write waits for the server, then invalidates what shows it — the group's rules and
 * every timetable week (any of them may hold a session that changed) — on success and failure alike, because a
 * refusal (a session someone else cancelled, a stale version) means what the browser shows is out of date.
 * Callers get the outcome and decide how to present it.
 */
export function useScheduleMutations() {
  const queryClient = useQueryClient();
  const addMutation = useAddScheduleRule();
  const endMutation = useEndScheduleRule();
  const extendMutation = useExtendSchedule();
  const cancelMutation = useCancelSession();

  const invalidate = (groupId?: string) =>
    Promise.all([
      ...(groupId === undefined ? [] : [queryClient.invalidateQueries({ queryKey: getListGroupScheduleRulesQueryKey(groupId) })]),
      queryClient.invalidateQueries({ queryKey: getGetTimetableQueryKey() }),
    ]);

  const run = async <T>(request: () => Promise<T>, groupId?: string): Promise<ScheduleOutcome<T>> => {
    try {
      return { ok: true, value: await request() };
    } catch (thrown) {
      const error = asApiError(thrown);
      refetchMeOnPermissionDenied(error, queryClient);
      return { ok: false, error };
    } finally {
      await invalidate(groupId);
    }
  };

  return {
    addRule: (groupId: string, data: AddScheduleRuleRequest) =>
      run(async () => Number((await addMutation.mutateAsync({ groupId, data })).sessionsCreated), groupId),

    endRule: ({ ruleId, groupId, lastDate, version }: { ruleId: string; groupId: string; lastDate: string; version: number | string }) =>
      run(() => endMutation.mutateAsync({ id: ruleId, data: { lastDate, version } }), groupId),

    extend: (through: string) => run(async () => Number((await extendMutation.mutateAsync({ data: { through } })).sessionsCreated)),

    cancelSession: ({ sessionId, version }: { sessionId: string; version: number | string }) =>
      run(() => cancelMutation.mutateAsync({ id: sessionId, data: { version } })),

    isAdding: addMutation.isPending,
    isEnding: endMutation.isPending,
    isExtending: extendMutation.isPending,
    isCancelling: cancelMutation.isPending,
  };
}
