import { MembershipStatus, StaffRole, type StaffMemberDto } from "@/api/generated/tutoring-centre";

/**
 * The account picker's convenience filter (Task 46.2): staff rows that could plausibly be linked to a teacher
 * record — active Teacher-role members not already linked to a teacher. This filters *data* (other people's
 * memberships), never what the current session may see or do; that is decided by permissions alone. It is not
 * the rule either: the server re-checks eligibility and its refusal is shown under the select.
 */
export function eligibleStaffForTeacher(
  staff: readonly StaffMemberDto[],
  currentUserId: string | null,
  linkedUserIds: ReadonlySet<string>,
): StaffMemberDto[] {
  return staff.filter(
    (member) =>
      member.role === StaffRole.teacher &&
      member.status === MembershipStatus.active &&
      member.userId !== currentUserId &&
      !linkedUserIds.has(member.userId),
  );
}
