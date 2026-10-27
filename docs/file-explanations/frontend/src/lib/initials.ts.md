# `frontend/src/lib/initials.ts`

## Purpose

Source comment (intent): First letter of up to the first two words, for avatar glyphs. Falls back to "?" for an empty name.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/lib/initials.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 11 lines and 338 bytes. Source hash: `c415e9dd4007a2e70cdca9e855ae641288eefba4549dcca7d6e8156faac70722`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 2](../../../../../frontend/src/lib/initials.ts#L2): `export function initialsOf(name: string): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/shell/AppShell.tsx:28](../../../../../frontend/src/features/shell/AppShell.tsx#L28)
- [frontend/src/features/teachers/TeachersList.tsx:54](../../../../../frontend/src/features/teachers/TeachersList.tsx#L54)
- [frontend/src/routes/select-centre.tsx:30](../../../../../frontend/src/routes/select-centre.tsx#L30)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/** First letter of up to the first two words, for avatar glyphs. Falls back to "?" for an empty name. */
export function initialsOf(name: string): string {
  const initials = name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((word) => word.charAt(0).toUpperCase())
    .join("");

  return initials === "" ? "?" : initials;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
