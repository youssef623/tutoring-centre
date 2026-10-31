import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, fireEvent } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { StudentBillingTab } from "./StudentBillingTab";
let allowed = true;
vi.mock("@/features/session/useCan", () => ({ useCan: () => allowed }));
const charge = {
  id: "charge-1",
  billingMonth: "2026-11",
  kind: "monthly",
  subjectName: "EVAL Physics",
  groupName: "EVAL Group",
  amount: "300.00",
  dueDate: "2026-11-05",
};
function show() {
  return render(
    <QueryClientProvider
      client={new QueryClient({ defaultOptions: { queries: { retry: false } } })}
    >
      <StudentBillingTab studentId="s-1" />
    </QueryClientProvider>,
  );
}
describe("Student billing", () => {
  beforeEach(async () => {
    allowed = true;
    await i18n.changeLanguage("en");
  });
  it("renders exact amount, loads a cursor page, and has no balance columns", async () => {
    server.use(
      http.get("/api/students/s-1/charges", ({ request }) =>
        HttpResponse.json(
          new URL(request.url).searchParams.has("cursor")
            ? { items: [{ ...charge, id: "charge-2", amount: "400.00" }], nextCursor: null }
            : { items: [charge], nextCursor: "next-page" },
        ),
      ),
    );
    show();
    expect(await screen.findByText("300.00")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Load more" }));
    expect(await screen.findByText("400.00")).toBeInTheDocument();
    expect(screen.getAllByText("300.00")).toHaveLength(1);
    expect(screen.queryByText(/outstanding|paid/i)).not.toBeInTheDocument();
  });
  it("renders empty state", async () => {
    server.use(
      http.get("/api/students/s-1/charges", () =>
        HttpResponse.json({ items: [], nextCursor: null }),
      ),
    );
    show();
    expect(await screen.findByText("No charges yet")).toBeInTheDocument();
  });
  it("shows loading then an error", async () => {
    server.use(
      http.get("/api/students/s-1/charges", () => new HttpResponse(null, { status: 500 })),
    );
    show();
    expect(screen.getByRole("status")).toBeInTheDocument();
    expect(await screen.findByRole("alert")).toBeInTheDocument();
  });
  it("does not load or render for a teacher", () => {
    allowed = false;
    const { container } = show();
    expect(container).toBeEmptyDOMElement();
  });
  it("keeps Arabic numeric text LTR", async () => {
    await i18n.changeLanguage("ar");
    server.use(
      http.get("/api/students/s-1/charges", () =>
        HttpResponse.json({ items: [charge], nextCursor: null }),
      ),
    );
    show();
    expect(await screen.findByText("٣٠٠٫٠٠")).toHaveAttribute("dir", "ltr");
  });
});
