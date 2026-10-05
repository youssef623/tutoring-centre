import "@testing-library/jest-dom/vitest";
import { cleanup } from "@testing-library/react";
import { afterAll, afterEach, beforeAll } from "vitest";
import "@/i18n";
import { server } from "./msw/server";

// jsdom has no matchMedia; Sonner's Toaster (rendered on every route via __root.tsx) reads it for
// prefers-color-scheme. A minimal stub is enough — no test asserts on an actual media query match.
window.matchMedia = (query: string) =>
  ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => undefined,
    removeListener: () => undefined,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
    dispatchEvent: () => false,
  }) as MediaQueryList;

// Any request without a handler fails the test instead of silently hitting the network.
beforeAll(() => {
  server.listen({ onUnhandledRequest: "error" });
});

afterEach(() => {
  cleanup(); // unmount rendered components (Vitest globals are off, so Testing Library can't do it automatically)
  server.resetHandlers(); // drop per-test overrides
});

afterAll(() => {
  server.close();
});
