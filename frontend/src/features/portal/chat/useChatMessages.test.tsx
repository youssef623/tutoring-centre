import { act } from "@testing-library/react";
import { afterEach, beforeEach, expect, it, vi } from "vitest";
import { apiFetch } from "@/api/apiFetch";
import { renderHookWithQueryClient } from "@/test/render";
import { useChatMessages } from "./useChatMessages";

vi.mock("@/api/apiFetch", () => ({ apiFetch: vi.fn() }));
const api = vi.mocked(apiFetch);
let replies = false;
let sends = 0;
let reads = 0;
beforeEach(() => {
  replies = false;
  sends = 0;
  reads = 0;
  Object.defineProperty(document, "visibilityState", { configurable: true, value: "visible" });
  api.mockImplementation((_url, init) => {
    if (init?.method === "POST") {
      sends++;
      return Promise.resolve({ messageId: "sent", alreadyRecorded: false });
    }
    reads++;
    return Promise.resolve({
      items: sends
        ? [
            ...(replies
              ? [
                  {
                    id: "reply",
                    direction: "outbound",
                    text: "Reply",
                    status: "sent",
                    createdAt: "2026-10-30T10:01:00Z",
                  },
                ]
              : []),
            {
              id: "sent",
              direction: "inbound",
              text: "Hello",
              status: null,
              createdAt: "2026-10-30T10:00:00Z",
            },
          ]
        : [],
      nextCursor: null,
    });
  });
});
afterEach(() => {
  vi.useRealTimers();
  vi.clearAllMocks();
  Object.defineProperty(document, "visibilityState", { configurable: true, value: "visible" });
});

it("polls only while visible and stops after reply", async () => {
  vi.useFakeTimers();
  const hook = renderHookWithQueryClient(() => useChatMessages());
  await act(async () => {
    await vi.advanceTimersByTimeAsync(20);
  });
  expect(hook.result.current.isSuccess).toBe(true);
  act(() => {
    hook.result.current.send("Hello");
  });
  await act(async () => {
    await vi.advanceTimersByTimeAsync(20);
  });
  expect(hook.result.current.sending).toBe(false);
  const before = reads;
  await act(async () => {
    await vi.advanceTimersByTimeAsync(3100);
  });
  expect(reads).toBeGreaterThan(before);
  act(() => {
    Object.defineProperty(document, "visibilityState", { configurable: true, value: "hidden" });
    document.dispatchEvent(new Event("visibilitychange"));
  });
  const hidden = reads;
  await act(async () => {
    await vi.advanceTimersByTimeAsync(10_000);
  });
  expect(reads).toBe(hidden);
  replies = true;
  act(() => {
    Object.defineProperty(document, "visibilityState", { configurable: true, value: "visible" });
    document.dispatchEvent(new Event("visibilitychange"));
  });
  await act(async () => {
    await vi.advanceTimersByTimeAsync(3100);
  });
  const replied = reads;
  await act(async () => {
    await vi.advanceTimersByTimeAsync(10_000);
  });
  expect(reads).toBe(replied);
  hook.unmount();
});

it("stops waiting after 60 seconds without reply", async () => {
  vi.useFakeTimers();
  const hook = renderHookWithQueryClient(() => useChatMessages());
  await act(async () => {
    await vi.advanceTimersByTimeAsync(20);
  });
  act(() => {
    hook.result.current.send("Hello");
  });
  await act(async () => {
    await vi.advanceTimersByTimeAsync(20);
  });
  await act(async () => {
    await vi.advanceTimersByTimeAsync(61_000);
  });
  const stopped = reads;
  await act(async () => {
    await vi.advanceTimersByTimeAsync(10_000);
  });
  expect(reads).toBe(stopped);
  hook.unmount();
});
