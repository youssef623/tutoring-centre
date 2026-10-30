import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import type { MeDto } from "@/api/generated/tutoring-centre";
import { meQueryOptions } from "@/features/session/meQueryOptions";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { PortalAccountSection } from "./PortalAccountSection";

const secretary: MeDto = {
  userId: "secretary-1",
  displayName: "Nile Secretary",
  email: "secretary@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "centre-1",
  activeRole: "secretary",
  memberships: [],
  permissions: ["portal.accounts.manage"],
  isOperator: false,
};
const password = "One-time-only-9Kq!";
const account = {
  linkId: "link-1",
  membershipId: "membership-1",
  email: "parent@example.test",
  status: "active",
  version: 7,
  createdAt: "2026-10-30T10:00:00Z",
};

function renderSection(me = secretary) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  queryClient.setQueryData(meQueryOptions.queryKey, me);
  server.use(http.get("/api/me", () => HttpResponse.json(me)));
  const view = render(
    <QueryClientProvider client={queryClient}>
      <PortalAccountSection kind="parent" subjectId="parent-1" />
    </QueryClientProvider>,
  );
  return { ...view, queryClient };
}

describe("PortalAccountSection", () => {
  beforeEach(async () => {
    await i18n.changeLanguage("en");
    server.use(http.get("/api/portal-accounts", () => HttpResponse.json({ items: [] })));
  });
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("shows account management to a secretary", async () => {
    renderSection();
    expect(await screen.findByText("No account yet")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Create account" })).toBeEnabled();
  });

  it("hides the section and never loads account emails for a teacher", async () => {
    const list = vi.fn(() => HttpResponse.json({ items: [account] }));
    server.use(http.get("/api/portal-accounts", list));
    renderSection({ ...secretary, activeRole: "teacher", permissions: ["attendance.view"] });
    await waitFor(() => {
      expect(screen.queryByRole("region", { name: "Portal account" })).not.toBeInTheDocument();
    });
    expect(screen.queryByText(account.email)).not.toBeInTheDocument();
    expect(list).not.toHaveBeenCalled();
  });

  it("shows the password once, drops it on close, and never caches or persists it", async () => {
    let created = false;
    server.use(
      http.get("/api/portal-accounts", () =>
        HttpResponse.json({ items: created ? [account] : [] }),
      ),
      http.post("/api/portal-accounts", async ({ request }) => {
        expect(await request.json()).toEqual({
          kind: "parent",
          subjectId: "parent-1",
          email: account.email,
          preferredLocale: "ar",
        });
        created = true;
        return HttpResponse.json(
          {
            linkId: account.linkId,
            membershipId: account.membershipId,
            userId: "parent-user",
            temporaryPassword: password,
          },
          { status: 201, headers: { "Cache-Control": "no-store" } },
        );
      }),
    );
    const storage = vi.spyOn(Storage.prototype, "setItem");
    const { queryClient } = renderSection();
    fireEvent.click(await screen.findByRole("button", { name: "Create account" }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByLabelText("Email"), { target: { value: account.email } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Create account" }));
    expect(await screen.findByText(password)).toBeInTheDocument();
    expect(
      screen.getByText("Copy this password now. It cannot be shown again."),
    ).toBeInTheDocument();
    expect(
      JSON.stringify(
        queryClient
          .getQueryCache()
          .getAll()
          .map((query) => query.state.data),
      ),
    ).not.toContain(password);
    expect(queryClient.getMutationCache().getAll()).toHaveLength(0);
    fireEvent.click(screen.getByRole("button", { name: "Done" }));
    await waitFor(() => {
      expect(screen.queryByText(password)).not.toBeInTheDocument();
    });
    expect(await screen.findByText("Account active")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Copy password" })).not.toBeInTheDocument();
    expect(document.body.innerHTML).not.toContain(password);
    expect(storage).not.toHaveBeenCalled();
  });

  it("uses the current membership version when deactivating an account", async () => {
    let inactive = false;
    server.use(
      http.get("/api/portal-accounts", () =>
        HttpResponse.json({ items: [{ ...account, status: inactive ? "inactive" : "active" }] }),
      ),
      http.post("/api/portal-accounts/link-1/deactivate", async ({ request }) => {
        expect(await request.json()).toEqual({ version: 7 });
        inactive = true;
        return new HttpResponse(null, { status: 204 });
      }),
    );
    renderSection();
    fireEvent.click(await screen.findByRole("button", { name: "Deactivate" }));
    expect(await screen.findByText("Account inactive")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Reactivate" })).toBeEnabled();
  });
});
