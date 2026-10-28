import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { concurrentMarkProblem, problemDetails, rowProblem, windowClosedProblem } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { rosterDto, rosterUrl, secretaryMe, SessionId, teacherMe } from "./attendanceTestData";

const sessionPath = `/attendance/sessions/${SessionId}`;

interface MarkBody {
  marks: { studentId: string; status: string }[];
}

/** One status button inside a student's toggle. */
function choice(studentName: string, status: string) {
  return within(screen.getByRole("radiogroup", { name: `Attendance status for ${studentName}` })).getByRole("radio", { name: status });
}

function tap(studentName: string, status: string) {
  fireEvent.click(choice(studentName, status));
}

/** Serves the roster and records every request to it and to the mark endpoint, in order. */
function serveRoster(options: { roster?: () => unknown; mark?: (body: MarkBody, call: number) => Response | Promise<Response> } = {}) {
  const events: string[] = [];
  const marks: MarkBody[] = [];
  server.use(
    http.get(rosterUrl, () => {
      events.push("GET");
      return HttpResponse.json(options.roster?.() ?? rosterDto());
    }),
    http.put(rosterUrl, async ({ request }) => {
      const body = (await request.json()) as MarkBody;
      marks.push(body);
      events.push("PUT");
      return options.mark?.(body, marks.length) ?? HttpResponse.json({ created: body.marks.length, updated: 0, unchanged: 0 });
    }),
  );
  return { events, marks };
}

describe("SessionAttendancePage", () => {
  beforeEach(() => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("Save sends only the rows that differ from the record, and says what was saved", async () => {
    const { marks } = serveRoster({ mark: () => HttpResponse.json({ created: 1, updated: 1, unchanged: 0 }) });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    expect(screen.getByText("No unsaved changes")).toBeInTheDocument();

    tap("Laila Samir", "Late"); // recorded Present: a change
    tap("Omar Nabil", "Present"); // not recorded: a new record
    tap("Laila Samir", "Present"); // back to what the server has...
    tap("Laila Samir", "Late"); // ...and away again
    expect(screen.getByText("2 unsaved changes")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Saved: 1 new, 1 changed")).toBeInTheDocument();
    expect(marks).toEqual([
      {
        marks: [
          { studentId: "st-laila", status: "late" },
          { studentId: "st-omar", status: "present" },
        ],
      },
    ]);
    expect(await screen.findByText("No unsaved changes")).toBeInTheDocument();
  });

  it("sends nothing but the allowed keys: marks, each with a studentId and a status", async () => {
    const { marks } = serveRoster();

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Mona Adel", "Absent");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(marks).toHaveLength(1);
    });
    expect(Object.keys(marks[0] ?? {})).toEqual(["marks"]);
    expect(marks[0]?.marks.map((mark) => Object.keys(mark).sort())).toEqual([["status", "studentId"]]);
  });

  it("Mark all present sets only the students with no record, then Save sends just those", async () => {
    const { marks } = serveRoster();

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Mona Adel", "Absent"); // a deliberate tap is kept
    fireEvent.click(screen.getByRole("button", { name: "Mark all present" }));

    expect(choice("Omar Nabil", "Present")).toHaveAttribute("aria-checked", "true");
    expect(choice("Mona Adel", "Absent")).toHaveAttribute("aria-checked", "true");
    expect(choice("Laila Samir", "Present")).toHaveAttribute("aria-checked", "true");
    expect(screen.getByText("2 unsaved changes")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Save" }));
    await waitFor(() => {
      expect(marks).toHaveLength(1);
    });
    expect(marks[0]?.marks).toEqual([
      { studentId: "st-omar", status: "present" },
      { studentId: "st-mona", status: "absent" },
    ]);
  });

  it("on a concurrent-mark 409 it refetches the roster and resends once", async () => {
    const { events, marks } = serveRoster({
      mark: (body, call) => (call === 1 ? concurrentMarkProblem() : HttpResponse.json({ created: 0, updated: body.marks.length, unchanged: 0 })),
    });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Omar Nabil", "Present");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Saved: 0 new, 1 changed")).toBeInTheDocument();
    // Load, first try (409), refetch, second try; the draft is the same on both tries.
    expect(events.slice(0, 4)).toEqual(["GET", "PUT", "GET", "PUT"]);
    expect(marks).toHaveLength(2);
    expect(marks[1]).toEqual(marks[0]);
  });

  it("shows a second 409 to the user, keeps the draft and never retries in a loop", async () => {
    const { marks } = serveRoster({ mark: () => concurrentMarkProblem() });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Omar Nabil", "Present");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Attendance changed while you were editing. Refresh and try again.")).toBeInTheDocument();
    await new Promise((resolve) => setTimeout(resolve, 400));
    expect(marks).toHaveLength(2);
    expect(screen.getByText("1 unsaved change")).toBeInTheDocument();
    expect(choice("Omar Nabil", "Present")).toHaveAttribute("aria-checked", "true");
  });

  it("with the window closed the toggles are disabled and the page says why", async () => {
    serveRoster({ roster: () => rosterDto({ window: "closed" }) });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });

    expect(screen.getByText(/Attendance for this session is closed/)).toBeInTheDocument();
    expect(choice("Omar Nabil", "Present")).toHaveAttribute("aria-disabled", "true");
    expect(screen.queryByRole("button", { name: "Mark all present" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Save" })).not.toBeInTheDocument();
  });

  it("on a window-closed 422 it shows the closed banner, refetches and keeps the draft", async () => {
    let closed = false;
    serveRoster({
      roster: () => rosterDto({ window: closed ? "closed" : "open" }),
      mark: () => {
        closed = true;
        return windowClosedProblem();
      },
    });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Omar Nabil", "Present");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText(/Attendance for this session is closed/)).toBeInTheDocument();
    expect(choice("Omar Nabil", "Present")).toHaveAttribute("aria-checked", "true");
    expect(choice("Omar Nabil", "Present")).toHaveAttribute("aria-disabled", "true");
  });

  it("puts a record-corrected refusal on the row the server names", async () => {
    serveRoster({
      mark: () => rowProblem(422, "attendance.record_corrected", "A corrected record must be changed through a correction.", "marks[0].status"),
    });

    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Laila Samir", "Late");
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("Some rows could not be saved")).toBeInTheDocument();
    // Once under the row, once in the summary at the top (where the student's name leads the line).
    expect(screen.getAllByText(/The office corrected this record, so it cannot be changed here\./)).toHaveLength(2);
    const row = screen.getByRole("radiogroup", { name: "Attendance status for Laila Samir" }).closest("li");
    expect(row).not.toBeNull();
    expect(within(row as HTMLElement).getByText(/The office corrected this record/)).toBeInTheDocument();
  });

  it("a teacher has no Correct action; the secretary has one on each recorded row only", async () => {
    serveRoster();

    const { unmount } = renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    expect(screen.queryByRole("button", { name: /^Correct/ })).not.toBeInTheDocument();
    unmount();

    server.use(http.get("/api/me", () => HttpResponse.json(secretaryMe)));
    renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    expect(await screen.findAllByRole("button", { name: /^Correct/ })).toHaveLength(1);
  });

  it("a 404 shows the Session not found state with a way back", async () => {
    server.use(http.get(rosterUrl, () => problemDetails(404, "attendance.session_not_found", "This session could not be found.")));

    renderRouter(sessionPath);

    expect(await screen.findByText("Session not found", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to sessions" })).toHaveAttribute("href", "/attendance");
  });

  it("asks before leaving with unsaved changes, and lets the user stay or leave", async () => {
    serveRoster();
    server.use(http.get("/api/attendance/sessions", () => HttpResponse.json({ date: "2026-12-05", timeZoneId: "Africa/Cairo", items: [] })));

    const { router } = renderRouter(sessionPath);
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    tap("Omar Nabil", "Present");

    fireEvent.click(screen.getByRole("link", { name: "Back to sessions" }));
    expect(await screen.findByText("Leave without saving?")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Keep editing" }));
    await waitFor(() => {
      expect(screen.queryByText("Leave without saving?")).not.toBeInTheDocument();
    });
    expect(router.state.location.pathname).toBe(sessionPath);

    fireEvent.click(screen.getByRole("link", { name: "Back to sessions" }));
    fireEvent.click(await screen.findByRole("button", { name: "Discard and leave" }));
    await waitFor(() => {
      expect(router.state.location.pathname).toBe("/attendance");
    });
  });

  it("reads right to left in Arabic, with the time kept left to right and the student count pluralised", async () => {
    serveRoster();
    await i18n.changeLanguage("ar");

    renderRouter(sessionPath);

    // 15:00Z in Cairo is 5 PM; the range is one left-to-right piece inside the right-to-left page.
    const time = await screen.findByText(/(5|٥):(00|٠٠).*(6|٦):(30|٣٠)/, {}, { timeout: 5000 });
    expect(time).toHaveAttribute("dir", "ltr");
    expect(document.documentElement).toHaveAttribute("dir", "rtl");
    expect(screen.getByText(/(3|٣) طلاب/)).toBeInTheDocument();
  });
});
