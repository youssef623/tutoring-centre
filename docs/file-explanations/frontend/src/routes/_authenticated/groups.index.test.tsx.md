# `frontend/src/routes/_authenticated/groups.index.test.tsx`

## Purpose

Declares `me`, `group`, `problem`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 287 lines and 12,473 bytes. Source hash: `0c1d1813b2b40620a7cf236e0b02e83ca51540f993cf598ee96694d731308b04`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`
- Line 7: `import { selectOption } from "@/test/selectOption";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L14): `function me(role: "owner" | "teacher", permissions: string[]) {`
- [Line 31](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L31): `function group(overrides: Record<string, unknown> = {}) {`
- [Line 56](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L56): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { selectOption } from "@/test/selectOption";

const meUrl = "/api/me";
const groupsUrl = "/api/groups";
const subjectsUrl = "/api/subjects";
const teachersUrl = "/api/teachers";

function me(role: "owner" | "teacher", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Two-Centre Teacher",
    email: role === "owner" ? "owner@nile.test" : "teacher@both.test",
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role }],
    permissions,
  };
}

const ownerMe = me("owner", ["groups.manage", "groups.view", "subjects.manage", "subjects.view", "teachers.manage", "teachers.view"]);
// What a Teacher holds: groups.view without groups.manage, and no teachers.view at all.
const teacherMe = me("teacher", ["groups.view", "subjects.view"]);

function group(overrides: Record<string, unknown> = {}) {
  return {
    id: "g-1",
    name: "Physics A",
    subjectId: "s-phys",
    subjectName: "Physics",
    teacherId: "t-1",
    teacherName: "Amina Hassan",
    capacity: 20,
    enrolledCount: 0,
    status: "active",
    version: 3,
    ...overrides,
  };
}

const subjects = [
  { id: "s-phys", name: "Physics", status: "active", version: 1 },
  { id: "s-chem", name: "Chemistry", status: "active", version: 1 },
];
const teachers = [
  { id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },
  { id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 0, version: 1 },
];

function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {
  return HttpResponse.json(
    { title: "Error", status, code, detail, ...(errors === undefined ? {} : { errors }) },
    { status },
  );
}

describe("GroupsPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(groupsUrl, () => HttpResponse.json({ items: [group()] })),
      http.get(subjectsUrl, () => HttpResponse.json({ items: subjects })),
      http.get(teachersUrl, () => HttpResponse.json({ items: teachers })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("lists groups with a link to each group's page and seats as enrolled / capacity", async () => {
    server.use(http.get(groupsUrl, () => HttpResponse.json({ items: [group(), group({ id: "g-2", name: "Chem B", enrolledCount: 2, capacity: 2 })] })));

    renderRouter("/groups");

    // First render in the file pays the cold-start cost; a full parallel suite run can push it past 3s.
    const link = await screen.findByRole("link", { name: "Physics A" }, { timeout: 5000 });
    expect(link).toHaveAttribute("href", "/groups/g-1?tab=overview");
    const chemRow = screen.getByRole("link", { name: "Chem B" }).closest("tr") as HTMLElement;
    const seats = within(chemRow).getByText("2 / 2");
    expect(seats).toHaveAttribute("dir", "ltr");
    expect(within(chemRow).getByText("Full")).toBeInTheDocument();
  });

```

The excerpt stops at line 90; use the original file for the remaining 197 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
