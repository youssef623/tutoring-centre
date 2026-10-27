# `frontend/src/features/groups/groupSchemas.ts`

## Purpose

Source comment (intent): Client-side convenience only — presence and ranges, nothing else; the server repeats every rule (Group.Create/Rename/ChangeCapacity, the handlers' subject and teacher checks, name uniqueness). Messages are translation keys in the "groups" namespace. Capacity is a number end to end, never sent as a string.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/groups/groupSchemas.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,630 bytes. Source hash: `05d2a04b482b48996f081533e8a98e6814c7aad6137396aa7f016565109e6eb0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/groups/groupSchemas.ts#L8): `export const groupNameSchema = z.string().trim().min(1, "validation.nameRequired").max(80, "validation.nameTooLong");`
- [Line 10](../../../../../../frontend/src/features/groups/groupSchemas.ts#L10): `export const groupCapacitySchema = z`
- [Line 16](../../../../../../frontend/src/features/groups/groupSchemas.ts#L16): `export const createGroupFormSchema = z.object({`
- [Line 23](../../../../../../frontend/src/features/groups/groupSchemas.ts#L23): `export type CreateGroupFormValues = z.infer<typeof createGroupFormSchema>;`
- [Line 25](../../../../../../frontend/src/features/groups/groupSchemas.ts#L25): `export const renameGroupFormSchema = z.object({ name: groupNameSchema });`
- [Line 27](../../../../../../frontend/src/features/groups/groupSchemas.ts#L27): `export type RenameGroupFormValues = z.infer<typeof renameGroupFormSchema>;`
- [Line 29](../../../../../../frontend/src/features/groups/groupSchemas.ts#L29): `export const groupCapacityFormSchema = z.object({ capacity: groupCapacitySchema });`
- [Line 31](../../../../../../frontend/src/features/groups/groupSchemas.ts#L31): `export type GroupCapacityFormValues = z.infer<typeof groupCapacityFormSchema>;`
- [Line 33](../../../../../../frontend/src/features/groups/groupSchemas.ts#L33): `export const assignGroupTeacherFormSchema = z.object({`
- [Line 37](../../../../../../frontend/src/features/groups/groupSchemas.ts#L37): `export type AssignGroupTeacherFormValues = z.infer<typeof assignGroupTeacherFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/groups/CreateGroupDialog.tsx:28](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L28)
- [frontend/src/features/groups/GroupEditDialogs.tsx:32](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L32)
- [frontend/src/features/groups/GroupEditDialogs.tsx:33](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L33)
- [frontend/src/features/groups/GroupEditDialogs.tsx:34](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L34)
- [frontend/src/features/groups/GroupEditDialogs.tsx:35](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L35)
- [frontend/src/features/groups/GroupEditDialogs.tsx:36](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L36)
- [frontend/src/features/groups/GroupEditDialogs.tsx:37](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L37)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
