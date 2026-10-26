import { afterEach, describe, expect, it } from "vitest";
import i18next from "@/i18n";
import { messageFor } from "./errorMessages";
import type { ApiError, ErrorKind } from "./errors";

describe("messageFor", () => {
  afterEach(async () => {
    // Leave i18next in English so other test files are unaffected.
    await i18next.changeLanguage("en");
  });

  it("returns the specific dictionary message for a known code", () => {
    // Arrange
    const error: ApiError = {
      kind: "validation",
      code: "centre.slug_invalid",
      message: "x",
      status: 400,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe(
      "The web address may only contain lowercase letters, digits and single hyphens.",
    );
  });

  it("falls back to the kind's message for an unknown code", () => {
    // Arrange
    const error: ApiError = {
      kind: "conflict",
      code: "centre.something_new",
      message: "x",
      status: 409,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("This conflicts with existing data.");
  });

  it("falls back to the generic message for an unknown code and unknown kind", () => {
    // Arrange
    const error: ApiError = {
      kind: "bogus" as ErrorKind,
      code: "totally.unknown",
      message: "x",
      status: 500,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("Something went wrong. Please try again.");
  });

  it("returns the English message for tenant.not_selected", () => {
    // Arrange
    const error: ApiError = {
      kind: "forbidden",
      code: "tenant.not_selected",
      message: "x",
      status: 403,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("Please select a centre to continue.");
  });

  it("returns the Arabic message for tenant.not_selected", async () => {
    // Arrange
    await i18next.changeLanguage("ar");
    const error: ApiError = {
      kind: "forbidden",
      code: "tenant.not_selected",
      message: "x",
      status: 403,
    };

    // Act
    const message = messageFor(error);

    // Assert
    expect(message).toBe("يرجى اختيار مركز للمتابعة.");
  });
});
