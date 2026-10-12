import { z } from "zod";
import { ParentRelationship } from "@/api/generated/tutoring-centre";

/**
 * Client-side convenience only; the server repeats every rule (RegisterStudentValidator, Student.Register,
 * Parent.Create, PhoneNumber.Create). Lengths mirror the server's own (Student.NameMaxLength = 120,
 * Student.SchoolYearMaxLength = 40, Parent.NameMaxLength = 120, PhoneMaxRawLength = 30); shape only, never a
 * phone-format rule — the server decides what a valid phone looks like.
 *
 * The parent's full name is intentionally not required here: whether it is needed at all depends on the
 * lookup's result, which this schema has no way to know. The form checks that presence itself, right before
 * submitting, once the lookup has resolved.
 */
const PhoneMaxRawLength = 30;

export const registerStudentFormSchema = z.object({
  fullName: z.string().trim().min(1, "validation.fullNameRequired").max(120, "validation.fullNameTooLong"),
  phone: z.string().trim().max(PhoneMaxRawLength, "validation.phoneTooLong"),
  schoolYear: z.string().trim().max(40, "validation.schoolYearTooLong"),
  relationship: z.enum([
    ParentRelationship.father,
    ParentRelationship.mother,
    ParentRelationship.guardian,
    ParentRelationship.other,
  ]),
  parent: z.object({
    phone: z.string().trim().min(1, "validation.parentPhoneRequired").max(PhoneMaxRawLength, "validation.parentPhoneTooLong"),
    fullName: z.string().trim().max(120, "validation.parentFullNameTooLong"),
  }),
});

export type RegisterStudentFormValues = z.infer<typeof registerStudentFormSchema>;
