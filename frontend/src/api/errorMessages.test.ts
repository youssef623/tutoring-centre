import { afterEach, describe, expect, it } from "vitest";
import i18next from "@/i18n";
import { messageFor } from "./errorMessages";
import type { ApiError, ErrorKind } from "./errors";

describe("messageFor", () => {
  it.each([
    ["en", "attendance.window_closed", "rule", 422, "The attendance window has closed. Use a correction to change a recorded status."],
    ["en", "attendance.correction_reason_invalid", "validation", 400, "Enter a correction reason containing 3 to 200 characters."],
    ["ar", "attendance.window_closed", "rule", 422, "انتهت فترة تسجيل الحضور. استخدم التصحيح لتغيير حالة مسجلة."],
    ["ar", "attendance.correction_reason_invalid", "validation", 400, "أدخل سببًا للتصحيح يتكون من 3 إلى 200 حرف."],
  ] as const)("translates %s %s", async (language, code, kind, status, expected) => {
    await i18next.changeLanguage(language);
    expect(messageFor({ kind, code, status, message: "server prose" })).toBe(expected);
  });

  it.each([
    ["en","card.invalid_format","The card token format is invalid."],
    ["en","card.reference_required","A required card reference is missing."],
    ["en","card.already_revoked","This card has already been revoked."],
    ["en","card.not_recognised","Card not recognised. Please contact the office."],
    ["en","card.revoked","This card has been revoked. Please contact the office."],
    ["en","card.not_found","This card could not be found."],
    ["en","card.student_archived","This student is archived and cannot use a card."],
    ["en","card.student_not_in_group","This student is not enrolled in this group."],
    ["ar","card.invalid_format","صيغة رمز البطاقة غير صالحة."],
    ["ar","card.reference_required","أحد المراجع المطلوبة للبطاقة مفقود."],
    ["ar","card.already_revoked","تم إلغاء هذه البطاقة بالفعل."],
    ["ar","card.not_recognised","البطاقة غير معروفة. يرجى مراجعة الإدارة."],
    ["ar","card.revoked","تم إلغاء هذه البطاقة. يرجى مراجعة الإدارة."],
    ["ar","card.not_found","تعذر العثور على هذه البطاقة."],
    ["ar","card.student_archived","هذا الطالب مؤرشف ولا يمكنه استخدام بطاقة."],
    ["ar","card.student_not_in_group","هذا الطالب غير مسجل في هذه المجموعة."],
  ] as const)("translates card code %s %s", async (language, code, expected) => {
    await i18next.changeLanguage(language);
    expect(messageFor({ kind: "validation", code, status: 400, message: "server prose" })).toBe(expected);
  });

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
