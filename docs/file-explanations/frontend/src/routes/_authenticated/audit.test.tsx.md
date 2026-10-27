# `frontend/src/routes/_authenticated/audit.test.tsx`

## Purpose

Declares `entry`, `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/audit.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 286 lines and 10,055 bytes. Source hash: `b4d51d9816dc346a80bc8e7bb86fd07f357f0de8780d4865d9568a47661ec881`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { selectOption } from "@/test/selectOption";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L24): `function entry(overrides: Record<string, unknown> = {}) {`
- [Line 40](../../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L40): `function problem(status: number, code: string, detail: string) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/assemble.py:24](../../../../../onboarding-work/assemble.py#L24)
- [docs/onboarding-work/file_guides.py:121](../../../../../onboarding-work/file_guides.py#L121)
- [docs/onboarding-work/inspect-tests.py:21](../../../../../onboarding-work/inspect-tests.py#L21)
- [docs/onboarding-work/reconcile_backend_ledger.py:13](../../../../../onboarding-work/reconcile_backend_ledger.py#L13)
- [frontend/src/api/generated/tutoring-centre.ts:977](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L977)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:36](../../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L36)
- [frontend/src/features/enrollments/RosterTable.tsx:26](../../../../../../frontend/src/features/enrollments/RosterTable.tsx#L26)
- [frontend/src/features/schedule/TimetablePage.tsx:226](../../../../../../frontend/src/features/schedule/TimetablePage.tsx#L226)
- [src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs:30](../../../../../../src/TutoringCentre.Api/Http/ProxyOptionsValidator.cs#L30)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs:48](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/ExtendSchedule/ExtendScheduleHandler.cs#L48)
- [src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs:33](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Audit/AuditEntryConfiguration.cs#L33)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs:83](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/AuditInterceptor.cs#L83)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs:53](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TenantWriteGuardInterceptor.cs#L53)
- [src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs:49](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs#L49)
- [src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs:35](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/AuditReadService.cs#L35)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:55](../../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L55)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs:127](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs#L127)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:35](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L35)
- [tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs:57](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs#L57)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const auditUrl = "/api/audit";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["audit.view"],
};

function entry(overrides: Record<string, unknown> = {}) {
  return {
    id: "a-1",
    occurredAt: "2026-10-01T12:00:00Z",
    actorType: "user",
    actorUserId: "u-1",
    actorDisplayName: "Nile Owner",
    action: "updated",
    entityType: "membership",
    entityId: "m-1",
    changes: [],
    correlationId: null,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string) {
  return HttpResponse.json({ title: "Error", status, code, detail }, { status });
}

describe("AuditPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(auditUrl, () => HttpResponse.json({ items: [], nextCursor: null })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    let releaseList: (() => void) | undefined;
    const gate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(auditUrl, async () => {
        await gate;
        return HttpResponse.json({ items: [entry()], nextCursor: null });
      }),
    );

    renderRouter("/audit");

    expect(await screen.findByRole("status", { name: "Loading audit log" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("Updated")).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(auditUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/audit");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(auditUrl, () => HttpResponse.json({ items: [entry()], nextCursor: null })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("Updated")).toBeInTheDocument();
  });

  it("shows the empty state when there is no activity for the filters", async () => {
    renderRouter("/audit");

```

The excerpt stops at line 90; use the original file for the remaining 196 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
