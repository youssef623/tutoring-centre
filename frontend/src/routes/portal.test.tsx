import { screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import type { MeDto } from "@/api/generated/tutoring-centre";
import i18n from "@/i18n";
import portalAr from "@/i18n/locales/ar/portal.json";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

function session(role: MeDto["activeRole"]): MeDto {
  return {
    userId: "user-1",
    displayName: "Account profile name",
    email: "portal@example.test",
    preferredLocale: "en",
    mustChangePassword: false,
    activeCentreId: "centre-nile",
    activeRole: role,
    memberships: [
      {
        centreId: "centre-nile",
        centreName: "Nile Tutoring Centre",
        centreSlug: "nile-centre",
        timeZoneId: "Africa/Cairo",
        role: role ?? "owner",
      },
    ],
    permissions: role === "parent" || role === "student" ? ["attendance.view", "portal.chat"] : [],
    isOperator: false,
    portal:
      role === "parent" || role === "student"
        ? { kind: role, subjectName: "Server-resolved subject" }
        : null,
  };
}

function serveMe(me: MeDto) {
  server.use(http.get("/api/me", () => HttpResponse.json(me)));
}

describe("portal route guards", () => {
  beforeEach(async () => {
    await i18n.changeLanguage("en");
  });
  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it.each(["parent", "student"] as const)(
    "redirects %s from a staff route before loading staff data",
    async (role) => {
      serveMe(session(role));
      const students = vi.fn(() => HttpResponse.json({ items: [], nextCursor: null }));
      server.use(http.get("/api/students", students));
      const { router } = renderRouter("/students");
      expect(
        await screen.findByRole("heading", { name: "Centre portal" }, { timeout: 5000 }),
      ).toBeInTheDocument();
      expect(router.state.location.pathname).toBe("/portal");
      expect(students).not.toHaveBeenCalled();
      expect(screen.queryByRole("link", { name: "Students" })).not.toBeInTheDocument();
    },
  );

  it.each(["owner", "secretary", "teacher"] as const)(
    "redirects %s from the portal to the staff home",
    async (role) => {
      serveMe(session(role));
      const { router } = renderRouter("/portal");
      expect(
        await screen.findByRole(
          "heading",
          { name: "Welcome, Account profile name" },
          { timeout: 5000 },
        ),
      ).toBeInTheDocument();
      expect(router.state.location.pathname).toBe("/");
      expect(screen.queryByRole("heading", { name: "Centre portal" })).not.toBeInTheDocument();
    },
  );

  it("never renders the portal shell for an unknown role", async () => {
    serveMe(session("future-role" as MeDto["activeRole"]));
    const { router } = renderRouter("/portal");
    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/");
    });
    expect(
      await screen.findByRole("heading", { name: "Welcome, Account profile name" }),
    ).toBeInTheDocument();
    expect(screen.queryByRole("navigation", { name: "Portal navigation" })).not.toBeInTheDocument();
  });

  it("requires authentication before rendering a portal", async () => {
    const { router } = renderRouter("/portal");
    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
    expect(screen.queryByRole("heading", { name: "Centre portal" })).not.toBeInTheDocument();
  });

  it("renders the server-resolved subject in Arabic RTL with only disabled future services", async () => {
    await i18n.changeLanguage("ar");
    const name = "محمود سامي";
    serveMe({
      ...session("parent"),
      preferredLocale: "ar",
      portal: { kind: "parent", subjectName: name },
    });
    renderRouter("/portal");
    expect(
      await screen.findByRole("heading", { name: portalAr.title }, { timeout: 5000 }),
    ).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
    expect(screen.getByText("Nile Tutoring Centre")).toBeInTheDocument();
    expect(screen.getByRole("button", { name })).toBeInTheDocument();
    expect(screen.queryByText("Account profile name")).not.toBeInTheDocument();
    const navigation = screen.getByRole("navigation", { name: portalAr.navigation });
    const buttons = within(navigation).getAllByRole("button");
    expect(buttons).toHaveLength(2);
    expect(buttons[0]).toHaveTextContent(portalAr.chat);
    expect(buttons[1]).toHaveTextContent(portalAr.attendance);
    for (const button of buttons) {
      expect(button).toBeDisabled();
      expect(button).toHaveTextContent(portalAr.coming);
    }
  });
});
