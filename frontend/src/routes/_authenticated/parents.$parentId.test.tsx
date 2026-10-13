import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const parentUrl = "/api/parents/p-1";

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

function parentDetail(overrides: Record<string, unknown> = {}) {
  return {
    id: "p-1",
    fullName: "Mahmoud Hassan",
    phone: "+201000000001",
    version: 1,
    children: [
      { studentId: "s-1", fullName: "Ahmed Mahmoud", status: "active", relationship: "father", isPrimary: true },
      { studentId: "s-2", fullName: "Sara Mahmoud", status: "archived", relationship: "father", isPrimary: false },
    ],
    ...overrides,
  };
}

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("ParentDetailPage", () => {
  beforeEach(() => {
    server.use(http.get(meUrl, () => HttpResponse.json(nileSecretaryMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("lists every child with name link, relationship, primary badge and status", async () => {
    server.use(http.get(parentUrl, () => HttpResponse.json(parentDetail())));

    renderRouter("/parents/p-1");

    expect(await screen.findByRole("link", { name: "Ahmed Mahmoud" }, { timeout: 5000 })).toHaveAttribute(
      "href",
      "/students/s-1",
    );
    expect(screen.getByRole("link", { name: "Sara Mahmoud" })).toHaveAttribute("href", "/students/s-2");
    expect(screen.getByText("Primary contact")).toBeInTheDocument();
    expect(screen.getAllByText("Father")).toHaveLength(2);
    expect(screen.getByText("Archived")).toBeInTheDocument();
  });

  it("shows the empty state when the parent has no children linked", async () => {
    server.use(http.get(parentUrl, () => HttpResponse.json(parentDetail({ children: [] }))));

    renderRouter("/parents/p-1");

    expect(await screen.findByText("No students linked", {}, { timeout: 5000 })).toBeInTheDocument();
  });

  it("a 404 shows the Parent not found state with a link back to students", async () => {
    server.use(http.get(parentUrl, () => problem(404, "parent.not_found", "This parent could not be found.")));

    renderRouter("/parents/p-1");

    expect(await screen.findByText("Parent not found", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to students" })).toHaveAttribute("href", "/students");
  });

  it("a phone conflict (409 parent.phone_taken) on edit shows a field error on phone", async () => {
    server.use(http.get(parentUrl, () => HttpResponse.json(parentDetail())));
    server.use(
      http.put(parentUrl, () =>
        problem(409, "parent.phone_taken", "A parent with this phone number already exists.", {
          phone: ["A parent with this phone number already exists."],
        }),
      ),
    );

    renderRouter("/parents/p-1");
    fireEvent.click(await screen.findByRole("button", { name: "Edit" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Phone"), { target: { value: "+201099990009" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    expect(await screen.findByText("A parent with this phone number already exists.")).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: "Edit parent" })).toBeInTheDocument();
  });

  it("edit sends the loaded version and invalidates on success", async () => {
    server.use(http.get(parentUrl, () => HttpResponse.json(parentDetail())));
    let body: unknown;
    server.use(
      http.put(parentUrl, async ({ request }) => {
        body = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
    );

    renderRouter("/parents/p-1");
    fireEvent.click(await screen.findByRole("button", { name: "Edit" }, { timeout: 5000 }));
    fireEvent.change(await screen.findByLabelText("Full name"), { target: { value: "Mahmoud H. Hassan" } });
    fireEvent.click(screen.getByRole("button", { name: "Save" }));

    await waitFor(() => {
      expect(body).toEqual({ fullName: "Mahmoud H. Hassan", phone: "+201000000001", version: 1 });
    });
    expect(await screen.findByText("Parent updated.")).toBeInTheDocument();
  });

  it("with only students.view, Edit is not rendered", async () => {
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileSecretaryMe, permissions: ["students.view"] })),
      http.get(parentUrl, () => HttpResponse.json(parentDetail())),
    );

    renderRouter("/parents/p-1");

    await screen.findByText("Mahmoud Hassan", {}, { timeout: 5000 });
    expect(screen.queryByRole("button", { name: "Edit" })).not.toBeInTheDocument();
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");
    server.use(http.get(parentUrl, () => HttpResponse.json(parentDetail())));

    renderRouter("/parents/p-1");

    expect(await screen.findByText("الأبناء", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
