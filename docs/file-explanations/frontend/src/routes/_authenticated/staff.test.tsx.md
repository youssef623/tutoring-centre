# `frontend/src/routes/_authenticated/staff.test.tsx`

## Purpose

Declares `member`, `problem`, `was`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 375 lines and 15,014 bytes. Source hash: `b265be6b5c258462a082ed6288442fc9b147bcd50757333b5c5b68aaeeb27e69`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { selectOption } from "@/test/selectOption";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L24): `function member(overrides: Record<string, unknown> = {}) {`
- [Line 38](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L38): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 268](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L268): `problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),`
- [Line 280](../../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L280): `await screen.findByText("This record was changed by someone else. Reload and try again."),`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:70](../../../../../../db/bootstrap-roles.sh#L70)
- [docs/onboarding-work/extend_backend_purposes.py:12](../../../../../onboarding-work/extend_backend_purposes.py#L12)
- [frontend/e2e/first-login.spec.ts:23](../../../../../../frontend/e2e/first-login.spec.ts#L23)
- [frontend/e2e/staff-audit.spec.ts:23](../../../../../../frontend/e2e/staff-audit.spec.ts#L23)
- [frontend/src/features/staff/ChangeRoleDialog.tsx:28](../../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L28)
- [frontend/src/features/staff/StaffList.tsx:47](../../../../../../frontend/src/features/staff/StaffList.tsx#L47)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:62](../../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L62)
- [frontend/src/features/teachers/eligibleStaff.ts:15](../../../../../../frontend/src/features/teachers/eligibleStaff.ts#L15)
- [frontend/src/routes/_authenticated/staff.tsx:53](../../../../../../frontend/src/routes/_authenticated/staff.tsx#L53)
- [src/TutoringCentre.Application/Staff/StaffErrors.cs:8](../../../../../../src/TutoringCentre.Application/Staff/StaffErrors.cs#L8)
- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:219](../../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L219)
- [tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs:114](../../../../../../tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs#L114)
- [tests/TutoringCentre.Api.Tests/Staff/StaffCrossTenantProbeTests.cs:72](../../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffCrossTenantProbeTests.cs#L72)
- [tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs:211](../../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L211)
- [tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs:154](../../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L154)
- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:84](../../../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L84)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs:84](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs#L84)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs:30](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs#L30)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs:97](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs#L97)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const staffUrl = "/api/staff";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["staff.manage", "staff.view"],
};

function member(overrides: Record<string, unknown> = {}) {
  return {
    membershipId: "m-1",
    userId: "u-2",
    displayName: "Nile Secretary",
    email: "secretary@nile.test",
    role: "secretary",
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

describe("StaffPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(staffUrl, () => HttpResponse.json({ items: [] })),
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
      http.get(staffUrl, async () => {
        await listGate;
        return HttpResponse.json({ items: [member()] });
      }),
    );

    renderRouter("/staff");

    expect(await screen.findByRole("status", { name: "Loading staff" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("Nile Secretary")).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(staffUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/staff");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member()] })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("Nile Secretary")).toBeInTheDocument();
  });

  it("the current user's row is marked You and has no action menu; inactive rows are muted", async () => {
    server.use(
```

The excerpt stops at line 90; use the original file for the remaining 285 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
