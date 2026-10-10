import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const settingsUrl = "/api/centre/settings";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["centre.settings.manage"],
};

function settings(overrides: Record<string, unknown> = {}) {
  return {
    name: "Nile Tutoring Centre",
    slug: "nile-centre",
    timeZoneId: "Africa/Cairo",
    defaultLocale: "en",
    version: 1,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("SettingsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(settingsUrl, () => HttpResponse.json(settings())),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the form prefilled with the current settings", async () => {
    let releaseSettings: (() => void) | undefined;
    const gate = new Promise<void>((resolve) => {
      releaseSettings = resolve;
    });
    server.use(
      http.get(settingsUrl, async () => {
        await gate;
        return HttpResponse.json(settings());
      }),
    );

    renderRouter("/settings");

    expect(await screen.findByRole("status", { name: "Loading settings" }, { timeout: 3000 })).toBeInTheDocument();
    releaseSettings?.();

    expect(await screen.findByLabelText("Name")).toHaveValue("Nile Tutoring Centre");
    expect(screen.getByLabelText("Web address")).toHaveValue("nile-centre");
    expect(screen.getByLabelText("Time zone")).toHaveValue("Africa/Cairo");
    expect(screen.getByLabelText("Web address")).toBeDisabled();
    expect(screen.getByLabelText("Time zone")).toBeDisabled();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(settingsUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/settings");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(settingsUrl, () => HttpResponse.json(settings())));
    fireEvent.click(retryButton);

    expect(await screen.findByLabelText("Name")).toHaveValue("Nile Tutoring Centre");
  });

  it("Save is disabled until a field changes, then saves and invalidates /api/me", async () => {
    let body: unknown;
    let meCalls = 0;
    server.use(
      http.get(meUrl, () => {
        meCalls += 1;
        return HttpResponse.json(nileOwnerMe);
      }),
    );
    server.use(
      http.put(settingsUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/settings");
    const nameInput = await screen.findByLabelText("Name");
    const saveButton = screen.getByRole("button", { name: "Save" });
    expect(saveButton).toBeDisabled();

    fireEvent.change(nameInput, { target: { value: "Nile Tutoring Centre — Giza" } });
    expect(saveButton).toBeEnabled();
    const callsBeforeSave = meCalls;
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(body).toEqual({ name: "Nile Tutoring Centre — Giza", defaultLocale: "en", version: 1 });
    });
    expect(await screen.findByText("Settings saved.")).toBeInTheDocument();
    await waitFor(() => {
      expect(meCalls).toBeGreaterThan(callsBeforeSave);
    });
  });

  it("changing the default language select sends the new value", async () => {
    let body: unknown;
    server.use(
      http.put(settingsUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/settings");
    await screen.findByLabelText("Name");
    await selectOption("Default language", "العربية");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ name: "Nile Tutoring Centre", defaultLocale: "ar", version: 1 });
    });
  });

  it("client validation blocks an empty name with no request sent", async () => {
    let saveCalls = 0;
    server.use(
      http.put(settingsUrl, () => {
        saveCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/settings");
    const nameInput = await screen.findByLabelText("Name");
    fireEvent.change(nameInput, { target: { value: "" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Centre name is required.")).toBeInTheDocument();
    expect(saveCalls).toBe(0);
  });

  it("a 409 concurrency.stale on save shows the conflict toast and refetches settings", async () => {
    let settingsCalls = 0;
    server.use(
      http.get(settingsUrl, () => {
        settingsCalls += 1;
        return HttpResponse.json(settings());
      }),
    );
    server.use(
      http.put(settingsUrl, () =>
        problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),
      ),
    );

    renderRouter("/settings");
    const nameInput = await screen.findByLabelText("Name");
    fireEvent.change(nameInput, { target: { value: "Renamed Centre" } });
    const callsBeforeSave = settingsCalls;
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("This record was changed by someone else. Reload and try again."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(settingsCalls).toBeGreaterThan(callsBeforeSave);
    });
  });

  it("a 400 field error from the server is mapped onto the name field", async () => {
    server.use(
      http.put(settingsUrl, () =>
        problem(400, "validation.failed", "Some fields are invalid.", {
          name: ["Centre name must be at most 120 characters."],
        }),
      ),
    );

    renderRouter("/settings");
    const nameInput = await screen.findByLabelText("Name");
    fireEvent.change(nameInput, { target: { value: "Renamed Centre" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Centre name must be at most 120 characters.")).toBeInTheDocument();
  });

  it("without centre.settings.manage, the no-access state is shown and no settings request is issued", async () => {
    let settingsCalls = 0;
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileOwnerMe, permissions: [] })),
      http.get(settingsUrl, () => {
        settingsCalls += 1;
        return HttpResponse.json(settings());
      }),
    );

    renderRouter("/settings");

    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
    expect(settingsCalls).toBe(0);
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");

    renderRouter("/settings");

    expect(await screen.findByRole("heading", { name: "إعدادات المركز" })).toBeInTheDocument();
    expect(await screen.findByLabelText("الاسم")).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
