import { screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { Route as AttendanceIndexRoute } from "@/routes/_authenticated/attendance.index";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

function meWith(permissions: string[]) {
  return {
    userId: "u-1",
    displayName: "Hana Teacher",
    email: "hana@nile.test",
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: "teacher",
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "teacher" }],
    permissions,
  };
}

function signInWith(permissions: string[]) {
  server.use(http.get("/api/me", () => HttpResponse.json(meWith(permissions))));
}

describe("attendance navigation", () => {
  it("shows the Attendance item with attendance.view, and not Students without students.view", async () => {
    signInWith(["attendance.view", "groups.view"]);

    renderRouter("/");

    const nav = within(await screen.findByRole("navigation", {}, { timeout: 3000 }));
    expect(nav.getByRole("link", { name: "Attendance" })).toHaveAttribute("href", "/attendance");
    expect(nav.queryByRole("link", { name: "Students" })).not.toBeInTheDocument();
  });

  it("hides the Attendance item without attendance.view", async () => {
    signInWith(["groups.view"]);

    renderRouter("/");

    const nav = within(await screen.findByRole("navigation", {}, { timeout: 3000 }));
    expect(nav.getByRole("link", { name: "Groups" })).toBeInTheDocument();
    expect(nav.queryByRole("link", { name: "Attendance" })).not.toBeInTheDocument();
  });

  it("keeps a valid date in the URL across a load", async () => {
    signInWith(["attendance.view"]);

    const { router } = renderRouter("/attendance?date=2026-12-05");

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/attendance");
      expect(router.state.matches.at(-1)?.search).toEqual({ date: "2026-12-05" });
    });
  });

  it("validates date as an ISO calendar date and drops anything else", () => {
    const validate = AttendanceIndexRoute.options.validateSearch as (search: Record<string, unknown>) => unknown;

    expect(validate({ date: "2026-12-05" })).toEqual({ date: "2026-12-05" });
    expect(validate({ date: "tomorrow" })).toEqual({});
    expect(validate({ date: "2026-02-30" })).toEqual({});
    expect(validate({})).toEqual({});
  });

  it("shows the no-access state on the sessions page without attendance.view", async () => {
    signInWith(["groups.view"]);

    renderRouter("/attendance");
    expect(await screen.findByRole("heading", { name: "You don't have access to this page" }, { timeout: 3000 })).toBeInTheDocument();
  });
});
