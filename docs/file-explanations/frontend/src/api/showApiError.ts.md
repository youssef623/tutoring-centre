# frontend/src/api/showApiError.ts

## Purpose

UI helpers for API failures: a toast with the translated message and correlation reference, and mapping server field errors onto form fields.

## Where It Fits

frontend/src/api. `showApiError` is used by `routes/select-centre.tsx`; `applyFieldErrors` by `routes/login.tsx`. Depends on `sonner`, `messageFor` and `ApiError`.

## Walkthrough

`showApiError(error)` (6-11): `options = {}` or `{ description: "Reference: <correlationId>" }`; `toast.error(messageFor(error), options)`. `applyFieldErrors<TField>(error, setError, knownFields)` (17-35): returns if there are no `fieldErrors`; type guard `isKnownField`; for each `[field, messages]` it takes the first message (`messages.at(0)`) and, for known fields only, calls `setError(field, { type: "server", message })`. Signature is compatible with React Hook Form's `setError`.

## Concepts Used

### Forms and schema validation (React Hook Form + Zod)

#### What it means

React Hook Form tracks form fields with little re-rendering; Zod describes the valid shape of the data in one schema; a resolver connects them so the form shows field errors before any request is sent. Server-side field errors can be mapped back onto the same fields.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod](../../../../PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod).)

#### Where it appears in this file

Server errors onto form fields.

#### How it works here

`applyFieldErrors` (17-35).

#### Why it matters here

Server validation uses the same field names as the form, so a 400 can mark the right input; unknown keys are ignored.

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Correlation reference.

#### How it works here

Lines 7-8.

#### Why it matters here

A user can quote the reference; the same id is in the server logs and the Problem Details body.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The toast text `Reference: ...` is hard-coded English, while the message itself is translated (the same string exists translated in `status.json` as `systemInfo.reference` for the card).

## Related Files

- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
- [`frontend/src/api/errors.ts`](errors.ts.md)
- [`frontend/src/api/showApiError.test.ts`](showApiError.test.ts.md)
- [`frontend/src/routes/login.tsx`](../routes/login.tsx.md)
- [`frontend/src/routes/select-centre.tsx`](../routes/select-centre.tsx.md)
