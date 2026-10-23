import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";


const meUrl = "/api/me";
const teachersUrl = "/api/teachers";
const staffUrl = "/api/staff";

function me(role: "owner" | "secretary", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Nile Secretary",
    email: `${role}@nile.test`,
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

// The session's capabilities come from /api/me's permission list, exactly as in production — never a mocked useCan.
const ownerMe = me("owner", ["groups.manage", "groups.view", "staff.manage", "staff.view", "subjects.view", "teachers.manage", "teachers.view"]);
const secretaryMe = me("secretary", ["groups.manage", "groups.view", "subjects.view", "teachers.view"]);

function teacher(overrides: Record<string, unknown> = {}) {
  return {
    id: "t-1",
    fullName: "Amina Hassan",
    phone: null,
    status: "active",
    userId: null,
    userDisplayName: null,
    activeGroupCount: 0,
    version: 7,
    ...overrides,
  };
}

function staffMember(overrides: Record<string, unknown> = {}) {
  return {
    membershipId: "m-1",
    userId: "u-eligible",
    displayName: "Eligible Teacher",
    email: "eligible@nile.test",
    role: "teacher",
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

/** Picks a staff option by its display name (its accessible name also carries the email). */
async function pickStaff(displayName: string) {
  fireEvent.click(await screen.findByRole("combobox", { name: "Staff account" }));
  const option = await screen.findByRole("option", { name: new RegExp(displayName) });
  fireEvent.pointerDown(option);
  fireEvent.click(option);
}

async function openRowMenu(name: string) {
  fireEvent.click(await screen.findByRole("button", { name: `Actions for ${name}` }));
}

describe("TeachersPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(teachersUrl, () => HttpResponse.json({ items: [teacher()] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("owner sees the add button and row actions", async () => {
    renderRouter("/teachers");

    // First render in the file pays the cold-start cost; a full parallel suite run can push it past 3s.
    expect(await screen.findByText("Amina Hassan", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getAllByRole("button", { name: "Add teacher" }).length).toBeGreaterThan(0);
    expect(screen.getByRole("button", { name: "Actions for Amina Hassan" })).toBeInTheDocument();
  });

  it("secretary sees the same list read-only: no add button, no row actions", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(secretaryMe)));

    renderRouter("/teachers");

    expect(await screen.findByText("Amina Hassan")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add teacher" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
  });

  it("shows linked accounts by display name and the active-group count", async () => {
    server.use(
      http.get(teachersUrl, () =>
        HttpResponse.json({
          items: [teacher({ userId: "u-2", userDisplayName: "Two-Centre Teacher", activeGroupCount: 3 })],
        }),
      ),
    );

    renderRouter("/teachers");

    const row = (await screen.findByText("Amina Hassan")).closest("tr");
    expect(row).not.toBeNull();
    expect(within(row as HTMLElement).getByText("Two-Centre Teacher")).toBeInTheDocument();
    expect(within(row as HTMLElement).getByText("3 groups")).toBeInTheDocument();
  });

  it("create sends { fullName, phone } only", async () => {
    let body: unknown;
    server.use(
      http.get(teachersUrl, () => HttpResponse.json({ items: [] })),
      http.post(teachersUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "t-new" }, { status: 201, headers: { Location: "/api/teachers/t-new" } });
      }),
    );

    renderRouter("/teachers");
    fireEvent.click((await screen.findAllByRole("button", { name: "Add teacher" }))[0] as HTMLElement);
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "هالة سمير" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ fullName: "هالة سمير", phone: null });
    });
    expect(await screen.findByText("Teacher added.")).toBeInTheDocument();
  });

  it("a stale edit shows the conflict toast and refetches the list", async () => {
    let listCalls = 0;
    server.use(
      http.get(teachersUrl, () => {
        listCalls += 1;
        return HttpResponse.json({ items: [teacher()] });
      }),
      http.put(`${teachersUrl}/t-1`, () =>
        problem(409, "concurrency.stale", "This record was changed by someone else."),
      ),
    );

    renderRouter("/teachers");
    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Edit details" }));
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "Amina H." } });
    const before = listCalls;
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("This record was changed by someone else. Reload and try again."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(listCalls).toBeGreaterThan(before);
    });
  });

  it("the staff list is requested only when the account dialog opens, and offers only eligible accounts", async () => {
    let staffCalls = 0;
    server.use(
      http.get(teachersUrl, () =>
        HttpResponse.json({
          items: [teacher(), teacher({ id: "t-2", fullName: "Karim Fathy", userId: "u-linked", userDisplayName: "Linked One" })],
        }),
      ),
      http.get(staffUrl, () => {
        staffCalls += 1;
        return HttpResponse.json({
          items: [
            staffMember(),
            staffMember({ membershipId: "m-2", userId: "u-sec", displayName: "A Secretary", email: "sec@nile.test", role: "secretary" }),
            staffMember({ membershipId: "m-3", userId: "u-gone", displayName: "Inactive Teacher", email: "gone@nile.test", status: "inactive" }),
            staffMember({ membershipId: "m-4", userId: "u-linked", displayName: "Linked One", email: "linked@nile.test" }),
          ],
        });
      }),
    );

    renderRouter("/teachers");
    await screen.findByText("Amina Hassan");
    expect(staffCalls).toBe(0);

    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Link account" }));
    expect(await screen.findByText("This account will see only this teacher's groups.")).toBeInTheDocument();
    fireEvent.click(await screen.findByRole("combobox", { name: "Staff account" }));

    expect(await screen.findByRole("option", { name: /Eligible Teacher/ })).toBeInTheDocument();
    expect(screen.queryByRole("option", { name: /A Secretary/ })).not.toBeInTheDocument();
    expect(screen.queryByRole("option", { name: /Inactive Teacher/ })).not.toBeInTheDocument();
    expect(screen.queryByRole("option", { name: /Linked One/ })).not.toBeInTheDocument();
    expect(staffCalls).toBe(1);
  });

  it("linking sends { userId, version }", async () => {
    let body: unknown;
    server.use(
      http.get(staffUrl, () => HttpResponse.json({ items: [staffMember()] })),
      http.put(`${teachersUrl}/t-1/account`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/teachers");
    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Link account" }));
    await pickStaff("Eligible Teacher");
    fireEvent.click(screen.getByRole("button", { name: "Link account" }));

    await waitFor(() => {
      expect(body).toEqual({ userId: "u-eligible", version: 7 });
    });
    expect(await screen.findByText("Account linked.")).toBeInTheDocument();
  });

  it.each([
    [409, "teacher.account_taken", "This account is already linked to another teacher."],
    [422, "teacher.user_not_eligible", "This person is not an eligible Teacher at this centre."],
  ])("a %i %s is shown under the select and refetches the staff list", async (status, code, message) => {
    let staffCalls = 0;
    server.use(
      http.get(staffUrl, () => {
        staffCalls += 1;
        return HttpResponse.json({ items: [staffMember()] });
      }),
      http.put(`${teachersUrl}/t-1/account`, () => problem(status, code, "Refused.")),
    );

    renderRouter("/teachers");
    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Link account" }));
    await pickStaff("Eligible Teacher");
    fireEvent.click(screen.getByRole("button", { name: "Link account" }));

    const fieldError = await screen.findByRole("alert");
    expect(fieldError).toHaveTextContent(message);
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    await waitFor(() => {
      expect(staffCalls).toBeGreaterThan(1);
    });
  });

  it("removing a link sends userId null", async () => {
    let body: unknown;
    server.use(
      http.get(teachersUrl, () =>
        HttpResponse.json({ items: [teacher({ userId: "u-2", userDisplayName: "Two-Centre Teacher" })] }),
      ),
      http.get(staffUrl, () => HttpResponse.json({ items: [] })),
      http.put(`${teachersUrl}/t-1/account`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/teachers");
    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Link account" }));
    fireEvent.click(await screen.findByRole("button", { name: "Remove link" }));

    await waitFor(() => {
      expect(body).toEqual({ userId: null, version: 7 });
    });
  });

  it("archiving a teacher with active groups shows the specific explanation", async () => {
    server.use(
      http.post(`${teachersUrl}/t-1/archive`, () =>
        problem(422, "teacher.has_active_groups", "The teacher has active groups."),
      ),
    );

    renderRouter("/teachers");
    await openRowMenu("Amina Hassan");
    fireEvent.click(await screen.findByRole("menuitem", { name: "Archive" }));
    expect(await screen.findByRole("heading", { name: 'Archive "Amina Hassan"?' })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Archive teacher" }));

    expect(
      await screen.findByText("This teacher has active groups. Reassign or archive them first."),
    ).toBeInTheDocument();
  });

  it("in Arabic the document is rtl, labels are Arabic and counts use Arabic plural forms with Western digits", async () => {
    server.use(
      http.get(teachersUrl, () =>
        HttpResponse.json({
          items: [
            teacher({ activeGroupCount: 2 }),
            teacher({ id: "t-3", fullName: "Mona Said", activeGroupCount: 3 }),
            teacher({ id: "t-4", fullName: "Huda Ali", activeGroupCount: 11 }),
          ],
        }),
      ),
    );
    await i18n.changeLanguage("ar");

    renderRouter("/teachers");

    expect(await screen.findByRole("heading", { name: "المعلمون" })).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
    expect(await screen.findByText("مجموعتان")).toBeInTheDocument();
    expect(screen.getByText("3 مجموعات")).toBeInTheDocument();
    expect(screen.getByText("11 مجموعة")).toBeInTheDocument();
  });
});
