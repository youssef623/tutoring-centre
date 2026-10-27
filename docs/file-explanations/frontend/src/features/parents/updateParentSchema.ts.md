# `frontend/src/features/parents/updateParentSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (UpdateParentValidator, Parent.Update, PhoneNumber.Create).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/parents/updateParentSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 9 lines and 490 bytes. Source hash: `ecd177bfcd9bf31b423a16b7de907a4bc5b6864a9f6e89537d985ffb52664351`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../frontend/src/features/parents/updateParentSchema.ts#L4): `export const updateParentFormSchema = z.object({`
- [Line 9](../../../../../../frontend/src/features/parents/updateParentSchema.ts#L9): `export type UpdateParentFormValues = z.infer<typeof updateParentFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/parents/ParentEditDialog.tsx:11](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L11)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { z } from "zod";

/** Client-side convenience only; the server repeats every rule (UpdateParentValidator, Parent.Update, PhoneNumber.Create). */
export const updateParentFormSchema = z.object({
  fullName: z.string().trim().min(1, "validation.fullNameRequired").max(120, "validation.fullNameTooLong"),
  phone: z.string().trim().min(1, "validation.phoneRequired").max(30, "validation.phoneTooLong"),
});

export type UpdateParentFormValues = z.infer<typeof updateParentFormSchema>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
