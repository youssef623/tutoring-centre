import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { group, roster, rosterEntry, secretaryMe, teacherMe } from "@/features/enrollments/enrollmentTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const rosterUrl = "/api/groups/g-1/roster";

describe("GroupRosterTab", () => {
  let rosterCalls = 0;

  beforeEach(() => {
    rosterCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group())),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get(rosterUrl, () => {
        rosterCalls += 1;
        return HttpResponse.json(roster());
      }),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the roster with seats and linked names", async () => {
    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByRole("status", { name: "Loading the roster" }, { timeout: 5000 })).toBeInTheDocument();
    const table = await screen.findByRole("table");
    expect(within(table).getByRole("link", { name: "Ahmed Mahmoud" })).toHaveAttribute("href", "/students/st-1");
    expect(within(table).getByRole("columnheader", { name: "Enrolled on" })).toBeInTheDocument();
    expect(screen.getByText("2 / 20")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Enroll student" })).toBeEnabled();
    expect(screen.getByRole("button", { name: "Actions for Sara Ali" })).toBeInTheDocument();
  });

  it("an empty roster says so and offers the enroll button", async () => {
    server.use(http.get(rosterUrl, () => HttpResponse.json(roster([]))));

    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("No students enrolled yet", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Enroll student" })).toBeInTheDocument();
  });

  it("a failed load shows the error with Retry, and Retry loads the roster", async () => {
    let fail = true;
    server.use(
      http.get(rosterUrl, () =>
        fail ? problemDetails(500, "server.error", "Boom.") : HttpResponse.json(roster()),
      ),
    );

    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("The roster could not be loaded", {}, { timeout: 5000 })).toBeInTheDocument();
    fail = false;
    fireEvent.click(screen.getByRole("button", { name: "Retry" }));
    expect(await screen.findByText("Sara Ali")).toBeInTheDocument();
  });

  it("with enrollments.view only (the group's teacher) shows plain names and no actions", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(teacherMe)));

    renderRouter("/groups/g-1?tab=roster");

    const table = await screen.findByRole("table", {}, { timeout: 5000 });
    expect(within(table).getByText("Ahmed Mahmoud")).toBeInTheDocument();
    expect(within(table).queryByRole("link")).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Enroll student" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /^Actions for/ })).not.toBeInTheDocument();
  });

  it("at capacity the enroll button is disabled with its reason, and Full is written out", async () => {
    server.use(http.get(rosterUrl, () => HttpResponse.json(roster(undefined, { capacity: 2 }))));

    renderRouter("/groups/g-1?tab=roster");

    const button = await screen.findByRole("button", { name: "Enroll student" }, { timeout: 5000 });
    expect(button).toHaveAttribute("aria-disabled", "true");
    expect(button).toHaveAccessibleDescription("This group is full. Withdraw a student or raise the capacity first.");
    expect(screen.getByText("Full")).toBeInTheDocument();
    fireEvent.click(button);
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });

  it("withdraw asks for confirmation naming student and group, then sends only the row's version", async () => {
    let body: unknown;
    server.use(
      http.post("/api/enrollments/e-2/withdraw", async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/groups/g-1?tab=roster");
    fireEvent.click(await screen.findByRole("button", { name: "Actions for Sara Ali" }, { timeout: 5000 }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Withdraw" }));
    const confirm = await screen.findByRole("alertdialog");
    expect(within(confirm).getByRole("heading", { name: "Withdraw Sara Ali from Physics A?" })).toBeInTheDocument();
    expect(body).toBeUndefined();
    const before = rosterCalls;
    fireEvent.click(within(confirm).getByRole("button", { name: "Withdraw" }));

    expect(await screen.findByText("Sara Ali was withdrawn from Physics A.")).toBeInTheDocument();
    expect(body).toStrictEqual({ version: 4 });
    await waitFor(() => {
      expect(rosterCalls).toBeGreaterThan(before);
    });
  });

  it("a stale withdraw is a toast followed by a refetch, not an error screen", async () => {
    server.use(http.post("/api/enrollments/e-1/withdraw", () => problemDetails(409, "concurrency.stale", "Stale.")));

    renderRouter("/groups/g-1?tab=roster");
    fireEvent.click(await screen.findByRole("button", { name: "Actions for Ahmed Mahmoud" }, { timeout: 5000 }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Withdraw" }));
    const before = rosterCalls;
    fireEvent.click(within(await screen.findByRole("alertdialog")).getByRole("button", { name: "Withdraw" }));

    expect(
      await screen.findByText("This enrollment was changed by someone else. The list has been refreshed."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(rosterCalls).toBeGreaterThan(before);
    });
    expect(screen.getByRole("table")).toBeInTheDocument();
  });

  it("in Arabic the page runs RTL while the seat pair stays LTR", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(rosterUrl, () => HttpResponse.json(roster([rosterEntry({ studentName: "أحمد محمود" })], { enrolledCount: 12 }))));

    renderRouter("/groups/g-1?tab=roster");

    expect(await screen.findByText("أحمد محمود", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(document.documentElement).toHaveAttribute("dir", "rtl");
    expect(screen.getByText("12 / 20")).toHaveAttribute("dir", "ltr");
    expect(screen.getByRole("button", { name: "إلحاق طالب" })).toBeInTheDocument();
  });
});
