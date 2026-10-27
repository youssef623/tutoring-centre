# `frontend/src/routes/_authenticated.test.tsx`

## Purpose

Declares `me`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/routes/_authenticated.test.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 93 lines and 3,943 bytes. Source hash: `ae66854383fb9b9c543e6d1f77f96231df2dc9b79946cfc913c53e937bf6d0fa`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderRouter } from "@/test/renderRouter";`
- Line 6: `import { meQueryOptions } from "@/features/session/meQueryOptions";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 15](../../../../../frontend/src/routes/_authenticated.test.tsx#L15): `function me(overrides: Partial<{ activeCentreId: string | null; memberships: unknown[] }> = {}) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { meQueryOptions } from "@/features/session/meQueryOptions";

const meUrl = "/api/me";
const selectCentreUrl = "/api/session/centre";
const logoutUrl = "/api/auth/logout";

const nileMembership = { centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "teacher" };
const maadiMembership = { centreId: "c-maadi", centreName: "Maadi Learning Hub", centreSlug: "maadi-hub", role: "teacher" };

function me(overrides: Partial<{ activeCentreId: string | null; memberships: unknown[] }> = {}) {
  const memberships = overrides.memberships ?? [nileMembership, maadiMembership];
  const activeCentreId = overrides.activeCentreId === undefined ? "c-nile" : overrides.activeCentreId;
  return {
    userId: "u-1",
    displayName: "Two-Centre Teacher",
    email: "teacher@both.test",
    preferredLocale: "en",
    activeCentreId,
    activeRole: activeCentreId === null ? null : "teacher",
    memberships,
    permissions: [],
  };
}

describe("the protected layout's guard", () => {
  it("redirects a signed-out visitor to /login?redirect=%2F", async () => {
    // The default msw handler already returns 401 for /api/me.
    const { router } = renderRouter("/");

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
    expect(router.state.location.search).toEqual({ redirect: "/" });
  });

  it("redirects a signed-in user with no active centre to /select-centre", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me({ activeCentreId: null }))));

    const { router } = renderRouter("/");

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/select-centre");
    });
  });

  it("renders the shell with the display name and active centre when a centre is set", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/");

    expect(await screen.findByText("Two-Centre Teacher", {}, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.getByText("Nile Tutoring Centre")).toBeInTheDocument();
  });
});

describe("global 401 handling", () => {
  it("a later request returning 401 clears the cache and returns to login", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    const { router, queryClient } = renderRouter("/");
    await screen.findByText("Two-Centre Teacher");

    // Switch centre (a real mutation already in the app) now comes back unauthenticated,
    // as if the session had just been revoked server-side.
    server.use(http.post(selectCentreUrl, () => HttpResponse.json({ title: "Unauthorized", status: 401 }, { status: 401 })));
    fireEvent.click(await screen.findByRole("button", { name: "Two-Centre Teacher" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Switch centre" }));
    fireEvent.click(await screen.findByRole("button", { name: /Maadi Learning Hub/ }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
    expect(queryClient.getQueryData(meQueryOptions.queryKey)).toBeUndefined();
  });

  it("logout clears the cache", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())), http.post(logoutUrl, () => new HttpResponse(null, { status: 204 })));

    const { router, queryClient } = renderRouter("/");
    fireEvent.click(await screen.findByRole("button", { name: "Two-Centre Teacher" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Log out" }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
```

The excerpt stops at line 90; use the original file for the remaining 3 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
