# `frontend/src/features/enrollments/EnrollStudentDialog.test.tsx`

## Purpose

Declares `openDialog`, `pickOmar`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 162 lines and 6,419 bytes. Source hash: `a6e74ee006ceb34bf3c280693bac180b81358ff29cae6cc5251c52d7bd99e360`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { afterEach, beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import i18n from "@/i18n";`
- Line 5: `import { group, roster, secretaryMe, studentListItem } from "@/features/enrollments/enrollmentTestData";`
- Line 6: `import { alreadyEnrolledProblem, groupFullProblem, problemDetails } from "@/test/msw/problemDetails";`
- Line 7: `import { server } from "@/test/msw/server";`
- Line 8: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 49](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L49): `async function openDialog() {`
- [Line 56](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L56): `async function pickOmar(dialog: HTMLElement) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { group, roster, secretaryMe, studentListItem } from "@/features/enrollments/enrollmentTestData";
import { alreadyEnrolledProblem, groupFullProblem, problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const enrollUrl = "/api/groups/g-1/enrollments";

describe("EnrollStudentDialog", () => {
  let rosterCalls = 0;
  let groupCalls = 0;
  let studentSearches: URLSearchParams[] = [];

  beforeEach(() => {
    rosterCalls = 0;
    groupCalls = 0;
    studentSearches = [];
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => {
        groupCalls += 1;
        return HttpResponse.json(group());
      }),
      http.get("/api/groups", () => HttpResponse.json({ items: [group()] })),
      http.get("/api/groups/g-1/roster", () => {
        rosterCalls += 1;
        return HttpResponse.json(roster());
      }),
      http.get("/api/students", ({ request }) => {
        studentSearches.push(new URL(request.url).searchParams);
        return HttpResponse.json({
          items: [
            studentListItem(),
            studentListItem({ id: "st-1", fullName: "Ahmed Mahmoud", schoolYear: "Grade 9", primaryParentName: "Mahmoud Hassan" }),
          ],
          nextCursor: null,
        });
      }),
    );
  });

  afterEach(async () => {
    await i18n.changeLanguage("en");
  });

  async function openDialog() {
    renderRouter("/groups/g-1?tab=roster");
    fireEvent.click(await screen.findByRole("button", { name: "Enroll student" }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog");
    return dialog;
  }

  async function pickOmar(dialog: HTMLElement) {
    fireEvent.click(await within(dialog).findByRole("option", { name: /Omar Khaled/ }));
    fireEvent.click(within(dialog).getByRole("button", { name: "Enroll" }));
  }

  it("searches through the existing students list with a page of 10, and shows no contact details", async () => {
    const dialog = await openDialog();

    expect(await within(dialog).findByRole("option", { name: /Omar Khaled/ })).toHaveTextContent("Grade 10 · Khaled Omar");
    expect(within(dialog).queryByText(/\+2010/)).not.toBeInTheDocument();
    fireEvent.change(within(dialog).getByRole("searchbox", { name: "Search students" }), { target: { value: "احمد" } });

    await waitFor(() => {
      expect(studentSearches.at(-1)?.get("search")).toBe("احمد");
    });
    expect(studentSearches.every((params) => params.get("pageSize") === "10")).toBe(true);
  });

  it("enroll sends only { studentId }, then closes, toasts and refetches the roster", async () => {
    let body: unknown;
    server.use(
      http.post(enrollUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "e-9" }, { status: 201 });
      }),
    );
    const dialog = await openDialog();
    const submit = within(dialog).getByRole("button", { name: "Enroll" });
    expect(submit).toBeDisabled();
    const before = rosterCalls;

    await pickOmar(dialog);

    expect(await screen.findByText("Omar Khaled was enrolled in Physics A.")).toBeInTheDocument();
    expect(body).toStrictEqual({ studentId: "st-3" });
```

The excerpt stops at line 90; use the original file for the remaining 72 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
