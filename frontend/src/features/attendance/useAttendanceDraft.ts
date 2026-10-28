import { useCallback, useState } from "react";
import {
  AttendanceStatus,
  type AttendanceMarkRequest,
  type AttendanceStudentDto,
} from "@/api/generated/tutoring-centre";

/** The marks a draft would send against a roster: only rows whose draft differs from the server's record. */
export function changedMarks(
  items: readonly AttendanceStudentDto[],
  draft: Readonly<Record<string, AttendanceStatus>>,
): AttendanceMarkRequest[] {
  return items.flatMap((item) => {
    const status = draft[item.studentId];
    return status !== undefined && status !== item.record?.status ? [{ studentId: item.studentId, status }] : [];
  });
}

/**
 * The teacher's unsaved taps, keyed by student ID, held in component state and nowhere else: the query cache
 * stays the only copy of the records, and nothing is written to browser storage. A tap that puts a row back on
 * the status the server already has is simply not a change. The draft is never edited by a refetch, so a roster
 * that reloads under the teacher (a conflict, a closed window) leaves their taps in place.
 */
export function useAttendanceDraft() {
  const [draft, setDraft] = useState<Record<string, AttendanceStatus>>({});

  const setStatus = useCallback((studentId: string, status: AttendanceStatus) => {
    setDraft((previous) => ({ ...previous, [studentId]: status }));
  }, []);

  /** Present for every student with no record and no tap yet; recorded rows and deliberate taps are left alone. */
  const markUnrecordedPresent = useCallback((items: readonly AttendanceStudentDto[]) => {
    setDraft((previous) => {
      const next = { ...previous };
      for (const item of items) {
        if (item.record === null && next[item.studentId] === undefined) {
          next[item.studentId] = AttendanceStatus.present;
        }
      }
      return next;
    });
  }, []);

  const discard = useCallback(() => {
    setDraft({});
  }, []);

  return { draft, setStatus, markUnrecordedPresent, discard };
}
