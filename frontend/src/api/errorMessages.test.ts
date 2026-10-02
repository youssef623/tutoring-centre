import { describe, expect, it } from "vitest";
import { messageFor } from "@/api/errorMessages";
import type { ApiError } from "@/api/errors";

function apiError(overrides: Partial<ApiError>): ApiError {
  return {
    kind: "unexpected",
    code: "unknown.code",
    message: "dev message",
    status: 500,
    ...overrides,
  };
}

describe("messageFor", () => {
  it("returns the specific message for a known code", () => {
    const error = apiError({ kind: "validation", code: "centre.slug_invalid" });

    expect(messageFor(error, "en")).toBe("Centre slug is invalid.");
  });

  it("falls back to the kind's message for an unknown code", () => {
    const error = apiError({ kind: "notFound", code: "centre.not_found_unlisted" });

    expect(messageFor(error, "en")).toBe("The requested item could not be found.");
  });

  it("falls back to the generic message for an unknown kind", () => {
    const error = {
      ...apiError({ code: "totally.unknown" }),
      kind: "not-a-real-kind" as ApiError["kind"],
    };

    expect(messageFor(error, "en")).toBe("Something went wrong. Please try again.");
  });
});
