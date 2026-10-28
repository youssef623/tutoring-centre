import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import type { AttendanceHistoryItemDto } from "@/api/generated/tutoring-centre";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const me = {
  userId: "u-1",
  displayName: "Nile Secretary",
  email: "secretary@nile.test",
  preferredLocale: "en",
  activeCentreId: "c-nile",
  activeRole: "secretary",
  memberships: [
    {
      centreId: "c-nile",
      centreName: "Nile",
      centreSlug: "nile",
      role: "secretary",
      timeZoneId: "Africa/Cairo",
    },
  ],
  permissions: ["groups.view", "students.view", "attendance.view"],
};
function record(overrides: Partial<AttendanceHistoryItemDto> = {}): AttendanceHistoryItemDto {
  return {
    recordId: "r-1",
    sessionId: "session-1",
    localDate: "2026-10-20",
    startsAt: "2026-10-20T15:00:00Z",
    groupId: "g-1",
    groupName: "Physics A",
    subjectName: "Physics",
    studentId: "s-1",
    studentName: "Laila Withdrawn",
    status: "present",
    source: "manual",
    correctionCount: 0,
    ...overrides,
  };
}
const groupPath = "/groups/g-1?tab=attendance&from=2026-10-01&to=2026-10-28";

describe("Attendance history", () => {
  beforeEach(() => {
    server.use(
      http.get("/api/me", () => HttpResponse.json(me)),
      http.get("/api/groups/g-1", () =>
        HttpResponse.json({
          id: "g-1",
          name: "Physics A",
          subjectId: "sub-1",
          subjectName: "Physics",
          teacherId: "t-1",
          teacherName: "Amina",
          capacity: 20,
          enrolledCount: 1,
          status: "active",
          version: 1,
        }),
      ),
      http.get("/api/students/s-1", () =>
        HttpResponse.json({
          id: "s-1",
          fullName: "Laila Withdrawn",
          phone: null,
          schoolYear: null,
          status: "active",
          version: 1,
          parents: [],
        }),
      ),
      http.get("/api/attendance", () => HttpResponse.json({ items: [], nextCursor: null })),
    );
  });
  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("loads all pages, derives rows and columns only from history, and leaves missing cells empty", async () => {
    const cursors: (string | null)[] = [];
    server.use(
      http.get("/api/attendance", ({ request }) => {
        const url = new URL(request.url);
        const cursor = url.searchParams.get("cursor");
        cursors.push(cursor);
        expect(url.searchParams.get("groupId")).toBe("g-1");
        return HttpResponse.json(
          cursor === null
            ? { items: [record()], nextCursor: "page-two" }
            : {
                items: [
                  record({
                    recordId: "r-2",
                    sessionId: "session-2",
                    studentId: "s-2",
                    studentName: "Omar",
                    localDate: "2026-10-21",
                    startsAt: "2026-10-21T15:00:00Z",
                    status: "absent",
                    source: "qr",
                    correctionCount: 1,
                  }),
                ],
                nextCursor: null,
              },
        );
      }),
    );
    renderRouter(groupPath);
    const table = await screen.findByRole("table", {}, { timeout: 5000 });
    expect(cursors).toEqual([null, "page-two"]);
    expect(within(table).getAllByRole("columnheader")).toHaveLength(3);
    expect(within(table).getAllByLabelText("Not recorded")).toHaveLength(2);
    expect(within(table).getByText("Laila Withdrawn")).toBeInTheDocument();
    expect(within(table).getByRole("img", { name: "QR code" })).toBeInTheDocument();
    expect(within(table).getByRole("img", { name: "Corrected once" })).toBeInTheDocument();
    expect(within(table).getAllByRole("link")[0]).toHaveAttribute(
      "href",
      "/attendance/sessions/session-1",
    );
  });

  it("writes range controls to the URL and rejects a 120-day pasted range without requesting history", async () => {
    let requests = 0;
    server.use(
      http.get("/api/attendance", () => {
        requests++;
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );
    const { router } = renderRouter("/groups/g-1?tab=attendance&from=2026-06-01&to=2026-10-01");
    expect(await screen.findByRole("alert", {}, { timeout: 5000 })).toHaveTextContent(
      "at most 92 days",
    );
    expect(requests).toBe(0);
    fireEvent.change(screen.getByLabelText("From"), { target: { value: "2026-09-01" } });
    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({
        tab: "attendance",
        from: "2026-09-01",
        to: "2026-10-01",
      });
    });
    expect(await screen.findByText("No attendance recorded in this period")).toBeInTheDocument();
  });

  it("persists the default four-week range calculated in the centre zone", async () => {
    const { router } = renderRouter("/groups/g-1?tab=attendance");
    await screen.findByText("No attendance recorded in this period", {}, { timeout: 5000 });
    const from = screen.getByLabelText<HTMLInputElement>("From").value;
    const to = screen.getByLabelText<HTMLInputElement>("To").value;
    expect((Date.parse(to) - Date.parse(from)) / 86400000).toBe(27);
    expect(router.state.location.search).toMatchObject({ from, to });
  });

  it("hides the fourth tab and sends no history request without attendance.view", async () => {
    let requests = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json({ ...me, permissions: ["groups.view"] })),
      http.get("/api/attendance", () => {
        requests++;
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );
    renderRouter(groupPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    expect(screen.queryByRole("tab", { name: "Attendance" })).not.toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Overview" })).toHaveAttribute("aria-selected", "true");
    expect(requests).toBe(0);
  });

  it("student card loads older pages on demand and labels counts with their period", async () => {
    const cursors: (string | null)[] = [];
    server.use(
      http.get("/api/attendance", ({ request }) => {
        const url = new URL(request.url);
        const cursor = url.searchParams.get("cursor");
        cursors.push(cursor);
        expect(url.searchParams.get("studentId")).toBe("s-1");
        return HttpResponse.json(
          cursor === null
            ? { items: [record()], nextCursor: "older" }
            : {
                items: [record({ recordId: "r-old", status: "late", correctionCount: 1 })],
                nextCursor: null,
              },
        );
      }),
    );
    renderRouter("/students/s-1");
    expect(
      await screen.findByText(
        "In this period: 1 present, 0 late, 0 absent, 0 excused",
        {},
        { timeout: 5000 },
      ),
    ).toBeInTheDocument();
    expect(cursors).toEqual([null]);
    fireEvent.click(screen.getByRole("button", { name: "Load more" }));
    expect(
      await screen.findByText("In this period: 1 present, 1 late, 0 absent, 0 excused"),
    ).toBeInTheDocument();
    expect(cursors).toEqual([null, "older"]);
    expect(screen.queryByRole("button", { name: "Load more" })).not.toBeInTheDocument();
    expect(screen.getByRole("img", { name: "Corrected once" })).toBeInTheDocument();
  });

  it("student card sends no history request without attendance.view", async () => {
    let requests = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json({ ...me, permissions: ["students.view"] })),
      http.get("/api/attendance", () => {
        requests++;
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );
    renderRouter("/students/s-1");
    await screen.findByRole("heading", { name: "Laila Withdrawn" }, { timeout: 5000 });
    expect(screen.queryByRole("heading", { name: "Attendance" })).not.toBeInTheDocument();
    expect(requests).toBe(0);
  });

  it("supports Arabic while isolating dates left-to-right", async () => {
    await i18n.changeLanguage("ar");
    server.use(
      http.get("/api/attendance", () => HttpResponse.json({ items: [record()], nextCursor: null })),
    );
    renderRouter(groupPath);
    const table = await screen.findByRole("table", {}, { timeout: 5000 });
    expect(within(table).getByRole("link").firstElementChild).toHaveAttribute("dir", "ltr");
    expect(within(table).getByLabelText("حاضر")).toBeInTheDocument();
    expect(document.documentElement).toHaveAttribute("dir", "rtl");
  });

  it("stops on a failed second page and Retry restarts the range", async () => {
    let fail = true;
    server.use(
      http.get("/api/attendance", ({ request }) => {
        const cursor = new URL(request.url).searchParams.get("cursor");
        if (cursor && fail)
          return HttpResponse.json({ title: "Error", status: 500 }, { status: 500 });
        return HttpResponse.json({
          items: [record({ recordId: cursor ? "r-2" : "r-1" })],
          nextCursor: cursor ? null : "page-two",
        });
      }),
    );
    renderRouter(groupPath);
    const retry = await screen.findByRole("button", { name: "Retry" }, { timeout: 8000 });
    fail = false;
    fireEvent.click(retry);
    expect(await screen.findByRole("table", {}, { timeout: 5000 })).toBeInTheDocument();
  });
});
