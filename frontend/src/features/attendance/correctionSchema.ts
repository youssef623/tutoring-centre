import { z } from "zod";
import { AttendanceStatus } from "@/api/generated/tutoring-centre";

/** The correction reason's bounds; the server enforces the same ones (`correction_reason_invalid`). */
export const ReasonMinLength = 3;
export const ReasonMaxLength = 200;

/**
 * Client-side convenience only: a status was chosen and the trimmed reason is 3-200 characters. The server
 * repeats both rules and adds the ones that need data (the session has started, the status actually changes,
 * the version is current). Messages are translation keys in the "attendance" namespace.
 */
export const correctionFormSchema = z.object({
  status: z.enum(
    [AttendanceStatus.present, AttendanceStatus.late, AttendanceStatus.absent, AttendanceStatus.excused],
    { error: "correction.validation.statusRequired" },
  ),
  reason: z
    .string()
    .trim()
    .min(ReasonMinLength, "correction.validation.reasonTooShort")
    .max(ReasonMaxLength, "correction.validation.reasonTooLong"),
});

export type CorrectionFormValues = z.infer<typeof correctionFormSchema>;
