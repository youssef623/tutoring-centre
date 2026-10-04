import { beforeEach, describe, expect, it, vi } from "vitest";
import type { ApiError } from "./errors";
import { applyFieldErrors, showApiError } from "./showApiError";

const toastError = vi.hoisted(() => vi.fn());
vi.mock("sonner", () => ({ toast: { error: toastError } }));

describe("showApiError", () => {
  beforeEach(() => {
    toastError.mockClear();
  });

  it("shows the translated message with a reference when a correlation id exists", () => {
    // Arrange
    const error: ApiError = {
      kind: "conflict",
      code: "centre.something_new",
      message: "x",
      status: 409,
      correlationId: "corr-1",
    };

    // Act
    showApiError(error, "en");

    // Assert
    expect(toastError).toHaveBeenCalledWith("This conflicts with existing data.", {
      description: "Reference: corr-1",
    });
  });

  it("shows no description when there is no correlation id", () => {
    // Arrange
    const error: ApiError = { kind: "unexpected", code: "x", message: "x", status: 500 };

    // Act
    showApiError(error, "en");

    // Assert
    expect(toastError).toHaveBeenCalledWith("Something went wrong. Please try again.", {});
  });
});

describe("applyFieldErrors", () => {
  it("applies the first message of known fields only", () => {
    // Arrange
    const error: ApiError = {
      kind: "validation",
      code: "validation.failed",
      message: "x",
      status: 400,
      fieldErrors: { name: ["Name is required.", "Second"], other: ["Ignored"] },
    };
    const setError = vi.fn();

    // Act
    applyFieldErrors(error, setError, ["name", "email"] as const);

    // Assert
    expect(setError).toHaveBeenCalledTimes(1);
    expect(setError).toHaveBeenCalledWith("name", { type: "server", message: "Name is required." });
  });

  it("does nothing when the error has no field errors", () => {
    // Arrange
    const error: ApiError = { kind: "forbidden", code: "x", message: "x", status: 403 };
    const setError = vi.fn();

    // Act
    applyFieldErrors(error, setError, ["name"] as const);

    // Assert
    expect(setError).not.toHaveBeenCalled();
  });
});
