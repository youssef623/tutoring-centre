# `frontend/src/features/staff/staffSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (CreateStaffValidator). Messages are translation keys, resolved within the "staff" namespace — never hard-coded English.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/staff/staffSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 15 lines and 749 bytes. Source hash: `6a03028aee9d449cd8a1b470ba237bff6cd4b31f6b983975405b3d24a3b97b20`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`
- Line 2: `import { StaffRole } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/features/staff/staffSchema.ts#L8): `export const createStaffFormSchema = z.object({`
- [Line 15](../../../../../../frontend/src/features/staff/staffSchema.ts#L15): `export type CreateStaffFormValues = z.infer<typeof createStaffFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/staff/StaffFormDialog.tsx:11](../../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
