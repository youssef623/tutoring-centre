import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, secretaryMe } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rulesUrl = "/api/groups/g-1/schedule-rules";

/** Base UI selects commit a click only after a pointerdown on the same option (see test/selectOption.ts). */
async function choose(dialog: HTMLElement, label: string, option: string) {
  fireEvent.click(within(dialog).getByRole("combobox", { name: label }));
  const item = await screen.findByRole("option", { name: option });
  fireEvent.pointerDown(item);
  fireEvent.click(item);
}

async function openDialog() {
  renderRouter("/groups/g-1?tab=schedule");
  fireEvent.click(await screen.findByRole("button", { name: "Add weekly session" }, { timeout: 5000 }));
  return screen.findByRole("dialog", { name: "Add weekly session" });
}

/** Saturday 6:00 PM, 1 hour 30 minutes, Room 2: what the office types for the most common weekly session. */
async function fillSaturdayEvening(dialog: HTMLElement) {
  await choose(dialog, "Day", "Saturday");
  await choose(dialog, "Hour", "6");
  await choose(dialog, "Minute", "00");
  await choose(dialog, "AM or PM", "PM");
  await choose(dialog, "Length", "1 hour 30 minutes");
  fireEvent.change(within(dialog).getByRole("textbox", { name: "Room (optional)" }), { target: { value: "Room 2" } });
}

describe("AddScheduleRuleDialog", () => {
  let rulesCalls = 0;

  beforeEach(() => {
    vi.useFakeTimers({ toFake: ["Date"] });
    vi.setSystemTime(Now);
    rulesCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group())),
      http.get(rulesUrl, () => {
        rulesCalls += 1;
        return HttpResponse.json({ items: [] });
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("the start date defaults to today in the centre's zone, not the browser's", async () => {
    // 23:30 UTC on 19 April is already 01:30 on 20 April in Cairo.
    vi.setSystemTime(new Date("2027-04-19T23:30:00Z"));

    const dialog = await openDialog();

    expect(within(dialog).getByLabelText("Starts on")).toHaveValue("2027-04-20");
    expect(within(dialog).getByText("Times are in the centre's time zone (Africa/Cairo).")).toBeInTheDocument();
  });

  it("sends exactly the five contract fields as local values, then toasts the count and refreshes the rules", async () => {
    let body: unknown;
    server.use(
      http.post(rulesUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "r-new", sessionsCreated: 4 }, { status: 201 });
      }),
    );

    const dialog = await openDialog();
    await fillSaturdayEvening(dialog);
    const before = rulesCalls;
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));

    expect(await screen.findByText("4 sessions scheduled")).toBeInTheDocument();
    expect(body).toStrictEqual({
      dayOfWeek: "saturday",
      startTime: "18:00",
      durationMinutes: 90,
      room: "Room 2",
      effectiveFrom: "2027-04-20",
    });
    await waitFor(() => {
      expect(rulesCalls).toBeGreaterThan(before);
    });
  });

  it("the rule request never carries a teacher, a centre, an offset or an instant", async () => {
    let body: Record<string, unknown> = {};
    server.use(
      http.post(rulesUrl, async ({ request }) => {
        body = (await request.json()) as Record<string, unknown>;
        return HttpResponse.json({ id: "r-new", sessionsCreated: 4 }, { status: 201 });
      }),
    );

    const dialog = await openDialog();
    await fillSaturdayEvening(dialog);
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));
    await screen.findByText("4 sessions scheduled");

    expect(Object.keys(body).sort()).toEqual(["dayOfWeek", "durationMinutes", "effectiveFrom", "room", "startTime"]);
    expect(body).not.toHaveProperty("teacherId");
    expect(body).not.toHaveProperty("centreId");
    expect(body.startTime).toMatch(/^\d{2}:\d{2}$/);
    expect(body.effectiveFrom).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });

  it.each([
    ["schedule.teacher_double_booked", "The group's teacher already has a session at that time. Choose another day or time."],
    ["schedule.group_double_booked", "This group already has a session at that time. Choose another day or time."],
  ])("a 409 %s shows its own message and keeps the dialog open", async (code, message) => {
    server.use(http.post(rulesUrl, () => problemDetails(409, code, "Double booked.")));

    const dialog = await openDialog();
    await fillSaturdayEvening(dialog);
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));

    expect(await within(dialog).findByText(message)).toBeInTheDocument();
    expect(screen.getByRole("dialog", { name: "Add weekly session" })).toBeInTheDocument();
  });

  it("a 400 schedule.start_time_invalid lands under the start time", async () => {
    server.use(
      http.post(rulesUrl, () =>
        HttpResponse.json(
          {
            title: "One or more validation errors occurred.",
            status: 400,
            detail: "Start time must be on a 5-minute boundary.",
            code: "schedule.start_time_invalid",
            errors: { startTime: ["Start time must be on a 5-minute boundary."] },
          },
          { status: 400, headers: { "Content-Type": "application/problem+json" } },
        ),
      ),
    );

    const dialog = await openDialog();
    await fillSaturdayEvening(dialog);
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));

    const timeGroup = within(dialog).getByRole("group", { name: "Start time" });
    expect(await within(timeGroup).findByRole("alert")).toHaveTextContent("Start time must be on a 5-minute boundary.");
  });

  it("without a day nothing is sent and the day select says why", async () => {
    let sent = false;
    server.use(
      http.post(rulesUrl, () => {
        sent = true;
        return HttpResponse.json({ id: "r-new", sessionsCreated: 4 }, { status: 201 });
      }),
    );

    const dialog = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Add weekly session" }));

    expect(await within(dialog).findByText("Choose a day.")).toBeInTheDocument();
    expect(sent).toBe(false);
  });
});
