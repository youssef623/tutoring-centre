# `frontend/src/components/shared/ParentLookupField.tsx`

## Purpose

Source comment (intent): The phone input half of the parent lookup: a label, a left-to-right phone field, and whatever field error applies (a lookup-time `phone.invalid` takes priority over a submit-time error passed in from the form). It renders no panel of its own — what happens once a parent is found or not is each form's own layout (registration puts relationship between the phone and the branch; a dialog may differ), so that stays with the caller, which reads `lookup.found` after this field.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/ParentLookupField.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 58 lines and 1,832 bytes. Source hash: `c9cbe239ce4da9aceb60879c648107439cc4b9f886eb9b53f6f3bd63c6f0a519`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Loader2 } from "lucide-react";`
- Line 2: `import { Label } from "@/components/ui/label";`
- Line 3: `import { PhoneInput } from "@/components/shared/PhoneInput";`
- Line 4: `import type { useParentLookup } from "@/components/shared/useParentLookup";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../../frontend/src/components/shared/ParentLookupField.tsx#L13): `export function ParentLookupField({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/students/AddParentDialog.tsx:18](../../../../../../frontend/src/features/students/AddParentDialog.tsx#L18)
- [frontend/src/features/students/RegisterStudentForm.tsx:16](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Loader2 } from "lucide-react";
import { Label } from "@/components/ui/label";
import { PhoneInput } from "@/components/shared/PhoneInput";
import type { useParentLookup } from "@/components/shared/useParentLookup";

/**
 * The phone input half of the parent lookup: a label, a left-to-right phone field, and whatever field error
 * applies (a lookup-time `phone.invalid` takes priority over a submit-time error passed in from the form).
 * It renders no panel of its own — what happens once a parent is found or not is each form's own layout
 * (registration puts relationship between the phone and the branch; a dialog may differ), so that stays with
 * the caller, which reads `lookup.found` after this field.
 */
export function ParentLookupField({
  id,
  label,
  value,
  onChange,
  onBlur,
  disabled,
  error,
  lookup,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  disabled?: boolean;
  error?: string;
  lookup: ReturnType<typeof useParentLookup>;
}) {
  const displayError = lookup.fieldError ?? error;

  return (
    <div className="space-y-1.5">
      <Label htmlFor={id} className="gap-1.5">
        {label}
        {lookup.pending && <Loader2 className="size-3.5 animate-spin text-muted-foreground" aria-hidden="true" />}
      </Label>
      <PhoneInput
        id={id}
        value={value}
        disabled={disabled}
        aria-invalid={displayError !== undefined}
        aria-busy={lookup.pending}
        onChange={(next) => {
          lookup.onPhoneChange(next);
          onChange(next);
        }}
        onBlur={() => {
          onBlur?.();
          void lookup.ensureLookup(value);
        }}
      />
      {displayError !== undefined && <p className="text-sm text-destructive">{displayError}</p>}
    </div>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
