import { z } from "zod";

/** Client-side convenience only; the server repeats every rule (UpdateParentValidator, Parent.Update, PhoneNumber.Create). */
export const updateParentFormSchema = z.object({
  fullName: z.string().trim().min(1, "validation.fullNameRequired").max(120, "validation.fullNameTooLong"),
  phone: z.string().trim().min(1, "validation.phoneRequired").max(30, "validation.phoneTooLong"),
});

export type UpdateParentFormValues = z.infer<typeof updateParentFormSchema>;
