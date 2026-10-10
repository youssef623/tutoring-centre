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
    expect(queryClient.getQueryData(meQueryOptions.queryKey)).toBeUndefined();
  });
});
