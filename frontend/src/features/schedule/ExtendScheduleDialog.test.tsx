import { fireEvent, screen, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, secretaryMe, teachers, timetable } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

async function openDialog() {
  renderRouter("/timetable?week=2027-04-17");
  fireEvent.click(await screen.findByRole("button", { name: "Extend schedule" }, { timeout: 5000 }));
  return screen.findByRole("dialog", { name: "Extend the schedule" });
}

describe("ExtendScheduleDialog", () => {
  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get("/api/teachers", () => HttpResponse.json({ items: teachers })),
      http.get("/api/timetable", () => HttpResponse.json(timetable("2027-04-17"))),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("is bounded to today through 92 days ahead in the centre's zone, and sends the chosen date", async () => {
    let body: unknown;
    server.use(
      http.post("/api/schedule/extend", async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ sessionsCreated: 12 });
      }),
    );

    const dialog = await openDialog();
    const through = within(dialog).getByLabelText("Create sessions through");
    expect(through).toHaveAttribute("min", "2027-04-20");
    expect(through).toHaveAttribute("max", "2027-07-21");
    fireEvent.change(through, { target: { value: "2027-06-15" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await screen.findByText("12 sessions created")).toBeInTheDocument();
    expect(body).toStrictEqual({ through: "2027-06-15" });
  });

  it("reports a repeat that creates nothing as already up to date", async () => {
    server.use(http.post("/api/schedule/extend", () => HttpResponse.json({ sessionsCreated: 0 })));

    const dialog = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await screen.findByText("Already up to date")).toBeInTheDocument();
  });

  it("a date beyond 92 days is refused under the field before any request", async () => {
    let sent = false;
    server.use(
      http.post("/api/schedule/extend", () => {
        sent = true;
        return HttpResponse.json({ sessionsCreated: 0 });
      }),
    );

    const dialog = await openDialog();
    fireEvent.change(within(dialog).getByLabelText("Create sessions through"), { target: { value: "2027-07-22" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(await within(dialog).findByRole("alert")).toHaveTextContent("Choose a date from today up to");
    expect(sent).toBe(false);
  });

  it("a double booking found while extending names its kind and advises a review, keeping the dialog open", async () => {
    server.use(http.post("/api/schedule/extend", () => problemDetails(409, "schedule.teacher_double_booked", "Double booked.")));

    const dialog = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Extend" }));

    expect(
      await within(dialog).findByText(
        "Two weekly sessions of one teacher now overlap. Review the teacher's weekly sessions and end one of them.",
      ),
    ).toBeInTheDocument();
    expect(screen.getByRole("dialog", { name: "Extend the schedule" })).toBeInTheDocument();
  });
});
