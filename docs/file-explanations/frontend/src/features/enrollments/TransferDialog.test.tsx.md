# `frontend/src/features/enrollments/TransferDialog.test.tsx`

## Purpose

Declares `openTransfer`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 147 lines and 7,352 bytes. Source hash: `32fcb0e9290bb9a2b8b8ae0d5e2cc178c37375d3fa023e17069dd835dbb6689c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fireEvent, screen, waitFor, within } from "@testing-library/react";`
- Line 2: `import { http, HttpResponse } from "msw";`
- Line 3: `import { beforeEach, describe, expect, it } from "vitest";`
- Line 4: `import { group, roster, rosterEntry, secretaryMe } from "@/features/enrollments/enrollmentTestData";`
- Line 5: `import { groupFullProblem, problemDetails } from "@/test/msw/problemDetails";`
- Line 6: `import { server } from "@/test/msw/server";`
- Line 7: `import { renderRouter } from "@/test/renderRouter";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 47](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L47): `async function openTransfer() {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { group, roster, rosterEntry, secretaryMe } from "@/features/enrollments/enrollmentTestData";
import { groupFullProblem, problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

const transferUrl = "/api/enrollments/e-1/transfer";

const physicsB = group({ id: "g-2", name: "Physics B", teacherName: "Mona Said", capacity: 10, enrolledCount: 9 });
const physicsFull = group({ id: "g-3", name: "Physics C", teacherName: "Karim Adel", capacity: 5, enrolledCount: 5 });
const physicsArchived = group({ id: "g-4", name: "Physics Old", status: "archived" });

describe("TransferDialog", () => {
  let rosterCalls: Record<string, number> = {};
  let subjectListCalls = 0;
  let subjectGroups = [group(), physicsB, physicsFull, physicsArchived];

  beforeEach(() => {
    rosterCalls = { "g-1": 0, "g-2": 0 };
    subjectListCalls = 0;
    subjectGroups = [group(), physicsB, physicsFull, physicsArchived];
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/:groupId/roster", ({ params }) => {
        const groupId = String(params.groupId);
        rosterCalls[groupId] = (rosterCalls[groupId] ?? 0) + 1;
        return HttpResponse.json(
          groupId === "g-1" ? roster() : roster([rosterEntry({ enrollmentId: "e-5", studentId: "st-5", studentName: "Laila Fathy" })], { groupId }),
        );
      }),
      http.get("/api/groups/:groupId", ({ params }) =>
        HttpResponse.json(params.groupId === "g-2" ? physicsB : group()),
      ),
      http.get("/api/groups", ({ request }) => {
        const subjectId = new URL(request.url).searchParams.get("subjectId");
        if (subjectId === null) {
          return HttpResponse.json({ items: [group(), physicsB] });
        }
        subjectListCalls += 1;
        return HttpResponse.json({ items: subjectId === "s-phys" ? subjectGroups : [] });
      }),
    );
  });

  async function openTransfer() {
    const utils = renderRouter("/groups/g-1?tab=roster");
    fireEvent.click(await screen.findByRole("button", { name: "Actions for Ahmed Mahmoud" }, { timeout: 5000 }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Transfer to another group" }));
    const dialog = await screen.findByRole("dialog", { name: "Transfer Ahmed Mahmoud" });
    return { ...utils, dialog };
  }

  it("lists only other active groups of the same subject, with seats, and full ones disabled", async () => {
    const { dialog } = await openTransfer();

    const options = await within(dialog).findAllByRole("radio");
    expect(options).toHaveLength(2);
    expect(within(dialog).getByRole("radio", { name: /Physics B/ })).not.toHaveAttribute("aria-disabled", "true");
    expect(within(dialog).getByRole("radio", { name: /Physics C/ })).toHaveAttribute("aria-disabled", "true");
    expect(within(dialog).queryByRole("radio", { name: /Physics A|Physics Old/ })).not.toBeInTheDocument();
    expect(within(dialog).getByText("9 / 10")).toHaveAttribute("dir", "ltr");
    expect(within(dialog).getByText("Mona Said")).toBeInTheDocument();
    expect(subjectListCalls).toBe(1);
  });

  it("with no other group for the subject it says so", async () => {
    subjectGroups = [group()];
    const { dialog } = await openTransfer();

    expect(await within(dialog).findByText("No other group for this subject")).toBeInTheDocument();
    expect(within(dialog).getByRole("button", { name: "Transfer" })).toBeDisabled();
  });

  it("sends one request with only { targetGroupId, version } and refreshes both rosters", async () => {
    const requests: string[] = [];
    let body: unknown;
    server.events.on("request:start", ({ request }) => {
      if (request.method !== "GET") {
        requests.push(`${request.method} ${new URL(request.url).pathname}`);
      }
    });
    server.use(
      http.post(transferUrl, async ({ request }) => {
        body = await request.json();
        return HttpResponse.json({ id: "e-10" });
      }),
    );

```

The excerpt stops at line 90; use the original file for the remaining 57 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
