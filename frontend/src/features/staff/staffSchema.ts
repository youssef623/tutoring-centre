import { z } from "zod";
import { StaffRole } from "@/api/generated/tutoring-centre";

/**
 * Client-side convenience only; the server repeats every rule (CreateStaffValidator). Messages are translation
 * keys, resolved within the "staff" namespace — never hard-coded English.
 */
export const createStaffFormSchema = z.object({
  email: z.string().trim().min(1, "validation.emailRequired").pipe(z.email("validation.emailInvalid")),
  displayName: z.string().trim().min(1, "validation.displayNameRequired").max(120, "validation.displayNameTooLong"),
  role: z.enum([StaffRole.teacher, StaffRole.secretary, StaffRole.owner]),
  preferredLocale: z.enum(["ar", "en"]),
});

export type CreateStaffFormValues = z.infer<typeof createStaffFormSchema>;
