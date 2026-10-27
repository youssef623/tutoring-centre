# `frontend/src/routes/_authenticated/settings.test.tsx`

## Purpose

Declares `settings`, `problem`, `was`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 232 lines and 7,910 bytes. Source hash: `1d80998e04ed008101d2f8b917522c8b9a405b87b4e773de127bd347415d2656`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { selectOption } from "@/test/selectOption";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L24): `function settings(overrides: Record<string, unknown> = {}) {`
- [Line 35](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L35): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 172](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L172): `problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),`
- [Line 183](../../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L183): `await screen.findByText("This record was changed by someone else. Reload and try again."),`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:8](../../../../../onboarding-work/extend_backend_purposes.py#L8)
- [frontend/e2e/staff-access.spec.ts:32](../../../../../../frontend/e2e/staff-access.spec.ts#L32)
- [frontend/src/api/generated/tutoring-centre.ts:5827](../../../../../../frontend/src/api/generated/tutoring-centre.ts#L5827)
- [frontend/src/features/session/permissions.ts:8](../../../../../../frontend/src/features/session/permissions.ts#L8)
- [frontend/src/features/session/useSession.test.ts:44](../../../../../../frontend/src/features/session/useSession.test.ts#L44)
- [frontend/src/features/settings/settingsSchema.ts:4](../../../../../../frontend/src/features/settings/settingsSchema.ts#L4)
- [frontend/src/features/shell/AppShell.tsx:165](../../../../../../frontend/src/features/shell/AppShell.tsx#L165)
- [frontend/src/i18n/index.ts:12](../../../../../../frontend/src/i18n/index.ts#L12)
- [frontend/src/routeTree.gen.ts:20](../../../../../../frontend/src/routeTree.gen.ts#L20)
- [frontend/src/routes/_authenticated/settings.tsx:24](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L24)
- [src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs:20](../../../../../../src/TutoringCentre.Api/Endpoints/CentreSettingsEndpoints.cs#L20)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:17](../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L17)
- [src/TutoringCentre.Domain/Identity/Permissions.cs:11](../../../../../../src/TutoringCentre.Domain/Identity/Permissions.cs#L11)
- [tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs:20](../../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L20)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:41](../../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L41)
- [tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs:12](../../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L12)
- [tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs:19](../../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const settingsUrl = "/api/centre/settings";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["centre.settings.manage"],
};

function settings(overrides: Record<string, unknown> = {}) {
  return {
    name: "Nile Tutoring Centre",
    slug: "nile-centre",
    timeZoneId: "Africa/Cairo",
    defaultLocale: "en",
    version: 1,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("SettingsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(settingsUrl, () => HttpResponse.json(settings())),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the form prefilled with the current settings", async () => {
    let releaseSettings: (() => void) | undefined;
    const gate = new Promise<void>((resolve) => {
      releaseSettings = resolve;
    });
    server.use(
      http.get(settingsUrl, async () => {
        await gate;
        return HttpResponse.json(settings());
      }),
    );

    renderRouter("/settings");

    expect(await screen.findByRole("status", { name: "Loading settings" }, { timeout: 3000 })).toBeInTheDocument();
    releaseSettings?.();

    expect(await screen.findByLabelText("Name")).toHaveValue("Nile Tutoring Centre");
    expect(screen.getByLabelText("Web address")).toHaveValue("nile-centre");
    expect(screen.getByLabelText("Time zone")).toHaveValue("Africa/Cairo");
    expect(screen.getByLabelText("Web address")).toBeDisabled();
    expect(screen.getByLabelText("Time zone")).toBeDisabled();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(settingsUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/settings");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(settingsUrl, () => HttpResponse.json(settings())));
    fireEvent.click(retryButton);

    expect(await screen.findByLabelText("Name")).toHaveValue("Nile Tutoring Centre");
  });

  it("Save is disabled until a field changes, then saves and invalidates /api/me", async () => {
```

The excerpt stops at line 90; use the original file for the remaining 142 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
