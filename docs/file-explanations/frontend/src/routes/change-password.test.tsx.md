# `frontend/src/routes/change-password.test.tsx`

## Purpose

Declares `me`, `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/routes/change-password.test.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 215 lines and 9,478 bytes. Source hash: `3e2a81d8f419a5496c517d2b19d8bec91e43d21ad65dbad33d8819fce75085fb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../frontend/src/routes/change-password.test.tsx#L12): `function me(overrides: Record<string, unknown> = {}) {`
- [Line 27](../../../../../frontend/src/routes/change-password.test.tsx#L27): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const changePasswordUrl = "/api/auth/change-password";
const antiforgeryUrl = "/api/auth/antiforgery";

function me(overrides: Record<string, unknown> = {}) {
  return {
    userId: "u-1",
    displayName: "Nile Owner",
    email: "owner@nile.test",
    preferredLocale: "en",
    mustChangePassword: false,
    activeCentreId: "c-nile",
    activeRole: "owner",
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
    permissions: [],
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("ChangePasswordPage", () => {
  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("redirects to login when signed out", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json({ title: "Unauthorized", status: 401 }, { status: 401 })));

    const { router } = renderRouter("/change-password");

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
  });

  it("shows the voluntary subtitle for a user who isn't forced to change their password", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/change-password");

    expect(await screen.findByText("Choose a new password for your account.")).toBeInTheDocument();
  });

  it("shows the first-login subtitle when mustChangePassword is true", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me({ mustChangePassword: true }))));

    renderRouter("/change-password");

    expect(
      await screen.findByText("You're signing in with a temporary password. Choose a new password to continue."),
    ).toBeInTheDocument();
  });

  it("client validation blocks an empty submit with no request sent", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));
    let changeCalls = 0;
    server.use(
      http.post(changePasswordUrl, () => {
        changeCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/change-password");
    fireEvent.click(await screen.findByRole("button", { name: "Change password" }));

    expect(await screen.findByText("Enter your current password.")).toBeInTheDocument();
    expect(screen.getByText("The new password must be at least 10 characters.")).toBeInTheDocument();
    expect(changeCalls).toBe(0);
  });

  it("client validation blocks a new password that doesn't match the confirmation", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "OldPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
```

The excerpt stops at line 90; use the original file for the remaining 125 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
