import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const staffUrl = "/api/staff";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["staff.manage", "staff.view"],
};

function member(overrides: Record<string, unknown> = {}) {
  return {
    membershipId: "m-1",
    userId: "u-2",
    displayName: "Nile Secretary",
    email: "secretary@nile.test",
    role: "secretary",
    status: "active",
    version: 1,
    isCurrentUser: false,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("StaffPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(staffUrl, () => HttpResponse.json({ items: [] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    let releaseList: (() => void) | undefined;
    const listGate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(staffUrl, async () => {
        await listGate;
        return HttpResponse.json({ items: [member()] });
      }),
    );

    renderRouter("/staff");

    expect(await screen.findByRole("status", { name: "Loading staff" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("Nile Secretary")).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(staffUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/staff");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member()] })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("Nile Secretary")).toBeInTheDocument();
  });

  it("the current user's row is marked You and has no action menu; inactive rows are muted", async () => {
    server.use(
      http.get(staffUrl, () =>
        HttpResponse.json({
          items: [
            member({ membershipId: "m-owner", displayName: "Nile Owner", role: "owner", isCurrentUser: true }),
            member({ membershipId: "m-inactive", displayName: "Former Secretary", status: "inactive" }),
          ],
        }),
      ),
    );

    renderRouter("/staff");
    const tableBody = within(await screen.findByRole("table")).getAllByRole("rowgroup")[1];
    if (tableBody === undefined) {
      throw new Error("Expected the staff table to have a body.");
    }
    const ownerRow = within(tableBody).getByText("Nile Owner").closest("tr");
    const inactiveRow = within(tableBody).getByText("Former Secretary").closest("tr");
    if (ownerRow === null || inactiveRow === null) {
      throw new Error("Expected both rows to render.");
    }

    expect(within(ownerRow).getByText("You")).toBeInTheDocument();
    expect(within(ownerRow).queryByRole("button", { name: "Open actions menu" })).not.toBeInTheDocument();
    expect(within(inactiveRow).getByText("Inactive")).toBeInTheDocument();
  });

  it("add staff: creates a member, shows the one-time password once, then an empty form on reopen", async () => {
    let body: unknown;
    server.use(
      http.post(staffUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json(
          { membershipId: "m-new", temporaryPassword: "Temp1234Pass" },
          { status: 201 },
        );
      }),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Add staff" }));
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "new@nile.test" } });
    fireEvent.change(screen.getByLabelText("Name"), { target: { value: "New Person" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ email: "new@nile.test", displayName: "New Person", role: "teacher", preferredLocale: "en" });
    });
    expect(await screen.findByText("Temp1234Pass")).toBeInTheDocument();
    expect(screen.getByText(/will not be shown again/)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Done" }));
    fireEvent.click(await screen.findByRole("button", { name: "Add staff" }));
    expect(screen.queryByText("Temp1234Pass")).not.toBeInTheDocument();
    expect(await screen.findByLabelText("Email")).toHaveValue("");
  });

  it("add staff: an existing account shows the no-password message instead", async () => {
    server.use(
      http.post(staffUrl, () =>
        HttpResponse.json({ membershipId: "m-new", temporaryPassword: null }, { status: 201 }),
      ),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Add staff" }));
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "existing@nile.test" } });
    fireEvent.change(screen.getByLabelText("Name"), { target: { value: "Existing Person" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Account already exists")).toBeInTheDocument();
    expect(screen.getByText(/can now select this centre/)).toBeInTheDocument();
  });

  it("add staff: a 409 already_member shows the field error and keeps the dialog open", async () => {
    server.use(
      http.post(staffUrl, () =>
        problem(409, "staff.already_member", "This person is already a member of this centre.", {
          email: ["This person is already a member of this centre."],
        }),
      ),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Add staff" }));
    fireEvent.change(await screen.findByLabelText("Email"), { target: { value: "secretary@nile.test" } });
    fireEvent.change(screen.getByLabelText("Name"), { target: { value: "Nile Secretary" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("This person is already a member of this centre.")).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Add staff" })).toBeInTheDocument();
  });

  it("add staff: client validation blocks an empty email with no request sent", async () => {
    let createCalls = 0;
    server.use(
      http.post(staffUrl, () => {
        createCalls += 1;
        return HttpResponse.json({ membershipId: "m-new", temporaryPassword: "x" }, { status: 201 });
      }),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Add staff" }));
    fireEvent.change(await screen.findByLabelText("Name"), { target: { value: "No Email" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Email is required.")).toBeInTheDocument();
    expect(createCalls).toBe(0);
  });

  it("change role: Save is disabled until the role changes, then sends { role, version }", async () => {
    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member({ role: "teacher" })] })));
    let body: unknown;
    server.use(
      http.put(`${staffUrl}/m-1/role`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }, { timeout: 3000 }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Change role" }));

    const saveButton = await screen.findByRole("button", { name: "Save" });
    expect(saveButton).toBeDisabled();
    expect(screen.getByText(/signed out/)).toBeInTheDocument();

    await selectOption("Role", "Assistant");
    expect(saveButton).toBeEnabled();
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(body).toEqual({ role: "secretary", version: 1 });
    });
    expect(await screen.findByText("Role changed.")).toBeInTheDocument();
  });

  it("change role is not offered on your own row or on inactive rows", async () => {
    server.use(
      http.get(staffUrl, () =>
        HttpResponse.json({
          items: [
            member({ membershipId: "m-owner", displayName: "Nile Owner", isCurrentUser: true }),
            member({ membershipId: "m-inactive", displayName: "Former Secretary", status: "inactive" }),
          ],
        }),
      ),
    );

    renderRouter("/staff");
    const inactiveRow = (await screen.findByText("Former Secretary", {}, { timeout: 5000 })).closest("tr");
    const ownerRow = screen.getByText("Nile Owner", { selector: "td *" }).closest("tr");
    if (inactiveRow === null || ownerRow === null) {
      throw new Error("Expected both rows to render.");
    }

    // Your own row has no action menu at all.
    expect(within(ownerRow).queryByRole("button", { name: "Open actions menu" })).toBeNull();

    // An inactive row's menu offers only Reactivate, never Change role or Deactivate.
    fireEvent.click(within(inactiveRow).getByRole("button", { name: "Open actions menu" }));
    expect(await screen.findByRole("menuitem", { name: "Reactivate" })).toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Change role" })).not.toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Deactivate" })).not.toBeInTheDocument();
  });

  it("a stale 409 on change role shows the conflict toast and refetches the list", async () => {
    let listCalls = 0;
    server.use(
      http.get(staffUrl, () => {
        listCalls += 1;
        return HttpResponse.json({ items: [member()] });
      }),
    );
    server.use(
      http.put(`${staffUrl}/m-1/role`, () =>
        problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),
      ),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Change role" }));
    await selectOption("Role", "Owner");
    const callsBeforeSave = listCalls;
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("This record was changed by someone else. Reload and try again."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(listCalls).toBeGreaterThan(callsBeforeSave);
    });
    await waitFor(() => {
      expect(screen.queryByRole("heading", { name: /Change role for/ })).not.toBeInTheDocument();
    });
  });

  it("deactivate: the confirmation names the person and the centre-only scope, then requires confirmation", async () => {
    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member({ displayName: "Nile Secretary" })] })));
    let deactivateCalls = 0;
    server.use(
      http.post(`${staffUrl}/m-1/deactivate`, () => {
        deactivateCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Deactivate" }));

    expect(await screen.findByRole("heading", { name: "Deactivate Nile Secretary?" })).toBeInTheDocument();
    expect(screen.getByText("Nile Secretary will lose access to this centre only. Their account is never deleted.")).toBeInTheDocument();
    expect(deactivateCalls).toBe(0);

    fireEvent.click(screen.getByRole("button", { name: "Deactivate" }));

    await waitFor(() => {
      expect(deactivateCalls).toBe(1);
    });
    expect(await screen.findByText("Staff member deactivated.")).toBeInTheDocument();
  });

  it("reactivate: offered only on inactive rows, with no confirmation", async () => {
    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member({ status: "inactive" })] })));
    let reactivateCalls = 0;
    server.use(
      http.post(`${staffUrl}/m-1/reactivate`, () => {
        reactivateCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/staff");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    expect(screen.queryByRole("menuitem", { name: "Deactivate" })).not.toBeInTheDocument();
    fireEvent.click(await screen.findByRole("menuitem", { name: "Reactivate" }));

    await waitFor(() => {
      expect(reactivateCalls).toBe(1);
    });
    expect(await screen.findByText("Staff member reactivated.")).toBeInTheDocument();
  });

  it("with staff.manage, the Add button and row menus are present", async () => {
    // The default nileOwnerMe mock already holds both staff.manage and staff.view.
    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member()] })));

    renderRouter("/staff");

    await screen.findByText("Nile Secretary");
    expect(screen.getByRole("button", { name: "Add staff" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Open actions menu" })).toBeInTheDocument();
  });

  it("without staff.view, the no-access state is shown and no staff request is issued", async () => {
    let staffCalls = 0;
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileOwnerMe, permissions: [] })),
      http.get(staffUrl, () => {
        staffCalls += 1;
        return HttpResponse.json({ items: [] });
      }),
    );

    renderRouter("/staff");

    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to dashboard" })).toBeInTheDocument();
    expect(staffCalls).toBe(0);
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(staffUrl, () => HttpResponse.json({ items: [member()] })));

    renderRouter("/staff");

    expect(await screen.findByRole("heading", { name: "الموظفون" })).toBeInTheDocument();
    expect(await screen.findByText("نشط")).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
