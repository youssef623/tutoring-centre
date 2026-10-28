import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import type { ApiError } from "@/api/errors";
import {
  getGetSessionAttendanceQueryKey,
  getListAttendanceSessionsQueryKey,
  useMarkAttendance,
  type AttendanceMarkRequest,
  type AttendanceStudentDto,
  type MarkAttendanceResult,
  type SessionAttendanceDto,
} from "@/api/generated/tutoring-centre";
import { showApiError } from "@/api/showApiError";
import { refetchMeOnPermissionDenied } from "@/features/session/refetchMeOnPermissionDenied";
import { rowErrorsFrom, type RowErrors } from "./rowErrors";

type Attempt =
  | { kind: "nothing" }
  | { kind: "saved"; result: MarkAttendanceResult }
  | { kind: "failed"; error: ApiError; sent: AttendanceMarkRequest[]; roster: readonly AttendanceStudentDto[] };

/**
 * Saving a register, with every answer the server can give treated as a normal state. There are no optimistic
 * writes: the cache changes only when the server's roster is refetched. The draft is kept on every refusal and
 * cleared only on success.
 *
 * A first mark of a student that someone else records a moment earlier surfaces as `attendance.concurrent_mark`
 * (409). That is retried exactly once: refetch the roster, work out again which rows still differ, send them.
 * Resending turns the lost insert into an update. A second 409 is shown like any other error and nothing loops.
 */
export function useSaveAttendance(sessionId: string, onSaved: () => void) {
  const { t } = useTranslation("attendance");
  const queryClient = useQueryClient();
  const mutation = useMarkAttendance();
  const [rowErrors, setRowErrors] = useState<RowErrors>({});
  const [windowClosed, setWindowClosed] = useState(false);

  const rosterKey = getGetSessionAttendanceQueryKey(sessionId);
  const refetchRoster = () => queryClient.refetchQueries({ queryKey: rosterKey, exact: true });
  const invalidateAll = () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: rosterKey }),
      queryClient.invalidateQueries({ queryKey: getListAttendanceSessionsQueryKey() }),
    ]);

  /** One request for whatever differs from the roster as it is in the cache right now. */
  const attempt = async (changesFor: (items: readonly AttendanceStudentDto[]) => AttendanceMarkRequest[]): Promise<Attempt> => {
    const roster = queryClient.getQueryData<SessionAttendanceDto>(rosterKey)?.items ?? [];
    const sent = changesFor(roster);
    if (sent.length === 0) {
      return { kind: "nothing" };
    }

    try {
      return { kind: "saved", result: await mutation.mutateAsync({ sessionId, data: { marks: sent } }) };
    } catch (thrown) {
      return { kind: "failed", error: asApiError(thrown), sent, roster };
    }
  };

  const fail = async (error: ApiError, sent: AttendanceMarkRequest[], roster: readonly AttendanceStudentDto[]) => {
    refetchMeOnPermissionDenied(error, queryClient);

    if (error.code === "attendance.window_closed") {
      setWindowClosed(true);
    } else {
      const rows = rowErrorsFrom(error, sent, roster);
      if (Object.keys(rows).length > 0) {
        setRowErrors(rows);
      } else {
        showApiError(error);
      }
    }

    // What the browser shows is out of date after any refusal; the draft stays as the teacher left it.
    await refetchRoster();
    await queryClient.invalidateQueries({ queryKey: getListAttendanceSessionsQueryKey() });
  };

  /** `changesFor` is asked again after a conflict, against the refetched roster. Resolves true when saved. */
  const save = async (changesFor: (items: readonly AttendanceStudentDto[]) => AttendanceMarkRequest[]): Promise<boolean> => {
    setRowErrors({});
    setWindowClosed(false);

    let outcome = await attempt(changesFor);
    if (outcome.kind === "failed" && outcome.error.code === "attendance.concurrent_mark") {
      await refetchRoster();
      outcome = await attempt(changesFor);
    }

    if (outcome.kind === "failed") {
      await fail(outcome.error, outcome.sent, outcome.roster);
      return false;
    }

    if (outcome.kind === "nothing") {
      toast.info(t("save.nothingLeft"));
    } else {
      const created = Number(outcome.result.created);
      const updated = Number(outcome.result.updated);
      const unchanged = Number(outcome.result.unchanged);
      toast.success(t(unchanged > 0 ? "save.savedWithUnchanged" : "save.saved", { created, updated, unchanged }));
    }

    // The draft is dropped only once the refetched roster has landed, so rows never flash their old state.
    await invalidateAll();
    onSaved();
    return true;
  };

  return { save, isSaving: mutation.isPending, rowErrors, windowClosed };
}
