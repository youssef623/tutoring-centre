# `frontend/src/app/queryClient.ts`

## Purpose

Source comment (intent): Set once from main.tsx, after the router exists, so a session-expiry redirect can go through it.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/app/queryClient.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 2,711 bytes. Source hash: `4ad5bd2e4f1a8a0bddbf2a3f0f5fbe45069cab1392e44d22725c5c27ef250a36`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";`
- Line 2: `import { asApiError } from "@/api/apiFetch";`
- Line 3: `import { getLoginMutationKey } from "@/api/generated/tutoring-centre";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../frontend/src/app/queryClient.ts#L9): `export function setLoginRedirect(fn: LoginRedirect): void {`
- [Line 15](../../../../../frontend/src/app/queryClient.ts#L15): `function isLoginMutation(mutationKey: readonly unknown[] | undefined): boolean {`
- [Line 28](../../../../../frontend/src/app/queryClient.ts#L28): `export function createAppQueryClient(): QueryClient {`
- [Line 33](../../../../../frontend/src/app/queryClient.ts#L33): `function handleSessionExpiry(error: unknown): void {`
- [Line 74](../../../../../frontend/src/app/queryClient.ts#L74): `export const queryClient = createAppQueryClient();`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/generated/tutoring-centre.ts:832](../../../../../frontend/src/api/generated/tutoring-centre.ts#L832)
- [frontend/src/features/enrollments/useEnrollmentMutations.ts:24](../../../../../frontend/src/features/enrollments/useEnrollmentMutations.ts#L24)
- [frontend/src/features/groups/CreateGroupDialog.tsx:47](../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L47)
- [frontend/src/features/groups/GroupEditDialogs.tsx:56](../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L56)
- [frontend/src/features/parents/ParentEditDialog.tsx:46](../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L46)
- [frontend/src/features/schedule/useScheduleMutations.ts:25](../../../../../frontend/src/features/schedule/useScheduleMutations.ts#L25)
- [frontend/src/features/session/refetchMeOnPermissionDenied.ts:6](../../../../../frontend/src/features/session/refetchMeOnPermissionDenied.ts#L6)
- [frontend/src/features/session/useSignOut.ts:9](../../../../../frontend/src/features/session/useSignOut.ts#L9)
- [frontend/src/features/staff/ChangeRoleDialog.tsx:36](../../../../../frontend/src/features/staff/ChangeRoleDialog.tsx#L36)
- [frontend/src/features/staff/StaffFormDialog.tsx:36](../../../../../frontend/src/features/staff/StaffFormDialog.tsx#L36)
- [frontend/src/features/staff/StaffList.tsx:51](../../../../../frontend/src/features/staff/StaffList.tsx#L51)
- [frontend/src/features/students/AddParentDialog.tsx:54](../../../../../frontend/src/features/students/AddParentDialog.tsx#L54)
- [frontend/src/features/students/ParentsCard.tsx:49](../../../../../frontend/src/features/students/ParentsCard.tsx#L49)
- [frontend/src/features/students/RegisterStudentForm.tsx:38](../../../../../frontend/src/features/students/RegisterStudentForm.tsx#L38)
- [frontend/src/features/students/StudentEditDialog.tsx:47](../../../../../frontend/src/features/students/StudentEditDialog.tsx#L47)
- [frontend/src/features/subjects/SubjectFormDialog.tsx:42](../../../../../frontend/src/features/subjects/SubjectFormDialog.tsx#L42)
- [frontend/src/features/subjects/SubjectsList.tsx:58](../../../../../frontend/src/features/subjects/SubjectsList.tsx#L58)
- [frontend/src/features/teachers/TeacherAccountDialog.tsx:55](../../../../../frontend/src/features/teachers/TeacherAccountDialog.tsx#L55)
- [frontend/src/features/teachers/TeacherFormDialog.tsx:47](../../../../../frontend/src/features/teachers/TeacherFormDialog.tsx#L47)
- [frontend/src/features/teachers/TeachersList.tsx:71](../../../../../frontend/src/features/teachers/TeachersList.tsx#L71)
- 12 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { MutationCache, QueryCache, QueryClient } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import { getLoginMutationKey } from "@/api/generated/tutoring-centre";

/** Set once from main.tsx, after the router exists, so a session-expiry redirect can go through it. */
type LoginRedirect = (redirectTarget: string) => void;
let loginRedirect: LoginRedirect | null = null;

export function setLoginRedirect(fn: LoginRedirect): void {
  loginRedirect = fn;
}

const loginMutationKey = getLoginMutationKey();

function isLoginMutation(mutationKey: readonly unknown[] | undefined): boolean {
  return mutationKey !== undefined && JSON.stringify(mutationKey) === JSON.stringify(loginMutationKey);
}

/**
 * Builds a query client wired the same way the app's own instance is: a session can end server-side at any
 * time (Day 16 revocation, rate limiting aside), so any request — not just the one on page load — can come
 * back unauthenticated. The login mutation and the "me" query handle a 401 themselves (the session query by
 * resolving it to `null`, never throwing) and are excluded.
 *
 * A factory, not just the one exported instance below, so tests can get a fresh client with the real
 * session-expiry wiring instead of a reimplementation of it.
 */
export function createAppQueryClient(): QueryClient {
  // Holds the client once built, so the error handlers below (defined before it exists) can reach it.
  // A property on a const ref, not a reassigned `let`: the handlers never run until after it is set.
  const self: { client: QueryClient | null } = { client: null };

  function handleSessionExpiry(error: unknown): void {
    if (asApiError(error).kind !== "unauthenticated") {
      return;
    }

    if (window.location.pathname === "/login") {
      return;
    }

    self.client?.clear();
    loginRedirect?.(window.location.pathname + window.location.search);
  }

  const client = new QueryClient({
    defaultOptions: {
      queries: {
        retry: 1, // one retry hides a blip without delaying a real failure for long
        staleTime: 30_000, // data is fresh for 30 s, so remounting a screen doesn't refetch immediately
      },
    },
    queryCache: new QueryCache({
      onError: (error) => {
        handleSessionExpiry(error);
      },
    }),
    mutationCache: new MutationCache({
      onError: (error, _variables, _context, mutation) => {
        if (isLoginMutation(mutation.options.mutationKey)) {
          return;
        }

        handleSessionExpiry(error);
      },
    }),
  });

  self.client = client;
  return client;
}

/** Shared defaults for every query in the app. */
export const queryClient = createAppQueryClient();
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
