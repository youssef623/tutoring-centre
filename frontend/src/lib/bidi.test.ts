import { describe, expect, it } from "vitest";
import { isolateLtr } from "@/lib/bidi";

describe("isolateLtr", () => {
  it("wraps the value in a left-to-right isolate and its closing pop", () => {
    expect(isolateLtr("3f2a9c")).toBe("\u20663f2a9c\u2069");
  });
});
