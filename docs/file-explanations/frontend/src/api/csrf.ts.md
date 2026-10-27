# `frontend/src/api/csrf.ts`

## Purpose

Source comment (intent): The current CSRF token, fetched once (through the generated client) and cached in memory.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/api/csrf.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,053 bytes. Source hash: `fc5cfa10f3cfe8ab5984e00f9f8aea353c3ee4610e6b5829a54ef868523cffa5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { getAntiforgeryToken } from "./generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../frontend/src/api/csrf.ts#L7): `async function fetchToken(): Promise<string> {`
- [Line 19](../../../../../frontend/src/api/csrf.ts#L19): `export function getCsrfToken(): Promise<string> {`
- [Line 25](../../../../../frontend/src/api/csrf.ts#L25): `export function refreshCsrfToken(): Promise<string> {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.ts:1](../../../../../frontend/src/api/apiFetch.ts#L1)
- [frontend/src/features/session/useSignOut.ts:3](../../../../../frontend/src/features/session/useSignOut.ts#L3)
- [frontend/src/routes/change-password.tsx:11](../../../../../frontend/src/routes/change-password.tsx#L11)
- [frontend/src/routes/login.tsx:12](../../../../../frontend/src/routes/login.tsx#L12)
- [frontend/src/routes/select-centre.tsx:22](../../../../../frontend/src/routes/select-centre.tsx#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { getAntiforgeryToken } from "./generated/tutoring-centre";

// In-memory only — never localStorage, sessionStorage or a cookie the script writes. A script injected later
// (e.g. via a dependency compromise) can't read a token that was never stored anywhere readable.
let cachedToken: Promise<string> | null = null;

async function fetchToken(): Promise<string> {
  try {
    const response = await getAntiforgeryToken();
    return response.token;
  } catch (error) {
    // Don't cache a failure forever — the next call should try again instead of being stuck.
    cachedToken = null;
    throw error;
  }
}

/** The current CSRF token, fetched once (through the generated client) and cached in memory. */
export function getCsrfToken(): Promise<string> {
  cachedToken ??= fetchToken();
  return cachedToken;
}

/** Forces the next {@link getCsrfToken} call to fetch a fresh token — the token is bound to the signed-in user. */
export function refreshCsrfToken(): Promise<string> {
  cachedToken = fetchToken();
  return cachedToken;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
