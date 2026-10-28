import { AttendanceStatus } from "@/api/generated/tutoring-centre";

/** The four statuses in the order the toggle shows them. */
export const AttendanceStatusOrder: readonly AttendanceStatus[] = [
  AttendanceStatus.present,
  AttendanceStatus.late,
  AttendanceStatus.absent,
  AttendanceStatus.excused,
];

export function isAttendanceStatus(value: string | undefined): value is AttendanceStatus {
  return AttendanceStatusOrder.some((status) => status === value);
}
