import { fireEvent, screen, waitFor, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { problemDetails, staleProblem } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { correctUrl, record, rosterDto, rosterUrl, secretaryMe, SessionId, student } from "./attendanceTestData";

const sessionPath = `/attendance/sessions/${SessionId}`;

interface CorrectionBody {
  status: string;
  reason: string;
  version: number | string;
}

/** Laila is recorded Present at version 7; the secretary opens the dialog for her row. */
async function openDialog() {
  const view = renderRouter(sessionPath);
  await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
  fireEvent.click(await screen.findByRole("button", { name: /^Correct/ }));
  return { ...view, dialog: await screen.findByRole("dialog") };
}

function serve(correction?: (body: CorrectionBody) => Response) {
  const calls = { roster: 0, correct: [] as CorrectionBody[] };
  server.use(
    http.get("/api/me", () => HttpResponse.json(secretaryMe)),
    http.get(rosterUrl, () => {
      calls.roster += 1;
      return HttpResponse.json(rosterDto({ items: [student("st-laila", "Laila Samir", record({ version: 7 })), student("st-omar", "Omar Nabil")] }));
    }),
    http.post(correctUrl("r-1"), async ({ request }) => {
      const body = (await request.json()) as CorrectionBody;
      calls.correct.push(body);
      return correction?.(body) ?? new HttpResponse(null, { status: 204 });
    }),
  );
  return calls;
}

describe("CorrectionDialog", () => {
  beforeEach(() => {
    server.use(http.get("/api/me", () => HttpResponse.json(secretaryMe)));
  });

  it("shows the current status and source, disables the current status and states that reasons are kept", async () => {
    serve();

    const { dialog } = await openDialog();

    expect(within(dialog).getByText("Current: Present · Manual")).toBeInTheDocument();
    expect(within(dialog).getByRole("radio", { name: /Present/ })).toHaveAttribute("aria-disabled", "true");
    expect(within(dialog).getByRole("radio", { name: "Late" })).not.toHaveAttribute("aria-disabled", "true");
    expect(within(dialog).getByText("Reasons are kept in the audit log permanently.")).toBeInTheDocument();
  });

  it("checks the status and a trimmed reason of 3 to 200 characters before sending anything", async () => {
    const calls = serve();

    const { dialog } = await openDialog();
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));
    expect(await within(dialog).findByText("Choose the new status.")).toBeInTheDocument();
    expect(await within(dialog).findByText("Enter a reason of at least 3 characters.")).toBeInTheDocument();

    fireEvent.click(within(dialog).getByRole("radio", { name: "Late" }));
    fireEvent.change(within(dialog).getByLabelText("Reason for the correction"), { target: { value: "  ab  " } });
    expect(within(dialog).getByText("2 / 200")).toBeInTheDocument();
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));
    expect(await within(dialog).findByText("Enter a reason of at least 3 characters.")).toBeInTheDocument();

    fireEvent.change(within(dialog).getByLabelText("Reason for the correction"), { target: { value: "x".repeat(201) } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));
    expect(await within(dialog).findByText("The reason can be at most 200 characters.")).toBeInTheDocument();
    expect(calls.correct).toHaveLength(0);
  });

  it("sends the record's version with the trimmed reason, then toasts, refetches and closes", async () => {
    const calls = serve();

    const { dialog } = await openDialog();
    const rosterBefore = calls.roster;
    fireEvent.click(within(dialog).getByRole("radio", { name: "Late" }));
    fireEvent.change(within(dialog).getByLabelText("Reason for the correction"), { target: { value: "  Parent confirmed a late arrival  " } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));

    expect(await screen.findByText("Correction saved")).toBeInTheDocument();
    expect(calls.correct).toEqual([{ status: "late", reason: "Parent confirmed a late arrival", version: 7 }]);
    expect(Object.keys(calls.correct[0] ?? {}).sort()).toEqual(["reason", "status", "version"]);
    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
    expect(calls.roster).toBeGreaterThan(rosterBefore);
  });

  it("on a stale version it shows the conflict, refetches and closes so the user decides again", async () => {
    const calls = serve(() => staleProblem());

    const { dialog } = await openDialog();
    const rosterBefore = calls.roster;
    fireEvent.click(within(dialog).getByRole("radio", { name: "Late" }));
    fireEvent.change(within(dialog).getByLabelText("Reason for the correction"), { target: { value: "Parent confirmed a late arrival" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));

    expect(await screen.findByText("This record was changed by someone else. Reload and try again.")).toBeInTheDocument();
    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
    expect(calls.roster).toBeGreaterThan(rosterBefore);
    expect(calls.correct).toHaveLength(1);
  });

  it("keeps the dialog open and explains a refusal about the correction itself", async () => {
    serve(() => problemDetails(422, "attendance.correction_too_early", "Corrections start with the session."));

    const { dialog } = await openDialog();
    fireEvent.click(within(dialog).getByRole("radio", { name: "Absent" }));
    fireEvent.change(within(dialog).getByLabelText("Reason for the correction"), { target: { value: "Marked absent by mistake" } });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save correction" }));

    expect(await within(dialog).findByText("Attendance can only be corrected once the session has started.")).toBeInTheDocument();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });
});
