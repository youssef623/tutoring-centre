import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const changePasswordUrl = "/api/auth/change-password";
const antiforgeryUrl = "/api/auth/antiforgery";

function me(overrides: Record<string, unknown> = {}) {
  return {
    userId: "u-1",
    displayName: "Nile Owner",
    email: "owner@nile.test",
    preferredLocale: "en",
    mustChangePassword: false,
    activeCentreId: "c-nile",
    activeRole: "owner",
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
    permissions: [],
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("ChangePasswordPage", () => {
  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("redirects to login when signed out", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json({ title: "Unauthorized", status: 401 }, { status: 401 })));

    const { router } = renderRouter("/change-password");

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/login");
    });
  });

  it("shows the voluntary subtitle for a user who isn't forced to change their password", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/change-password");

    expect(await screen.findByText("Choose a new password for your account.")).toBeInTheDocument();
  });

  it("shows the first-login subtitle when mustChangePassword is true", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me({ mustChangePassword: true }))));

    renderRouter("/change-password");

    expect(
      await screen.findByText("You're signing in with a temporary password. Choose a new password to continue."),
    ).toBeInTheDocument();
  });

  it("client validation blocks an empty submit with no request sent", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));
    let changeCalls = 0;
    server.use(
      http.post(changePasswordUrl, () => {
        changeCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/change-password");
    fireEvent.click(await screen.findByRole("button", { name: "Change password" }));

    expect(await screen.findByText("Enter your current password.")).toBeInTheDocument();
    expect(screen.getByText("The new password must be at least 10 characters.")).toBeInTheDocument();
    expect(changeCalls).toBe(0);
  });

  it("client validation blocks a new password that doesn't match the confirmation", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "OldPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "SomethingElse1" } });
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    expect(await screen.findByText("The passwords don't match.")).toBeInTheDocument();
  });

  it("success: submits the passwords, refreshes CSRF, invalidates /api/me, toasts, and goes to the dashboard", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(me({ activeCentreId: "c-nile" }))));
    let body: unknown;
    let antiforgeryCalls = 0;
    server.use(
      http.post(changePasswordUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
      http.get(antiforgeryUrl, () => {
        antiforgeryCalls += 1;
        return HttpResponse.json({ token: "test-csrf-token" });
      }),
    );

    const { router } = renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "OldPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "BrandNewPassword1" } });
    const callsBeforeSubmit = antiforgeryCalls;
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    await waitFor(() => {
      expect(body).toEqual({ currentPassword: "OldPassword123", newPassword: "BrandNewPassword1" });
    });
    expect(await screen.findByText("Password changed.")).toBeInTheDocument();
    await waitFor(() => {
      expect(antiforgeryCalls).toBeGreaterThan(callsBeforeSubmit);
    });
    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/");
    });
  });

  it("success with no active centre goes to the centre picker instead", async () => {
    // Stateful: mirrors the real server, which clears mustChangePassword once the change succeeds — the
    // centre picker's own route guard would otherwise bounce straight back to /change-password.
    let mustChangePassword = true;
    server.use(
      http.get(meUrl, () => HttpResponse.json(me({ activeCentreId: null, mustChangePassword }))),
      http.post(changePasswordUrl, () => {
        mustChangePassword = false;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    const { router } = renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "TempPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/select-centre");
    });
  });

  it("a wrong current password sets a field error on the current password field", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(me())),
      http.post(changePasswordUrl, () =>
        problem(422, "auth.current_password_invalid", "The current password you entered is incorrect."),
      ),
    );

    renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "WrongPassword1" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    expect(await screen.findByText("The current password you entered is incorrect.")).toBeInTheDocument();
  });

  it("a weak-password field error from the server is mapped onto the new password field", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(me())),
      http.post(changePasswordUrl, () =>
        problem(400, "auth.password_too_weak", "This password does not meet the requirements.", {
          newPassword: ["This password does not meet the requirements."],
        }),
      ),
    );

    renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "OldPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "weakpassword" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "weakpassword" } });
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    expect(await screen.findByText("This password does not meet the requirements.")).toBeInTheDocument();
  });

  it("a 429 rate limit shows the generic toast", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(me())),
      http.post(changePasswordUrl, () => problem(429, "auth.rate_limited", "Too many attempts.")),
    );

    renderRouter("/change-password");
    fireEvent.change(await screen.findByLabelText("Current password"), { target: { value: "OldPassword123" } });
    fireEvent.change(screen.getByLabelText("New password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.change(screen.getByLabelText("Confirm new password"), { target: { value: "BrandNewPassword1" } });
    fireEvent.click(screen.getByRole("button", { name: "Change password" }));

    expect(await screen.findByText("Too many attempts. Please wait a minute and try again.")).toBeInTheDocument();
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(meUrl, () => HttpResponse.json(me())));

    renderRouter("/change-password");

    expect(await screen.findByRole("heading", { name: "تغيير كلمة المرور" })).toBeInTheDocument();
    expect(screen.getByLabelText("كلمة المرور الحالية")).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
