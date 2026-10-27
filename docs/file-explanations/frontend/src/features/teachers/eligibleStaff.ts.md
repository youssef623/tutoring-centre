# `frontend/src/features/teachers/eligibleStaff.ts`

## Purpose

Source comment (intent): The account picker's convenience filter (Task 46.2): staff rows that could plausibly be linked to a teacher record — active Teacher-role members not already linked to a teacher. This filters *data* (other people's memberships), never what the current session may see or do; that is decided by permissions alone. It is not the rule either: the server re-checks eligibility and its refusal is shown under the select.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/eligibleStaff.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 929 bytes. Source hash: `456112699176581c7b80f5e71e802da9a71b0c950e0e909b2b28c4bd2b32ad6c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { MembershipStatus, StaffRole, type StaffMemberDto } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/features/teachers/eligibleStaff.ts#L9): `export function eligibleStaffForTeacher(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/teachers/TeacherAccountDialog.tsx:17](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
