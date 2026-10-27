# `frontend/src/routes/_authenticated/parents.$parentId.test.tsx`

## Purpose

Declares `parentDetail`, `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 146 lines and 5,753 bytes. Source hash: `bb38e12c6608b051785c3fc9c3b33931838050f680c11c5dcd4579696d88c85a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L23): `function parentDetail(overrides: Record<string, unknown> = {}) {`
- [Line 37](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L37): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
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
```

The excerpt stops at line 90; use the original file for the remaining 56 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
