import i18next from "@/i18n";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";
import type { AttendanceMarkRequest, AttendanceStudentDto } from "@/api/generated/tutoring-centre";

export interface RowError {
  studentName: string;
  message: string;
}

/** Row errors by student ID. */
export type RowErrors = Readonly<Record<string, RowError>>;

const MarkFieldPath = /^marks\[(\d+)\]\./;

/**
 * Month 1's field-error helper, for rows. The server names a failing row by its position in the request
 * (`marks[2].studentId`); this turns each such path into the student that was sent at that position, with the
 * message for the error's code (the attendance namespace first, then the one error path's text). Paths that are
 * not a row of this save are ignored, as unknown fields are in `applyFieldErrors`.
 */
export function rowErrorsFrom(
  error: ApiError,
  sent: readonly AttendanceMarkRequest[],
  roster: readonly AttendanceStudentDto[],
): RowErrors {
  const result: Record<string, RowError> = {};

  for (const path of Object.keys(error.fieldErrors ?? {})) {
    const match = MarkFieldPath.exec(path);
    const studentId = match === null ? undefined : sent[Number(match[1])]?.studentId;
    if (studentId === undefined) {
      continue;
    }

    const own = i18next.t(`roster.rowErrors.${error.code.replace("attendance.", "")}`, { ns: "attendance", defaultValue: "" });
    result[studentId] = {
      studentName: roster.find((item) => item.studentId === studentId)?.studentName ?? "",
      message: own !== "" ? own : messageFor(error),
    };
  }

  return result;
}
