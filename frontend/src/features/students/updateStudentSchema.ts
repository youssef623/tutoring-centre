import { z } from "zod";

/**
 * Client-side convenience only; the server repeats every rule (UpdateStudentValidator, Student.UpdateDetails,
 * PhoneNumber.Create). Same lengths and same keys as the registration form's student fields, reused rather than
 * duplicated — this is the same shape, just without the parent half.
 */
export const updateStudentFormSchema = z.object({
  fullName: z.string().trim().min(1, "validation.fullNameRequired").max(120, "validation.fullNameTooLong"),
  phone: z.string().trim().max(30, "validation.phoneTooLong"),
  schoolYear: z.string().trim().max(40, "validation.schoolYearTooLong"),
});

export type UpdateStudentFormValues = z.infer<typeof updateStudentFormSchema>;
