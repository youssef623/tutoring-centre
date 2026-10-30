import { fireEvent, screen } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { ConversationsPage } from "./ConversationsPage";

beforeEach(async () => {
  await i18n.changeLanguage("en");
});

it("shows failed outbound status and provider-neutral details", async () => {
  server.use(
    http.get("/api/conversations", () =>
      HttpResponse.json({
        items: [
          {
            id: "web",
            address: "Nile Parent",
            channel: "web",
            lastMessageAt: "2026-10-30T10:00:00Z",
          },
        ],
      }),
    ),
    http.get("/api/conversations/web/messages", () =>
      HttpResponse.json({
        items: [
          {
            id: "message-1",
            text: "<img onerror=alert(1)>",
            direction: "outbound",
            status: "failed",
            failureCode: "delivery.failed",
            channel: "web",
            correlationId: "trace-1",
            createdAt: "2026-10-30T10:00:00Z",
          },
        ],
        nextCursor: null,
      }),
    ),
  );
  const { container } = renderWithQueryClient(<ConversationsPage />);
  fireEvent.click(await screen.findByText("Nile Parent"));
  expect(await screen.findByText("Failed")).toBeInTheDocument();
  fireEvent.click(screen.getByRole("button", { name: "Details" }));
  expect(await screen.findByRole("dialog")).toBeInTheDocument();
  expect(screen.getByText("delivery.failed")).toBeInTheDocument();
  expect(screen.getByText("trace-1")).toBeInTheDocument();
  expect(screen.getByText("message-1")).toBeInTheDocument();
  expect(container.querySelector("img")).toBeNull();
});

it("renders existing inbound rows without new fields", async () => {
  server.use(
    http.get("/api/conversations", () =>
      HttpResponse.json({
        items: [{ id: "old", address: "+20••••1234", lastMessageAt: "2026-10-30T10:00:00Z" }],
      }),
    ),
    http.get("/api/conversations/old/messages", () =>
      HttpResponse.json({
        items: [{ id: "old-message", text: "Old message", createdAt: "2026-10-30T10:00:00Z" }],
        nextCursor: null,
      }),
    ),
  );
  renderWithQueryClient(<ConversationsPage />);
  fireEvent.click(await screen.findByText("+20••••1234"));
  expect(await screen.findByText("Old message")).toBeInTheDocument();
});
