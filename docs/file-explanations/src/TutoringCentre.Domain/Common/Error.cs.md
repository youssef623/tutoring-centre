# `src/TutoringCentre.Domain/Common/Error.cs`

## Purpose

Source summary (intent, not independent execution proof): An expected business failure. `Code` is a stable, machine-readable contract ("&lt;feature&gt;.&lt;reason&gt;", e.g. "centre.slug_invalid") that the frontend translates; `Message` is for developers and logs.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Domain/Common/Error.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 35 lines and 1,840 bytes. Source hash: `021a7446014519a640c56f84e003031697f9d0fcdd6075eb98774c74f87da4d9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L14): `public sealed record Error(`
- [Line 20](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L20): `public static Error Validation(string code, string message) => new(code, message, ErrorKind.Validation);`
- [Line 24](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L24): `public static Error Validation(string code, string message, IReadOnlyDictionary<string, string[]> fields)`
- [Line 30](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L30): `public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);`
- [Line 31](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L31): `public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);`
- [Line 32](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L32): `public static Error Rule(string code, string message) => new(code, message, ErrorKind.Rule);`
- [Line 33](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L33): `public static Error Forbidden(string code, string message) => new(code, message, ErrorKind.Forbidden);`
- [Line 34](../../../../../src/TutoringCentre.Domain/Common/Error.cs#L34): `public static Error Unauthenticated(string code, string message) => new(code, message, ErrorKind.Unauthenticated);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/e2e/auth.spec.ts:7](../../../../../frontend/e2e/auth.spec.ts#L7)
- [frontend/e2e/enrollment.spec.ts:7](../../../../../frontend/e2e/enrollment.spec.ts#L7)
- [frontend/e2e/first-login.spec.ts:7](../../../../../frontend/e2e/first-login.spec.ts#L7)
- [frontend/e2e/full-centre-setup.spec.ts:7](../../../../../frontend/e2e/full-centre-setup.spec.ts#L7)
- [frontend/e2e/schedule.spec.ts:7](../../../../../frontend/e2e/schedule.spec.ts#L7)
- [frontend/e2e/staff-access.spec.ts:7](../../../../../frontend/e2e/staff-access.spec.ts#L7)
- [frontend/e2e/staff-audit.spec.ts:7](../../../../../frontend/e2e/staff-audit.spec.ts#L7)
- [frontend/e2e/students.spec.ts:7](../../../../../frontend/e2e/students.spec.ts#L7)
- [frontend/e2e/subjects.spec.ts:7](../../../../../frontend/e2e/subjects.spec.ts#L7)
- [frontend/e2e/teachers-groups.spec.ts:7](../../../../../frontend/e2e/teachers-groups.spec.ts#L7)
- [frontend/src/api/apiFetch.ts:14](../../../../../frontend/src/api/apiFetch.ts#L14)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:43](../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L43)
- [frontend/src/features/status/api.ts:22](../../../../../frontend/src/features/status/api.ts#L22)
- [frontend/src/main.tsx:25](../../../../../frontend/src/main.tsx#L25)
- [frontend/src/routes/_authenticated/audit.test.tsx:41](../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L41)
- [frontend/src/routes/_authenticated/groups.$groupId.test.tsx:61](../../../../../frontend/src/routes/_authenticated/groups.$groupId.test.tsx#L61)
- [frontend/src/routes/_authenticated/groups.index.test.tsx:58](../../../../../frontend/src/routes/_authenticated/groups.index.test.tsx#L58)
- [frontend/src/routes/_authenticated/parents.$parentId.test.tsx:39](../../../../../frontend/src/routes/_authenticated/parents.$parentId.test.tsx#L39)
- [frontend/src/routes/_authenticated/settings.test.tsx:37](../../../../../frontend/src/routes/_authenticated/settings.test.tsx#L37)
- [frontend/src/routes/_authenticated/staff.test.tsx:40](../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L40)
- 202 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Domain.Common;

/// <summary>
/// An expected business failure. <see cref="Code"/> is a stable, machine-readable contract
/// ("&lt;feature&gt;.&lt;reason&gt;", e.g. "centre.slug_invalid") that the frontend translates;
/// <see cref="Message"/> is for developers and logs.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "C#-only solution; 'Error' is the agreed domain term (it is reserved only in Visual Basic).")]
public sealed record Error(
    string Code,
    string Message,
    ErrorKind Kind,
    IReadOnlyDictionary<string, string[]>? Fields = null)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorKind.Validation);

    /// <summary>A validation error naming the offending fields (camelCase field name → messages); becomes the HTTP 400 "errors" object (Day 11).</summary>
    /// <exception cref="ArgumentNullException"><paramref name="fields"/> is null.</exception>
    public static Error Validation(string code, string message, IReadOnlyDictionary<string, string[]> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        return new Error(code, message, ErrorKind.Validation, fields);
    }

    public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);
    public static Error Rule(string code, string message) => new(code, message, ErrorKind.Rule);
    public static Error Forbidden(string code, string message) => new(code, message, ErrorKind.Forbidden);
    public static Error Unauthenticated(string code, string message) => new(code, message, ErrorKind.Unauthenticated);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
