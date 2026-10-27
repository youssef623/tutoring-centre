# `frontend/src/features/subjects/subjectSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats every rule (Subject.Create/Rename). Messages are translation keys, resolved within the "subjects" namespace — never hard-coded English.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/subjects/subjectSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 17 lines and 503 bytes. Source hash: `21cef8a122e2213d89064cd238c8ae1be0c13b9ed09b6f97b5a53321a0f5af57`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/features/subjects/subjectSchema.ts#L7): `export const subjectNameSchema = z`
- [Line 13](../../../../../../frontend/src/features/subjects/subjectSchema.ts#L13): `export const subjectFormSchema = z.object({`
- [Line 17](../../../../../../frontend/src/features/subjects/subjectSchema.ts#L17): `export type SubjectFormValues = z.infer<typeof subjectFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/subjects/SubjectFormDialog.tsx:16](../../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L16)
- [frontend/src/features/subjects/subjectSchema.test.ts:2](../../../../../../frontend/src/features/subjects/subjectSchema.test.ts#L2)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
