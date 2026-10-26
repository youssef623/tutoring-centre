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
      ],
      permissions: [],
    };
    server.use(
      http.post(loginUrl, () => HttpResponse.json(me)),
      http.get("/api/me", () => HttpResponse.json(me)),
    );

    const { router } = renderRouter("/login?redirect=https%3A%2F%2Fevil.example");
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "owner@nile.test" } });
    fireEvent.change(screen.getByLabelText("Password"), { target: { value: "correct-password" } });
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/");
    });
  });
});
