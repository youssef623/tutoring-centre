import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { delay, http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const subjectsUrl = "/api/subjects";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
};

function subject(overrides: Record<string, unknown> = {}) {
  return { id: "s-1", name: "Mathematics", status: "active", version: 1, ...overrides };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("SubjectsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(nileOwnerMe)),
      http.get(subjectsUrl, () => HttpResponse.json({ items: [] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows a loading skeleton, then the rows", async () => {
    server.use(
      http.get(subjectsUrl, async () => {
        await delay(30);
        return HttpResponse.json({ items: [subject()] });
      }),
    );

    renderRouter("/subjects");

    expect(await screen.findByRole("status", { name: "Loading subjects" })).toBeInTheDocument();
    expect(await screen.findByText("Mathematics")).toBeInTheDocument();
  });

  it("shows the empty state when the centre has no subjects", async () => {
    renderRouter("/subjects");

    expect(await screen.findByText("No subjects yet")).toBeInTheDocument();
    expect(screen.getByText("Add your first subject to get started.")).toBeInTheDocument();
  });

  it("shows an error with Retry, which succeeds", async () => {
    server.use(http.get(subjectsUrl, () => problem(500, "server.unexpected", "Something broke.")));

    renderRouter("/subjects");
    const retryButton = await screen.findByRole("button", { name: "Retry" }, { timeout: 3000 });

    server.use(http.get(subjectsUrl, () => HttpResponse.json({ items: [subject()] })));
    fireEvent.click(retryButton);

    expect(await screen.findByText("Mathematics")).toBeInTheDocument();
  });

  it("the toggle requests includeArchived=true", async () => {
    let lastIncludeArchived: string | null = null;
    server.use(
      http.get(subjectsUrl, ({ request }) => {
        lastIncludeArchived = new URL(request.url).searchParams.get("includeArchived");
        return HttpResponse.json({ items: [] });
      }),
    );

    renderRouter("/subjects");
    await screen.findByText("No subjects yet");
    fireEvent.click(screen.getByRole("switch", { name: "Show archived" }));

    await waitFor(() => {
      expect(lastIncludeArchived).toBe("true");
    });
  });

  it("create submits { name } only and includes the CSRF header", async () => {
    let body: unknown;
    let csrfHeader: string | null = null;
    server.use(
      http.post(subjectsUrl, async ({ request }) => {
        body = await request.json();
        csrfHeader = request.headers.get("X-XSRF-TOKEN");
        return HttpResponse.json({ id: "s-new" }, { status: 201, headers: { Location: "/api/subjects/s-new" } });
      }),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Add subject" }));
    fireEvent.change(await screen.findByLabelText("Name"), { target: { value: "Biology" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ name: "Biology" });
    });
    expect(csrfHeader).toBe("test-csrf-token");
  });

  it("create with a 409 name conflict shows the field error and keeps the dialog open", async () => {
    server.use(
      http.post(subjectsUrl, () =>
        problem(409, "subject.name_taken", "A subject with this name already exists.", {
          name: ["A subject with this name already exists."],
        }),
      ),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Add subject" }));
    fireEvent.change(await screen.findByLabelText("Name"), { target: { value: "Biology" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("A subject with this name already exists.")).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Add subject" })).toBeInTheDocument();
    expect(screen.getByLabelText("Name")).toBeInTheDocument();
  });

  it("client validation blocks an empty name with no request sent", async () => {
    let createCalls = 0;
    server.use(
      http.post(subjectsUrl, () => {
        createCalls += 1;
        return HttpResponse.json({ id: "s-new" }, { status: 201 });
      }),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Add subject" }));
    fireEvent.click(await screen.findByRole("button", { name: "Save" }));

    expect(await screen.findByText("Subject name is required.")).toBeInTheDocument();
    expect(createCalls).toBe(0);
  });

  it("rename sends the version the dialog was opened with", async () => {
    server.use(http.get(subjectsUrl, () => HttpResponse.json({ items: [subject({ version: 7 })] })));
    let body: unknown;
    server.use(
      http.put(`${subjectsUrl}/s-1`, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Rename" }));
    fireEvent.change(await screen.findByLabelText("Name"), { target: { value: "Pure Mathematics" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ name: "Pure Mathematics", version: 7 });
    });
  });

  it("a stale 409 on rename shows the conflict toast and refetches the list", async () => {
    let listCalls = 0;
    server.use(
      http.get(subjectsUrl, () => {
        listCalls += 1;
        return HttpResponse.json({ items: [subject()] });
      }),
    );
    server.use(
      http.put(`${subjectsUrl}/s-1`, () =>
        problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),
      ),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Rename" }));
    fireEvent.change(await screen.findByLabelText("Name"), { target: { value: "Pure Mathematics" } });
    const callsBeforeSave = listCalls;
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(
      await screen.findByText("This record was changed by someone else. Reload and try again."),
    ).toBeInTheDocument();
    await waitFor(() => {
      expect(listCalls).toBeGreaterThan(callsBeforeSave);
    });
    await waitFor(() => {
      expect(screen.queryByRole("heading", { name: "Rename subject" })).not.toBeInTheDocument();
    });
  });

  it("archive requires confirmation before the request is sent", async () => {
    server.use(http.get(subjectsUrl, () => HttpResponse.json({ items: [subject({ name: "Mathematics" })] })));
    let archiveCalls = 0;
    server.use(
      http.post(`${subjectsUrl}/s-1/archive`, () => {
        archiveCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/subjects");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Archive" }));

    expect(await screen.findByRole("heading", { name: 'Archive "Mathematics"?' })).toBeInTheDocument();
    expect(archiveCalls).toBe(0);

    fireEvent.click(screen.getByRole("button", { name: "Archive subject" }));

    await waitFor(() => {
      expect(archiveCalls).toBe(1);
    });
  });

  it("restore is offered only for archived rows", async () => {
    server.use(
      http.get(subjectsUrl, () =>
        HttpResponse.json({
          items: [
            subject({ id: "s-1", name: "Mathematics", status: "active" }),
            subject({ id: "s-2", name: "Biology", status: "archived" }),
          ],
        }),
      ),
    );

    renderRouter("/subjects?archived=true");
    const mathRow = (await screen.findByText("Mathematics")).closest("tr");
    const biologyRow = screen.getByText("Biology").closest("tr");
    if (mathRow === null || biologyRow === null) {
      throw new Error("Expected both subject rows to render.");
    }

    fireEvent.click(within(mathRow).getByRole("button", { name: "Open actions menu" }));
    expect(await screen.findByRole("menuitem", { name: "Archive" })).toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Restore" })).not.toBeInTheDocument();
    fireEvent.keyDown(document.activeElement ?? document.body, { key: "Escape" });

    fireEvent.click(within(biologyRow).getByRole("button", { name: "Open actions menu" }));
    expect(await screen.findByRole("menuitem", { name: "Restore" })).toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Rename" })).not.toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Archive" })).not.toBeInTheDocument();
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");

    renderRouter("/subjects");

    expect(await screen.findByRole("heading", { name: "المواد" })).toBeInTheDocument();
    expect(screen.getByText("إظهار المؤرشفة")).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
