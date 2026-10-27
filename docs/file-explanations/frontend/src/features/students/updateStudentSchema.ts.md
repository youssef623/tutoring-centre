# `frontend/src/features/students/updateStudentSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (UpdateStudentValidator, Student.UpdateDetails, PhoneNumber.Create). Same lengths and same keys as the registration form's student fields, reused rather than duplicated — this is the same shape, just without the parent half.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/updateStudentSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 708 bytes. Source hash: `82a9a54b95840e474a499caf75c776c651b7dd075f7bbfb84de60482c74430fb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/students/updateStudentSchema.ts#L8): `export const updateStudentFormSchema = z.object({`
- [Line 14](../../../../../../frontend/src/features/students/updateStudentSchema.ts#L14): `export type UpdateStudentFormValues = z.infer<typeof updateStudentFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/students/StudentEditDialog.tsx:16](../../../../../../frontend/src/features/students/StudentEditDialog.tsx#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
