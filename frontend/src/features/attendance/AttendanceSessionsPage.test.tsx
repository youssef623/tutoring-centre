import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { secretaryMe, sessionItem, sessionsDto, sessionsUrl, teacherMe } from "./attendanceTestData";

describe("AttendanceSessionsPage", () => {
  beforeEach(() => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows loading skeleton cards, then the sessions sorted by start time", async () => {
    server.use(
      http.get(sessionsUrl, () =>
        HttpResponse.json(
          sessionsDto([
            sessionItem({ sessionId: "s-late", groupName: "Late group", startsAt: "2026-12-05T17:00:00Z", endsAt: "2026-12-05T18:00:00Z" }),
            sessionItem({ sessionId: "s-early", groupName: "Early group", startsAt: "2026-12-05T13:00:00Z", endsAt: "2026-12-05T14:00:00Z" }),
          ]),
        ),
      ),
    );

    renderRouter("/attendance");

    expect(await screen.findByRole("status", { name: "Loading sessions" }, { timeout: 3000 })).toBeInTheDocument();
    const cards = await screen.findAllByRole("link", { name: /(Early|Late) group/ });
    expect(cards.map((card) => (card.textContent.includes("Early group") ? "early" : "late"))).toEqual(["early", "late"]);
    expect(screen.queryByRole("status", { name: "Loading sessions" })).not.toBeInTheDocument();
  });

  it("shows an error with Retry, which loads the sessions", async () => {
    let calls = 0;
    server.use(
      http.get(sessionsUrl, () => {
        calls += 1;
        // The query client retries a failed read once, so the first two calls both fail.
        return calls <= 2 ? HttpResponse.json({ title: "Error", status: 500 }, { status: 500 }) : HttpResponse.json(sessionsDto([sessionItem()]));
      }),
    );

    renderRouter("/attendance");

    fireEvent.click(await screen.findByRole("button", { name: "Retry" }, { timeout: 5000 }));
    expect(await screen.findByRole("link", { name: /Physics A/ })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Retry" })).not.toBeInTheDocument();
  });

  it("shows the empty state when there are no sessions on the day", async () => {
    server.use(http.get(sessionsUrl, () => HttpResponse.json(sessionsDto([]))));

    renderRouter("/attendance");

    expect(await screen.findByText("No sessions on this day", {}, { timeout: 3000 })).toBeInTheDocument();
  });

  it("formats times in the zone the response names, whatever zone the browser is in", async () => {
    server.use(http.get(sessionsUrl, () => HttpResponse.json(sessionsDto([sessionItem()]))));

    renderRouter("/attendance");

    // 15:00Z–16:30Z is 5:00–6:30 PM in Cairo in December.
    const time = await screen.findByText(/5:00.*6:30\s*PM/, {}, { timeout: 3000 });
    expect(time).toHaveAttribute("dir", "ltr");
    expect(screen.getByText("12 / 18")).toHaveAttribute("dir", "ltr");
  });

  it("links an open session to its register, and shows a cancelled one muted, labelled and not clickable", async () => {
    server.use(
      http.get(sessionsUrl, () =>
        HttpResponse.json(sessionsDto([sessionItem(), sessionItem({ sessionId: "s-gone", groupName: "Chem B", status: "cancelled" })])),
      ),
    );

    renderRouter("/attendance");

    expect(await screen.findByRole("link", { name: /Physics A/ }, { timeout: 3000 })).toHaveAttribute("href", "/attendance/sessions/s-1");
    expect(screen.getByText("Cancelled")).toBeInTheDocument();
    expect(screen.getByText("Chem B")).toBeInTheDocument();
    expect(screen.queryByRole("link", { name: /Chem B/ })).not.toBeInTheDocument();
  });

  it("shows the server's window state, not one worked out from the browser's clock", async () => {
    server.use(
      http.get(sessionsUrl, () =>
        HttpResponse.json(
          sessionsDto([
            // Years away from the test machine's clock, yet the server says closed: the badge says closed.
            sessionItem({ sessionId: "s-closed", groupName: "Closed group", window: "closed" }),
            sessionItem({ sessionId: "s-wait", groupName: "Waiting group", window: "not_open" }),
          ]),
        ),
      ),
    );

    renderRouter("/attendance");

    const closed = await screen.findByRole("link", { name: /Closed group/ }, { timeout: 3000 });
    expect(within(closed).getByText("Closed")).toBeInTheDocument();
    // Opens 30 minutes before the 5:00 PM start, in Cairo time.
    const waiting = screen.getByRole("link", { name: /Waiting group/ });
    expect(within(waiting).getByText(/Opens at .*4:30\s*PM/)).toBeInTheDocument();
  });

  it("keeps the day in the URL: the date is requested, the arrows step it and Today clears it", async () => {
    const requested: (string | null)[] = [];
    server.use(
      http.get(sessionsUrl, ({ request }) => {
        const date = new URL(request.url).searchParams.get("date");
        requested.push(date);
        return HttpResponse.json(sessionsDto([sessionItem()], { date: date ?? "2026-12-05" }));
      }),
    );

    const { router } = renderRouter("/attendance?date=2026-12-06");

    await screen.findByRole("link", { name: /Physics A/ }, { timeout: 3000 });
    expect(requested).toEqual(["2026-12-06"]);

    fireEvent.click(screen.getByRole("button", { name: "Next day" }));
    await waitFor(() => {
      expect(router.state.location.search).toEqual({ date: "2026-12-07" });
    });
    await waitFor(() => {
      expect(requested).toContain("2026-12-07");
    });

    fireEvent.click(await screen.findByRole("button", { name: "Today" }));
    await waitFor(() => {
      expect(router.state.location.search).toEqual({});
    });
    await waitFor(() => {
      expect(requested).toContain(null);
    });
  });

  it("reads right to left in Arabic, with times and counts kept left to right", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json({ ...secretaryMe, preferredLocale: "ar" })));
    server.use(http.get(sessionsUrl, () => HttpResponse.json(sessionsDto([sessionItem()]))));
    await i18n.changeLanguage("ar");

    renderRouter("/attendance");

    expect(await screen.findByText("12 / 18", {}, { timeout: 3000 })).toHaveAttribute("dir", "ltr");
    expect(document.documentElement).toHaveAttribute("dir", "rtl");
    expect(screen.getByText("مسجّل")).toBeInTheDocument();
  });
});
