import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const groupUrl = "/api/groups/g-1";
const teachersUrl = "/api/teachers";

function me(role: "owner" | "teacher", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Two-Centre Teacher",
    email: role === "owner" ? "owner@nile.test" : "teacher@both.test",
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role, timeZoneId: "Africa/Cairo" }],
    permissions,
  };
}

const ownerMe = me("owner", [
  "enrollments.manage",
  "enrollments.view",
  "groups.manage",
  "groups.view",
  "schedule.view",
  "students.view",
  "subjects.view",
  "teachers.manage",
  "teachers.view",
]);
const teacherMe = me("teacher", ["groups.view", "subjects.view"]);

function group(overrides: Record<string, unknown> = {}) {
  return {
    id: "g-1",
    name: "Physics A",
    subjectId: "s-phys",
    subjectName: "Physics",
    teacherId: "t-1",
    teacherName: "Amina Hassan",
    capacity: 20,
    enrolledCount: 4,
    status: "active",
    version: 11,
    ...overrides,
  };
}

const teachers = [
  { id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },
  { id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 0, version: 1 },
];

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("GroupDetailPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(groupUrl, () => HttpResponse.json(group())),
      http.get(teachersUrl, () => HttpResponse.json({ items: teachers })),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get(`${groupUrl}/roster`, () =>
        HttpResponse.json({ groupId: "g-1", groupName: "Physics A", capacity: 20, enrolledCount: 0, items: [] }),
      ),
      http.get(`${groupUrl}/schedule-rules`, () => HttpResponse.json({ items: [] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows the overview with seats as a left-to-right number pair", async () => {
    renderRouter("/groups/g-1");

    // First render in the file pays the cold-start cost; a full parallel suite run can push it past 3s.
    expect(await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Overview" })).toHaveAttribute("aria-selected", "true");
    expect(screen.getByText("4 / 20")).toHaveAttribute("dir", "ltr");
  });

  it("a 404 shows the single not-found state", async () => {
    server.use(http.get(groupUrl, () => problem(404, "group.not_found", "This group could not be found.")));

    renderRouter("/groups/g-1");

    expect(await screen.findByRole("heading", { name: "Group not found" }, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to groups" })).toBeInTheDocument();
  });

  it("the tab parameter selects the tab, and Roster shows the roster", async () => {
    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("No students enrolled yet", {}, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Roster" })).toHaveAttribute("aria-selected", "true");
  });

  it("choosing a tab writes it to the URL", async () => {
    const { router } = renderRouter("/groups/g-1");

    fireEvent.click(await screen.findByRole("tab", { name: "Schedule" }, { timeout: 3000 }));

    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({ tab: "schedule" });
    });
    // Day 53: the Schedule tab is no longer a placeholder; with no rules it shows its empty state.
    expect(await screen.findByText("No weekly sessions yet")).toBeInTheDocument();
  });

  it("rename sends the new name with the loaded version", async () => {
    let body: unknown;
    server.use(
      http.put(groupUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Rename" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("textbox", { name: "Name" }), { target: { value: "Physics B" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ name: "Physics B", version: 11 });
    });
  });

  it("change capacity sends a number with the loaded version", async () => {
    let body: unknown;
    server.use(
      http.put(`${groupUrl}/capacity`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Change capacity" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("spinbutton", { name: "Capacity" }), { target: { value: "25" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ capacity: 25, version: 11 });
    });
  });

  it("change teacher sends the chosen teacher with the loaded version", async () => {
    let body: unknown;
    server.use(
      http.put(`${groupUrl}/teacher`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Change teacher" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    // The select is disabled until the teachers list arrives; its current value then shows by name.
    expect(await within(dialog).findByText("Amina Hassan")).toBeInTheDocument();
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Teacher" }));
    const option = await screen.findByRole("option", { name: "Mona Said" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ teacherId: "t-2", version: 11 });
    });
  });

  it("a stale 409 shows the conflict toast and refetches the group", async () => {
    let groupCalls = 0;
    server.use(
      http.get(groupUrl, () => {
        groupCalls += 1;
        return HttpResponse.json(group());
      }),
      http.put(groupUrl, () => problem(409, "concurrency.stale", "Stale.")),
    );

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Rename" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("textbox", { name: "Name" }), { target: { value: "Physics B" } });
    const before = groupCalls;
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("This record was changed by someone else. Reload and try again."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(groupCalls).toBeGreaterThan(before);
    });
  });

  it("an enrolment rule refusal gets its specific message", async () => {
    server.use(http.put(`${groupUrl}/capacity`, () => problem(422, "group.capacity_below_enrolled", "Below enrolled.")));

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Change capacity" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("spinbutton", { name: "Capacity" }), { target: { value: "2" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("Capacity cannot be lower than the number of students already enrolled."),
    ).toBeInTheDocument();
  });

  it("changing to a teacher busy at one of the group's sessions says so and keeps the current teacher", async () => {
    server.use(http.put(`${groupUrl}/teacher`, () => problem(409, "schedule.teacher_double_booked", "Double booked.")));

    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Change teacher" }, { timeout: 3000 }));
    const dialog = await screen.findByRole("dialog");
    expect(await within(dialog).findByText("Amina Hassan")).toBeInTheDocument();
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Teacher" }));
    const option = await screen.findByRole("option", { name: "Mona Said" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("That teacher already has a session at one of this group's times. The group keeps its current teacher."),
    ).toBeInTheDocument();
  });

  it("an archived group hides the edit actions and explains why", async () => {
    server.use(http.get(groupUrl, () => HttpResponse.json(group({ status: "archived" }))));

    renderRouter("/groups/g-1");

    expect(await screen.findByText("This group is archived", {}, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Rename" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Change capacity" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Change teacher" })).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Restore" })).toBeInTheDocument();
  });

  it("a session with groups.view only sees the page read-only", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(teacherMe)));

    renderRouter("/groups/g-1");

    expect(await screen.findByRole("heading", { name: "Physics A" }, { timeout: 3000 })).toBeInTheDocument();
    for (const name of ["Rename", "Change capacity", "Change teacher", "Archive", "Restore"]) {
      expect(screen.queryByRole("button", { name })).not.toBeInTheDocument();
    }
  });
});
