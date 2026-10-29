import { webcrypto } from "node:crypto";
import { act, fireEvent, screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import i18n from "@/i18n";
import { rosterDto, teacherMe } from "@/features/attendance/attendanceTestData";
import { problemDetails } from "@/test/msw/problemDetails";
import { server } from "@/test/msw/server";
import { renderRouter } from "@/test/renderRouter";
import { renderHookWithQueryClient } from "@/test/render";
import { startDecoder } from "./decoder";
import { useScanQueue } from "./useScanQueue";

vi.mock("./decoder", () => ({ startDecoder: vi.fn() }));
const Url = "/api/sessions/s-1/scans";
const Now = new Date("2026-12-05T15:01:00Z");
const token = (index = 1) => `TCM1.${String(index).padStart(43, "A")}`;
const result = (outcome = "recorded", index = 1) => ({ outcome, studentId: `student-${String(index)}`, studentName: `Student ${String(index)}`, status: "present", recordedAt: Now.toISOString() });
let decoded: ((text: string) => void) | undefined;
let trackStop = vi.fn();
let stop = vi.fn();
let stream: MediaStream;
let media = vi.fn<() => Promise<MediaStream>>();

async function open() {
  const view = renderRouter("/attendance/sessions/s-1/scan");
  await screen.findByText("Point the camera at a student card", {}, { timeout: 5000 });
  return view;
}
async function emit(text: string) { await act(async () => { decoded?.(text); await Promise.resolve(); }); }

describe("ScannerPage", () => {
  beforeEach(async () => {
    await i18n.changeLanguage("en");
    vi.useFakeTimers({ toFake: ["Date"] }); vi.setSystemTime(Now);
    vi.stubGlobal("crypto", webcrypto);
    decoded = undefined; trackStop = vi.fn(); stop = vi.fn();
    stream = { getTracks: () => [{ stop: trackStop }] } as unknown as MediaStream;
    media = vi.fn<() => Promise<MediaStream>>().mockResolvedValue(stream);
    Object.defineProperty(navigator, "mediaDevices", { configurable: true, value: { getUserMedia: media } });
    vi.mocked(startDecoder).mockImplementation((_stream, _video, callback) => { decoded = callback; return Promise.resolve({ stop }); });
    server.use(
      http.get("/api/me", () => HttpResponse.json(teacherMe)),
      http.get("/api/sessions/s-1/attendance", () => HttpResponse.json(rosterDto())),
      http.post(Url, () => HttpResponse.json(result())),
    );
  });
  afterEach(() => { vi.useRealTimers(); vi.restoreAllMocks(); vi.unstubAllGlobals(); });

  it("ignores foreign QR, suppresses three-second repeats, then returns amber", async () => {
    let calls = 0;
    server.use(http.post(Url, () => HttpResponse.json(result(++calls === 1 ? "recorded" : "alreadyRecorded"))));
    await open(); await emit("https://example.test");
    await emit(token()); await screen.findByText(/Recorded — Present/);
    await emit(token());
    expect(calls).toBe(1);
    vi.setSystemTime(new Date(Now.getTime() + 3001)); await emit(token());
    await screen.findByText(/Already recorded \(Present/);
    expect(document.querySelector('[data-outcome="amber"]')).toBeInTheDocument();
    expect(calls).toBe(2);
  });

  it("retries concurrent_mark once and never retries it a third time", async () => {
    let calls = 0;
    server.use(http.post(Url, () => ++calls === 1 ? problemDetails(409, "attendance.concurrent_mark", "Race") : HttpResponse.json(result())));
    await open(); await emit(token()); await screen.findByText(/Recorded — Present/); expect(calls).toBe(2);
    server.use(http.post(Url, () => { calls++; return problemDetails(409, "attendance.concurrent_mark", "Race"); }));
    await emit(token(2)); await screen.findByText("Another scan is recording this arrival. Try again."); expect(calls).toBe(4);
  });

  it("honours Retry-After and pauses decoding", async () => {
    let calls = 0;
    server.use(http.post(Url, () => { calls++; return HttpResponse.json({ status: 429, title: "Rate limit", code: "auth.rate_limited" }, { status: 429, headers: { "Retry-After": "1" } }); }));
    await open(); await emit(token());
    await screen.findByText("Too many scans — wait a moment"); await screen.findByText("Camera paused");
    expect(trackStop).toHaveBeenCalled();
    await emit(token(2)); expect(calls).toBe(1);
    await waitFor(() => { expect(screen.getByText("Point the camera at a student card")).toBeInTheDocument(); }, { timeout: 3000 });
  });

  it("stops when the window closes and links back to roster", async () => {
    server.use(http.post(Url, () => problemDetails(422, "attendance.window_closed", "Closed")));
    await open(); await emit(token());
    await screen.findAllByText("The attendance window is closed. Return to the roster.");
    expect(trackStop).toHaveBeenCalled();
    expect(screen.queryByRole("button", { name: "Manual entry" })).not.toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Back to roster" })).toHaveAttribute("href", "/attendance/sessions/s-1");
  });

  it.each([
    ["card.not_recognised", "Card not recognised"],
    ["card.revoked", "This card was replaced or revoked. Ask the office for the current card."],
    ["attendance.student_not_in_session", "This student is not expected at this session."],
  ])("shows red text and icon for %s", async (code, message) => {
    server.use(http.post(Url, () => problemDetails(code === "card.not_recognised" ? 404 : 422, code, "Refused")));
    await open(); await emit(token()); await screen.findByText(message);
    expect(document.querySelector('[data-outcome="red"] svg')).toBeInTheDocument();
  });

  it("offers manual mode after denial, validates shape, clears input and caches no token", async () => {
    media.mockRejectedValue(new DOMException("Denied", "NotAllowedError"));
    let calls = 0; let reads = 0;
    server.use(
      http.get("/api/sessions/s-1/attendance", () => { reads++; return HttpResponse.json(rosterDto()); }),
      http.post(Url, () => { calls++; return HttpResponse.json(result()); }),
    );
    const view = renderRouter("/attendance/sessions/s-1/scan");
    fireEvent.click(await screen.findByRole("button", { name: "Use manual entry" }, { timeout: 5000 }));
    const input = screen.getByLabelText("Card token");
    expect(input).toHaveAttribute("autocomplete", "off"); expect(input).toHaveAttribute("spellcheck", "false");
    fireEvent.change(input, { target: { value: "TCM1.short" } }); fireEvent.click(screen.getByRole("button", { name: "Record arrival" }));
    expect(await screen.findByText("Enter a valid student card token.")).toBeInTheDocument(); expect(calls).toBe(0);
    fireEvent.change(input, { target: { value: token() } }); fireEvent.click(screen.getByRole("button", { name: "Record arrival" }));
    await screen.findByText(/Recorded — Present/); expect(input).toHaveValue("");
    await waitFor(() => { expect(reads).toBe(2); });
    expect(document.body.innerHTML).not.toContain(token());
    expect(JSON.stringify(view.queryClient.getMutationCache().getAll().map((mutation) => mutation.state.variables))).not.toContain(token());
    expect(JSON.stringify(view.queryClient.getQueryCache().getAll().map((query) => query.state.data))).not.toContain(token());
  });

  it("releases tracks when hidden and on unmount", async () => {
    let hidden = false;
    vi.spyOn(document, "hidden", "get").mockImplementation(() => hidden);
    const view = await open(); hidden = true; fireEvent(document, new Event("visibilitychange"));
    await waitFor(() => { expect(trackStop).toHaveBeenCalled(); });
    hidden = false; fireEvent(document, new Event("visibilitychange"));
    await screen.findByText("Point the camera at a student card");
    trackStop.mockClear(); view.unmount(); expect(trackStop).toHaveBeenCalled();
  });

  it("stops a stream whose permission request completes after unmount", async () => {
    let release: ((stream: MediaStream) => void) | undefined;
    media.mockReturnValue(new Promise((resolve) => { release = resolve; }));
    const view = renderRouter("/attendance/sessions/s-1/scan");
    await waitFor(() => { expect(media).toHaveBeenCalled(); }); view.unmount();
    await act(async () => { release?.(stream); await Promise.resolve(); });
    await waitFor(() => { expect(trackStop).toHaveBeenCalled(); });
  });

  it("serializes requests and bounds waiting codes to five", async () => {
    let release: (() => void) | undefined; const gate = new Promise<void>((resolve) => { release = resolve; });
    let active = 0; let max = 0; let calls = 0;
    server.use(http.post(Url, async () => { calls++; active++; max = Math.max(max, active); if (calls === 1) await gate; active--; return HttpResponse.json(result()); }));
    const view = renderHookWithQueryClient(() => useScanQueue("s-1"));
    await act(async () => { await Promise.all(Array.from({ length: 8 }, (_, index) => view.result.current.submit(token(index)))); });
    await waitFor(() => { expect(calls).toBe(1); });
    await act(async () => { release?.(); await Promise.resolve(); });
    await waitFor(() => { expect(calls).toBe(6); }); expect(max).toBe(1);
  });

  it("keeps only ten names/statuses/times/outcomes in history", async () => {
    let calls = 0;
    server.use(http.post(Url, () => HttpResponse.json(result("recorded", ++calls))));
    const view = renderHookWithQueryClient(() => useScanQueue("s-1"));
    for (let index = 0; index < 12; index++) {
      await act(async () => { await view.result.current.submit(token(index)); });
      await waitFor(() => { expect(calls).toBe(index + 1); });
      await waitFor(() => { expect(view.result.current.last[0]?.studentName).toBe(`Student ${String(index + 1)}`); });
    }
    expect(view.result.current.last).toHaveLength(10);
    expect(Object.keys(view.result.current.last[0] ?? {}).sort()).toEqual(["outcome", "recordedAt", "status", "studentName"]);
    expect(JSON.stringify(view.result.current.last)).not.toContain("TCM1.");
  });
});
