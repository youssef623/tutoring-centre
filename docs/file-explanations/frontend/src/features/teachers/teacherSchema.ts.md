# `frontend/src/features/teachers/teacherSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only (presence and length); the server repeats every rule (Teacher.Create/ UpdateDetails, PhoneNumber.Create). Messages are translation keys in the "teachers" namespace.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/teachers/teacherSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 12 lines and 521 bytes. Source hash: `d3bb2ecefc347a5e92d3a8c1aa940d0c77028e303eb450fe1c8ac312d37598f5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/features/teachers/teacherSchema.ts#L7): `export const teacherFormSchema = z.object({`
- [Line 12](../../../../../../frontend/src/features/teachers/teacherSchema.ts#L12): `export type TeacherFormValues = z.infer<typeof teacherFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/teachers/TeacherFormDialog.tsx:29](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L29)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
