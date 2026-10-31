import { fireEvent, screen, within, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { group, secretaryMe, teacherMe } from "@/features/enrollments/enrollmentTestData";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
describe("group fee editing", () => {
  beforeEach(async () => {
    await i18n.changeLanguage("en");
    server.use(
      http.get("/api/me", () => HttpResponse.json(secretaryMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group({ monthlyFee: "0.00" }))),
    );
  });
  it("shows next-month helper and sends a valid fee without changing the name", async () => {
    let payload: unknown;
    server.use(
      http.put("/api/groups/g-1", async ({ request }) => {
        payload = await request.json();
        return new HttpResponse(null, { status: 204 });
      }),
      http.get("/api/groups", () => HttpResponse.json({ items: [] })),
    );
    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Rename" }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog");
    expect(within(dialog).getByText("Applies from next month")).toBeInTheDocument();
    fireEvent.change(within(dialog).getByLabelText("Monthly fee (EGP)"), {
      target: { value: "300.00" },
    });
    fireEvent.click(within(dialog).getByRole("button", { name: "Save" }));
    await waitFor(() => {
      expect(payload).toEqual({ name: "Physics A", version: 3, monthlyFee: 300 });
    });
  });
  it("rejects three decimals without posting a rounded fee", async () => {
    let requests = 0;
    server.use(
      http.put("/api/groups/g-1", () => {
        requests++;
        return new HttpResponse(null, { status: 204 });
      }),
    );
    renderRouter("/groups/g-1");
    fireEvent.click(await screen.findByRole("button", { name: "Rename" }, { timeout: 5000 }));
    const dialog = await screen.findByRole("dialog");
    fireEvent.change(within(dialog).getByLabelText("Monthly fee (EGP)"), {
      target: { value: "10.123" },
    });
    const form = dialog.querySelector("form");
    if (!form) throw new Error("Form missing.");
    fireEvent.submit(form);
    expect(
      await within(dialog).findByText("Enter a nonnegative fee with at most two decimal places."),
    ).toBeInTheDocument();
    expect(requests).toBe(0);
  });
  it("does not show fee or editing controls to a teacher", async () => {
    server.use(
      http.get("/api/me", () => HttpResponse.json(teacherMe)),
      http.get("/api/groups/g-1", () => HttpResponse.json(group())),
    );
    renderRouter("/groups/g-1");
    await screen.findByRole("heading", { name: "Physics A" }, { timeout: 5000 });
    expect(screen.queryByText("Monthly fee (EGP)")).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Rename" })).not.toBeInTheDocument();
  });
});
