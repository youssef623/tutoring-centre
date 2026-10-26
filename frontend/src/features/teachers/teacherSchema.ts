import { z } from "zod";

/**
 * Client-side convenience only (presence and length); the server repeats every rule (Teacher.Create/
 * UpdateDetails, PhoneNumber.Create). Messages are translation keys in the "teachers" namespace.
 */
export const teacherFormSchema = z.object({
  fullName: z.string().trim().min(1, "validation.fullNameRequired").max(120, "validation.fullNameTooLong"),
  phone: z.string().trim().max(30, "validation.phoneTooLong"),
});

export type TeacherFormValues = z.infer<typeof teacherFormSchema>;
