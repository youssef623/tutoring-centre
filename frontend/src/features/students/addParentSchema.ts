import { z } from "zod";
import { ParentRelationship } from "@/api/generated/tutoring-centre";

/**
 * Client-side convenience only; the server repeats every rule. Same shape and same validation keys as the
 * registration form's parent half (CreateParentValidator mirrors RegisterStudentValidator's new-parent rules).
 */
export const addParentFormSchema = z.object({
  phone: z.string().trim().min(1, "validation.parentPhoneRequired").max(30, "validation.parentPhoneTooLong"),
  fullName: z.string().trim().max(120, "validation.parentFullNameTooLong"),
  relationship: z.enum([
    ParentRelationship.father,
    ParentRelationship.mother,
    ParentRelationship.guardian,
    ParentRelationship.other,
  ]),
});

export type AddParentFormValues = z.infer<typeof addParentFormSchema>;
