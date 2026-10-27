# `frontend/src/features/students/addParentSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule. Same shape and same validation keys as the registration form's parent half (CreateParentValidator mirrors RegisterStudentValidator's new-parent rules).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/addParentSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 19 lines and 787 bytes. Source hash: `26c5c251331c409bf51e1a969ce0b8ee5bd981adaef80ec3ff4e6fa05f3ef38c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`
- Line 2: `import { ParentRelationship } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/students/addParentSchema.ts#L8): `export const addParentFormSchema = z.object({`
- [Line 19](../../../../../../frontend/src/features/students/addParentSchema.ts#L19): `export type AddParentFormValues = z.infer<typeof addParentFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/students/AddParentDialog.tsx:21](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L21)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
