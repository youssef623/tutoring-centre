# `frontend/src/routes/_authenticated/teachers.test.tsx`

## Purpose

Source comment (intent): Picks a staff option by its display name (its accessible name also carries the email).

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 322 lines and 12,444 bytes. Source hash: `67cb52d1f18d1efbf4032078b2a4c451ca6776b4876266043b92a66b86c34a71`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L13): `function me(role: "owner" | "secretary", permissions: string[]) {`
- [Line 30](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L30): `function teacher(overrides: Record<string, unknown> = {}) {`
- [Line 44](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L44): `function staffMember(overrides: Record<string, unknown> = {}) {`
- [Line 58](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L58): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 66](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L66): `async function pickStaff(displayName: string) {`
- [Line 73](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L73): `async function openRowMenu(name: string) {`
- [Line 154](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L154): `problem(409, "concurrency.stale", "This record was changed by someone else."),`
- [Line 166](../../../../../../frontend/src/routes/_authenticated/teachers.test.tsx#L166): `await screen.findByText("This record was changed by someone else. Reload and try again."),`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:10](../../../../../onboarding-work/extend_backend_purposes.py#L10)
- [docs/onboarding-work/read-generated-models.py:46](../../../../../onboarding-work/read-generated-models.py#L46)
- [frontend/e2e/auth.spec.ts:34](../../../../../../frontend/e2e/auth.spec.ts#L34)
- [frontend/e2e/enrollment.spec.ts:25](../../../../../../frontend/e2e/enrollment.spec.ts#L25)
- [frontend/e2e/full-centre-setup.spec.ts:67](../../../../../../frontend/e2e/full-centre-setup.spec.ts#L67)
- [frontend/e2e/schedule.spec.ts:40](../../../../../../frontend/e2e/schedule.spec.ts#L40)
- [frontend/e2e/students.spec.ts:95](../../../../../../frontend/e2e/students.spec.ts#L95)
- [frontend/e2e/subjects.spec.ts:72](../../../../../../frontend/e2e/subjects.spec.ts#L72)
- [frontend/e2e/teachers-groups.spec.ts:7](../../../../../../frontend/e2e/teachers-groups.spec.ts#L7)
- [frontend/src/api/generated/tutoring-centre.ts:130](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L130)
- [frontend/src/features/audit/auditFormatting.ts:15](../../../../../../frontend/src/features/audit/auditFormatting.ts#L15)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:57](../../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L57)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:69](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L69)
- [frontend/src/features/enrollments/enrollmentTestData.ts:3](../../../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L3)
- [frontend/src/features/groups/CreateGroupDialog.tsx:35](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L35)
- [frontend/src/features/groups/GroupEditDialogs.tsx:278](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L278)
- [frontend/src/features/groups/GroupsList.tsx:147](../../../../../../frontend/src/features/groups/GroupsList.tsx#L147)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:95](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L95)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:84](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L84)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:87](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L87)
- 129 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";


const meUrl = "/api/me";
const teachersUrl = "/api/teachers";
const staffUrl = "/api/staff";

function me(role: "owner" | "secretary", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Nile Secretary",
    email: `${role}@nile.test`,
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

// The session's capabilities come from /api/me's permission list, exactly as in production — never a mocked useCan.
const ownerMe = me("owner", ["groups.manage", "groups.view", "staff.manage", "staff.view", "subjects.view", "teachers.manage", "teachers.view"]);
const secretaryMe = me("secretary", ["groups.manage", "groups.view", "subjects.view", "teachers.view"]);

function teacher(overrides: Record<string, unknown> = {}) {
  return {
    id: "t-1",
    fullName: "Amina Hassan",
    phone: null,
    status: "active",
    userId: null,
    userDisplayName: null,
    activeGroupCount: 0,
    version: 7,
    ...overrides,
  };
}

function staffMember(overrides: Record<string, unknown> = {}) {
  return {
    membershipId: "m-1",
    userId: "u-eligible",
    displayName: "Eligible Teacher",
    email: "eligible@nile.test",
    role: "teacher",
    status: "active",
    version: 1,
    isCurrentUser: false,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

/** Picks a staff option by its display name (its accessible name also carries the email). */
async function pickStaff(displayName: string) {
  fireEvent.click(await screen.findByRole("combobox", { name: "Staff account" }));
  const option = await screen.findByRole("option", { name: new RegExp(displayName) });
  fireEvent.pointerDown(option);
  fireEvent.click(option);
}

async function openRowMenu(name: string) {
  fireEvent.click(await screen.findByRole("button", { name: `Actions for ${name}` }));
}

describe("TeachersPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(teachersUrl, () => HttpResponse.json({ items: [teacher()] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("owner sees the add button and row actions", async () => {
    renderRouter("/teachers");
```

The excerpt stops at line 90; use the original file for the remaining 232 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
