# `frontend/src/routes/_authenticated/groups.$groupId.test.tsx`

## Purpose

Declares `me`, `group`, `problem`, `was`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 265 lines and 10,915 bytes. Source hash: `5e19745e62dac69ebf9208070186112459fa76bcbfab62ad62246e099c916579`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { server } from "@/test/msw/server";`
- Line 6: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L12): `function me(role: "owner" | "teacher", permissions: string[]) {`
- [Line 38](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L38): `function group(overrides: Record<string, unknown> = {}) {`
- [Line 59](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L59): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 204](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L204): `await screen.findByText("This record was changed by someone else. Reload and try again."),`

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

const meUrl = "/api/me";
const groupUrl = "/api/groups/g-1";
const teachersUrl = "/api/teachers";

function me(role: "owner" | "teacher", permissions: string[]) {
  return {
    userId: "u-me",
    displayName: role === "owner" ? "Nile Owner" : "Two-Centre Teacher",
    email: role === "owner" ? "owner@nile.test" : "teacher@both.test",
    preferredLocale: "en",
    activeCentreId: "c-nile",
    activeRole: role,
    memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role, timeZoneId: "Africa/Cairo" }],
    permissions,
  };
}

const ownerMe = me("owner", [
  "enrollments.manage",
  "enrollments.view",
  "groups.manage",
  "groups.view",
  "schedule.view",
  "students.view",
  "subjects.view",
  "teachers.manage",
  "teachers.view",
]);
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
    enrolledCount: 4,
    status: "active",
    version: 11,
    ...overrides,
  };
}

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

describe("GroupDetailPage", () => {
  beforeEach(() => {
    server.use(
      http.get(meUrl, () => HttpResponse.json(ownerMe)),
      http.get(groupUrl, () => HttpResponse.json(group())),
      http.get(teachersUrl, () => HttpResponse.json({ items: teachers })),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get(`${groupUrl}/roster`, () =>
        HttpResponse.json({ groupId: "g-1", groupName: "Physics A", capacity: 20, enrolledCount: 0, items: [] }),
      ),
      http.get(`${groupUrl}/schedule-rules`, () => HttpResponse.json({ items: [] })),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  it("shows the overview with seats as a left-to-right number pair", async () => {
    renderRouter("/groups/g-1");

    // First render in the file pays the cold-start cost; a full parallel suite run can push it past 3s.
    expect(await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Overview" })).toHaveAttribute("aria-selected", "true");
    expect(screen.getByText("4 / 20")).toHaveAttribute("dir", "ltr");
```

The excerpt stops at line 90; use the original file for the remaining 175 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
