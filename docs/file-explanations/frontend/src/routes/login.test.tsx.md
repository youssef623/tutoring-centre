# frontend/src/routes/login.test.tsx

## Purpose

Tests of the login page: empty submit, generic 401 message, 429 message, successful redirect and rejection of an external redirect.

## Where It Fits

frontend/src/routes. Uses `renderRouter` with MSW.

## Walkthrough

Test 1: an empty submit shows `Email is required.` and `Password is required.` and no login request is made (call counter 0). Test 2: a 401 `auth.invalid_credentials` shows an alert `Incorrect email or password.` that does not contain the typed email. Test 3: a 429 `auth.rate_limited` shows `Too many attempts. Please wait a minute and try again.` Test 4: success with an active centre at `/login?redirect=%2F` ends at pathname `/`. Test 5: `redirect=https%3A%2F%2Fevil.example` is ignored and the user lands on `/`. 5 tests.

## Concepts Used

### Forms and schema validation (React Hook Form + Zod)

#### What it means

React Hook Form tracks form fields with little re-rendering; Zod describes the valid shape of the data in one schema; a resolver connects them so the form shows field errors before any request is sent. Server-side field errors can be mapped back onto the same fields.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod](../../../../PROJECT_OVERVIEW2.md#633-forms-and-validation-with-react-hook-form-and-zod).)

#### Where it appears in this file

Validation and messages.

#### How it works here

Tests 1-3.

#### Why it matters here

Shows the exact messages users see.

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Open-redirect protection.

#### How it works here

Test 5 (lines 80-105).

#### Why it matters here

Proves `resolveTarget` refuses an absolute external URL.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/routes/login.tsx`](login.tsx.md)
- [`frontend/src/test/renderRouter.tsx`](../test/renderRouter.tsx.md)
