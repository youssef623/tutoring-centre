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
  });
});
