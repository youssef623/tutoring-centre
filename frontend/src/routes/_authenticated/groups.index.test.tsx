import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { selectOption } from "@/test/selectOption";

const meUrl = "/api/me";
const groupsUrl = "/api/groups";
const subjectsUrl = "/api/subjects";
const teachersUrl = "/api/teachers";

function me(role: "owner" | "teacher", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Two-Centre Teacher",
    email: role === "owner" ? "owner@nile.test" : "teacher@both.test",
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

const ownerMe = me("owner", ["groups.manage", "groups.view", "subjects.manage", "subjects.view", "teachers.manage", "teachers.view"]);
// What a Teacher holds: groups.view without groups.manage, and no teachers.view at all.
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
    enrolledCount: 0,
    status: "active",
    version: 3,
    ...overrides,
  };
}

const subjects = [
  { id: "s-phys", name: "Physics", status: "active", version: 1 },
  { id: "s-chem", name: "Chemistry", status: "active", version: 1 },
];
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

describe("GroupsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(groupsUrl, () => HttpResponse.json({ items: [group()] })),
      http.get(subjectsUrl, () => HttpResponse.json({ items: subjects })),
      http.get(teachersUrl, () => HttpResponse.json({ items: teachers })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("lists groups with a link to each group's page and seats as enrolled / capacity", async () => {
    server.use(http.get(groupsUrl, () => HttpResponse.json({ items: [group(), group({ id: "g-2", name: "Chem B", enrolledCount: 2, capacity: 2 })] })));

    renderRouter("/groups");

    // First render in the file pays the cold-start cost; a full parallel suite run can push it past 3s.
    const link = await screen.findByRole("link", { name: "Physics A" }, { timeout: 5000 });
    expect(link).toHaveAttribute("href", "/groups/g-1?tab=overview");
    const chemRow = screen.getByRole("link", { name: "Chem B" }).closest("tr") as HTMLElement;
    const seats = within(chemRow).getByText("2 / 2");
    expect(seats).toHaveAttribute("dir", "ltr");
    expect(within(chemRow).getByText("Full")).toBeInTheDocument();
  });

  it("filters write the URL parameters and the request's query parameters", async () => {
    const requests: URLSearchParams[] = [];
    server.use(
      http.get(groupsUrl, ({ request }) => {
        requests.push(new URL(request.url).searchParams);
        return HttpResponse.json({ items: [group()] });
      }),
    );

    const { router } = renderRouter("/groups");
    await screen.findByRole("link", { name: "Physics A" }, { timeout: 3000 });

    await selectOption("Subject", "Chemistry");
    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({ subject: "s-chem" });
    });
    await waitFor(() => {
      expect(requests.at(-1)?.get("subjectId")).toBe("s-chem");
    });

    await selectOption("Teacher", "Mona Said");
    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({ subject: "s-chem", teacher: "t-2" });
    });
    await waitFor(() => {
      expect(requests.at(-1)?.get("teacherId")).toBe("t-2");
    });

    fireEvent.click(screen.getByRole("switch", { name: "Show archived" }));
    await waitFor(() => {
      expect(requests.at(-1)?.get("includeArchived")).toBe("true");
    });
  });

  it("with groups.view only: no create button, no teacher filter, and /api/teachers is never requested", async () => {
    let teacherCalls = 0;
    server.use(
      http.get(meUrl, () => HttpResponse.json(teacherMe)),
      http.get(teachersUrl, () => {
        teacherCalls += 1;
        return HttpResponse.json({ items: teachers });
      }),
    );

    renderRouter("/groups");

    expect(await screen.findByRole("link", { name: "Physics A" }, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Create group" })).not.toBeInTheDocument();
    expect(screen.queryByRole("combobox", { name: "Teacher" })).not.toBeInTheDocument();
    expect(teacherCalls).toBe(0);
  });

  it("an empty list for a session without groups.manage shows the teacher's text", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(teacherMe)),
      http.get(groupsUrl, () => HttpResponse.json({ items: [] })),
    );

    renderRouter("/groups");

    expect(await screen.findByText("No groups are assigned to you", {}, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.queryByText("No groups yet")).not.toBeInTheDocument();
  });

  it("an empty list for a manager offers to create the first group", async () => {
    server.use(http.get(groupsUrl, () => HttpResponse.json({ items: [] })));

    renderRouter("/groups");

    expect(await screen.findByText("No groups yet", {}, { timeout: 3000 })).toBeInTheDocument();
    expect(screen.getAllByRole("button", { name: "Create group" })).toHaveLength(2);
  });

  it("create sends exactly four fields with a numeric capacity, then opens the new group", async () => {
    let body: unknown;
    server.use(
      http.post(groupsUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "g-new" }, { status: 201, headers: { Location: "/api/groups/g-new" } });
      }),
      http.get(`${groupsUrl}/g-new`, () => HttpResponse.json(group({ id: "g-new", name: "Chemistry 2", capacity: 15 }))),
    );

    const { router } = renderRouter("/groups");
    fireEvent.click((await screen.findAllByRole("button", { name: "Create group" }, { timeout: 3000 }))[0] as HTMLElement);
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("textbox", { name: "Name" }), { target: { value: "Chemistry 2" } });
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Subject" }));
    let option = await screen.findByRole("option", { name: "Chemistry" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Teacher" }));
    option = await screen.findByRole("option", { name: "Mona Said" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.change(within(dialog).getByRole("spinbutton", { name: "Capacity" }), { target: { value: "15" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Create group" }));

    await waitFor(() => {
      expect(body).toEqual({ name: "Chemistry 2", subjectId: "s-chem", teacherId: "t-2", capacity: 15 });
    });
    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/groups/g-new");
    });
  });

  it("the subject and teacher selects show their hint until something is chosen", async () => {
    renderRouter("/groups");
    fireEvent.click((await screen.findAllByRole("button", { name: "Create group" }, { timeout: 3000 }))[0] as HTMLElement);
    const dialog = await screen.findByRole("dialog");

    expect(await within(dialog).findByText("Choose a subject")).toBeInTheDocument();
    expect(await within(dialog).findByText("Choose a teacher")).toBeInTheDocument();
  });

  it("create maps group.name_taken onto the name field", async () => {
    server.use(
      http.post(groupsUrl, () =>
        problem(409, "group.name_taken", "A group with this name already exists.", { name: ["A group with this name already exists."] }),
      ),
    );

    renderRouter("/groups");
    fireEvent.click((await screen.findAllByRole("button", { name: "Create group" }, { timeout: 3000 }))[0] as HTMLElement);
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("textbox", { name: "Name" }), { target: { value: "Physics A" } });
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Subject" }));
    let option = await screen.findByRole("option", { name: "Physics" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Teacher" }));
    option = await screen.findByRole("option", { name: "Amina Hassan" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("button", { name: "Create group" }));

    const nameInput = within(dialog).getByRole("textbox", { name: "Name" });
    await waitFor(() => {
      expect(nameInput).toHaveAttribute("aria-invalid", "true");
    });
    expect(within(dialog).getByText("A group with this name already exists.")).toBeInTheDocument();
  });

  it("a refusal about the subject lands under the subject select and refetches the subjects", async () => {
    let subjectCalls = 0;
    server.use(
      http.get(subjectsUrl, () => {
        subjectCalls += 1;
        return HttpResponse.json({ items: subjects });
      }),
      http.post(groupsUrl, () => problem(422, "group.subject_archived", "The subject is archived.")),
    );

    renderRouter("/groups");
    fireEvent.click((await screen.findAllByRole("button", { name: "Create group" }, { timeout: 3000 }))[0] as HTMLElement);
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("textbox", { name: "Name" }), { target: { value: "Physics B" } });
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Subject" }));
    let option = await screen.findByRole("option", { name: "Physics" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    fireEvent.click(within(dialog).getByRole("combobox", { name: "Teacher" }));
    option = await screen.findByRole("option", { name: "Amina Hassan" });
    fireEvent.pointerDown(option);
    fireEvent.click(option);
    const before = subjectCalls;
    fireEvent.click(within(dialog).getByRole("button", { name: "Create group" }));

    expect(await within(dialog).findByText("This subject is archived.")).toBeInTheDocument();
    expect(within(dialog).getByRole("combobox", { name: "Subject" })).toHaveAttribute("aria-invalid", "true");
    await waitFor(() => {
      expect(subjectCalls).toBeGreaterThan(before);
    });
  });

  it("client checks cover presence and range before any request", async () => {
    let createCalls = 0;
    server.use(
      http.post(groupsUrl, () => {
        createCalls += 1;
        return HttpResponse.json({ id: "g-x" }, { status: 201 });
      }),
    );

    renderRouter("/groups");
    fireEvent.click((await screen.findAllByRole("button", { name: "Create group" }, { timeout: 3000 }))[0] as HTMLElement);
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByRole("spinbutton", { name: "Capacity" }), { target: { value: "0" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Create group" }));

    expect(await within(dialog).findByText("Group name is required.")).toBeInTheDocument();
    expect(within(dialog).getByText("Choose a subject.")).toBeInTheDocument();
    expect(within(dialog).getByText("Choose a teacher.")).toBeInTheDocument();
    expect(within(dialog).getByText("Capacity must be between 1 and 200.")).toBeInTheDocument();
    expect(createCalls).toBe(0);
  });
});
