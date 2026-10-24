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

    // Load the target group's roster first, so its cached copy is what a stale UI would show afterwards.
    const { router } = renderRouter("/groups/g-2?tab=roster");
    expect(await screen.findByText("Laila Fathy", {}, { timeout: 5000 })).toBeInTheDocument();
    await router.navigate({ to: "/groups/$groupId", params: { groupId: "g-1" }, search: { tab: "roster" } });
    fireEvent.click(await screen.findByRole("button", { name: "Actions for Ahmed Mahmoud" }));
    fireEvent.click(await screen.findByRole("menuitem", { name: "Transfer to another group" }));
    const dialog = await screen.findByRole("dialog", { name: "Transfer Ahmed Mahmoud" });
    fireEvent.click(await within(dialog).findByRole("radio", { name: /Physics B/ }));
    const before = { ...rosterCalls };
    fireEvent.click(within(dialog).getByRole("button", { name: "Transfer" }));

    expect(await screen.findByText("Ahmed Mahmoud was transferred to Physics B.")).toBeInTheDocument();
    expect(body).toStrictEqual({ targetGroupId: "g-2", version: 7 });
    expect(requests.filter((request) => request.startsWith("POST /api/enrollments"))).toEqual([`POST ${transferUrl}`]);
    await waitFor(() => {
      expect(rosterCalls["g-1"]).toBeGreaterThan(before["g-1"] ?? 0);
    });

    // The target roster was invalidated too: going back to it fetches it again instead of reusing the cache.
    await router.navigate({ to: "/groups/$groupId", params: { groupId: "g-2" }, search: { tab: "roster" } });
    await waitFor(() => {
      expect(rosterCalls["g-2"]).toBeGreaterThan(before["g-2"] ?? 0);
    });
    server.events.removeAllListeners();
  });

  it("409 group.full keeps the student, shows the message and refetches the target list", async () => {
    server.use(http.post(transferUrl, () => groupFullProblem()));
    const { dialog } = await openTransfer();
    fireEvent.click(await within(dialog).findByRole("radio", { name: /Physics B/ }));
    const before = subjectListCalls;
    subjectGroups = [group(), { ...physicsB, enrolledCount: 10 }, physicsFull];
    fireEvent.click(within(dialog).getByRole("button", { name: "Transfer" }));

    expect(await within(dialog).findByText("This group is full")).toBeInTheDocument();
    await waitFor(() => {
      expect(subjectListCalls).toBeGreaterThan(before);
    });
    await waitFor(() => {
      expect(within(dialog).getByRole("radio", { name: /Physics B/ })).toHaveAttribute("aria-disabled", "true");
    });
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("a 422 subject mismatch from the server is shown in the dialog in words", async () => {
    server.use(
      http.post(transferUrl, () => problemDetails(422, "enrollment.subject_mismatch", "Different subject.")),
    );
    const { dialog } = await openTransfer();
    fireEvent.click(await within(dialog).findByRole("radio", { name: /Physics B/ }));
    fireEvent.click(within(dialog).getByRole("button", { name: "Transfer" }));

    expect(
      await within(dialog).findByText("A student can only be transferred to a group of the same subject."),
    ).toBeInTheDocument();
  });
});
