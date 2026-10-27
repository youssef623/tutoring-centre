# `frontend/src/lib/bidi.ts`

## Purpose

Source comment (intent): Wraps a left-to-right value (a correlation ID, say) in Unicode's left-to-right isolate (U+2066 … U+2069) before it is interpolated into a translated sentence. Without it, an ID that starts with digits is reordered against the Arabic label around it by the bidi algorithm; with it, the ID reads as one left-to-right run and the sentence keeps its own direction. Plain text, so it works inside any `t()` call; the audit page's `<span dir="ltr">` is the same idea for markup.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/lib/bidi.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 585 bytes. Source hash: `802ea5f568391b80feef88dca5d231f7d5488c5ee8583391b479325cf9a7a0d8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../frontend/src/lib/bidi.ts#L8): `export function isolateLtr(value: string): string {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/audit/AuditLog.tsx:13](../../../../../frontend/src/features/audit/AuditLog.tsx#L13)
- [frontend/src/features/enrollments/GroupRosterTab.tsx:20](../../../../../frontend/src/features/enrollments/GroupRosterTab.tsx#L20)
- [frontend/src/features/schedule/GroupScheduleTab.tsx:21](../../../../../frontend/src/features/schedule/GroupScheduleTab.tsx#L21)
- [frontend/src/features/schedule/TimetablePage.tsx:26](../../../../../frontend/src/features/schedule/TimetablePage.tsx#L26)
- [frontend/src/features/staff/StaffList.tsx:45](../../../../../frontend/src/features/staff/StaffList.tsx#L45)
- [frontend/src/features/status/SystemInfoCard.tsx:10](../../../../../frontend/src/features/status/SystemInfoCard.tsx#L10)
- [frontend/src/features/students/StudentsList.tsx:29](../../../../../frontend/src/features/students/StudentsList.tsx#L29)
- [frontend/src/features/subjects/SubjectsList.tsx:47](../../../../../frontend/src/features/subjects/SubjectsList.tsx#L47)
- [frontend/src/lib/bidi.test.ts:2](../../../../../frontend/src/lib/bidi.test.ts#L2)
- [frontend/src/routes/_authenticated/parents.$parentId.tsx:27](../../../../../frontend/src/routes/_authenticated/parents.$parentId.tsx#L27)
- [frontend/src/routes/_authenticated/settings.tsx:29](../../../../../frontend/src/routes/_authenticated/settings.tsx#L29)
- [frontend/src/routes/_authenticated/students.$studentId.tsx:44](../../../../../frontend/src/routes/_authenticated/students.$studentId.tsx#L44)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/**
 * Wraps a left-to-right value (a correlation ID, say) in Unicode's left-to-right isolate (U+2066 … U+2069) before it
 * is interpolated into a translated sentence. Without it, an ID that starts with digits is reordered against the
 * Arabic label around it by the bidi algorithm; with it, the ID reads as one left-to-right run and the sentence
 * keeps its own direction. Plain text, so it works inside any `t()` call; the audit page's `<span dir="ltr">` is the
 * same idea for markup.
 */
export function isolateLtr(value: string): string {
  return `\u2066${value}\u2069`;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
