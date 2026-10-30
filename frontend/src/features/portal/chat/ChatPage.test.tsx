import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { beforeEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { ChatPage } from "./ChatPage";
import { MessageList } from "./MessageList";

describe("Portal chat", () => {
  beforeEach(async () => {
    await i18n.changeLanguage("en");
    server.use(
      http.get("/api/portal/chat/messages", () =>
        HttpResponse.json({ items: [], nextCursor: null }),
      ),
    );
  });

  it("shows optimistic message then exactly one stored message", async () => {
    let stored = false;
    let complete: (() => void) | undefined;
    const pending = new Promise<void>((resolve) => {
      complete = resolve;
    });
    server.use(
      http.post("/api/portal/chat/messages", async () => {
        await pending;
        stored = true;
        return HttpResponse.json(
          { messageId: "stored-id", alreadyRecorded: false },
          { status: 201 },
        );
      }),
      http.get("/api/portal/chat/messages", () =>
        HttpResponse.json({
          items: stored
            ? [
                {
                  id: "stored-id",
                  text: "مرحبا",
                  direction: "inbound",
                  status: null,
                  createdAt: "2026-10-30T10:00:00Z",
                },
              ]
            : [],
          nextCursor: null,
        }),
      ),
    );
    renderWithQueryClient(<ChatPage />);
    await screen.findByText("Ask about attendance");
    fireEvent.change(screen.getByLabelText("Message"), { target: { value: "مرحبا" } });
    fireEvent.keyDown(screen.getByLabelText("Message"), { key: "Enter" });
    expect(await screen.findByText("Sending…")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Send" })).toBeDisabled();
    await act(async () => {
      complete?.();
      await pending;
    });
    await waitFor(() => {
      expect(screen.queryByText("Sending…")).not.toBeInTheDocument();
    });
    expect(screen.getAllByText("مرحبا")).toHaveLength(1);
  });

  it("retry reuses clientMessageId and shows rate limit error", async () => {
    const ids: string[] = [];
    server.use(
      http.post("/api/portal/chat/messages", async ({ request }) => {
        const body = (await request.json()) as { clientMessageId: string };
        ids.push(body.clientMessageId);
        return ids.length === 1
          ? HttpResponse.json({ title: "Too many requests", status: 429 }, { status: 429 })
          : HttpResponse.json({ messageId: "saved", alreadyRecorded: true });
      }),
    );
    renderWithQueryClient(<ChatPage />);
    await screen.findByText("Ask about attendance");
    fireEvent.change(screen.getByLabelText("Message"), { target: { value: "Hello" } });
    fireEvent.click(screen.getByRole("button", { name: "Send" }));
    expect(
      await screen.findByText("Too many messages. Wait a minute, then retry."),
    ).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Try again" }));
    await waitFor(() => {
      expect(ids).toHaveLength(2);
    });
    expect(ids[0]).toBe(ids[1]);
  });

  it("renders hostile markup as text and uses direction-independent alignment", () => {
    const text = '<img src=x onerror="alert(1)">';
    const { container } = renderWithQueryClient(
      <MessageList
        items={[
          {
            id: "xss",
            text,
            direction: "inbound",
            status: null,
            createdAt: "2026-10-30T10:00:00Z",
          },
        ]}
        optimistic={null}
        failed={false}
      />,
    );
    expect(screen.getByText(text)).toBeInTheDocument();
    expect(container.querySelector("img")).toBeNull();
    expect(container.querySelector("li")).toHaveClass("items-end");
  });

  it("disables empty and overlong messages", async () => {
    renderWithQueryClient(<ChatPage />);
    await screen.findByText("Ask about attendance");
    const send = screen.getByRole("button", { name: "Send" });
    expect(send).toBeDisabled();
    fireEvent.change(screen.getByLabelText("Message"), { target: { value: "x".repeat(1001) } });
    expect(send).toBeDisabled();
    expect(screen.getByText("1001 / 1000")).toBeInTheDocument();
  });
});
