# `frontend/src/features/students/registerStudentSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (RegisterStudentValidator, Student.Register, Parent.Create, PhoneNumber.Create). Lengths mirror the server's own (Student.NameMaxLength = 120, Student.SchoolYearMaxLength = 40, Parent.NameMaxLength = 120, PhoneMaxRawLength = 30); shape only, never a phone-format rule — the server decides what a valid phone looks like. The parent's full name is intentionally not required here: whether it is needed at all depends on the lookup's result, which this schema has no way to know. The form checks that presence itself, right before submitting, once the lookup has resolved.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/students/registerStudentSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,578 bytes. Source hash: `11b96c4e7f72763a61424529066e0c70bd9a8d6e49a6cec6b5f2d65f31460c15`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`
- Line 2: `import { ParentRelationship } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../../frontend/src/features/students/registerStudentSchema.ts#L16): `export const registerStudentFormSchema = z.object({`
- [Line 32](../../../../../../frontend/src/features/students/registerStudentSchema.ts#L32): `export type RegisterStudentFormValues = z.infer<typeof registerStudentFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/students/RegisterStudentForm.tsx:19](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L19)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
