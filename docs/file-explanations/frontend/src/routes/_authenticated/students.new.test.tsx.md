# `frontend/src/routes/_authenticated/students.new.test.tsx`

## Purpose

Declares `problem`, `fillStudentName`, `fillParentPhone`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 286 lines and 10,906 bytes. Source hash: `8e1df81081c11c62a05ee6efac85e93f2b10bef194cec26d09f28f941939609c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { selectOption } from "@/test/selectOption";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L25): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 32](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L32): `async function fillStudentName(name: string) {`
- [Line 36](../../../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L36): `async function fillParentPhone(phone: string) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 196 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
