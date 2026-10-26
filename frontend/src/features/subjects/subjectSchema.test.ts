import { describe, expect, it } from "vitest";
import { subjectNameSchema } from "./subjectSchema";

describe("subjectNameSchema", () => {
  it("rejects an empty name", () => {
    const result = subjectNameSchema.safeParse("");

    expect(result.success).toBe(false);
  });

  it("accepts a name of exactly 80 characters", () => {
    const result = subjectNameSchema.safeParse("a".repeat(80));

    expect(result.success).toBe(true);
  });

  it("rejects a name of 81 characters", () => {
    const result = subjectNameSchema.safeParse("a".repeat(81));

    expect(result.success).toBe(false);
  });

  it("trims surrounding spaces", () => {
    const result = subjectNameSchema.safeParse("  Mathematics  ");

    expect(result.success).toBe(true);
    expect(result.success && result.data).toBe("Mathematics");
  });

  it("rejects a name that is only whitespace", () => {
    const result = subjectNameSchema.safeParse("    ");

    expect(result.success).toBe(false);
  });
});
