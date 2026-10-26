import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const studentsUrl = "/api/students";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["students.manage", "students.view"],
};

const nileTeacherMe = {
  ...nileOwnerMe,
  activeRole: "teacher",
  permissions: [],
};

function student(overrides: Record<string, unknown> = {}) {
  return {
    id: "st-1",
    fullName: "أحمد محمود",
    schoolYear: "الصف الثالث الثانوي",
    status: "active",
    primaryParentName: "محمود حسن",
    primaryParentPhone: "+201012345678",
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string) {
  return HttpResponse.json({ title: "Error", status, code, detail }, { status });
}

describe("StudentsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(studentsUrl, () => HttpResponse.json({ items: [], nextCursor: null })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    let releaseList: (() => void) | undefined;
    const listGate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(studentsUrl, async () => {
        await listGate;
        return HttpResponse.json({ items: [student()], nextCursor: null });
      }),
    );

    renderRouter("/students");

    expect(await screen.findByRole("status", { name: "Loading students" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

    expect(await screen.findByText("أحمد محمود")).toBeInTheDocument();
  });

  it("shows the no-search empty state with a register link", async () => {
    renderRouter("/students");

    expect(await screen.findByText("No students yet")).toBeInTheDocument();
    expect(screen.getAllByRole("link", { name: "Register student" }).length).toBeGreaterThan(0);
  });

  it("shows the no-match empty state when a search returns nothing", async () => {
    server.use(
      http.get(studentsUrl, ({ request }) => {
        const search = new URL(request.url).searchParams.get("search");
        return HttpResponse.json({ items: search === null ? [student()] : [], nextCursor: null });
      }),
    );

    renderRouter("/students?q=zzz");

    expect(await screen.findByText('No students match "zzz".')).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(studentsUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/students");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(studentsUrl, () => HttpResponse.json({ items: [student()], nextCursor: null })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("أحمد محمود")).toBeInTheDocument();
  });

  it("typing sends exactly one request with search after the debounce", async () => {
    vi.useFakeTimers({ shouldAdvanceTime: true });
    const searchCalls: Array<string | null> = [];
    server.use(
      http.get(studentsUrl, ({ request }) => {
        searchCalls.push(new URL(request.url).searchParams.get("search"));
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );

    renderRouter("/students");
    await vi.waitFor(() => {
      expect(searchCalls.length).toBeGreaterThan(0);
    });
    const callsBeforeTyping = searchCalls.length;

    const input = screen.getByRole("searchbox");
    fireEvent.change(input, { target: { value: "ا" } });
    fireEvent.change(input, { target: { value: "اح" } });
    fireEvent.change(input, { target: { value: "احمد" } });

    await vi.advanceTimersByTimeAsync(300);

    await vi.waitFor(() => {
      expect(searchCalls.length).toBe(callsBeforeTyping + 1);
    });
    expect(searchCalls.at(-1)).toBe("احمد");
    vi.useRealTimers();
  });

  it("the archived switch requests includeArchived=true", async () => {
    let lastIncludeArchived: string | null = null;
    server.use(
      http.get(studentsUrl, ({ request }) => {
        lastIncludeArchived = new URL(request.url).searchParams.get("includeArchived");
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );

    renderRouter("/students");
    await screen.findByText("No students yet");
    fireEvent.click(screen.getByRole("switch", { name: "Show archived" }));

    await waitFor(() => {
      expect(lastIncludeArchived).toBe("true");
    });
  });

  it("load more requests the next cursor and appends the next page", async () => {
    server.use(
      http.get(studentsUrl, ({ request }) => {
        const cursor = new URL(request.url).searchParams.get("cursor");
        return cursor === null
          ? HttpResponse.json({ items: [student({ id: "st-1", fullName: "أحمد محمود" })], nextCursor: "page-2" })
          : HttpResponse.json({ items: [student({ id: "st-2", fullName: "زينب علي" })], nextCursor: null });
      }),
    );

    renderRouter("/students");
    await screen.findByText("أحمد محمود");
    fireEvent.click(screen.getByRole("button", { name: "Load more" }));

    expect(await screen.findByText("زينب علي")).toBeInTheDocument();
    expect(screen.getByText("أحمد محمود")).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Load more" })).not.toBeInTheDocument();
  });

  it("without students.manage the register button and link are absent", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileOwnerMe, permissions: ["students.view"] })),
      http.get(studentsUrl, () => HttpResponse.json({ items: [student()], nextCursor: null })),
    );

    renderRouter("/students");

    await screen.findByText("أحمد محمود");
    expect(screen.queryByRole("link", { name: "Register student" })).not.toBeInTheDocument();
  });

  it("in Arabic the document direction is rtl and the phone cell stays ltr", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(studentsUrl, () => HttpResponse.json({ items: [student()], nextCursor: null })));

    renderRouter("/students");

    expect(await screen.findByRole("heading", { name: "الطلاب" })).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
    const phoneCell = await screen.findByText("+201012345678");
    expect(phoneCell).toHaveAttribute("dir", "ltr");
  });

  it("no request is sent to any URL containing a centre id", async () => {
    const requestedUrls: string[] = [];
    server.use(
      http.get(studentsUrl, ({ request }) => {
        requestedUrls.push(request.url);
        return HttpResponse.json({ items: [], nextCursor: null });
      }),
    );

    renderRouter("/students");
    await screen.findByText("No students yet");

    for (const url of requestedUrls) {
      expect(url).not.toContain("c-nile");
      expect(url).not.toMatch(/centreId/i);
    }
  });

  it("a teacher without students.view sees the no-access state, no navigation item", async () => {
    server.use(http.get(meUrl, () => HttpResponse.json(nileTeacherMe)));

    renderRouter("/students");

    expect(await screen.findByText("You don't have access to this page")).toBeInTheDocument();
    expect(screen.queryByRole("link", { name: "Students" })).not.toBeInTheDocument();
  });
});
