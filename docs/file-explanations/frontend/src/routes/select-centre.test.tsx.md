# `frontend/src/routes/select-centre.test.tsx`

## Purpose

Declares `meWithMemberships`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/routes/select-centre.test.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 92 lines and 3,675 bytes. Source hash: `c614d59a6caee60e0cc997a59eed77349238adf4515a6e7a9c5f0a76c17231b0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { screen, waitFor } from "@testing-library/react";`
- Line 2: `import { fireEvent } from "@testing-library/react";`
- Line 3: `import { http, HttpResponse } from "msw";`
- Line 4: `import { describe, expect, it } from "vitest";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../frontend/src/routes/select-centre.test.tsx#L14): `function meWithMemberships(memberships: unknown[], activeCentreId: string | null = null) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { screen, waitFor } from "@testing-library/react";
import { fireEvent } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const selectCentreUrl = "/api/session/centre";

const nileMembership = { centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "teacher" };
const maadiMembership = { centreId: "c-maadi", centreName: "Maadi Learning Hub", centreSlug: "maadi-hub", role: "teacher" };

function meWithMemberships(memberships: unknown[], activeCentreId: string | null = null) {
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

describe("SelectCentrePage", () => {
  it("lists two centres with their translated role", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(meWithMemberships([nileMembership, maadiMembership]))));

    renderRouter("/select-centre");

    expect(await screen.findByRole("button", { name: /Nile Tutoring Centre/ })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Maadi Learning Hub/ })).toBeInTheDocument();
  });

  it("choosing a centre posts its id and navigates to /", async () => {
    let postedCentreId: string | undefined;
    let meCallCount = 0;
    server.use(
      http.get(meUrl, () => {
        meCallCount += 1;
        return HttpResponse.json(
          meCallCount === 1
            ? meWithMemberships([nileMembership, maadiMembership])
            : meWithMemberships([nileMembership, maadiMembership], "c-maadi"),
        );
      }),
      http.post(selectCentreUrl, async ({ request }) => {
        const body = (await request.json()) as { centreId: string };
        postedCentreId = body.centreId;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    const { router } = renderRouter("/select-centre");
    fireEvent.click(await screen.findByRole("button", { name: /Maadi Learning Hub/ }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/");
    });
    expect(postedCentreId).toBe("c-maadi");
  });

  it("no active memberships shows the no-active-centre state with the signed-in email and a single logout button", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(meWithMemberships([]))));

    renderRouter("/select-centre");

    expect(await screen.findByRole("heading", { name: "No active centre" })).toBeInTheDocument();
    expect(screen.getByText("teacher@both.test")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Check again/ })).toBeInTheDocument();
    // The header's own logout button is hidden in this state, so exactly one "Log out" button exists (the body's).
    expect(screen.getAllByRole("button", { name: "Log out" })).toHaveLength(1);
  });

  it("check again refetches /api/me and shows the list once a membership appears", async () => {
    let callCount = 0;
    server.use(
      http.get(meUrl, () => {
        callCount += 1;
        return HttpResponse.json(callCount === 1 ? meWithMemberships([]) : meWithMemberships([nileMembership]));
      }),
    );

    renderRouter("/select-centre");
    fireEvent.click(await screen.findByRole("button", { name: /Check again/ }));

    expect(await screen.findByRole("button", { name: /Nile Tutoring Centre/ })).toBeInTheDocument();
    expect(callCount).toBe(2);
```

The excerpt stops at line 90; use the original file for the remaining 2 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
