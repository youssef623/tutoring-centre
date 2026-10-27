# `frontend/src/routes/_authenticated/students.index.test.tsx`

## Purpose

Declares `student`, `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.index.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 225 lines and 7,980 bytes. Source hash: `071561b0e5a00982c0da983daea87cfa945527c0c5888603e841233405ddc37d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 28](../../../../../../frontend/src/routes/_authenticated/students.index.test.tsx#L28): `function student(overrides: Record<string, unknown> = {}) {`
- [Line 40](../../../../../../frontend/src/routes/_authenticated/students.index.test.tsx#L40): `function problem(status: number, code: string, detail: string) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:9](../../../../../onboarding-work/extend_backend_purposes.py#L9)
- [frontend/e2e/enrollment.spec.ts:40](../../../../../../frontend/e2e/enrollment.spec.ts#L40)
- [frontend/e2e/full-centre-setup.spec.ts:183](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L183)
- [frontend/e2e/students.spec.ts:27](../../../../../../frontend/e2e/students.spec.ts#L27)
- [frontend/src/features/audit/auditFormatting.ts:12](../../../../../../frontend/src/features/audit/auditFormatting.ts#L12)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:52](../../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L52)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:31](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L31)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:51](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L51)
- [frontend/src/features/enrollments/EnrollStudentDialog.tsx:56](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.tsx#L56)
- [frontend/src/features/enrollments/EnrollmentRowMenu.tsx:11](../../../../../../frontend/src/features/enrollments/EnrollmentRowMenu.tsx#L11)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:40](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L40)
- [frontend/src/features/enrollments/RosterTable.tsx:34](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L34)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:149](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L149)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.tsx:24](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.tsx#L24)
- [frontend/src/features/enrollments/StudentPicker.tsx:27](../../../../../../frontend/src/features/enrollments/StudentPicker.tsx#L27)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:117](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L117)
- [frontend/src/features/enrollments/TransferDialog.tsx:66](../../../../../../frontend/src/features/enrollments/TransferDialog.tsx#L66)
- [frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx:47](../../../../../../frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx#L47)
- [frontend/src/features/enrollments/enrollmentMessages.ts:18](../../../../../../frontend/src/features/enrollments/enrollmentMessages.ts#L18)
- [frontend/src/features/students/AddParentDialog.tsx:45](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L45)
- 55 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const studentsUrl = "/api/students";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["students.manage", "students.view"],
};

const nileTeacherMe = {
  ...nileOwnerMe,
  activeRole: "teacher",
  permissions: [],
};

function student(overrides: Record<string, unknown> = {}) {
  return {
    id: "st-1",
    fullName: "أحمد محمود",
    schoolYear: "الصف الثالث الثانوي",
    status: "active",
    primaryParentName: "محمود حسن",
    primaryParentPhone: "+201012345678",
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string) {
  return HttpResponse.json({ title: "Error", status, code, detail }, { status });
}

describe("StudentsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(studentsUrl, () => HttpResponse.json({ items: [], nextCursor: null })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    let releaseList: (() => void) | undefined;
    const listGate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(studentsUrl, async () => {
        await listGate;
        return HttpResponse.json({ items: [student()], nextCursor: null });
      }),
    );

    renderRouter("/students");

    expect(await screen.findByRole("status", { name: "Loading students" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("أحمد محمود")).toBeInTheDocument();
  });

  it("shows the no-search empty state with a register link", async () => {
    renderRouter("/students");

    expect(await screen.findByText("No students yet")).toBeInTheDocument();
    expect(screen.getAllByRole("link", { name: "Register student" }).length).toBeGreaterThan(0);
  });

  it("shows the no-match empty state when a search returns nothing", async () => {
    server.use(
      http.get(studentsUrl, ({ request }) => {
        const search = new URL(request.url).searchParams.get("search");
        return HttpResponse.json({ items: search === null ? [student()] : [], nextCursor: null });
      }),
    );

```

The excerpt stops at line 90; use the original file for the remaining 135 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
