import { http, HttpResponse } from "msw";
import { fireEvent, screen, waitFor } from "@testing-library/react";
import { beforeEach, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { AttendancePage } from "./AttendancePage";
const state = vi.hoisted(() => ({ month: "2026-10", navigate: vi.fn(), kind: "parent" }));
vi.mock("@/routes/portal.attendance", () => ({
  Route: { useSearch: () => ({ month: state.month }), useNavigate: () => state.navigate },
}));
vi.mock("@/features/session/useCentreTimeZone", () => ({
  useCentreTimeZone: () => "Africa/Cairo",
}));
vi.mock("@/features/session/useSession", () => ({
  useSession: () => ({
    me: {
      portal: {
        kind: state.kind,
        children: [
          { id: "first-id", name: "Ahmed" },
          { id: "second-id", name: "Salma" },
        ],
      },
    },
  }),
}));
beforeEach(async () => {
  state.kind = "parent";
  await i18n.changeLanguage("en");
});
it("changes student filter, preserves local calendar month and shows an empty state", async () => {
  const requested: URL[] = [];
  server.use(
    http.get("/api/attendance", ({ request }) => {
      requested.push(new URL(request.url));
      return HttpResponse.json({ items: [], nextCursor: null });
    }),
  );
  const { container } = renderWithQueryClient(<AttendancePage />);
  await screen.findByText("No attendance recorded for this month");
  fireEvent.change(screen.getByLabelText("Child"), { target: { value: "second-id" } });
  await waitFor(() => {
    expect(requested.at(-1)?.searchParams.get("studentId")).toBe("second-id");
  });
  expect(requested.at(-1)?.searchParams.get("from")).toBe("2026-10-01");
  expect(requested.at(-1)?.searchParams.get("to")).toBe("2026-10-31");
  fireEvent.click(screen.getByLabelText("Previous month"));
  expect(state.navigate).toHaveBeenCalledWith({ search: { month: "2026-09" } });
  expect(container.textContent).not.toContain("first-id");
});
it("shows a retryable request error", async () => {
  server.use(http.get("/api/attendance", () => new HttpResponse(null, { status: 500 })));
  renderWithQueryClient(<AttendancePage />);
  expect(await screen.findByRole("alert")).toHaveTextContent("Attendance could not be loaded.");
});

it("hides the child selector for a student in Arabic", async () => {
  state.kind = "student";
  await i18n.changeLanguage("ar");
  server.use(http.get("/api/attendance", () => HttpResponse.json({ items: [], nextCursor: null })));
  renderWithQueryClient(<AttendancePage />);
  expect(await screen.findByText("لم يتم تسجيل حضور لهذا الشهر")).toBeInTheDocument();
  expect(screen.queryByRole("combobox")).not.toBeInTheDocument();
});

it("shows loading and then all four text-labelled statuses across keyset pages", async () => {
  let release: (() => void) | undefined;
  const gate = new Promise<void>((resolve) => {
    release = resolve;
  });
  const row = (index: number, status: string) => ({
    recordId: `record-${index.toString()}`,
    sessionId: `session-${index.toString()}`,
    localDate: "2026-10-15",
    startsAt: "2026-10-15T10:00:00Z",
    groupId: "group",
    groupName: `Math ${index.toString()}`,
    subjectName: "Math",
    studentId: "first-id",
    studentName: "Ahmed",
    status,
    source: "manual",
    correctionCount: 0,
  });
  server.use(
    http.get("/api/attendance", async ({ request }) => {
      await gate;
      return new URL(request.url).searchParams.has("cursor")
        ? HttpResponse.json({ items: [row(3, "absent"), row(4, "excused")], nextCursor: null })
        : HttpResponse.json({ items: [row(1, "present"), row(2, "late")], nextCursor: "next" });
    }),
  );
  renderWithQueryClient(<AttendancePage />);
  expect(await screen.findByRole("status")).toHaveTextContent("Loading attendance");
  release?.();
  await screen.findByRole("img", { name: "Present" });
  fireEvent.click(screen.getByRole("button", { name: "Load more" }));
  await screen.findByRole("img", { name: "Excused" });
  for (const name of ["Present", "Late", "Absent", "Excused"])
    expect(screen.getByRole("img", { name })).toBeInTheDocument();
  expect(screen.queryByRole("button", { name: "Load more" })).not.toBeInTheDocument();
});
