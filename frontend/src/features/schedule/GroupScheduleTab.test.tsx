import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { group, Now, rule, secretaryMe, teacherMe } from "@/features/schedule/scheduleTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rulesUrl = "/api/groups/g-1/schedule-rules";

describe("GroupScheduleTab", () => {
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
        return HttpResponse.json({
          items: [rule(), rule({ id: "r-2", dayOfWeek: "monday", startTime: "16:00", durationMinutes: 60, room: null, effectiveTo: "2027-04-12" })],
        });
      }),
    );
  });

  afterEach(async () => {
    vi.useRealTimers();
    await i18n.changeLanguage("en");
  });

  it("shows a loading state, then the rules Saturday first, times left to right and an ended rule labelled", async () => {
    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByRole("status", { name: "Loading weekly sessions" }, { timeout: 5000 })).toBeInTheDocument();
    const table = await screen.findByRole("table");
    const [saturday, monday] = within(table).getAllByRole("row").slice(1);
    if (saturday === undefined || monday === undefined) {
      throw new Error("Expected two rule rows.");
    }
    expect(within(saturday).getByText("Saturday")).toBeInTheDocument();
    expect(within(saturday).getByText(/6:00\s?–\s?7:30\sPM/u)).toHaveAttribute("dir", "ltr");
    expect(within(saturday).getByText("Active")).toBeInTheDocument();
    expect(within(monday).getByText("Monday")).toBeInTheDocument();
    expect(within(monday).getByText("No room")).toBeInTheDocument();
    // effectiveTo 12 April is before today (20 April in Cairo): ended, and offered no End action.
    expect(within(monday).getByText("Ended")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /^End Saturday at/ })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^End Monday at/ })).not.toBeInTheDocument();
  });

  it("an empty list says so and offers to add a weekly session", async () => {
    server.use(http.get(rulesUrl, () => HttpResponse.json({ items: [] })));

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByText("No weekly sessions yet", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Add weekly session" })).toBeInTheDocument();
  });

  it("a failed load shows the error with Retry, and Retry loads the rules", async () => {
    let fail = true;
    server.use(
      http.get(rulesUrl, () => (fail ? problemDetails(500, "server.error", "Boom.") : HttpResponse.json({ items: [rule()] }))),
    );

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByText("Weekly sessions could not be loaded", {}, { timeout: 5000 })).toBeInTheDocument();
    fail = false;
    fireEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("Saturday")).toBeInTheDocument();
  });

  it("with schedule.view only (the group's teacher) the rules are read-only", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));

    renderRouter("/groups/g-1?tab=schedule");

    expect(await screen.findByRole("table", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add weekly session" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^End / })).not.toBeInTheDocument();
  });

  it("end asks for a last date not before the rule's start, then sends exactly { lastDate, version }", async () => {
    let body: unknown;
    server.use(
      http.post("/api/schedule-rules/r-1/end", async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/groups/g-1?tab=schedule");
    fireEvent.click(await screen.findByRole("button", { name: /^End Saturday at/ }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog");
    expect(within(dialog).getByText("Sessions after the last date are cancelled. Earlier sessions stay as they are.")).toBeInTheDocument();
    const lastDate = within(dialog).getByLabelText("Last date");
    // Defaults to today in the centre's zone; a date before the rule took effect is refused before any request.
    expect(lastDate).toHaveValue("2027-04-20");
    fireEvent.change(lastDate, { target: { value: "2027-03-01" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "End weekly session" }));
    expect(await within(dialog).findByRole("alert")).toHaveTextContent("The last date cannot be before");
    expect(body).toBeUndefined();

    fireEvent.change(lastDate, { target: { value: "2027-04-27" } });
    const before = rulesCalls;
    fireEvent.click(within(dialog).getByRole("button", { name: "End weekly session" }));

    expect(await screen.findByText("Weekly session ended")).toBeInTheDocument();
    expect(body).toStrictEqual({ lastDate: "2027-04-27", version: 5 });
    await waitFor(() => {
      expect(rulesCalls).toBeGreaterThan(before);
    });
  });
});
