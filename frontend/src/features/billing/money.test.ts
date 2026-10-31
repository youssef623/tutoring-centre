import { describe, expect, it } from "vitest";
import { formatMoney } from "./money";
import { monthlyFeeSchema } from "@/features/groups/groupSchemas";
describe("exact money display and fee entry", () => {
  it("keeps two places without floating-point arithmetic", () => {
    expect(formatMoney("300.10", "en")).toBe("300.10");
    expect(formatMoney("99999999.99", "en")).toBe("99,999,999.99");
    expect(formatMoney("-0.25", "en")).toBe("-0.25");
  });
  it("localizes Arabic digits and decimal separator", () => {
    expect(formatMoney("300.10", "ar")).toBe("٣٠٠٫١٠");
  });
  it.each(["-5", "10.123", "100000000", "NaN"])("rejects invalid fee %s", (value) => {
    expect(monthlyFeeSchema.safeParse(value).success).toBe(false);
  });
  it("accepts zero and two-place fees", () => {
    expect(monthlyFeeSchema.safeParse("0.00").success).toBe(true);
    expect(monthlyFeeSchema.safeParse("300.00").success).toBe(true);
  });
});
