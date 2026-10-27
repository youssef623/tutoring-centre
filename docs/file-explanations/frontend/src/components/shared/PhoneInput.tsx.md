# `frontend/src/components/shared/PhoneInput.tsx`

## Purpose

Source comment (intent): A telephone input that is always left-to-right, even inside an Arabic form: a phone number has its own reading direction regardless of the surrounding page's, and without `dir="ltr"` the bidi algorithm reorders its digits and leading "+" in an RTL document. Carries no formatting or validation of its own beyond what the browser's own `type="tel"` offers — the server owns the real format (Day 41).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/PhoneInput.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 1,227 bytes. Source hash: `9908b0c39007a74f82c5ac602a115a68e9e4fea4123c4d22c2738d314c9d504f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { Input } from "@/components/ui/input";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../frontend/src/components/shared/PhoneInput.tsx#L9): `export function PhoneInput({`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/ParentLookupField.tsx:3](../../../../../../frontend/src/components/shared/ParentLookupField.tsx#L3)
- [frontend/src/components/shared/PhoneInput.test.tsx:3](../../../../../../frontend/src/components/shared/PhoneInput.test.tsx#L3)
- [frontend/src/features/parents/ParentEditDialog.tsx:23](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L23)
- [frontend/src/features/students/RegisterStudentForm.tsx:24](../../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L24)
- [frontend/src/features/students/StudentEditDialog.tsx:28](../../../../../../frontend/src/features/students/StudentEditDialog.tsx#L28)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:16](../../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { Input } from "@/components/ui/input";

/**
 * A telephone input that is always left-to-right, even inside an Arabic form: a phone number has its own
 * reading direction regardless of the surrounding page's, and without `dir="ltr"` the bidi algorithm reorders
 * its digits and leading "+" in an RTL document. Carries no formatting or validation of its own beyond what
 * the browser's own `type="tel"` offers — the server owns the real format (Day 41).
 */
export function PhoneInput({
  id,
  value,
  onChange,
  onBlur,
  placeholder,
  required,
  disabled,
  "aria-invalid": ariaInvalid,
  "aria-busy": ariaBusy,
}: {
  id?: string;
  value: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  "aria-invalid"?: boolean;
  "aria-busy"?: boolean;
}) {
  return (
    <Input
      id={id}
      type="tel"
      dir="ltr"
      className="text-start"
      value={value}
      placeholder={placeholder}
      required={required}
      disabled={disabled}
      aria-invalid={ariaInvalid}
      aria-busy={ariaBusy}
      onChange={(event) => {
        onChange(event.target.value);
      }}
      onBlur={onBlur}
    />
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
