import { screen, within } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import { group, me, secretaryMe } from "@/features/enrollments/enrollmentTestData";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";

describe("Dashboard overview", () => {
  let groupCalls = 0;

  beforeEach(() => {
    groupCalls = 0;
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups", () => {
        groupCalls += 1;
        return HttpResponse.json({
          items: [
            group({ id: "g-1", name: "Physics A", capacity: 20, enrolledCount: 4 }),
            group({ id: "g-2", name: "Physics B", capacity: 10, enrolledCount: 9 }),
            group({ id: "g-3", name: "Physics C", capacity: 2, enrolledCount: 2 }),
          ],
        });
      }),
    );
  });

  it("sums the server's seat counts into tiles", async () => {
    renderRouter("/");

    const tile = async (label: string) => (await screen.findByText(label, {}, { timeout: 5000 })).parentElement as HTMLElement;
    expect(within(await tile("Active groups")).getByText("3")).toBeInTheDocument();
    expect(within(await tile("Seats taken")).getByText("15")).toBeInTheDocument();
    expect(within(await tile("Open seats")).getByText("17")).toBeInTheDocument();
    expect(within(await tile("Full groups")).getByText("1")).toBeInTheDocument();
  });

  it("lists the groups filling up, fullest first, linking to their roster", async () => {
    renderRouter("/");

    const section = await screen.findByRole("region", { name: "Groups filling up" }, { timeout: 5000 });
    const links = within(section).getAllByRole("link").filter((link) => link.getAttribute("href")?.includes("tab=roster"));
    expect(links.map((link) => link.textContent)).toEqual(["Physics C", "Physics B"]);
    expect(within(section).queryByText("Physics A")).not.toBeInTheDocument();
    expect(within(section).getByText("Full")).toBeInTheDocument();
    expect(within(section).getByText("Almost full")).toBeInTheDocument();
  });

  it("shows only the shortcuts the session may use", async () => {
    renderRouter("/");

    expect(await screen.findByRole("link", { name: "Register student" }, { timeout: 5000 })).toHaveAttribute("href", "/students/new");
    expect(screen.getAllByRole("link", { name: "Groups" }).length).toBeGreaterThan(0);
  });

  it("without groups.view sends no groups request and shows no seat overview", async () => {
    server.use(http.get("/api/me", () => HttpResponse.json(me("teacher", ["subjects.view"]))));

    renderRouter("/");

    expect(await screen.findByRole("heading", { name: "Welcome, Nile teacher" }, { timeout: 5000 })).toBeInTheDocument();
    expect(screen.queryByText("Groups filling up")).not.toBeInTheDocument();
    expect(screen.queryByRole("link", { name: "Register student" })).not.toBeInTheDocument();
    expect(groupCalls).toBe(0);
  });
});
