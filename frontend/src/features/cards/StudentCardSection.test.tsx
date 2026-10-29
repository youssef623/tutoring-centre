import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { Card, CardUrl, issued, serveCards, Student, Token } from "./cardTestData";

describe("StudentCardSection", () => {
  beforeEach(async () => { await i18n.changeLanguage("en"); serveCards(); });
  afterEach(() => { vi.restoreAllMocks(); });

  it("issues a local QR SVG, caches no secret, writes neither storage, and drops QR on close", async () => {
    let hasCard = false;
    server.use(
      http.get(CardUrl, () => HttpResponse.json({ card: hasCard ? Card : null })),
      http.post(CardUrl, async ({ request }) => { expect(await request.text()).toBe(""); hasCard = true; return HttpResponse.json(issued(), { status: 201 }); }),
    );
    const storage = vi.spyOn(Storage.prototype, "setItem"); // Covers both localStorage and sessionStorage.
    const { queryClient } = renderRouter("/students/st-1");
    expect(await screen.findByText("No card issued", {}, { timeout: 5000 })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Issue" }));
    const dialog = await screen.findByRole("dialog");
    expect(within(dialog).getByRole("img", { name: "Student card QR code" })).toBeInTheDocument();
    expect(within(dialog).getByText(/cannot be shown again/)).toBeInTheDocument();
    expect(document.body.textContent).not.toContain(Token);
    expect(JSON.stringify(queryClient.getQueryCache().getAll().map((query) => query.state.data))).not.toContain(Token);
    expect(queryClient.getMutationCache().getAll()).toHaveLength(0);
    fireEvent.click(within(dialog).getByText("Close", { selector: "button" }));
    await waitFor(() => { expect(document.querySelectorAll(".card-qr")).toHaveLength(0); });
    expect(document.body.innerHTML).not.toContain(Token);
    expect(storage).not.toHaveBeenCalled();
  });

  it("requires explicit consequence confirmation before reissuing", async () => {
    serveCards(Card);
    let calls = 0;
    server.use(http.post(CardUrl, () => { calls++; return HttpResponse.json(issued(), { status: 201 }); }));
    renderRouter("/students/st-1");
    fireEvent.click(await screen.findByRole("button", { name: "Reissue" }));
    expect(screen.getByText("The current card stops working as soon as the new card is issued.")).toBeInTheDocument();
    expect(calls).toBe(0);
    fireEvent.click(screen.getByRole("button", { name: "Confirm reissue" }));
    expect(await screen.findByText("Card ready to print")).toBeInTheDocument();
    expect(calls).toBe(1);
  });

  it("disables issue for an archived student and explains why", async () => {
    serveCards(null, { ...Student, status: "archived" });
    renderRouter("/students/st-1");
    expect(await screen.findByText("Archived students cannot receive a new card.")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Issue" })).toBeDisabled();
  });

  it("offers issue again for a revoked card with its reason and date", async () => {
    serveCards({ ...Card, status: "revoked", revocationReason: "lost", revokedAt: Card.issuedAt });
    renderRouter("/students/st-1");
    expect(await screen.findByText(/Revoked: Lost/)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Issue" })).toBeEnabled();
    expect(screen.queryByRole("button", { name: "Revoke" })).not.toBeInTheDocument();
  });

  it("explains concurrent issuing without retaining a card", async () => {
    server.use(http.post(CardUrl, () => problemDetails(409, "card.concurrent_issue", "Concurrent issue")));
    renderRouter("/students/st-1");
    fireEvent.click(await screen.findByRole("button", { name: "Issue" }));
    expect(await screen.findByText("Someone else issued cards at the same time; issue again.")).toBeInTheDocument();
    expect(document.querySelector(".card-qr")).not.toBeInTheDocument();
  });
});
