# `frontend/src/components/shared/PhoneText.tsx`

## Purpose

Source comment (intent): Renders a phone number isolated as left-to-right text (Day 41): without `dir="ltr"`, a value like "+201012345678" sitting inside Arabic text gets its leading "+" reordered to the wrong end by the bidi algorithm, because the surrounding paragraph's direction, not the number's own, decides how an undirected run of digits and punctuation is laid out. `asLink` renders a `tel:` link for a phone meant to be called directly (a parent's contact number); plain text otherwise (a value shown for reference only).

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/components/shared/PhoneText.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 835 bytes. Source hash: `cc1b93e649a8035c46df695e465cd0f951f1e5cd05895a3a2d32f3eab0b2ae12`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../frontend/src/components/shared/PhoneText.tsx#L8): `export function PhoneText({ value, asLink = false }: { value: string; asLink?: boolean }) {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/components/shared/PhoneText.test.tsx:3](../../../../../../frontend/src/components/shared/PhoneText.test.tsx#L3)
- [frontend/src/features/students/ParentsCard.tsx:35](../../../../../../frontend/src/features/students/ParentsCard.tsx#L35)
- [frontend/src/features/students/StudentsList.tsx:25](../../../../../../frontend/src/features/students/StudentsList.tsx#L25)
- [frontend/src/features/teachers/TeachersList.tsx:25](../../../../../../frontend/src/features/teachers/TeachersList.tsx#L25)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:12](../../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L12)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:32](../../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L32)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
/**
 * Renders a phone number isolated as left-to-right text (Day 41): without `dir="ltr"`, a value like
 * "+201012345678" sitting inside Arabic text gets its leading "+" reordered to the wrong end by the bidi
 * algorithm, because the surrounding paragraph's direction, not the number's own, decides how an undirected
 * run of digits and punctuation is laid out. `asLink` renders a `tel:` link for a phone meant to be called
 * directly (a parent's contact number); plain text otherwise (a value shown for reference only).
 */
export function PhoneText({ value, asLink = false }: { value: string; asLink?: boolean }) {
  if (asLink) {
    return (
      <a href={`tel:${value}`} dir="ltr" className="inline-block underline-offset-2 hover:underline">
        {value}
      </a>
    );
  }

  return <span dir="ltr">{value}</span>;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
