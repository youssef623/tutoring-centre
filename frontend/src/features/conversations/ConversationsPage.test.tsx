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

it("a custom secretary role without assistant.view keeps the messages view", async () => {
  let timelineReads = 0;
  server.use(
    http.get("/api/me", () =>
      HttpResponse.json({
        userId: "secretary",
        displayName: "Secretary",
        email: "secretary@example.test",
        preferredLocale: "en",
        mustChangePassword: false,
        activeCentreId: "centre",
        activeRole: "secretary",
        memberships: [],
        permissions: ["conversations.view"],
        isOperator: false,
        portal: null,
      }),
    ),
    http.get("/api/conversations", () =>
      HttpResponse.json({
        items: [{ id: "legacy", address: "Legacy family", lastMessageAt: "2026-10-30T10:00:00Z" }],
      }),
    ),
    http.get("/api/conversations/legacy/messages", () =>
      HttpResponse.json({ items: [], nextCursor: null }),
    ),
    http.get("/api/conversations/legacy/timeline", () => {
      timelineReads++;
      return HttpResponse.json({ items: [], nextCursor: null });
    }),
  );
  renderWithQueryClient(<ConversationsPage />);
  fireEvent.click(await screen.findByText("Legacy family"));
  expect(await screen.findByText("No messages yet")).toBeInTheDocument();
  expect(screen.queryByText("Assistant activity")).not.toBeInTheDocument();
  expect(timelineReads).toBe(0);
});

it("loads allowed, denied and guard-blocked activity through the timeline in Arabic", async () => {
  await i18n.changeLanguage("ar");
  const tool = {
    toolName: "get_attendance",
    argumentSummary: "child_1",
    resultSummary: "1 record",
    durationMs: 4,
  };
  server.use(
    http.get("/api/me", () =>
      HttpResponse.json({
        userId: "owner",
        displayName: "Owner",
        email: "owner@example.test",
        preferredLocale: "ar",
        mustChangePassword: false,
        activeCentreId: "centre",
        activeRole: "owner",
        memberships: [],
        permissions: ["conversations.view", "assistant.view"],
        isOperator: false,
        portal: null,
      }),
    ),
    http.get("/api/conversations", () =>
      HttpResponse.json({
        items: [
          {
            id: "demo",
            address: "Demo family",
            channel: "web",
            lastMessageAt: "2026-10-30T10:00:00Z",
          },
        ],
      }),
    ),
    http.get("/api/conversations/demo/messages", () =>
      HttpResponse.json({ items: [], nextCursor: null }),
    ),
    http.get("/api/conversations/demo/timeline", () =>
      HttpResponse.json({
        items: [
          {
            id: "denied",
            kind: "run",
            at: "2026-10-30T10:02:00Z",
            status: "completed",
            steps: 1,
            guardReason: "guard.identifier",
            toolCalls: [
              {
                ...tool,
                decision: "denied",
                reasonCode: "tool.handle_unknown",
                resultSummary: null,
              },
            ],
          },
          {
            id: "allowed",
            kind: "run",
            at: "2026-10-30T10:01:00Z",
            status: "completed",
            steps: 2,
            toolCalls: [{ ...tool, decision: "allowed" }],
          },
        ],
        nextCursor: null,
      }),
    ),
  );
  renderWithQueryClient(<ConversationsPage />);
  fireEvent.click(await screen.findByText("Demo family"));
  const activities = await screen.findAllByText("نشاط المساعد");
  for (const activity of activities) fireEvent.click(activity);
  expect(screen.getByText("مسموح")).toBeInTheDocument();
  expect(screen.getByText("مرفوض")).toBeInTheDocument();
  expect(screen.getByText("احتوى الرد على معرف داخلي.")).toBeInTheDocument();
  expect(screen.getByText("هذا الطالب غير متاح.")).toBeInTheDocument();
});
