import { waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderHookWithQueryClient } from "@/test/render";
import { useSession } from "./useSession";

describe("useSession", () => {
  it("resolves to a signed-out session (not an error) on a 401 from /api/me", async () => {
    // Arrange (default MSW handler: 401)

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });
    expect(result.current.me).toBeNull();
    expect(result.current.isError).toBe(false);
  });

  it("resolves to the signed-in user's profile when /api/me succeeds", async () => {
    // Arrange
    server.use(
      http.get("/api/me", () =>
        HttpResponse.json({
          userId: "01a1071e-016b-77bc-8135-aaa275518b2a",
          displayName: "Nile Owner",
          email: "owner@nile.test",
          preferredLocale: "en",
          activeCentreId: "01a0febd-7bd7-7b2c-aea0-d975b64f6d06",
          activeRole: "owner",
          memberships: [
            {
              centreId: "01a0febd-7bd7-7b2c-aea0-d975b64f6d06",
              centreName: "Nile Tutoring Centre",
              centreSlug: "nile-centre",
              role: "owner",
            },
          ],
          permissions: [
            "audit.view",
            "centre.settings.manage",
            "staff.manage",
            "staff.view",
            "subjects.manage",
            "subjects.view",
          ],
        }),
      ),
    );

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });
    expect(result.current.me?.displayName).toBe("Nile Owner");
    expect(result.current.me?.activeRole).toBe("owner");
  });

  it("reports a real error for a non-401 failure, e.g. a 500", async () => {
    // Arrange
    server.use(
      http.get("/api/me", () =>
        HttpResponse.json(
          { title: "An unexpected error occurred.", status: 500, code: "server.unexpected" },
          { status: 500 },
        ),
      ),
    );

    // Act
    const { result } = renderHookWithQueryClient(() => useSession());

    // Assert
    await waitFor(() => {
      expect(result.current.isError).toBe(true);
    });
    expect(result.current.me).toBeNull();
  });
});
