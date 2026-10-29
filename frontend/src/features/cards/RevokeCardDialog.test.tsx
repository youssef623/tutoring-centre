import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { toast } from "sonner";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { Card, RevokeUrl } from "./cardTestData";
import { RevokeCardDialog } from "./RevokeCardDialog";

describe("RevokeCardDialog", () => {
  beforeEach(async () => { await i18n.changeLanguage("en"); });
  afterEach(() => { vi.restoreAllMocks(); });
  it("sends only permitted reason and version, never reissued", async () => {
    let body: unknown;
    server.use(http.post(RevokeUrl, async ({ request }) => { body = await request.json(); return new HttpResponse(null, { status: 204 }); }));
    const close = vi.fn(); const refresh = vi.fn().mockResolvedValue(undefined);
    renderWithQueryClient(<RevokeCardDialog card={Card} onClose={close} onRefresh={refresh} />);
    expect(screen.getAllByRole("option")).toHaveLength(3);
    expect(screen.queryByRole("option", { name: "Reissued" })).not.toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Reason", { exact: true }), { target: { value: "damaged" } });
    fireEvent.click(screen.getByRole("button", { name: "Revoke" }));
    await waitFor(() => { expect(close).toHaveBeenCalledOnce(); });
    expect(body).toEqual({ reason: "damaged", version: 17 });
    expect(refresh).toHaveBeenCalledOnce();
  });

  it.each([
    [409, "concurrency.stale", "The card changed. Status refreshed; review before trying again."],
    [422, "card.already_revoked", "This card is already revoked. Status refreshed."],
  ])("refreshes and closes on %s %s", async (status, code, message) => {
    server.use(http.post(RevokeUrl, () => problemDetails(status, code, "Changed")));
    const close = vi.fn(); const refresh = vi.fn().mockResolvedValue(undefined);
    const error = vi.spyOn(toast, "error");
    renderWithQueryClient(<RevokeCardDialog card={Card} onClose={close} onRefresh={refresh} />);
    fireEvent.click(screen.getByRole("button", { name: "Revoke" }));
    await waitFor(() => { expect(close).toHaveBeenCalledOnce(); });
    expect(refresh).toHaveBeenCalledOnce();
    expect(error).toHaveBeenCalledWith(message);
  });
});
