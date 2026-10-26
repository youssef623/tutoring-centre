import { z } from "zod";

/**
 * Client-side convenience only — presence and ranges, nothing else; the server repeats every rule
 * (Group.Create/Rename/ChangeCapacity, the handlers' subject and teacher checks, name uniqueness). Messages
 * are translation keys in the "groups" namespace. Capacity is a number end to end, never sent as a string.
 */
export const groupNameSchema = z.string().trim().min(1, "validation.nameRequired").max(80, "validation.nameTooLong");

export const groupCapacitySchema = z
  .number({ error: "validation.capacityRequired" })
  .int("validation.capacityWhole")
  .min(1, "validation.capacityRange")
  .max(200, "validation.capacityRange");

export const createGroupFormSchema = z.object({
  name: groupNameSchema,
  subjectId: z.string({ error: "validation.subjectRequired" }).min(1, "validation.subjectRequired"),
  teacherId: z.string({ error: "validation.teacherRequired" }).min(1, "validation.teacherRequired"),
  capacity: groupCapacitySchema,
});

export type CreateGroupFormValues = z.infer<typeof createGroupFormSchema>;

export const renameGroupFormSchema = z.object({ name: groupNameSchema });

export type RenameGroupFormValues = z.infer<typeof renameGroupFormSchema>;

export const groupCapacityFormSchema = z.object({ capacity: groupCapacitySchema });

export type GroupCapacityFormValues = z.infer<typeof groupCapacityFormSchema>;

export const assignGroupTeacherFormSchema = z.object({
  teacherId: z.string({ error: "validation.teacherRequired" }).min(1, "validation.teacherRequired"),
});

export type AssignGroupTeacherFormValues = z.infer<typeof assignGroupTeacherFormSchema>;
