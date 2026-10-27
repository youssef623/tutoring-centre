# `frontend/src/features/auth/changePasswordSchema.ts`

## Purpose

Current source file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/auth/changePasswordSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 932 bytes. Source hash: `29a7b66c38257168d7592d4155829a85265cc2609026645db2b3c98714e2ec45`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../frontend/src/features/auth/changePasswordSchema.ts#L7): `export const changePasswordFormSchema = z`
- [Line 22](../../../../../../frontend/src/features/auth/changePasswordSchema.ts#L22): `export type ChangePasswordFormValues = z.infer<typeof changePasswordFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/change-password.tsx:15](../../../../../../frontend/src/routes/change-password.tsx#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { z } from "zod";

// Mirrors the server policy (Identity's Password.RequiredLength, Day 15) — client-side convenience only; the
// server is the single source of truth and repeats every one of these rules.
const MinPasswordLength = 10;

export const changePasswordFormSchema = z
  .object({
    currentPassword: z.string().min(1, "validation.currentPasswordRequired"),
    newPassword: z.string().min(MinPasswordLength, "validation.newPasswordTooShort"),
    confirmPassword: z.string().min(1, "validation.confirmPasswordRequired"),
  })
  .refine((data) => data.newPassword !== data.currentPassword, {
    message: "validation.newPasswordSameAsCurrent",
    path: ["newPassword"],
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "validation.confirmPasswordMismatch",
    path: ["confirmPassword"],
  });

export type ChangePasswordFormValues = z.infer<typeof changePasswordFormSchema>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
