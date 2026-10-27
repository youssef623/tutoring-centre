# `frontend/src/components/shared/useParentLookup.ts`

## Purpose

Source comment (intent): Owns the parent-lookup mutation and its result, independent of any single form: the registration form and the parents card's "Add parent" dialog each drive their own instance. A result is only ever read against the exact phone it was looked up for (`lookedUpPhoneRef`) — changing the field clears it immediately (`onPhoneChange`), and a response that resolves after the field has moved on to a different number (`latestPhoneRef` no longer matches) is discarded rather than applied, so a later, different number never reuses an earlier lookup's result. The lookup itself is a mutation (Day 42 rules), never a cached query, so the phone never sits in a query-cache key.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/useParentLookup.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 76 lines and 2,745 bytes. Source hash: `5ebce58b90e98e6dc10858ee55f787e69acd21f9891f5b57723465476dcc69a0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useRef, useState } from "react";`
- Line 2: `import { asApiError } from "@/api/apiFetch";`
- Line 3: `import { showApiError } from "@/api/showApiError";`
- Line 4: `import { useLookupParent, type ParentDto } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../frontend/src/components/shared/useParentLookup.ts#L6): `export type ParentLookupOutcome = { ok: true; parent: ParentDto | null } | { ok: false };`
- [Line 17](../../../../../../frontend/src/components/shared/useParentLookup.ts#L17): `export function useParentLookup() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/enrich_frontend.py:13](../../../../../onboarding-work/enrich_frontend.py#L13)
- [frontend/src/components/shared/ParentLookupField.tsx:4](../../../../../../frontend/src/components/shared/ParentLookupField.tsx#L4)
- [frontend/src/features/students/AddParentDialog.tsx:19](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L19)
- [frontend/src/features/students/RegisterStudentForm.tsx:17](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L17)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { useRef, useState } from "react";
import { asApiError } from "@/api/apiFetch";
import { showApiError } from "@/api/showApiError";
import { useLookupParent, type ParentDto } from "@/api/generated/tutoring-centre";

export type ParentLookupOutcome = { ok: true; parent: ParentDto | null } | { ok: false };

/**
 * Owns the parent-lookup mutation and its result, independent of any single form: the registration form
 * and the parents card's "Add parent" dialog each drive their own instance. A result is only ever read
 * against the exact phone it was looked up for (`lookedUpPhoneRef`) — changing the field clears it immediately
 * (`onPhoneChange`), and a response that resolves after the field has moved on to a different number
 * (`latestPhoneRef` no longer matches) is discarded rather than applied, so a later, different number never
 * reuses an earlier lookup's result. The lookup itself is a mutation (Day 42 rules), never a cached query, so
 * the phone never sits in a query-cache key.
 */
export function useParentLookup() {
  const mutation = useLookupParent();
  const [found, setFound] = useState<ParentDto | null>(null);
  const [fieldError, setFieldError] = useState<string | null>(null);
  const lookedUpPhoneRef = useRef<string | null>(null);
  const latestPhoneRef = useRef("");

  const reset = () => {
    setFound(null);
    setFieldError(null);
    lookedUpPhoneRef.current = null;
  };

  const onPhoneChange = (nextPhone: string) => {
    latestPhoneRef.current = nextPhone;
    if (nextPhone !== lookedUpPhoneRef.current) {
      setFound(null);
      setFieldError(null);
    }
  };

  const ensureLookup = async (phone: string, options?: { force?: boolean }): Promise<ParentLookupOutcome> => {
    if (phone.trim() === "") {
      reset();
      return { ok: true, parent: null };
    }

    if (options?.force !== true && lookedUpPhoneRef.current === phone) {
      return { ok: true, parent: found };
    }

    setFieldError(null);
    try {
      const result = await mutation.mutateAsync({ data: { phone } });
      if (latestPhoneRef.current !== phone) {
        return { ok: true, parent: null };
      }

      lookedUpPhoneRef.current = phone;
      setFound(result.parent);
      return { ok: true, parent: result.parent };
    } catch (thrown) {
      if (latestPhoneRef.current !== phone) {
        return { ok: false };
      }

      const apiError = asApiError(thrown);
      const message = apiError.fieldErrors?.phone?.at(0);
      if (message !== undefined) {
        setFieldError(message);
      } else {
        showApiError(apiError);
      }

      return { ok: false };
    }
  };

  return { found, fieldError, pending: mutation.isPending, ensureLookup, onPhoneChange, reset };
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
