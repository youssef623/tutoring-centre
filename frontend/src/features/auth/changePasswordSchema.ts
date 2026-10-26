import { z } from "zod";

// Mirrors the server policy (Identity's Password.RequiredLength, Day 15) — client-side convenience only; the
// server is the single source of truth and repeats every one of these rules.
const MinPasswordLength = 10;

export const changePasswordFormSchema = z
  .object({
    currentPassword: z.string().min(1, "validation.currentPasswordRequired"),
    newPassword: z.string().min(MinPasswordLength, "validation.newPasswordTooShort"),
    confirmPassword: z.string().min(1, "validation.confirmPasswordRequired"),
  })
  .refine((data) => data.newPassword !== data.currentPassword, {
    message: "validation.newPasswordSameAsCurrent",
    path: ["newPassword"],
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "validation.confirmPasswordMismatch",
    path: ["confirmPassword"],
  });

export type ChangePasswordFormValues = z.infer<typeof changePasswordFormSchema>;
