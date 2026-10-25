import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, secretaryMe, session, teacherMe, teachers, timetable } from "@/features/schedule/scheduleTestData";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

describe("TimetablePage", () => {
  let requestedWeeks: string[] = [];
  let teachersCalls = 0;
  let sessions = [session()];
  let responseZone = "Africa/Cairo";

  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    requestedWeeks = [];
    teachersCalls = 0;
    sessions = [session()];
    responseZone = "Africa/Cairo";
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get("/api/teachers", () => {
        teachersCalls += 1;
        return HttpResponse.json({ items: teachers });
      }),
      http.get("/api/timetable", ({ request }) => {
        const weekStart = new URL(request.url).searchParams.get("weekStart") ?? "";
        requestedWeeks.push(weekStart);
        return HttpResponse.json(timetable(weekStart, weekStart === "2027-04-24" ? sessions : [], responseZone));
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("without a week in the URL opens the centre's current week, from Saturday", async () => {
    renderRouter("/timetable");

    expect(await screen.findByText("No sessions this week", {}, { timeout: 5000 })).toBeInTheDocument();
    // Tuesday 20 April in Cairo belongs to the week of Saturday 17 April.
    expect(requestedWeeks).toEqual(["2027-04-17"]);
    expect(screen.getByRole("button", { name: "This week" })).toBeDisabled();
  });

  it("places sessions by the server's localDate and formats times in the response's zone", async () => {
    // 22:30Z on Saturday is 00:30 on Sunday in Cairo: the server says Sunday, and so does the grid.
    sessions = [
      session(),
      session({ id: "s-2", groupName: "Late Group", localDate: "2027-04-25", startsAt: "2027-04-24T22:30:00Z", endsAt: "2027-04-24T23:30:00Z" }),
    ];

    renderRouter("/timetable?week=2027-04-24");

    const saturday = await screen.findByRole("region", { name: /^Saturday/ }, { timeout: 5000 });
    const sunday = screen.getByRole("region", { name: /^Sunday/ });
    expect(within(saturday).getByText("Physics A")).toBeInTheDocument();
    expect(within(saturday).queryByText("Late Group")).not.toBeInTheDocument();
    expect(within(sunday).getByText("Late Group")).toBeInTheDocument();
    expect(within(saturday).getByText(/^6:00\s?–\s?7:30\sPM$/u)).toHaveAttribute("dir", "ltr");
    expect(within(sunday).getByText(/^12:30\s?–\s?1:30\sAM$/u)).toBeInTheDocument();
  });

  it("uses the zone the response names, whatever the browser's or the membership's zone is", async () => {
    // 16:00Z is 8 PM in Dubai (UTC+4): only the response's timeZoneId explains that text.
    responseZone = "Asia/Dubai";

    renderRouter("/timetable?week=2027-04-24");

    const saturday = await screen.findByRole("region", { name: /^Saturday/ }, { timeout: 5000 });
    expect(within(saturday).getByText(/^8:00\s?–\s?9:30\sPM$/u)).toBeInTheDocument();
  });

  it("keeps a cancelled session visible, struck through and labelled", async () => {
    sessions = [session({ status: "cancelled" })];

    renderRouter("/timetable?week=2027-04-24");

    const card = await screen.findByRole("article", { name: /^Physics A/ }, { timeout: 5000 });
    expect(within(card).getByText("Cancelled")).toBeInTheDocument();
    expect(within(card).getByText(/6:00/u)).toHaveClass("line-through");
    // A cancelled session offers nothing to cancel.
    expect(within(card).queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
  });

  it("week navigation goes through the URL and fetches that week", async () => {
    const { router } = renderRouter("/timetable?week=2027-04-17");
    expect(await screen.findByText("No sessions this week", {}, { timeout: 5000 })).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Next week" }));

    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({ week: "2027-04-24" });
    });
    expect(await screen.findByText("Physics A")).toBeInTheDocument();
    expect(requestedWeeks).toContain("2027-04-24");

    fireEvent.click(screen.getByRole("button", { name: "Previous week" }));
    await waitFor(() => {
      expect(router.state.location.search).toMatchObject({ week: "2027-04-17" });
    });
  });

  it("cancel asks for confirmation naming group, date and time, then sends only the version", async () => {
    let body: unknown;
    server.use(
      http.post("/api/sessions/s-1/cancel", async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/timetable?week=2027-04-24");
    fireEvent.click(await screen.findByRole("button", { name: /^Actions for Physics A at/ }, { timeout: 5000 }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Cancel session" }));
    const confirm = await screen.findByRole("alertdialog");
    expect(within(confirm).getByRole("heading", { name: "Cancel Physics A on Saturday, April 24 at 6:00 PM?" })).toBeInTheDocument();
    fireEvent.click(within(confirm).getByRole("button", { name: "Cancel session" }));

    expect(await screen.findByText("Session cancelled")).toBeInTheDocument();
    expect(body).toStrictEqual({ version: 9 });
  });

  it("without schedule.manage or teachers.view there is no manage control, no teacher filter and no teachers request", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));

    renderRouter("/timetable?week=2027-04-24");

    expect(await screen.findByText("Physics A", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Extend schedule" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
    expect(screen.queryByRole("combobox", { name: "Teacher" })).not.toBeInTheDocument();
    expect(screen.getByRole("combobox", { name: "Group" })).toBeInTheDocument();
    expect(teachersCalls).toBe(0);
  });

  it("with schedule.manage an empty week hints at adding weekly sessions or extending", async () => {
    renderRouter("/timetable?week=2027-05-29");

    expect(await screen.findByText("No sessions this week", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByText("Add weekly sessions on a group's Schedule tab, or extend the schedule to fill later weeks.")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Extend schedule" })).toBeInTheDocument();
  });

  it("in Arabic the page runs right to left, Saturday first, while times stay left to right", async () => {
    await i18n.changeLanguage("ar");

    renderRouter("/timetable?week=2027-04-24");

    const saturday = await screen.findByRole("region", { name: /^السبت/ }, { timeout: 5000 });
    expect(document.documentElement).toHaveAttribute("dir", "rtl");
    const regions = screen.getAllByRole("region");
    expect(regions[0]).toBe(saturday);
    const time = within(saturday).getByRole("article").querySelector("time");
    expect(time).toHaveAttribute("dir", "ltr");
    expect(time).toHaveAttribute("datetime", "2027-04-24T16:00:00Z");
  });
});
