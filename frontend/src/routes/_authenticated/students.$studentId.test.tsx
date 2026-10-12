import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const studentUrl = "/api/students/s-1";

const nileSecretaryMe = {
  userId: "u-1",
  displayName: "Nile Secretary",
  email: "secretary@nile.test",
  preferredLocale: "en",
  mustChangePassword: false,
  activeCentreId: "c-nile",
  activeRole: "secretary",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "secretary" }],
  permissions: ["students.manage", "students.view"],
};

function parent(overrides: Record<string, unknown> = {}) {
  return { parentId: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", relationship: "father", isPrimary: true, ...overrides };
}

function studentDetail(overrides: Record<string, unknown> = {}) {
  return {
    id: "s-1",
    fullName: "Ahmed Mahmoud",
    phone: null,
    schoolYear: null,
    status: "active",
    version: 1,
    parents: [parent()],
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

/** Rows render primary first (ParentsCard), so the non-primary parent's row menu is always the second one. */
function secondRowMenuButton(): HTMLElement {
  const button = screen.getAllByRole("button", { name: "Open actions menu" })[1];
  if (button === undefined) {
    throw new Error("Expected a second row menu button.");
  }

  return button;
}

describe("StudentDetailPage", () => {
  beforeEach(() => {
    server.use(http.get(meUrl, () => HttpResponse.json(nileSecretaryMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("a 404 shows the Student not found state with a link back to the list", async () => {
    server.use(http.get(studentUrl, () => problem(404, "student.not_found", "This student could not be found.")));

    renderRouter("/students/s-1");

    expect(await screen.findByText("Student not found", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to students" })).toHaveAttribute("href", "/students");
  });

  it("edit sends the loaded version", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));
    let body: unknown;
    server.use(
      http.put(studentUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Edit" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "Ahmed M. Mahmoud" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ fullName: "Ahmed M. Mahmoud", phone: null, schoolYear: null, version: 1 });
    });
    expect(await screen.findByText("Student updated.")).toBeInTheDocument();
  });

  it("a stale 409 on edit shows a toast, refetches and closes the dialog", async () => {
    let getCalls = 0;
    server.use(
      http.get(studentUrl, () => {
        getCalls += 1;
        return HttpResponse.json(studentDetail());
      }),
    );
    server.use(http.put(studentUrl, () => problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again.")));

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Edit" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "Ahmed M. Mahmoud" } });
    const callsBeforeSave = getCalls;
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("This record was changed by someone else. Reload and try again.")).toBeInTheDocument();
    await waitFor(() => {
      expect(getCalls).toBeGreaterThan(callsBeforeSave);
    });
    await waitFor(() => {
      expect(screen.queryByRole("heading", { name: "Edit student" })).not.toBeInTheDocument();
    });
  });

  it("archived hides Edit, Archive and the parents card's manage actions, and shows a banner", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail({ status: "archived" }))));

    renderRouter("/students/s-1");

    expect(await screen.findByText("This student is archived", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Edit" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Archive" })).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Restore" })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add parent" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Open actions menu" })).not.toBeInTheDocument();
  });

  it("archive confirmation names the student and requires confirmation", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));
    let archiveCalls = 0;
    server.use(
      http.post(`${studentUrl}/archive`, () => {
        archiveCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Archive" }, { timeout: 5000 }));

    expect(await screen.findByRole("heading", { name: 'Archive "Ahmed Mahmoud"?' })).toBeInTheDocument();
    expect(archiveCalls).toBe(0);

    fireEvent.click(screen.getByRole("button", { name: "Archive student" }));

    await waitFor(() => {
      expect(archiveCalls).toBe(1);
    });
    expect(await screen.findByText("Student archived.")).toBeInTheDocument();
  });

  it("restore requires no confirmation", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail({ status: "archived" }))));
    let restoreCalls = 0;
    server.use(
      http.post(`${studentUrl}/restore`, () => {
        restoreCalls += 1;
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Restore" }, { timeout: 5000 }));

    await waitFor(() => {
      expect(restoreCalls).toBe(1);
    });
    expect(await screen.findByText("Student restored.")).toBeInTheDocument();
  });

  it("parents card: no Unlink or Make primary on the only, primary parent", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail({ parents: [parent()] }))));

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Open actions menu" }, { timeout: 5000 }));

    expect(screen.queryByRole("menuitem", { name: "Make primary" })).not.toBeInTheDocument();
    expect(screen.queryByRole("menuitem", { name: "Unlink" })).not.toBeInTheDocument();
  });

  it("parents card: a non-primary parent (not the only one) offers both Make primary and Unlink", async () => {
    server.use(
      http.get(studentUrl, () =>
        HttpResponse.json(
          studentDetail({ parents: [parent(), parent({ parentId: "p-2", fullName: "Huda Ali", isPrimary: false })] }),
        ),
      ),
    );

    renderRouter("/students/s-1");
    await screen.findByText("Huda Ali", {}, { timeout: 5000 });
    // Primary first: the non-primary row's menu is the second one.
    fireEvent.click(secondRowMenuButton());

    expect(await screen.findByRole("menuitem", { name: "Make primary" })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: "Unlink" })).toBeInTheDocument();
  });

  it("parents card: Add parent is not offered at four parents", async () => {
    server.use(
      http.get(studentUrl, () =>
        HttpResponse.json(
          studentDetail({
            parents: [
              parent(),
              parent({ parentId: "p-2", fullName: "P2", isPrimary: false }),
              parent({ parentId: "p-3", fullName: "P3", isPrimary: false }),
              parent({ parentId: "p-4", fullName: "P4", isPrimary: false }),
            ],
          }),
        ),
      ),
    );

    renderRouter("/students/s-1");
    await screen.findByText("Mahmoud Hassan", {}, { timeout: 5000 });

    expect(screen.queryByRole("button", { name: "Add parent" })).not.toBeInTheDocument();
  });

  it("make primary refetches the student so the next action uses a fresh version", async () => {
    let getCalls = 0;
    server.use(
      http.get(studentUrl, () => {
        getCalls += 1;
        return HttpResponse.json(
          studentDetail({
            version: getCalls,
            parents: [parent(), parent({ parentId: "p-2", fullName: "Huda Ali", isPrimary: false })],
          }),
        );
      }),
    );
    server.use(http.post(`${studentUrl}/parents/p-2/make-primary`, () => new HttpResponse(null, { status: 204 })));

    renderRouter("/students/s-1");
    await screen.findByText("Huda Ali", {}, { timeout: 5000 });
    const callsBeforeAction = getCalls;
    // Primary first: the non-primary row's menu is the second one.
    fireEvent.click(secondRowMenuButton());
    fireEvent.click(await screen.findByRole("menuitem", { name: "Make primary" }));

    expect(await screen.findByText("Primary contact updated.")).toBeInTheDocument();
    await waitFor(() => {
      expect(getCalls).toBeGreaterThan(callsBeforeAction);
    });
  });

  it("add parent: an unmatched phone creates the parent, then links it", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));
    server.use(http.post("/api/parents/lookup", () => HttpResponse.json({ parent: null })));
    let createBody: unknown;
    server.use(
      http.post("/api/parents", async ({ request }) => {
        createBody = await request.json();
        return HttpResponse.json({ id: "p-new" }, { status: 201 });
      }),
    );
    let linkBody: unknown;
    server.use(
      http.post(`${studentUrl}/parents`, async ({ request }) => {
        linkBody = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Add parent" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Parent's phone"), { target: { value: "+201099990009" } });
    fireEvent.blur(screen.getByLabelText("Parent's phone"));
    fireEvent.change(await screen.findByLabelText("Parent's full name"), { target: { value: "Huda Ali" } });
    fireEvent.click(screen.getByRole("button", { name: "Link" }));

    await waitFor(() => {
      expect(createBody).toEqual({ fullName: "Huda Ali", phone: "+201099990009" });
    });
    await waitFor(() => {
      expect(linkBody).toEqual({ parentId: "p-new", relationship: "father", version: 1 });
    });
    expect(await screen.findByText("Parent linked.", {}, { timeout: 5000 })).toBeInTheDocument();
  });

  it("add parent: if linking fails after the parent was created, a retry only repeats the link", async () => {
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));
    server.use(http.post("/api/parents/lookup", () => HttpResponse.json({ parent: null })));
    let createCalls = 0;
    server.use(
      http.post("/api/parents", () => {
        createCalls += 1;
        return HttpResponse.json({ id: "p-new" }, { status: 201 });
      }),
    );
    let linkCalls = 0;
    server.use(
      http.post(`${studentUrl}/parents`, () => {
        linkCalls += 1;
        return linkCalls === 1
          ? HttpResponse.json({ title: "Error", status: 500, code: "server.unexpected", detail: "Something broke." }, { status: 500 })
          : new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/students/s-1");
    fireEvent.click(await screen.findByRole("button", { name: "Add parent" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Parent's phone"), { target: { value: "+201099990009" } });
    fireEvent.blur(screen.getByLabelText("Parent's phone"));
    fireEvent.change(await screen.findByLabelText("Parent's full name"), { target: { value: "Huda Ali" } });
    fireEvent.click(screen.getByRole("button", { name: "Link" }));

    expect(await screen.findByText(/now exists but could not be linked/, {}, { timeout: 5000 })).toBeInTheDocument();
    fireEvent.click(await screen.findByRole("button", { name: "Retry link" }));

    await waitFor(() => {
      expect(linkCalls).toBe(2);
    });
    expect(createCalls).toBe(1);
    expect(await screen.findByText("Parent linked.", {}, { timeout: 5000 })).toBeInTheDocument();
  });

  it("with only students.view, no manage action is rendered anywhere on the page", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileSecretaryMe, permissions: ["students.view"] })),
      http.get(studentUrl, () => HttpResponse.json(studentDetail())),
    );

    renderRouter("/students/s-1");

    await screen.findByText("Ahmed Mahmoud", {}, { timeout: 5000 });
    expect(screen.queryByRole("button", { name: "Edit" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Archive" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Restore" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Add parent" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Open actions menu" })).not.toBeInTheDocument();
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(studentUrl, () => HttpResponse.json(studentDetail())));

    renderRouter("/students/s-1");

    expect(await screen.findByText("أولياء الأمور", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
