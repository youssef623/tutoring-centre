# `frontend/src/routes/login.test.tsx`

## Purpose

Declares `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../frontend/src/routes/login.test.tsx). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 108 lines and 4,543 bytes. Source hash: `b09a87342118518fbc3c8df8a116e5e0ea4a77d2a135cc37df019e20a75a8dec`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { describe, expect, it } from "vitest";`
- Line 4: `import { server } from "@/test/msw/server";`
- Line 5: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../frontend/src/routes/login.test.tsx#L9): `function problem(status: number, code: string, detail: string) {`

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

const loginUrl = "/api/auth/login";

function problem(status: number, code: string, detail: string) {
  return HttpResponse.json({ title: "Error", status, code, detail }, { status });
}

describe("LoginPage", () => {
  it("empty submit shows translated field errors and sends no request", async () => {
    let loginCalls = 0;
    server.use(http.post(loginUrl, () => {
      loginCalls += 1;
      return problem(401, "auth.invalid_credentials", "Incorrect email or password.");
    }));

    renderRouter("/login");
    fireEvent.click(await screen.findByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("Email is required.")).toBeInTheDocument();
    expect(screen.getByText("Password is required.")).toBeInTheDocument();
    expect(loginCalls).toBe(0);
  });

  it("a 401 shows the generic message and never mentions the email", async () => {
    server.use(http.post(loginUrl, () => problem(401, "auth.invalid_credentials", "Incorrect email or password.")));

    renderRouter("/login");
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "owner@nile.test" } });
    fireEvent.change(screen.getByLabelText("Password"), { target: { value: "wrong-password" } });
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    const alert = await screen.findByRole("alert");
    expect(alert).toHaveTextContent("Incorrect email or password.");
    expect(alert).not.toHaveTextContent("owner@nile.test");
  });

  it("a 429 shows the rate-limited message", async () => {
    server.use(http.post(loginUrl, () => problem(429, "auth.rate_limited", "Too many login attempts.")));

    renderRouter("/login");
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "owner@nile.test" } });
    fireEvent.change(screen.getByLabelText("Password"), { target: { value: "whatever" } });
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByRole("alert")).toHaveTextContent("Too many attempts. Please wait a minute and try again.");
  });

  it("success with an active centre navigates to the redirect value", async () => {
    const me = {
      userId: "01a0febd-0000-0000-0000-000000000001",
      displayName: "Nile Owner",
      email: "owner@nile.test",
      preferredLocale: "en",
      activeCentreId: "01a0febd-0000-0000-0000-00000000c001",
      activeRole: "owner",
      memberships: [
        { centreId: "01a0febd-0000-0000-0000-00000000c001", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" },
      ],
      permissions: [],
    };
    server.use(
      http.post(loginUrl, () => HttpResponse.json(me)),
      http.get("/api/me", () => HttpResponse.json(me)),
    );

    const { router } = renderRouter("/login?redirect=%2F");
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "owner@nile.test" } });
    fireEvent.change(screen.getByLabelText("Password"), { target: { value: "correct-password" } });
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/");
    });
  });

  it("an external redirect is ignored and lands on /", async () => {
    const me = {
      userId: "01a0febd-0000-0000-0000-000000000001",
      displayName: "Nile Owner",
      email: "owner@nile.test",
      preferredLocale: "en",
      activeCentreId: "01a0febd-0000-0000-0000-00000000c001",
      activeRole: "owner",
      memberships: [
        { centreId: "01a0febd-0000-0000-0000-00000000c001", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" },
```

The excerpt stops at line 90; use the original file for the remaining 18 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
