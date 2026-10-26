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
    expect(Object.keys(body as object)).toEqual(["studentId"]);
    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
    expect(rosterCalls).toBeGreaterThan(before);
  });

  it("students already on the roster are marked and cannot be picked", async () => {
    const dialog = await openDialog();

    const ahmed = await within(dialog).findByRole("option", { name: /Ahmed Mahmoud/ });
    expect(ahmed).toHaveAttribute("aria-disabled", "true");
    expect(ahmed).toHaveTextContent("Already in this group");
    fireEvent.click(ahmed);
    expect(ahmed).toHaveAttribute("aria-selected", "false");
    expect(within(dialog).getByRole("button", { name: "Enroll" })).toBeDisabled();
  });

  it("the list is keyboard operable: arrows move, Enter picks", async () => {
    const dialog = await openDialog();
    const listbox = await within(dialog).findByRole("listbox", { name: "Students" });

    fireEvent.keyDown(listbox, { key: "ArrowDown" });
    fireEvent.keyDown(listbox, { key: "ArrowUp" });
    fireEvent.keyDown(listbox, { key: "Enter" });

    expect(within(dialog).getByRole("option", { name: /Omar Khaled/ })).toHaveAttribute("aria-selected", "true");
    expect(within(dialog).getByRole("button", { name: "Enroll" })).toBeEnabled();
  });

  it("409 group.full keeps the dialog open with its message and refetches the roster and the group", async () => {
    server.use(http.post(enrollUrl, () => groupFullProblem()));
    const dialog = await openDialog();
    const rosterBefore = rosterCalls;
    const groupBefore = groupCalls;

    await pickOmar(dialog);

    expect(await within(dialog).findByText("This group is full")).toBeInTheDocument();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    await waitFor(() => {
      expect(rosterCalls).toBeGreaterThan(rosterBefore);
      expect(groupCalls).toBeGreaterThan(groupBefore);
    });
  });

  it("409 enrollment.already_enrolled keeps the dialog open with its own message and refetches the roster", async () => {
    server.use(http.post(enrollUrl, () => alreadyEnrolledProblem()));
    const dialog = await openDialog();
    const before = rosterCalls;

    await pickOmar(dialog);

    expect(await within(dialog).findByRole("alert")).toHaveTextContent("Already in this group");
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    await waitFor(() => {
      expect(rosterCalls).toBeGreaterThan(before);
    });
  });

  it("a 404 (the group is gone) toasts and closes the dialog", async () => {
    server.use(http.post(enrollUrl, () => problemDetails(404, "group.not_found", "Gone.")));
    const dialog = await openDialog();

    await pickOmar(dialog);

    expect(await screen.findByText("This group could not be found.")).toBeInTheDocument();
    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
  });
});
