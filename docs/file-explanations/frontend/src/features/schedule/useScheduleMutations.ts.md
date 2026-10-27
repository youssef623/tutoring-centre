# `frontend/src/features/schedule/useScheduleMutations.ts`

## Purpose

Source comment (intent): The one definition of the four schedule writes, shared by the group's Schedule tab and the timetable. No optimistic updates: every write waits for the server, then invalidates what shows it — the group's rules and every timetable week (any of them may hold a session that changed) — on success and failure alike, because a refusal (a session someone else cancelled, a stale version) means what the browser shows is out of date. Callers get the outcome and decide how to present it.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/schedule/useScheduleMutations.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 66 lines and 2,904 bytes. Source hash: `d72142604d3f5ae0a3fdfa934b5c0bd716dd1123145733be5ad8e3f52d439a42`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useQueryClient } from "@tanstack/react-query";`
- Line 2: `import { asApiError } from "@/api/apiFetch";`
- Line 3: `import type { ApiError } from "@/api/errors";`
- Line 4: `import {`
- Line 13: `import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../../frontend/src/features/schedule/useScheduleMutations.ts#L15): `export type ScheduleOutcome<T = undefined> = { ok: true; value: T } | { ok: false; error: ApiError };`
- [Line 24](../../../../../../frontend/src/features/schedule/useScheduleMutations.ts#L24): `export function useScheduleMutations() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:17](../../../../../onboarding-work/enrich_frontend.py#L17)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:34](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L34)
- [frontend/src/features/schedule/CancelSessionDialog.tsx:16](../../../../../../frontend/src/features/schedule/CancelSessionDialog.tsx#L16)
- [frontend/src/features/schedule/EndScheduleRuleDialog.tsx:19](../../../../../../frontend/src/features/schedule/EndScheduleRuleDialog.tsx#L19)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:18](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
