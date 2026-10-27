# `frontend/src/features/settings/settingsSchema.ts`

## Purpose

Source comment (intent): Client-side convenience only; the server repeats the rule (Centre.UpdateSettings). Messages are translation keys, resolved within the "settings" namespace.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/settings/settingsSchema.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 556 bytes. Source hash: `8511c2a1043a74e9ba812c16da439071e828d21c5b6f17f4f064229bff5f35dd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { z } from "zod";`
- Line 2: `import { SupportedLocale } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/settings/settingsSchema.ts#L5): `export const centreSettingsFormSchema = z.object({`
- [Line 10](../../../../../../frontend/src/features/settings/settingsSchema.ts#L10): `export type CentreSettingsFormValues = z.infer<typeof centreSettingsFormSchema>;`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/settings.tsx:24](../../../../../../frontend/src/routes/_authenticated/settings.tsx#L24)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { z } from "zod";
import { SupportedLocale } from "@/api/generated/tutoring-centre";

/** Client-side convenience only; the server repeats the rule (Centre.UpdateSettings). Messages are translation keys, resolved within the "settings" namespace. */
export const centreSettingsFormSchema = z.object({
  name: z.string().trim().min(1, "validation.nameRequired").max(120, "validation.nameTooLong"),
  defaultLocale: z.enum([SupportedLocale.ar, SupportedLocale.en]),
});

export type CentreSettingsFormValues = z.infer<typeof centreSettingsFormSchema>;
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
