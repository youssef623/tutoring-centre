# `frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx`

## Purpose

Declares `enrollment`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 162 lines and 7,186 bytes. Source hash: `baed822eb2da2fd984ca71e9d0457a669d5141e1b183c9bb0934d17ced1ccb52`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";`
- Line 5: `import { groupFullProblem, problemDetails } from "@/test/msw/problemDetails";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`
- Line 8: `import { selectOption } from "@/test/selectOption";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L23): `function enrollment(overrides: Record<string, unknown> = {}) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../../onboarding-work/extend_backend_purposes.py#L9)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [frontend/e2e/enrollment.spec.ts:7](../../../../../../frontend/e2e/enrollment.spec.ts#L7)
- [frontend/src/features/audit/auditFormatting.ts:17](../../../../../../frontend/src/features/audit/auditFormatting.ts#L17)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:137](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L137)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:63](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L63)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:11](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L11)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:129](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L129)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:137](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L137)
- [frontend/src/features/enrollments/enrollmentMessages.ts:12](../../../../../../frontend/src/features/enrollments/enrollmentMessages.ts#L12)
- [frontend/src/test/msw/problemDetails.ts:33](../../../../../../frontend/src/test/msw/problemDetails.ts#L33)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:379](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L379)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs:20](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/WithdrawEnrollment/WithdrawEnrollmentHandler.cs#L20)
- [src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L9)
- [src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs:31](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/IEnrollmentRepository.cs#L31)
- [src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs:12](../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L12)
- [src/TutoringCentre.Domain/Academics/Enrollment.cs:53](../../../../../../src/TutoringCentre.Domain/Academics/Enrollment.cs#L53)
- [src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs:21](../../../../../../src/TutoringCentre.Infrastructure/Audit/AuditEntityTypeNames.cs#L21)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs:40](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Academics/EnrollmentConfiguration.cs#L40)
- [src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs:52](../../../../../../src/TutoringCentre.Infrastructure/Persistence/ConstraintErrorCatalog.cs#L52)
- 22 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";
import { groupFullProblem, problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { selectOption } from "@/test/selectOption";

const studentUrl = "/api/students/s-1";
const enrollmentsUrl = `${studentUrl}/enrollments`;

const studentDetail = {
  id: "s-1",
  fullName: "Ahmed Mahmoud",
  phone: null,
  schoolYear: null,
  status: "active",
  version: 1,
  parents: [{ parentId: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", relationship: "father", isPrimary: true }],
};

function enrollment(overrides: Record<string, unknown> = {}) {
  return {
    enrollmentId: "e-1",
    groupId: "g-1",
    groupName: "Physics A",
    subjectName: "Physics",
    teacherName: "Amina Hassan",
    status: "active",
    enrolledAt: "2026-10-01T09:00:00+00:00",
    endedAt: null,
    endReason: null,
    version: 7,
    ...overrides,
  };
}

const endedEnrollment = enrollment({
  enrollmentId: "e-0",
  groupId: "g-0",
  groupName: "Chemistry A",
  subjectName: "Chemistry",
  status: "ended",
  endedAt: "2026-09-20T09:00:00+00:00",
  endReason: "transferred",
});

const mathA = group({ id: "g-5", name: "Math A", subjectId: "s-math", subjectName: "Math", teacherName: "Karim Adel", capacity: 10, enrolledCount: 3 });
const mathFull = group({ id: "g-6", name: "Math B", subjectId: "s-math", subjectName: "Math", capacity: 2, enrolledCount: 2 });

describe("StudentEnrollmentsCard", () => {
  let enrollmentCalls = 0;

  beforeEach(() => {
    enrollmentCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get(studentUrl, () => HttpResponse.json(studentDetail)),
      http.get(enrollmentsUrl, () => {
        enrollmentCalls += 1;
        // Deliberately ended-first: the card, not the server's order, puts active enrollments first.
        return HttpResponse.json({ items: [endedEnrollment, enrollment()] });
      }),
      http.get("/api/subjects", () =>
        HttpResponse.json({
          items: [
            { id: "s-math", name: "Math", status: "active", version: 1 },
            { id: "s-phys", name: "Physics", status: "active", version: 1 },
          ],
        }),
      ),
      http.get("/api/groups", () => HttpResponse.json({ items: [mathA, mathFull] })),
    );
  });

  it("without enrollments.view there is no card and no enrollment request", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(me("secretary", ["students.manage", "students.view"]))));

    renderRouter("/students/s-1");

    expect(await screen.findByRole("heading", { name: "Ahmed Mahmoud" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("heading", { name: "Groups" })).not.toBeInTheDocument();
    expect(enrollmentCalls).toBe(0);
  });

  it("shows current groups first with their details, then past ones with end date and reason", async () => {
    renderRouter("/students/s-1");

    const current = await screen.findByRole("list", { name: "Current groups" }, { timeout: 5000 });
```

The excerpt stops at line 90; use the original file for the remaining 72 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
