import { describe, expect, it } from "vitest";
import { isProblemDetails } from "./problemDetails";

describe("isProblemDetails", () => {
  it("returns true for a valid Problem Details object", () => {
    // Arrange
    const value = { title: "Not found", status: 404 };

    // Act
    const result = isProblemDetails(value);

    // Assert
    expect(result).toBe(true);
  });

  it("returns false for a plain string", () => {
    // Arrange
    const value = "oops";

    // Act
    const result = isProblemDetails(value);

    // Assert
    expect(result).toBe(false);
  });

  it("returns false when status is missing", () => {
    // Arrange
    const value = { title: "x" };

    // Act
    const result = isProblemDetails(value);

    // Assert
    expect(result).toBe(false);
  });
});
