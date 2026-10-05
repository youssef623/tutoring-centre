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

  it("no active memberships shows the empty state", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(meWithMemberships([]))));

    renderRouter("/select-centre");

    expect(await screen.findByText("Your account has no active centre. Contact your centre owner.")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Log out" })).toBeInTheDocument();
  });
});
