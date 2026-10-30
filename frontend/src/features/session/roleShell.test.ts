import { describe, expect, it } from "vitest";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { isPortalRole, sessionHome } from "./roleShell";

const me: MeDto = {
  userId: "user-1",
  displayName: "Portal user",
  email: "parent@example.test",
  preferredLocale: "ar",
  mustChangePassword: false,
  activeCentreId: "nile-centre",
  activeRole: "parent",
  memberships: [],
  permissions: ["attendance.view", "portal.chat"],
  isOperator: false,
};

describe("role shell routing", () => {
  it.each(["parent", "student"] as const)("sends %s to the portal", (role) => {
    expect(isPortalRole(role)).toBe(true);
    expect(sessionHome({ ...me, activeRole: role })).toBe("/portal");
  });

  it.each(["owner", "secretary", "teacher", null] as const)(
    "keeps %s outside the portal shell",
    (role) => {
      expect(isPortalRole(role)).toBe(false);
      expect(sessionHome({ ...me, activeRole: role })).toBe("/");
    },
  );

  it("never treats an unknown role as a portal role", () => {
    const unknown = "future-role" as MeDto["activeRole"];
    expect(isPortalRole(unknown)).toBe(false);
    expect(sessionHome({ ...me, activeRole: unknown })).toBe("/");
  });

  it("requires a password change before selecting a centre or opening a shell", () => {
    expect(sessionHome({ ...me, mustChangePassword: true })).toBe("/change-password");
    expect(sessionHome({ ...me, mustChangePassword: true, activeCentreId: null })).toBe(
      "/change-password",
    );
  });

  it("requires centre selection before opening either shell", () => {
    expect(sessionHome({ ...me, activeCentreId: null })).toBe("/select-centre");
    expect(sessionHome({ ...me, activeCentreId: null, activeRole: "owner" })).toBe(
      "/select-centre",
    );
  });
});
