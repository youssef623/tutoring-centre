# `frontend/src/routes/_authenticated/subjects.test.tsx`

## Purpose

Declares `subject`, `problem`, `was`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 329 lines and 12,746 bytes. Source hash: `cb50602af7194d72ca2f8f955a780d0da5cbd19ea48421f44d8967e2ead1940b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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

- [Line 22](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L22): `function subject(overrides: Record<string, unknown> = {}) {`
- [Line 26](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L26): `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 193](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L193): `problem(409, "concurrency.stale", "This record was changed by someone else. Reload and try again."),`
- [Line 205](../../../../../../frontend/src/routes/_authenticated/subjects.test.tsx#L205): `await screen.findByText("This record was changed by someone else. Reload and try again."),`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:10](../../../../../onboarding-work/extend_backend_purposes.py#L10)
- [frontend/e2e/subjects.spec.ts:21](../../../../../../frontend/e2e/subjects.spec.ts#L21)
- [frontend/e2e/teachers-groups.spec.ts:48](../../../../../../frontend/e2e/teachers-groups.spec.ts#L48)
- [frontend/src/features/audit/auditFormatting.ts:9](../../../../../../frontend/src/features/audit/auditFormatting.ts#L9)
- [frontend/src/features/enrollments/EnrollInGroupDialog.tsx:104](../../../../../../frontend/src/features/enrollments/EnrollInGroupDialog.tsx#L104)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:102](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L102)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:55](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L55)
- [frontend/src/features/groups/CreateGroupDialog.tsx:34](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L34)
- [frontend/src/features/groups/GroupsList.tsx:146](../../../../../../frontend/src/features/groups/GroupsList.tsx#L146)
- [frontend/src/features/subjects/SubjectFormDialog.tsx:30](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L30)
- [frontend/src/features/subjects/SubjectSelect.tsx:35](../../../../../../frontend/src/features/subjects/SubjectSelect.tsx#L35)
- [frontend/src/features/subjects/SubjectsList.tsx:54](../../../../../../frontend/src/features/subjects/SubjectsList.tsx#L54)
- [frontend/src/routes/_authenticated/groups.$groupId.tsx:351](../../../../../../frontend/src/routes/_authenticated/groups.$groupId.tsx#L351)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:105](../../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L105)
- [frontend/src/routes/_authenticated/groups.index.tsx:19](../../../../../../frontend/src/routes/_authenticated/groups.index.tsx#L19)
- [frontend/src/routes/_authenticated/subjects.tsx:65](../../../../../../frontend/src/routes/_authenticated/subjects.tsx#L65)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:232](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L232)
- [src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs:24](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/EnrollmentErrors.cs#L24)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:29](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L29)
- [src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs:26](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs#L26)
- 51 further name-reference locations are omitted from this compact list.

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
const subjectsUrl = "/api/subjects";

const nileOwnerMe = {
  userId: "u-1",
  displayName: "Nile Owner",
  email: "owner@nile.test",
  preferredLocale: "en",
  activeCentreId: "c-nile",
  activeRole: "owner",
  memberships: [{ centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "owner" }],
  permissions: ["subjects.manage", "subjects.view"],
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
    // A manually-released gate, not a fixed delay: the request is guaranteed to still be pending when
    // the skeleton assertion runs, however fast or slow the machine running the test is.
    let releaseList: (() => void) | undefined;
    const listGate = new Promise<void>((resolve) => {
      releaseList = resolve;
    });
    server.use(
      http.get(subjectsUrl, async () => {
        await listGate;
        return HttpResponse.json({ items: [subject()] });
      }),
    );

    renderRouter("/subjects");

    expect(await screen.findByRole("status", { name: "Loading subjects" }, { timeout: 3000 })).toBeInTheDocument();
    releaseList?.();

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
```

The excerpt stops at line 90; use the original file for the remaining 239 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
