# `docs/architecture/api-conventions.md`

## Purpose

Documentation covering API conventions; Error shape; Result → HTTP mapping; Exceptions; Strict JSON. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/api-conventions.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 157 lines and 16,837 bytes. Source hash: `30b054f94b9583021384f9afca3fdd26c24af5be77048fabbb368d58725e3072`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 33](../../../architecture/api-conventions.md#L33): `**201 + Location (Day 26).** ToCreatedHttpResult is the one place a successful creation becomes 201 Created. The endpoint supplies a function from the result's value to (Location, Body); both are built only from server-generated values (the created resource's own ID), never from caller-supplied input. Failure still goes through the exact same Error.ToProblemResult() the other mapping uses — there is no second status switch to keep in sync.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# API conventions

## Error shape

**Rule.** Every error response is RFC 9457 Problem Details, `application/problem+json`, written by exactly one type: `ProblemResult` (`src/TutoringCentre.Api/Http/ProblemResult.cs`). No other code writes an error body.

Every error carries two extensions beyond the RFC fields:

| Extension | Source | Purpose |
| --- | --- | --- |
| `traceId` | `Activity.Current?.Id ?? HttpContext.TraceIdentifier` | Correlate with distributed tracing. |
| `correlationId` | `HttpContext.Items["CorrelationId"]`, set by `CorrelationIdMiddleware` | Correlate with the client's own logs; `null` only if the middleware did not run. |

Validation failures also carry `errors`: a field name (camelCase) → message list, built from `Error.Fields`.

**Nested field paths (Day 41).** A field inside a nested request object is still one dotted key in `errors`, never its own nested object: `RegisterStudentCommand`'s `Parent` value produces `errors["parent.fullName"]` or `errors["parent.phone"]`, not `errors.parent.fullName`. The dispatcher builds this by camel-casing each dotted segment of the validator's `PropertyName` (`Dispatcher.ToCamelCase`) — a validator that uses `SetValidator` on a nested property, or an `OverridePropertyName("Parent.Phone")` call, gets the nested path for free. A handler-level error that needs the same path for a field FluentValidation never sees (e.g. the nested parent's phone failing `PhoneNumber.Create`, not the validator's own shape check) builds the `Fields` dictionary directly with the same dotted key, by hand (`StudentErrors.ParentPhoneInvalid`).

## Result → HTTP mapping

**Rule.** `ResultHttpExtensions` (`src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`) is the single place a domain `Result`/`Error` becomes an HTTP response. Endpoints call `.ToHttpResult(...)` and never branch on `ErrorKind` themselves.

| `ErrorKind` | HTTP status | Example |
| --- | --- | --- |
| `Validation` | 400 | bad input shape |
```

The excerpt stops at line 24; use the original file for the remaining 133 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
