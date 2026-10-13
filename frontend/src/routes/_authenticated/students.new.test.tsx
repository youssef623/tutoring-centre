import { fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { selectOption } from "@/test/selectOption";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const meUrl = "/api/me";
const lookupUrl = "/api/parents/lookup";
const registerUrl = "/api/students";

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

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

async function fillStudentName(name: string) {
  fireEvent.change(await screen.findByLabelText("Full name", {}, { timeout: 5000 }), { target: { value: name } });
}

async function fillParentPhone(phone: string) {
  fireEvent.change(await screen.findByLabelText("Parent's phone", {}, { timeout: 5000 }), { target: { value: phone } });
  fireEvent.blur(screen.getByLabelText("Parent's phone"));
}

describe("RegisterStudentForm", () => {
  beforeEach(() => {
    server.use(http.get(meUrl, () => HttpResponse.json(nileSecretaryMe)));
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("lookup found: shows the existing-parent panel and sends parent: { id } only", async () => {
    server.use(
      http.post(lookupUrl, () =>
        HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } }),
      ),
    );
    let body: unknown;
    server.use(
      http.post(registerUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "s-new", parentId: "p-1" }, { status: 201 });
      }),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    await fillParentPhone("+201000000001");

    expect(await screen.findByText("Existing parent: Mahmoud Hassan")).toBeInTheDocument();
    expect(screen.queryByLabelText("Parent's full name")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    await waitFor(() => {
      expect(body).toEqual({
        fullName: "Ahmed Mahmoud",
        phone: null,
        schoolYear: null,
        parent: { id: "p-1" },
        relationship: "father",
      });
    });
  });

  it("lookup empty: shows the name field and sends parent: { fullName, phone }", async () => {
    server.use(http.post(lookupUrl, () => HttpResponse.json({ parent: null })));
    let body: unknown;
    server.use(
      http.post(registerUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "s-new", parentId: "p-new" }, { status: 201 });
      }),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    await fillParentPhone("+201099990000");
    expect(await screen.findByLabelText("Parent's full name")).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Parent's full name"), { target: { value: "Mahmoud Hassan" } });

    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    await waitFor(() => {
      expect(body).toEqual({
        fullName: "Ahmed Mahmoud",
        phone: null,
        schoolYear: null,
        parent: { fullName: "Mahmoud Hassan", phone: "+201099990000" },
        relationship: "father",
      });
    });
    // Exactly the five documented keys — nothing else leaks into the request body.
    expect(Object.keys(body as object).sort()).toEqual(
      ["fullName", "parent", "phone", "relationship", "schoolYear"].sort(),
    );
  });

  it("phone.invalid on lookup shows a field error under the parent phone input", async () => {
    server.use(http.post(lookupUrl, () => problem(400, "phone.invalid", "Phone number is not valid.", { phone: ["Phone number is not valid."] })));

    renderRouter("/students/new");
    await fillParentPhone("123");

    expect(await screen.findByText("Phone number is not valid.")).toBeInTheDocument();
  });

  it("changing the phone after a found lookup clears the panel", async () => {
    server.use(
      http.post(lookupUrl, () =>
        HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } }),
      ),
    );

    renderRouter("/students/new");
    await fillParentPhone("+201000000001");
    expect(await screen.findByText("Existing parent: Mahmoud Hassan")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("Parent's phone"), { target: { value: "+201000000002" } });

    expect(screen.queryByText("Existing parent: Mahmoud Hassan")).not.toBeInTheDocument();
    expect(screen.getByLabelText("Parent's full name")).toBeInTheDocument();
  });

  it("a 409 parent.phone_taken on submit re-runs the lookup and shows the existing-parent panel", async () => {
    let lookupCalls = 0;
    server.use(
      http.post(lookupUrl, () => {
        lookupCalls += 1;
        return lookupCalls === 1
          ? HttpResponse.json({ parent: null })
          : HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } });
      }),
    );
    server.use(
      http.post(registerUrl, () =>
        problem(409, "parent.phone_taken", "A parent with this phone number already exists.", {
          "parent.phone": ["A parent with this phone number already exists."],
        }),
      ),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    await fillParentPhone("+201000000001");
    await screen.findByLabelText("Parent's full name");
    fireEvent.change(screen.getByLabelText("Parent's full name"), { target: { value: "Mahmoud Hassan" } });

    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    expect(await screen.findByText("Existing parent: Mahmoud Hassan")).toBeInTheDocument();
    expect(lookupCalls).toBe(2);
  });

  it("server field errors on parent.phone and parent.fullName reach their inputs", async () => {
    server.use(http.post(lookupUrl, () => HttpResponse.json({ parent: null })));
    server.use(
      http.post(registerUrl, () =>
        problem(400, "validation.failed", "Some fields are invalid.", {
          "parent.phone": ["Phone number is not valid."],
          "parent.fullName": ["Parent name must be 2-120 characters."],
        }),
      ),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    await fillParentPhone("+201099990000");
    await screen.findByLabelText("Parent's full name");
    fireEvent.change(screen.getByLabelText("Parent's full name"), { target: { value: "M" } });

    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    expect(await screen.findByText("Phone number is not valid.")).toBeInTheDocument();
    expect(screen.getByText("Parent name must be 2-120 characters.")).toBeInTheDocument();
  });

  it("submit is disabled while the lookup is pending", async () => {
    let releaseLookup: (() => void) | undefined;
    const lookupGate = new Promise<void>((resolve) => {
      releaseLookup = resolve;
    });
    server.use(
      http.post(lookupUrl, async () => {
        await lookupGate;
        return HttpResponse.json({ parent: null });
      }),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    fireEvent.change(await screen.findByLabelText("Parent's phone"), { target: { value: "+201099990000" } });
    fireEvent.blur(screen.getByLabelText("Parent's phone"));
    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    expect(screen.getByRole("button", { name: "Register student" })).toBeDisabled();
    releaseLookup?.();
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Register student" })).not.toBeDisabled();
    });
  });

  it("an empty student name blocks submission with no request sent", async () => {
    let registerCalls = 0;
    server.use(
      http.post(registerUrl, () => {
        registerCalls += 1;
        return HttpResponse.json({ id: "s-new", parentId: "p-new" }, { status: 201 });
      }),
    );

    renderRouter("/students/new");
    fireEvent.click(await screen.findByRole("button", { name: "Register student" }, { timeout: 5000 }));

    expect(await screen.findByText("Student name is required.")).toBeInTheDocument();
    expect(registerCalls).toBe(0);
  });

  it("with only students.view, /students/new shows the no-access state and issues no request", async () => {
    let lookupCalls = 0;
    server.use(
      http.get(meUrl, () => HttpResponse.json({ ...nileSecretaryMe, permissions: ["students.view"] })),
      http.post(lookupUrl, () => {
        lookupCalls += 1;
        return HttpResponse.json({ parent: null });
      }),
    );

    renderRouter("/students/new");

    expect(await screen.findByText("You don't have access to this page", {}, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByLabelText("Full name")).not.toBeInTheDocument();
    expect(lookupCalls).toBe(0);
  });

  it("the relationship select changes the request's relationship value", async () => {
    server.use(http.post(lookupUrl, () => HttpResponse.json({ parent: null })));
    let body: { relationship?: string } | undefined;
    server.use(
      http.post(registerUrl, async ({ request }) => {
        body = (await request.json()) as { relationship?: string };
        return HttpResponse.json({ id: "s-new", parentId: "p-new" }, { status: 201 });
      }),
    );

    renderRouter("/students/new");
    await fillStudentName("Ahmed Mahmoud");
    await fillParentPhone("+201099990000");
    await screen.findByLabelText("Parent's full name");
    fireEvent.change(screen.getByLabelText("Parent's full name"), { target: { value: "Mahmoud Hassan" } });
    await selectOption("Relationship", "Mother");

    fireEvent.click(screen.getByRole("button", { name: "Register student" }));

    await waitFor(() => {
      expect(body?.relationship).toBe("mother");
    });
  });

  it("in Arabic the document direction is rtl and labels are Arabic", async () => {
    await i18n.changeLanguage("ar");

    renderRouter("/students/new");

    expect(await screen.findByRole("heading", { name: "تسجيل طالب" }, { timeout: 5000 })).toBeInTheDocument();
    expect(document.documentElement.dir).toBe("rtl");
  });
});
