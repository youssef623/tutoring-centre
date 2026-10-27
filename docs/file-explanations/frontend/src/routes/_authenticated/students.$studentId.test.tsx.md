# `frontend/src/routes/_authenticated/students.$studentId.test.tsx`

## Purpose

Source comment (intent): Rows render primary first (ParentsCard), so the non-primary parent's row menu is always the second one.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 351 lines and 14,295 bytes. Source hash: `d800f128864c6edb4a015f3538458478cc50116a563835c0286cb278f6048397`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L23): `function parent(overrides: Record<string, unknown> = {}) {`
- [Line 27](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L27): `function studentDetail(overrides: Record<string, unknown> = {}) {`
- [Line 40](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L40): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 48](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L48): `function secondRowMenuButton(): HTMLElement {`
- [Line 104](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L104): `server.use(http.put(studentUrl, () => problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again.")));`
- [Line 112](../../../../../../frontend/src/routes/_authenticated/students.$studentId.test.tsx#L112): `expect(await screen.findByText("This record was changed by someone else. Reload and try again.")).toBeInTheDocument();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/depth_audit.py:3](../../../../../onboarding-work/depth_audit.py#L3)
- [docs/onboarding-work/extend_backend_purposes.py:12](../../../../../onboarding-work/extend_backend_purposes.py#L12)
- [docs/onboarding-work/file_guides.py:80](../../../../../onboarding-work/file_guides.py#L80)
- [docs/onboarding-work/validate_docs.py:41](../../../../../onboarding-work/validate_docs.py#L41)
- [frontend/e2e/students.spec.ts:27](../../../../../../frontend/e2e/students.spec.ts#L27)
- [frontend/src/api/generated/tutoring-centre.ts:405](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L405)
- [frontend/src/components/shared/useParentLookup.ts:6](../../../../../../frontend/src/components/shared/useParentLookup.ts#L6)
- [frontend/src/features/audit/auditFormatting.ts:13](../../../../../../frontend/src/features/audit/auditFormatting.ts#L13)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:13](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L13)
- [frontend/src/features/parents/ParentEditDialog.tsx:37](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L37)
- [frontend/src/features/students/AddParentDialog.tsx:116](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L116)
- [frontend/src/features/students/ParentsCard.tsx:65](../../../../../../frontend/src/features/students/ParentsCard.tsx#L65)
- [frontend/src/features/students/RegisterStudentForm.tsx:27](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L27)
- [frontend/src/features/students/registerStudentSchema.ts:26](../../../../../../frontend/src/features/students/registerStudentSchema.ts#L26)
- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:68](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L68)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:91](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L91)
- [frontend/src/routes/_authenticated/students.new.test.tsx:50](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L50)
- [src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs:121](../../../../../../src/TutoringCentre.Api/Endpoints/StudentEndpoints.cs#L121)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L24)
- [src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs:30](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs#L30)
- 44 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const studentUrl = "/api/students/s-1";

const nileSecretaryMe = {
  userId: "u-1",
  displayName: "Nile Secretary",
  email: "secretary@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "secretary",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "secretary" }],
  permissions: ["students.manage", "students.view"],
};

function parent(overrides: Record<string, unknown> = {}) {
  return { parentId: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", relationship: "father", isPrimary: true, ...overrides };
}

function studentDetail(overrides: Record<string, unknown> = {}) {
  return {
    id: "s-1",
    fullName: "Ahmed Mahmoud",
    phone: null,
    schoolYear: null,
    status: "active",
    version: 1,
    parents: [parent()],
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

/** Rows render primary first (ParentsCard), so the non-primary parent's row menu is always the second one. */
function secondRowMenuButton(): HTMLElement {
  const button = screen.getAllByRole("button", { name: "Open actions menu" })[1];
  if (button === undefined) {
    throw new Error("Expected a second row menu button.");
  }

  return button;
}

describe("StudentDetailPage", () => {
  beforeEach(() => {
    server.use(http.get(meUrl, () => HttpResponse.json(nileSecretaryMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("a 404 shows the Student not found state with a link back to the list", async () => {
    server.use(http.get(studentUrl, () => problem(404, "student.not_found", "This student could not be found.")));

    renderRouter("/students/s-1");

    expect(await screen.findByText("Student not found", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to students" })).toHaveAttribute("href", "/students");
  });

  it("edit sends the loaded version", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));
    let body: unknown;
    server.use(
      http.put(studentUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Edit" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "Ahmed M. Mahmoud" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
```

The excerpt stops at line 90; use the original file for the remaining 261 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
