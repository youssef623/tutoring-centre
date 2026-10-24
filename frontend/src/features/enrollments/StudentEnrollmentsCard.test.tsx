import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";
import { groupFullProblem, problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { selectOption } from "@/test/selectOption";

const studentUrl = "/api/students/s-1";
const enrollmentsUrl = `${studentUrl}/enrollments`;

const studentDetail = {
  id: "s-1",
  fullName: "Ahmed Mahmoud",
  phone: null,
  schoolYear: null,
  status: "active",
  version: 1,
  parents: [{ parentId: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", relationship: "father", isPrimary: true }],
};

function enrollment(overrides: Record<string, unknown> = {}) {
  return {
    enrollmentId: "e-1",
    groupId: "g-1",
    groupName: "Physics A",
    subjectName: "Physics",
    teacherName: "Amina Hassan",
    status: "active",
    enrolledAt: "2026-10-01T09:00:00+00:00",
    endedAt: null,
    endReason: null,
    version: 7,
    ...overrides,
  };
}

const endedEnrollment = enrollment({
  enrollmentId: "e-0",
  groupId: "g-0",
  groupName: "Chemistry A",
  subjectName: "Chemistry",
  status: "ended",
  endedAt: "2026-09-20T09:00:00+00:00",
  endReason: "transferred",
});

const mathA = group({ id: "g-5", name: "Math A", subjectId: "s-math", subjectName: "Math", teacherName: "Karim Adel", capacity: 10, enrolledCount: 3 });
const mathFull = group({ id: "g-6", name: "Math B", subjectId: "s-math", subjectName: "Math", capacity: 2, enrolledCount: 2 });

describe("StudentEnrollmentsCard", () => {
  let enrollmentCalls = 0;

  beforeEach(() => {
    enrollmentCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get(studentUrl, () => HttpResponse.json(studentDetail)),
      http.get(enrollmentsUrl, () => {
        enrollmentCalls += 1;
        // Deliberately ended-first: the card, not the server's order, puts active enrollments first.
        return HttpResponse.json({ items: [endedEnrollment, enrollment()] });
      }),
      http.get("/api/subjects", () =>
        HttpResponse.json({
          items: [
            { id: "s-math", name: "Math", status: "active", version: 1 },
            { id: "s-phys", name: "Physics", status: "active", version: 1 },
          ],
        }),
      ),
      http.get("/api/groups", () => HttpResponse.json({ items: [mathA, mathFull] })),
    );
  });

  it("without enrollments.view there is no card and no enrollment request", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(me("secretary", ["students.manage", "students.view"]))));

    renderRouter("/students/s-1");

    expect(await screen.findByRole("heading", { name: "Ahmed Mahmoud" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("heading", { name: "Groups" })).not.toBeInTheDocument();
    expect(enrollmentCalls).toBe(0);
  });

  it("shows current groups first with their details, then past ones with end date and reason", async () => {
    renderRouter("/students/s-1");

    const current = await screen.findByRole("list", { name: "Current groups" }, { timeout: 5000 });
    expect(within(current).getByRole("link", { name: "Physics A" })).toHaveAttribute("href", "/groups/g-1?tab=roster");
    expect(within(current).getByText(/Since Oct 1, 2026/)).toBeInTheDocument();
    expect(within(current).getByText("Amina Hassan")).toBeInTheDocument();
    expect(within(current).getByRole("button", { name: "Actions for Physics A" })).toBeInTheDocument();

    const pastHeading = screen.getByRole("heading", { name: "Past groups" });
    expect(current.compareDocumentPosition(pastHeading) & Node.DOCUMENT_POSITION_FOLLOWING).toBeTruthy();
    expect(screen.getByText("Chemistry A")).toBeInTheDocument();
    expect(screen.getByText(/Ended Sep 20, 2026 · Transferred/)).toBeInTheDocument();
  });

  it("enrolls through the shared mutation: subject, then a group with seats, sending only { studentId }", async () => {
    let body: unknown;
    server.use(
      http.post("/api/groups/g-5/enrollments", async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "e-9" }, { status: 201 });
      }),
    );
    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Enroll in a group" }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog", { name: "Enroll in a group" });
    // The select stays disabled until the subjects list has arrived.
    await waitFor(() => {
      expect(within(dialog).getByRole("combobox", { name: "Subject" })).toBeEnabled();
    });
    await selectOption("Subject", "Math");

    expect(await within(dialog).findByRole("radio", { name: /Math B/ })).toHaveAttribute("aria-disabled", "true");
    expect(within(dialog).getByText("3 / 10")).toBeInTheDocument();
    const before = enrollmentCalls;
    fireEvent.click(within(dialog).getByRole("radio", { name: /Math A/ }));
    fireEvent.click(within(dialog).getByRole("button", { name: "Enroll" }));

    expect(await screen.findByText("Ahmed Mahmoud was enrolled in Math A.")).toBeInTheDocument();
    expect(body).toStrictEqual({ studentId: "s-1" });
    await waitFor(() => {
      expect(enrollmentCalls).toBeGreaterThan(before);
    });
  });

  it("a full group answered with 409 keeps the picker open with the reason", async () => {
    server.use(http.post("/api/groups/g-5/enrollments", () => groupFullProblem()));
    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Enroll in a group" }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog", { name: "Enroll in a group" });
    // The select stays disabled until the subjects list has arrived.
    await waitFor(() => {
      expect(within(dialog).getByRole("combobox", { name: "Subject" })).toBeEnabled();
    });
    await selectOption("Subject", "Math");
    fireEvent.click(await within(dialog).findByRole("radio", { name: /Math A/ }));
    fireEvent.click(within(dialog).getByRole("button", { name: "Enroll" }));

    expect(await within(dialog).findByText("This group is full")).toBeInTheDocument();
    expect(screen.getByRole("dialog", { name: "Enroll in a group" })).toBeInTheDocument();
  });

  it("archiving a student who still has groups shows the specific message and keeps the student", async () => {
    server.use(
      http.post(`${studentUrl}/archive`, () =>
        problemDetails(422, "student.has_active_enrollments", "Student has active enrollments."),
      ),
    );
    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Archive" }, { timeout: 5000 }));
    fireEvent.click(await screen.findByRole("button", { name: "Archive student" }));

    expect(await screen.findByText("Withdraw this student from their groups first.")).toBeInTheDocument();
    expect(screen.queryByText("This student is archived")).not.toBeInTheDocument();
  });
});
