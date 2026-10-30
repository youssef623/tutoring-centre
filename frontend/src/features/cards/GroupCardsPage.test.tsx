import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { BulkUrl, CardUrl, issued, roster, rosterEntry, RosterUrl, serveCards, Teacher, Token } from "./cardTestData";

const Confirmation = "I understand that existing cards for the selected students will stop working.";
describe("GroupCardsPage", () => {
  beforeEach(async () => { await i18n.changeLanguage("en"); serveCards(); });
  afterEach(() => { vi.restoreAllMocks(); });

  it("sends one bulk request with only selected IDs, persists nothing, and drops sheet on leave", async () => {
    const bodies: unknown[] = [];
    server.use(http.post(BulkUrl, async ({ request }) => {
      bodies.push(await request.json()); return HttpResponse.json({ groupId: "g-1", groupName: "Physics A", cards: [issued()] });
    }));
    const storage = vi.spyOn(Storage.prototype, "setItem");
    const { router, queryClient } = renderRouter("/groups/g-1/cards");
    // The lazy route loads the group and then its roster; cold Windows workers can exceed the default 1s.
    expect(await screen.findByText("2 selected", {}, { timeout: 5000 })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Select none" }));
    fireEvent.click(screen.getByLabelText("Ahmed Mahmoud"));
    expect(screen.getByRole("button", { name: "Issue selected cards" })).toBeDisabled();
    fireEvent.click(screen.getByLabelText(Confirmation));
    fireEvent.click(screen.getByRole("button", { name: "Issue selected cards" }));
    await waitFor(() => { expect(document.querySelectorAll(".card-screen-sheet .card-face")).toHaveLength(1); });
    expect(bodies).toEqual([{ studentIds: ["st-1"] }]);
    expect(JSON.stringify(queryClient.getQueryCache().getAll().map((query) => query.state.data))).not.toContain(Token);
    expect(queryClient.getMutationCache().getAll()).toHaveLength(0);
    await router.navigate({ to: "/groups/$groupId", params: { groupId: "g-1" }, search: { tab: "overview" } });
    fireEvent.click(await screen.findByRole("link", { name: "Print cards" }));
    expect(await screen.findByText("2 selected", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(document.querySelectorAll(".card-face")).toHaveLength(0);
    expect(storage).not.toHaveBeenCalled();
  });

  it("blocks 61 selections and allows exactly 60", async () => {
    const items = Array.from({ length: 61 }, (_, index) => rosterEntry({ enrollmentId: `e-${String(index)}`, studentId: `s-${String(index)}`, studentName: `Student ${String(index)}` }));
    server.use(http.get(RosterUrl, () => HttpResponse.json(roster(items))));
    let body: { studentIds: string[] } | undefined;
    server.use(http.post(BulkUrl, async ({ request }) => {
      body = await request.json() as { studentIds: string[] };
      return HttpResponse.json({ groupId: "g-1", groupName: "Physics A", cards: body.studentIds.map((id) => issued(id)) });
    }));
    renderRouter("/groups/g-1/cards");
    expect(await screen.findByText("Select at most 60 students.")).toBeInTheDocument();
    fireEvent.click(screen.getByLabelText(Confirmation));
    expect(screen.getByRole("button", { name: "Issue selected cards" })).toBeDisabled();
    expect(body).toBeUndefined();
    fireEvent.click(screen.getByLabelText("Student 60", { exact: true }));
    fireEvent.click(screen.getByRole("button", { name: "Issue selected cards" }));
    await waitFor(() => { expect(body?.studentIds).toHaveLength(60); });
    expect(body?.studentIds).not.toContain("s-60");
  });

  it("refreshes roster when a selected student leaves before issue", async () => {
    let reads = 0;
    server.use(
      http.get(RosterUrl, () => HttpResponse.json(roster(++reads === 1 ? undefined : [rosterEntry()]))),
      http.post(BulkUrl, () => problemDetails(422, "card.student_not_in_group", "Not current")),
    );
    renderRouter("/groups/g-1/cards");
    await screen.findByText("2 selected");
    fireEvent.click(screen.getByLabelText(Confirmation));
    fireEvent.click(screen.getByRole("button", { name: "Issue selected cards" }));
    expect(await screen.findByText("A selected student is no longer in this group. The roster has been refreshed.")).toBeInTheDocument();
    expect(screen.queryByLabelText("Sara Ali")).not.toBeInTheDocument();
    expect(screen.getByText("1 selected")).toBeInTheDocument();
    expect(reads).toBe(2);
  });

  it("hides student section and print action from Teacher and blocks direct print URL", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(Teacher)));
    let cardReads = 0;
    server.use(http.get(CardUrl, () => { cardReads++; return HttpResponse.json({ card: null }); }));
    const { router } = renderRouter("/students/st-1");
    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
    expect(screen.queryByRole("region", { name: "Student cards" })).not.toBeInTheDocument();
    expect(cardReads).toBe(0);
    await router.navigate({ to: "/groups/$groupId", params: { groupId: "g-1" }, search: { tab: "overview" } });
    await screen.findByRole("heading", { name: "Physics A" });
    expect(screen.queryByRole("link", { name: "Print cards" })).not.toBeInTheDocument();
    await router.navigate({ to: "/groups/$groupId/cards", params: { groupId: "g-1" } });
    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
  });

  it("shows Group not found for a foreign group", async () => {
    server.use(http.get("/api/groups/g-1", () => problemDetails(404, "group.not_found", "Not found")));
    renderRouter("/groups/g-1/cards");
    expect(await screen.findByRole("heading", { name: "Group not found" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Issue selected cards" })).not.toBeInTheDocument();
  });
});
