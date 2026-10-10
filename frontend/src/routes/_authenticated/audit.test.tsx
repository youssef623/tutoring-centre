import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const auditUrl = "/api/audit";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["audit.view"],
};

function entry(overrides: Record<string, unknown> = {}) {
  return {
    id: "a-1",
    occurredAt: "2026-10-01T12:00:00Z",
    actorType: "user",
    actorUserId: "u-1",
    actorDisplayName: "Nile Owner",
    action: "updated",
    entityType: "membership",
    entityId: "m-1",
    changes: [],
    correlationId: null,
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string) {
  return HttpResponse.json({ title: "Error", status, code, detail }, { status });
}

describe("AuditPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(auditUrl, () => HttpResponse.json({ items: [], nextCursor: null })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    let releaseList: (() => void) | undefined;
    const gate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(auditUrl, async () => {
        await gate;
        return HttpResponse.json({ items: [entry()], nextCursor: null });
      }),
    );

    renderRouter("/audit");

    expect(await screen.findByRole("status", { name: "Loading audit log" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("Updated")).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(auditUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/audit");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(auditUrl, () => HttpResponse.json({ items: [entry()], nextCursor: null })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("Updated")).toBeInTheDocument();
  });

  it("shows the empty state when there is no activity for the filters", async () => {
    renderRouter("/audit");

    expect(await screen.findByText("No activity for these filters.")).toBeInTheDocument();
  });

  it("renders translated field names and translated before/after values, never a raw key", async () => {
    server.use(
      http.get(auditUrl, () =>
        HttpResponse.json({
          items: [
            entry({
              action: "updated",
              entityType: "membership",
              changes: [
                { field: "role", before: "teacher", after: "secretary" },
                { field: "status", before: "active", after: "inactive" },
                { field: "userId", before: null, after: "u-2" },
              ],
            }),
          ],
          nextCursor: null,
        }),
      ),
    );

    renderRouter("/audit");

    expect(await screen.findByText("Staff membership")).toBeInTheDocument();
    expect(screen.getByText("Teacher")).toBeInTheDocument();
    expect(screen.getByText("Assistant")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    expect(screen.getByText("Inactive")).toBeInTheDocument();
    expect(screen.getByText("—")).toBeInTheDocument();
    expect(screen.getByText("u-2")).toBeInTheDocument();
    expect(screen.queryByText("role")).not.toBeInTheDocument();
    expect(screen.queryByText("status")).not.toBeInTheDocument();
  });

  it("the correlation reference isolates only the id in ltr direction and can be copied", async () => {
    // jsdom has no Clipboard API; stub it so the copy succeeds and the "Copied!" feedback is reachable.
    Object.defineProperty(navigator, "clipboard", {
      value: { writeText: vi.fn().mockResolvedValue(undefined) },
      configurable: true,
    });
    server.use(
      http.get(auditUrl, () =>
        HttpResponse.json({ items: [entry({ correlationId: "abc123def456" })], nextCursor: null }),
      ),
    );

    renderRouter("/audit");

    const idSpan = await screen.findByText("abc123def456");
    expect(idSpan).toHaveAttribute("dir", "ltr");
    // The surrounding label is not itself forced ltr — only the id is isolated (the bidi fix).
    expect(idSpan.parentElement).not.toHaveAttribute("dir", "ltr");

    fireEvent.click(screen.getByRole("button", { name: "Copy reference" }));
    expect(await screen.findByText("Copied!")).toBeInTheDocument();
  });

  it("Load more fetches the next page and appends its rows", async () => {
    server.use(
      http.get(auditUrl, ({ request }) => {
        const cursor = new URL(request.url).searchParams.get("cursor");
        if (cursor === null) {
          return HttpResponse.json({
            items: [entry({ id: "a-1", actorDisplayName: "Audit Actor One" })],
            nextCursor: "page-2",
          });
        }
        return HttpResponse.json({
          items: [entry({ id: "a-2", actorDisplayName: "Audit Actor Two" })],
          nextCursor: null,
        });
      }),
    );

    renderRouter("/audit");
    await screen.findByText("Audit Actor One");
    expect(screen.queryByText("Audit Actor Two")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Load more" }));

    expect(await screen.findByText("Audit Actor Two")).toBeInTheDocument();
    expect(screen.getByText("Audit Actor One")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Load more" })).not.toBeInTheDocument();
  });

  it("filters update the request: entity type, from and to", async () => {
    const requests: URLSearchParams[] = [];
    server.use(
      http.get(auditUrl, ({ request }) => {
        requests.push(new URL(request.url).searchParams);
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );

    renderRouter("/audit");
    await screen.findByText("No activity for these filters.");

    await selectOption("Entity type", "Staff membership");
    await waitFor(() => {
      expect(requests.at(-1)?.get("entityType")).toBe("membership");
    });

    fireEvent.change(screen.getByLabelText("From"), { target: { value: "2026-01-01" } });
    await waitFor(() => {
      expect(requests.at(-1)?.get("from")).toBe("2026-01-01");
    });

    fireEvent.change(screen.getByLabelText("To"), { target: { value: "2026-01-31" } });
    await waitFor(() => {
      const last = requests.at(-1);
      expect(last?.get("to")).toBe("2026-01-31");
      expect(last?.get("entityType")).toBe("membership");
      expect(last?.get("from")).toBe("2026-01-01");
    });
  });

  it("without audit.view, the no-access state is shown and no audit request is issued", async () => {
    let auditCalls = 0;
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileOwnerMe, permissions: [] })),
      http.get(auditUrl, () => {
        auditCalls += 1;
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );

    renderRouter("/audit");

    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
    expect(auditCalls).toBe(0);
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");
    server.use(
      http.get(auditUrl, () =>
        HttpResponse.json({
          items: [entry({ changes: [{ field: "role", before: "teacher", after: "owner" }] })],
          nextCursor: null,
        }),
      ),
    );

    renderRouter("/audit");

    expect(await screen.findByRole("heading", { name: "سجل النشاط" })).toBeInTheDocument();
    const changeItem = (await screen.findByText("معلم")).closest("li");
    if (changeItem === null) {
      throw new Error("Expected the role change list item to render.");
    }
    expect(within(changeItem).getByText("مالك")).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
