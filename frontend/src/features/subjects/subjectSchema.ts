import { z } from "zod";

/**
 * Client-side convenience only; the server repeats every rule (Subject.Create/Rename). Messages are translation
 * keys, resolved within the "subjects" namespace — never hard-coded English.
 */
export const subjectNameSchema = z
  .string()
  .trim()
  .min(1, "validation.nameRequired")
  .max(80, "validation.nameTooLong");

export const subjectFormSchema = z.object({
  name: subjectNameSchema,
});

export type SubjectFormValues = z.infer<typeof subjectFormSchema>;
