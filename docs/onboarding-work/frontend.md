<!-- Frontend concepts, independently checked against the current frontend files. -->

<a id="frontend-react"></a>
## 6.40 React, component ownership, server state and the browser entry point

#### What it means

A browser starts with an HTML document. React lets the application describe the changing part of that document as a tree of components. A component is a function that receives inputs, called props, and returns a description of the UI. JSX is the syntax used to write that description; it is transformed by the build tools into JavaScript. Returning JSX does not immediately issue a request or update an element. React renders the component tree, compares its next description with its previous description, then commits the necessary changes to the browser DOM.

Local component state is memory that React associates with a component instance. Calling a state setter schedules another render; it does not rewrite the local variable already captured by the running function. This distinction matters when reading an event handler that calls `setTargetGroupId` and then awaits a request. The running handler belongs to one render and retains that render's values. A later render receives the new state.

Server state is different. Students, groups, session information and timetable sessions belong to the server. The browser has temporary copies that may become stale because another member of staff changed the database. TanStack Query owns those copies, including their request state, cache keys, refetching and subscriptions. A dialog's open state belongs in React state; a roster belongs in the query cache.

#### The problem it solves

Without this separation a component might duplicate server data into local state:

```tsx
// Illustration only: this is not the project's implementation.
const [students, setStudents] = useState<Student[]>([]);
const [loading, setLoading] = useState(true);
useEffect(() => {
  fetch('/api/students').then(r => r.json()).then(setStudents)
    .finally(() => setLoading(false));
}, []);
```

That example omits error handling, stale requests, cache sharing, cancellation and refresh after a mutation. Copy it into three screens and each copy can disagree. The project's components instead subscribe to a query and render its explicit states. Local state remains useful for an archive confirmation, selected target, draft form field or mobile menu.

#### How it works

`frontend/index.html` supplies the root element and loads `src/main.tsx`. At `frontend/src/main.tsx:10`, `createRouter` receives the generated route tree and a context containing the shared query client. At line 23 the entry point finds `#root`; a missing element is an explicit exception. At line 28 `createRoot(...).render(...)` mounts `StrictMode`, `QueryClientProvider` and `RouterProvider`.

The provider makes the query client accessible to hooks underneath it. When `useQuery` or a generated hook subscribes to a cache entry, TanStack Query observes that entry. A completed fetch changes the entry's state; its observer notifies React; React schedules the subscribed component to render again. The component reads the new query result and returns different JSX. This is the mechanism behind a skeleton becoming a student table, rather than an unexplained statement that React updates automatically.

`StrictMode` is a development check around the tree. It can repeat development work to expose impure rendering and missing cleanup. It does not mean that a business mutation should run from a render function. Mutations here originate from submit and click handlers.

#### How THIS project implements it

`frontend/src/app/queryClient.ts:29` exports a factory with the application's real error wiring. Its query defaults are one retry and a 30,000 millisecond stale time. The exported shared client at the end of that file is the one used by `main.tsx`. Route guards use the same client through router context; components use it through `QueryClientProvider`. This avoids a guard checking one session cache while the shell observes another.

`frontend/src/features/dashboard/DashboardOverview.tsx:62` demonstrates server state. `GroupsOverview` calls `useListGroups`, renders a loading state, then an error state, then computes totals from `data.items`. It does not maintain a second local array of groups. `frontend/src/features/shell/AppShell.tsx:31` demonstrates local state: `mobileNavOpen` decides whether the navigation dialog is open. Opening the menu does not change server data and requires no API call.

The separation is also visible in `TransferDialog`: `targetGroupId` and `refusal` are local interaction state, while available groups come from `useListGroups`. The form is allowed to retain a refusal message while the query cache refreshes its latest list.

Verified source excerpt, `frontend/src/main.tsx:23` (quoted directly from the current file):

```tsx
const rootElement = document.getElementById("root");
if (rootElement === null) {
  throw new Error("Root element #root was not found in index.html.");
}

createRoot(rootElement).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>,
);
```

#### Execution example

1. The browser loads `index.html`, then `main.tsx` imports the route tree, styles and i18n setup.
2. The router matches the protected dashboard and runs its parent guard.
3. The guard asks the shared query client for the current session. A signed-in user with a selected centre reaches `AppShell`.
4. The dashboard mounts its group overview only when `groups.view` is held.
5. `useListGroups` subscribes to the generated list key and calls the generated request through `apiFetch` when data must be loaded.
6. The first render displays skeletons. The request resolves and the cache stores its response.
7. The query observer notifies React. `GroupsOverview` renders again and calculates active groups, enrolled seats, open seats and full groups.
8. React commits the resulting tiles and list of groups approaching capacity.

#### Why this project needs it

The same group appears in a list, detail page, roster, timetable filter and dashboard. Shared server state and predictable invalidation are necessary for those views to converge after changes. At the same time, dialogs need independent local drafts. Treating every value as global state would add coupling; treating every value as isolated local state would produce inconsistent copies.

#### Alternatives

Plain `fetch` with effects can work, but the application would own cache and race handling itself. Redux could hold application state, while Redux Toolkit Query could handle requests. SWR is another query cache. A server-rendered application would move some loading and redirects to the server. The repository explicitly selects TanStack Query and Router in `frontend/README.md`; it does not separately document why every competing library was rejected. The following is an interpretation based on the implementation: the current choice gives route guards and rendered components one shared server-state owner.

#### Important details

A stale time is not a security promise. Fresh cached data can still be obsolete after another user changes the server. Query keys are identity: two requests with different search filters need different keys. Components should not mutate cached response objects in place. Rendering must stay free of business side effects. React effects remain appropriate for browser synchronization such as the search debounce; the current repository does use `useEffect`, contrary to v2's earlier claim.

#### What I should understand before modifying this code

Determine who owns a new value before choosing a hook. Keep server results in query state, transient UI state in a component, and shareable navigation state in the URL. Read `StudentsList`, `SearchInput`, `AppShell` and `useEnrollmentMutations` together to see all four ownership cases. Do not introduce a second query client inside a feature. Preserve loading, error, empty and success branches when replacing a screen.

<a id="frontend-generation"></a>
## 6.41 OpenAPI, Orval and the contract-derived API client

#### What it means

An API contract describes routes, methods, parameters, request bodies, response bodies and status codes in a machine-readable form. OpenAPI is the format committed under `frontend/openapi/TutoringCentre.Api.json`. Orval reads it and generates TypeScript interfaces, request functions, query keys and React Query hooks. This is contract-derived client generation: endpoint declarations on the backend produce a document, and that document produces the frontend client. It is more precise to describe this as contract-derived than to imply that an independently authored specification is necessarily the first artifact.

A generated type answers what the client expects. It does not prove that an arbitrary runtime response matches that type. TypeScript types disappear from the running JavaScript. This project checks the minimal shape of failure bodies, but successful JSON is cast to the generated response type in one place.

#### The problem it solves

Without generation, an author might write a second copy of a request shape:

```ts
// Illustration only: a manually maintained client can silently drift.
await fetch(`/api/groups/${id}/capacity`, {
  method: 'POST', body: JSON.stringify({ capacity: 30 })
});
```

The real server could require a version and use another method or path. The generated hook carries the committed request's actual shape. Feature code supplies data to that hook instead of inventing a second protocol. When a field changes, regeneration changes TypeScript types, making incompatible uses visible during typechecking.

#### How it works

`frontend/orval.config.ts:4` defines one generator target. Its input is the committed OpenAPI document. Output uses `client: "react-query"`, `httpClient: "fetch"`, `mode: "single"` and the generated `src/api/generated/tutoring-centre.ts`. The mutator override points to `src/api/apiFetch.ts` and names `apiFetch`. `includeHttpResponseReturnType: false` tells generation that the mutator returns a parsed response body rather than a wrapper containing data, headers and status.

Generated functions build URLs and request options. Generated hooks wrap those functions with TanStack Query. A generated query-key helper produces the same key convention used by the hook. Feature mutations use these helpers when invalidating affected reads. `frontend/package.json:14` exposes generation as `npm run generate:api`.

#### How THIS project implements it

`frontend/src/features/students/StudentsList.tsx:5` imports `listStudents`, `getListStudentsQueryKey` and generated parameter/DTO types. The component needs infinite pagination rather than one page, so it uses the generated plain request function inside a hand-written `useInfiniteQuery`. Generation is therefore a reusable protocol layer, not a requirement to use one particular generated hook for every presentation.

`frontend/src/features/enrollments/useEnrollmentMutations.ts:6` imports generated query-key helpers and mutation hooks. Its transfer call sends one generated `useTransferEnrollment` mutation with an enrollment id, target group id and version. `frontend/src/features/schedule/useScheduleMutations.ts:5` does the same for adding rules, ending rules, extending the schedule and cancelling sessions.

The client contains both numeric and string representations in some generated integer types. Feature code consequently uses `Number(...)` for counts and accepts `number | string` for versions where generated contracts permit either. Do not erase those distinctions by assuming every serialized integer is always a JavaScript number.

Verified source excerpt, `frontend/orval.config.ts:10` (quoted directly from the current file):

```ts
      target: "./src/api/generated/tutoring-centre.ts",
      clean: true,
      override: {
        // Every generated request goes through apiFetch — the single place errors become ApiError.
        mutator: { path: "./src/api/apiFetch.ts", name: "apiFetch" },
        // apiFetch returns the parsed body (not { data, status, headers }), so generated types must describe the body only.
        fetch: { includeHttpResponseReturnType: false },
      },
    },
  },
});
```

#### Execution example

1. A backend contract change alters the shape of `AddScheduleRuleRequest`.
2. The backend document generation updates `frontend/openapi/TutoringCentre.Api.json`.
3. Running `npm run generate:api` updates the generated TypeScript file.
4. `AddScheduleRuleDialog` builds its body with `toAddScheduleRuleRequest`.
5. `useScheduleMutations.addRule` passes that body to `useAddScheduleRule`.
6. The generated request constructs the actual URL and method, then invokes `apiFetch`.
7. If the changed contract requires a field the mapper does not supply, `npm run typecheck` exposes the mismatch before a user opens the dialog.
8. If the runtime server deviates from the committed document, generation alone cannot detect that; integration and browser tests remain necessary.

#### Why this project needs it

The frontend now spans identity, staff, subjects, students, parents, teachers, groups, enrollments and scheduling. A manually duplicated protocol would be large. The generated file is intentionally excluded from normal lint and formatting so regenerated code remains tool-owned. The valuable hand-written boundary is the mutator and the feature-specific data mapping, rather than repetitive URL assembly.

#### Alternatives

`openapi-typescript` plus a smaller fetch client would generate types without all these hooks. Hand-written types reduce generation machinery but increase duplication. A shared TypeScript server/client system such as tRPC would require a different backend arrangement. A GraphQL schema and generator would use a different protocol. The repository documents generated clients as a stack decision in its README and ADR-derived documentation; it does not provide a detailed Orval-versus-every-generator comparison. The following is an interpretation based on the implementation: the selected generator fits the existing TanStack Query ownership model.

#### Important details

The generated `ProblemDetails` interface is distinct from the hand-written runtime `isProblemDetails` guard. Neither the generated enum-like permissions mirror nor the hand-written `ErrorKind` is automatically synchronized just because generation exists. `features/session/permissions.ts` explicitly describes its keys as a hand-maintained backend mirror. Generated file size is not evidence that every operation has a UI. Conversely, a feature can use the plain generated function even when it does not use a generated hook.

#### What I should understand before modifying this code

Change the server contract first, rebuild its document, regenerate, then modify consumers. Never hand-edit generated request paths. Use generated key helpers for invalidation. Search both hook and plain-function usages before declaring an endpoint unused. When interpreting a test fixture, compare its actual object with the current generated DTO; older fixtures can omit newly added information even if a permissive mock still returns them.

<a id="frontend-errors"></a>
## 6.42 The fetch boundary, typed failures and CSRF recovery

#### What it means

`fetch` resolves a response even when its status is 400 or 500. It normally rejects only when the request cannot complete at the network layer or is aborted. An application therefore needs an explicit rule for turning HTTP responses into useful values or useful errors. This project's mutator makes that rule once. A successful JSON body becomes the requested DTO, 204 becomes `undefined`, and a failed response becomes an `ApiRequestError` carrying a stable code, HTTP status, kind and optional field errors and correlation reference.

CSRF protection adds a second responsibility. Browsers attach the session cookie without the script reading it. Unsafe operations also send an antiforgery request token in `X-XSRF-TOKEN`. The token is acquired from the API and cached as an in-memory promise. The framework's antiforgery cookie and request token are a validated pair; they are not assumed to be identical strings.

#### The problem it solves

Without a shared boundary, every form would need to duplicate status checks and token handling:

```ts
// Illustration only: incomplete request handling.
const response = await fetch('/api/session/centre', { method: 'POST', body });
return response.json(); // 204 has no JSON; 403 may be a structured refusal.
```

That code fails on a legitimate empty success and does not attach CSRF proof. The real generated request delegates to `apiFetch`, so response handling remains identical across feature pages.

#### How it works

At `frontend/src/api/apiFetch.ts:13`, `ApiRequestError` extends `Error` while implementing the project's `ApiError` interface. It copies stable fields from a mapped error rather than exposing arbitrary server text as the user-facing message. `kindFromStatus` maps 400, 401, 403, 404, 409 and 422 to the corresponding kinds. Other statuses, including 429, receive `unexpected` unless their stable code is handled more specifically by a screen.

`readBody` reads text first, allowing an empty body, then tries JSON parsing. `sendOnce` attaches `credentials: "same-origin"`, performs the request, handles 204 before parsing, returns successful bodies and throws mapped failures. Its catch converts network-level rejection to `network.unreachable` with status 0. That includes a rejected fetch caused by cancellation; the current wrapper does not preserve an `AbortError` distinction.

`apiFetch` treats every method except GET and HEAD as unsafe. It builds headers, obtains the cached antiforgery token and sends once. Only a 403 with code `auth.csrf_invalid` triggers a refresh and a second attempt. There is no loop; failure of the second attempt escapes. A successful login, logout or centre selection also starts a token refresh. Their callers additionally await a refresh at the session boundary.

#### How THIS project implements it

`frontend/src/api/csrf.ts:5` stores `cachedToken: Promise<string> | null`. Concurrent callers can share one pending token request. A failed fetch resets the cache rather than preserving one rejected promise forever. `refreshCsrfToken` replaces the cached promise with a fresh request. This cache is not persisted in localStorage or sessionStorage.

`frontend/src/api/problemDetails.ts:18` checks only the minimum runtime shape: a non-null object with string `title` and numeric `status`. Optional extension fields are subsequently trusted. `frontend/src/api/errorMessages.ts:7` translates by stable code, then kind, then generic fallback. `showApiError` sends that translated message to Sonner and optionally adds a correlation reference. `fieldErrors.ts` adds translation-aware field placement for newer feature forms.

Verified source excerpt, `frontend/src/api/apiFetch.ts:156` (quoted directly from the current file):

```ts
 * A successful login, logout or centre selection refreshes the token, since it is bound to the signed-in user.
 */
export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {
  const method = (init?.method ?? "GET").toUpperCase();
  const isUnsafe = method !== "GET" && method !== "HEAD";

  let result: T;
  try {
    result = await sendOnce<T>(url, init, await buildHeaders(init, isUnsafe));
  } catch (error) {
    if (!isUnsafe || !isCsrfError(error)) {
      throw error;
    }

    await refreshCsrfToken();
    result = await sendOnce<T>(url, init, await buildHeaders(init, isUnsafe));
  }

  if (isUnsafe && SessionChangingPaths.has(url)) {
    void refreshCsrfToken();
  }

  return result;
```

#### Execution example

1. The group-create form calls `useCreateGroup().mutateAsync` with its four request fields.
2. Generated request code invokes `apiFetch` with the proper URL and method.
3. `buildHeaders` adds JSON headers and awaits `getCsrfToken`.
4. A cold token cache issues `GET /api/auth/antiforgery`; the browser receives the cookie half and the script receives the request token.
5. The mutation goes to the API with that token header and same-origin credentials.
6. If the token is stale, a 403 `auth.csrf_invalid` becomes `ApiRequestError`; the mutator obtains a new token and retries exactly once.
7. A real group refusal such as an archived teacher is not retried as a CSRF problem. The form catches the mapped error and places a translated message under its teacher select.
8. A successful create returns its generated response object. The form invalidates group queries and navigates to the new detail page.

#### Why this project needs it

The same security and failure conventions apply to dozens of operations. A missing token on one enrollment action would otherwise be easy to overlook. Code-based translation also separates developer diagnostics from user copy: the API's `detail` is available for diagnosis, while `messageFor` selects the localized text shown to the user.

#### Alternatives

Axios interceptors could centralize the same behavior. Separate wrappers for each feature would duplicate security behavior. Runtime schema validation of every success body would add stronger checks at the cost of schemas and parsing. Retrying every 403 would be wrong because forbidden is often a meaningful authorization refusal. The following is an interpretation based on the implementation: retrying only the explicit CSRF code heals expired proof without replaying unrelated failures.

#### Important details

Successful bodies are trusted with `as T`; malformed success JSON becomes `undefined` and can fail later. The Problem Details guard is deliberately minimal and does not deeply validate `errors`. The token is readable by application JavaScript while in memory; storage avoidance does not make it inaccessible to arbitrary script already executing in the same page. `HttpOnly` protects the session cookie from direct script reads, not from same-origin actions by compromised script. The wrapper's fire-and-forget refresh has no local rejection handler; do not describe all refresh failures as fully handled without examining that behavior.

#### What I should understand before modifying this code

Preserve the one-retry bound. Do not retry arbitrary mutations after ambiguous network failures. Keep error codes stable because screens and translations depend on them. Add a session-changing path to the mutator when appropriate, or explicitly refresh in that operation's handler as password change currently does. Use `asApiError` only for errors from this boundary; arbitrary exceptions intentionally become a generic client error. Inspect optional extension fields before strengthening runtime trust guarantees in documentation.

<a id="frontend-sessions"></a>
## 6.43 Sessions, route guards, first-login gates and permission-aware rendering

#### What it means

The browser owns a credential cookie that application JavaScript cannot read. The SPA learns the current user's profile by asking `/api/me`. A route guard runs before a matched page is rendered and can redirect it. A permission wrapper controls whether a piece of UI mounts. Both are usability controls: the API must independently enforce authentication, tenancy and permissions because a user controls the browser and can bypass its code.

Authentication, centre selection and first-login password change are separate states. A user can be authenticated without selecting a centre. A user can also be authenticated but required to replace a temporary password. The protected route layout checks these states in an intentional order.

#### The problem it solves

A screen that assumes every visitor is ready for ordinary centre operations will issue requests that predictably fail. Scattering redirects inside each page duplicates logic and can briefly show unusable content. Hard-coded role checks also couple every screen to a policy table:

```tsx
// Illustration only: policy duplicated into a page.
return me.activeRole === 'owner' ? <CreateButton /> : null;
```

The actual helper asks whether the returned permission set includes the required action. A backend policy change can alter the response without teaching every screen new role comparisons.

#### How it works

`frontend/src/features/session/meQueryOptions.ts:10` defines one session query. Its function calls generated `getMe`; a 401 resolves to `null`, while other failures throw. Signed out is data, not a transport error. `retry: false` avoids retrying a meaningful unauthenticated result. `useSession` exposes that data and its loading/error state.

`frontend/src/routes/_authenticated.tsx:15` uses the router's shared query client with `staleTime: "static"`. Its `beforeLoad` redirects null sessions to login, preserving `location.href`; then redirects `mustChangePassword` to `/change-password`; then redirects an absent active centre to `/select-centre`. Only afterward can the shell and child render. The centre-selection route repeats the first-login gate because selecting a centre is not a way around password replacement.

`useCan` checks `me.permissions.includes(permission)`. `Can` renders children only when allowed. `RouteGuard` displays an explicit no-access state and dashboard link otherwise. Protected data hooks belong inside the child content component so denying the wrapper prevents mounting the query, rather than merely hiding already fetched results.

#### How THIS project implements it

The permissions catalog in `frontend/src/features/session/permissions.ts:2` includes sixteen view/manage keys across subjects, staff, audit, centre settings, students, teachers, groups, enrollments and schedule. It is a manual frontend mirror. `frontend/src/features/shell/AppShell.tsx:132` uses these keys to show navigation. Group detail independently gates roster and schedule tabs by their respective view permissions, so a pasted tab search parameter falls back to overview when unavailable.

`frontend/src/app/queryClient.ts:34` handles later unauthenticated errors globally. It clears the client and calls the login redirect unless already on login. Query errors use that handler; mutation errors use it except for the login mutation, whose expected wrong-password 401 belongs to the login form. A permission denial is different: `refetchMeOnPermissionDenied` invalidates `/api/me` only for `auth.permission_denied`, allowing stale button visibility to catch up with the server's new permissions.

Verified source excerpt, `frontend/src/routes/_authenticated.tsx:16` (quoted directly from the current file):

```tsx
    const me = await context.queryClient.query({ ...meQueryOptions, staleTime: "static" });

    if (me === null) {
      throw redirect({ to: "/login", search: { redirect: location.href } });
    }

    // The server-side gate (Day 30) is the real control; this only sends the user to the pleasant form for it
    // instead of letting every other request 403 with auth.password_change_required.
    if (me.mustChangePassword) {
      throw redirect({ to: "/change-password" });
    }

    if (me.activeCentreId === null) {
      throw redirect({ to: "/select-centre" });
    }
  },
  component: AuthenticatedLayout,
```

#### Execution example

1. A newly created member signs in. Login stores the returned `MeDto` in the session cache.
2. Login may navigate to the dashboard or centre picker according to active-centre state.
3. Either protected layout or centre picker sees `mustChangePassword` and redirects to the dedicated form.
4. `change-password.tsx` validates current password, a new password of at least ten characters, a different new value, and matching confirmation.
5. Its generated mutation sends only current and new passwords. Confirmation is local validation, not part of the API body.
6. On success the form awaits CSRF refresh, invalidates the session query, shows confirmation and navigates according to selected-centre state.
7. The updated session allows normal routes to mount.
8. Later, if a group mutation receives `auth.permission_denied`, its feature handler invalidates session permissions. If instead any ordinary request discovers a lost session, global handling clears cache and navigates to login.

#### Why this project needs it

Staff now have different capabilities and can belong to multiple centres. Account creation also introduces temporary credentials. The current UI cannot treat sign-in as a single boolean. Central session state, ordered gates and permission-driven rendering make those server distinctions understandable to a staff member without pretending the browser enforces them.

#### Alternatives

A user context or Redux store could duplicate the current profile, but would need synchronization with the API. Per-page effects could redirect after rendering, with duplicated conditions and potential content flashes. Role-based UI switches could work for a fixed policy, but this repository intentionally chooses permissions. The following is an interpretation based on the implementation: hiding the data-owning child component is also a privacy-conscious way to avoid unnecessary forbidden requests.

#### Important details

The route guard's cached session is not a new server check on every navigation. Revocation may first become visible through the next API call. `useSignOut` awaits logout and CSRF refresh before clearing queries, so a refresh failure can interrupt its later local cleanup. The login page itself has no redirect-away guard for an already signed-in user. Centre selection invalidates session data, but the shown handler does not clear every previous centre's feature query. Because generated query keys primarily describe URLs and parameters, reusing a feature query after centre switching deserves review rather than an assumption that every cache entry includes tenant identity.

#### What I should understand before modifying this code

Put protected routes beneath `_authenticated`. Use permission keys for actions and mount data-owning components inside `RouteGuard`. Keep first-login checks ahead of ordinary centre routes. Distinguish a 401 from a 403 and a password-change refusal from a permission refusal. Audit cached data when adding centre-sensitive pages; server-side isolation and client-side cache isolation are separate requirements. Read the full cache lifecycle before promising that a session or centre transition erases all previous data.

<a id="frontend-forms"></a>
## 6.44 React Hook Form, Zod and field-level server errors

#### What it means

A form needs values, validation state, submission state and error presentation. React Hook Form supplies field registration and orchestration; Zod supplies schemas that accept or reject value shapes; the Zod resolver joins them. This is client-side convenience, not a replacement for backend validation. An attacker can omit the UI entirely. The server remains responsible for allowed operations and valid persisted data.

RHF's `register` connects an ordinary input to form tracking. `Controller` connects components whose value API differs from a native input, such as the shared phone input and Base UI select. `handleSubmit` first runs validation and only calls the mutation handler for valid values. `setError` adds a server or manual refusal to a specific field.

#### The problem it solves

Without a shared form pattern every dialog would manually read inputs, check lengths and maintain separate errors:

```ts
// Illustration only: repetitive manual form validation.
if (!name.trim()) setNameError('Name required');
if (capacity < 1) setCapacityError('Capacity invalid');
if (!teacherId) setTeacherError('Teacher required');
```

The project expresses these checks in schemas, then reuses the same field rendering convention. A client check can save a pointless request, but state-dependent conflicts such as a teacher archived a second ago still come back from the server.

#### How it works

The schema provides both runtime validation and an inferred TypeScript type. `z.infer<typeof schema>` prevents a separately handwritten form-values interface from drifting from validation. Translation keys are stored as schema error messages. Local errors are translated when rendered; errors marked `type: "server"` are already display text and are rendered as supplied by the feature's mapping.

`frontend/src/api/showApiError.ts:18` implements the original `applyFieldErrors`: it ignores unknown field names, uses only the first message and preserves server text. `frontend/src/api/fieldErrors.ts:21` implements `applyTranslatedFieldErrors`: when `errors.json` contains an exact code message, that translated message is applied to each recognized field; otherwise it falls back to field text. It returns whether anything was placed. `setCodedFieldError` handles a coded refusal that has no field dictionary.

#### How THIS project implements it

`frontend/src/routes/login.tsx:32` has a small email/password schema. It checks presence and email syntax but not all backend length limits. `frontend/src/features/auth/changePasswordSchema.ts:7` adds cross-field refinements for different current/new passwords and matching confirmation. A refinement's `path` decides which field displays the error.

`frontend/src/features/groups/CreateGroupDialog.tsx:49` uses a typed schema and default values. Its submit function sends exactly name, subject id, teacher id and capacity. A subject-archived or subject-not-found code lands under `subjectId` and invalidates subject choices; corresponding teacher failures land under `teacherId`. Other field errors use the translation-aware mapper. A successful response invalidates groups, closes the dialog and navigates to the created group's overview.

`RegisterStudentForm` cannot decide whether a new parent's name is required until lookup resolves. Its schema intentionally permits an empty parent name; submit requires it only in the new-parent branch. This is a concrete example of a shape rule depending on another operation's outcome rather than a reason to make every optional field globally required.

Verified source excerpt, `frontend/src/features/auth/changePasswordSchema.ts:7` (quoted directly from the current file):

```ts
export const changePasswordFormSchema = z
  .object({
    currentPassword: z.string().min(1, "validation.currentPasswordRequired"),
    newPassword: z.string().min(MinPasswordLength, "validation.newPasswordTooShort"),
    confirmPassword: z.string().min(1, "validation.confirmPasswordRequired"),
  })
  .refine((data) => data.newPassword !== data.currentPassword, {
    message: "validation.newPasswordSameAsCurrent",
    path: ["newPassword"],
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "validation.confirmPasswordMismatch",
    path: ["confirmPassword"],
  });

```

#### Execution example

1. A staff member opens Create Group; the form initializes empty name and relationships with capacity twenty.
2. RHF registration connects native inputs; `Controller` connects subject and teacher selects.
3. The user submits. Zod rejects missing required selections before a request.
4. After valid local input, the generated mutation sends its request through the CSRF-aware boundary.
5. If another staff member archived the selected teacher, the server refuses with a stable teacher-related code.
6. The catch branch maps that code under the teacher select, invalidates the teacher list and leaves the form available for correction.
7. A later successful submit receives the created id; the group list is marked stale and navigation opens the new detail page.
8. React re-renders each field's `aria-invalid` and error paragraph from RHF state rather than manually manipulating DOM text.

#### Why this project needs it

The application now contains many related dialogs with both local and state-dependent validation. Sharing the orchestration lets the code focus on business outcomes: duplicate parent, archived subject, stale group version or overlapping schedule. Translated errors are particularly necessary because developer English from a backend validator is not suitable Arabic UI copy.

#### Alternatives

Controlled `useState` inputs work for small forms but require more orchestration. Formik and TanStack Form are alternative state libraries; Yup and Valibot are alternative schema libraries. Native browser validation is useful but cannot express all translated cross-field and server outcome behavior. The repository does not explicitly state why RHF and Zod were chosen over every alternative. The following is an interpretation based on the implementation: schemas plus `Controller` fit repeated dialogs and custom headless UI controls.

#### Important details

Not every server-field path equals the UI path. Schedule errors for `startTime` map to the form's hour/time group. Parent errors use dotted names such as `parent.fullName`. A known-field allowlist avoids silently assigning arbitrary server keys. `noValidate` prevents browser validation bubbles from taking precedence over the project's chosen messages. Disabling submit during pending work improves interaction but does not enforce idempotency; the backend must still handle repeated or racing requests.

#### What I should understand before modifying this code

Update the schema, inferred values, known fields, generated contract and both locale resources together. Keep confirmation-only fields out of request bodies. Put a refusal under the field the user can correct; use a banner for a cross-field scheduling conflict and a toast for a completed operation. Preserve server authority: the client must handle a valid-looking selection becoming invalid after it was loaded. Do not put credentials or parent phones into URLs merely to simplify form state.

<a id="frontend-i18n"></a>
## 6.45 Internationalisation, RTL, bidirectional text and self-hosted assets

#### What it means

Internationalisation separates UI messages from code and provides language-dependent formatting. Localisation supplies the translated resources. Arabic also changes document direction, so translation is not only substituting words. Logical layout properties express start and end rather than fixed left and right. Mixed-direction content introduces a separate problem: a phone number, identifier or time range must remain left-to-right even inside an Arabic sentence.

ICU message syntax lets a message contain placeholders and plural rules. A message such as an enrollment confirmation is one translated sentence with named data, rather than English fragments glued around a student name. Browser `Intl` formatting handles language-specific dates and numbers, with an explicitly supplied time zone where the domain requires one.

#### The problem it solves

Without resources, a page contains literal English strings that never change. Without logical CSS, switching `dir` leaves borders and padding on the wrong side. Without isolation, punctuation and digits can reorder in an Arabic sentence:

```tsx
// Illustration only: physical layout and an unisolated technical value.
<div className="border-r pl-3">Reference: {correlationId}</div>
```

The project's equivalent uses logical classes and either markup isolation such as `<bdi>` or `isolateLtr` for plain-text interpolation. These solve different layers of the same reading problem.

#### How it works

`frontend/src/i18n/index.ts:1` imports i18next, the ICU plugin, the React bridge and all resources. The current resource sets contain fifteen namespaces in each language: common, status, errors, auth, shell, subjects, students, parents, staff, settings, audit, teachers, groups, enrollments and schedule. They are bundled, not fetched at runtime.

`detectInitialLanguage` reads `tcm.lang` if available, otherwise checks browser language, otherwise selects English. Its storage access is wrapped because browser storage can throw. `applyDocumentDirection` sets `document.documentElement.lang` and `dir` before rendering. The language-change listener repeats that operation and attempts to persist the preference. `useTranslation` subscribes rendered components to language changes; when the active language changes, subscribed text is recomputed during render.

#### How THIS project implements it

`frontend/src/features/language/LanguageSwitcher.tsx` calls `changeLanguage` and labels the action in the destination language. The shell uses `border-e`, `start-0` and inline-direction padding. Direction-aware transitions move the mobile navigation from the appropriate side. `frontend/src/lib/bidi.ts:7` wraps technical text in Unicode left-to-right isolate and pop-directional-isolate characters. That works inside translated plain strings where a JSX `<bdi>` cannot be inserted.

`frontend/src/features/audit/auditFormatting.ts:36` translates stored vocabulary before rendering. Roles reuse auth labels, locale uses common labels, and status, relationship, primary-parent flags, enrollment end reason and weekday values use audit translations. Raw user-provided names remain text rather than translation keys. This is important: data supplied by a parent is not a message key and must never be passed to `t` as though it were code-defined vocabulary.

Fonts are imported through local packages and emitted by Vite. `frontend/vite.config.ts:25` sets `assetsInlineLimit: 0`, ensuring even small font assets remain same-origin files rather than data URLs. The stylesheet defines the theme and font stack. The repository's frontend README explains that dependencies must fit the backend CSP instead of requiring an external script CDN or runtime `eval`.

Verified source excerpt, `frontend/src/lib/bidi.ts:7` (quoted directly from the current file):

```ts
 */
export function isolateLtr(value: string): string {
  return `\u2066${value}\u2069`;
```

#### Execution example

1. The user activates the language switcher while viewing a timetable.
2. i18next changes its current language and emits the language-change event.
3. The listener sets the root document to Arabic and RTL and attempts to save `tcm.lang`.
4. React translation subscriptions cause the timetable, navigation, buttons and dialogs to render localized text.
5. Logical layout utilities follow the document's direction; direction variants adjust arrow icons and mobile navigation.
6. Timetable formatters use the centre zone and the new locale to format the same instants. They do not change which instant a session represents.
7. Time containers remain LTR; user names use isolation or automatic direction.
8. An error reference interpolated with `isolateLtr` stays readable even when surrounded by Arabic punctuation.

#### Why this project needs it

Egyptian centre staff need Arabic UI alongside English. The application also displays Latin emails, identifiers, phone numbers and IANA zone names. Direction must be explicit for technical values while natural-language names remain readable in either script. The repeated use of `bdi`, `dir="auto"`, `dir="ltr"` and helper-based isolation shows this is an active implementation concern.

#### Alternatives

Separate RTL stylesheets could mirror physical CSS manually, with more maintenance. `react-intl` is another message system. Lazy-loaded resources could reduce initial loading at the cost of asynchronous language acquisition. External font CDNs would conflict with the project's self-hosting policy. The repository does not explicitly justify every formatting choice. The following is an interpretation based on the implementation: bundled resources and local assets simplify same-origin production hosting and avoid a second availability dependency for basic UI text.

#### Important details

`preferredLocale` returned by `/api/me` is not what `detectInitialLanguage` reads; startup uses storage and browser preference. Resource key parity proves both language files contain matching key structures, not that the translations are correct or visually fit. `showApiError` still hard-codes `Reference:` in one toast path. Audit timestamps use `Intl.DateTimeFormat` without an explicit centre zone, unlike the scheduling formatters; do not claim every timestamp everywhere uses centre time. Generated UI components require their own RTL review even when feature components use logical classes.

#### What I should understand before modifying this code

Add messages in both language resources, choose the correct namespace and run the parity script. Keep identifiers isolated during interpolation. Use logical CSS for feature layouts. Avoid adding arbitrary user text as translation keys. Verify formatting in Arabic with real mixed text, especially dates, ranges and correlation references. Keep new assets compatible with same-origin CSP and inspect Vite output when introducing a font or image dependency.

<a id="frontend-pagination"></a>
## 6.46 URL state, debounced search and cursor pagination

#### What it means

A useful list screen has at least three different kinds of state. Its current search and filters describe a navigable view, so they belong in the URL. The text currently being typed is an immediate local draft. The pages returned by the server are cached server state. Separating them allows a user to bookmark a filtered list, type without issuing a request on every keystroke, and request more data without replacing the entire list.

Cursor pagination means the server returns an opaque continuation value with a page. The browser sends that value back to request the next page. The browser does not calculate row offsets or interpret the cursor. The cursor belongs to the server's ordering and filtering contract. Changing the search term creates a different query, rather than continuing an old cursor under new filters.

#### The problem it solves

Without URL-owned filters, reloading loses the view and Back cannot restore it. Without debouncing, a five-character search can cause five requests. Without separate pagination state, loading more can replace earlier rows or accidentally append results from another search. A naive version illustrates the coupling:

```ts
// Illustration only: a page number and a local term hide the view from navigation.
const [term, setTerm] = useState('');
const [page, setPage] = useState(1);
fetch(`/api/students?search=${term}&page=${page}`);
```

The actual client does not invent a numbered pagination protocol. It uses the generated cursor parameters, puts search in route state and keys the cache by the current request parameters.

#### How it works

`frontend/src/components/shared/SearchInput.tsx:23` maintains a draft and the last synchronized external value. Input changes update the draft immediately. Its effect starts a timer, defaulting to 300 milliseconds; cleanup cancels that timer if the draft changes before it fires. A ref holds the latest callback. External URL changes reset the draft during render so Back navigation does not first paint obsolete text. Clearing is immediate because it is a deliberate action rather than an incomplete typing sequence.

`frontend/src/features/students/StudentsList.tsx:32` constructs generated list parameters with search, archive visibility and page size twenty-five. At line 43 `useInfiniteQuery` uses the generated list key, passes its `pageParam` as `cursor` and passes the cancellation signal to the generated function. `getNextPageParam` returns the server's `nextCursor` or `undefined`; undefined means there is no next page. Rendering flattens `data.pages` into visible items.

#### How THIS project implements it

`frontend/src/routes/_authenticated/students.index.tsx` owns the route's search parameter interpretation and navigation updates. The shared search input does not know about students or server normalization. Its caller changes route search; a new render passes that search to `StudentsList`; the generated key changes accordingly. Arabic normalization is a server responsibility, as the frontend README states; the input does not normalize a person's name itself.

The audit log follows the same continuation pattern in `frontend/src/features/audit/AuditLog.tsx:32`. Audit filters include entity type, date bounds and page size. `frontend/src/routes/_authenticated/audit.tsx:24` validates known entity types and builds URL state. `LoadMoreButton` is shared by feature lists, while audit currently renders its own load-more control. Both request more through the infinite query rather than assembling an independent fetch queue.

Verified source excerpt, `frontend/src/features/students/StudentsList.tsx:43` (quoted directly from the current file):

```tsx

  const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({
    queryKey: getListStudentsQueryKey(params),
    queryFn: ({ pageParam, signal }) => listStudents({ ...params, cursor: pageParam }, { signal }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  });
```

#### Execution example

1. A secretary opens `/students` and the route parses its search state.
2. `StudentsList` asks for the first page with page size twenty-five and no cursor.
3. Typing into `SearchInput` changes the visible input immediately. Earlier timer callbacks are cancelled as typing continues.
4. After the debounce, `onChange` updates route search. The list receives the new term.
5. The generated key now describes that term and filters, so the query loads its own first page rather than appending to the unfiltered cache entry.
6. A response supplies `items` and `nextCursor`. The table renders all pages currently held by that query.
7. Clicking Load More calls `fetchNextPage`; its query function sends the returned cursor with unchanged filters.
8. Back navigation restores previous URL state. The search input synchronizes its local draft, and the query cache can supply the corresponding earlier view.

#### Why this project needs it

Student and audit collections can grow beyond a convenient single response. Office staff also need to return to searches and share precise views. The separation keeps browsing behavior coherent while the backend owns filtering, ordering and continuation semantics. The same design is reused for a smaller student picker with a different page size rather than downloading every contact record into the enrollment dialog.

#### Alternatives

Offset pagination is easier to describe with page numbers but reacts differently to concurrent inserts. Fetching all rows and filtering locally can be acceptable for a tiny fixed dictionary, not a growing student collection. A search term in React context would persist while mounted but still lack browser navigation semantics. The repository does not explicitly compare cursor pagination with every alternative in frontend code. The following is an interpretation based on the implementation: opaque cursors keep ordering knowledge on the server while infinite queries provide a direct load-more UI.

#### Important details

The cursor must not be decoded or edited in the client. Query keys must include filters. A new search must start a new continuation chain. Debouncing is not cancellation: a request already issued still needs the query's signal and correct cache key. A loading-next-page indicator differs from initial loading; earlier visible rows should remain available. Tests should distinguish an empty entire collection from no matches for a search because the user actions differ.

#### What I should understand before modifying this code

Read route parsing, `SearchInput`, list parameters and `getNextPageParam` together. Preserve the external reset path for browser navigation. Do not add a second manual page counter to an infinite query. Use server-provided continuation values and generated URL builders. Keep sensitive data, especially parent lookup phones, out of navigable query parameters when the existing protocol intentionally uses a mutation body.

<a id="frontend-parent-lookup"></a>
## 6.47 Parent lookup, conditional registration and stale-response protection

#### What it means

Registering a student can mean linking an existing parent or creating a new parent. Those are different request shapes. The client first asks the server whether a parent exists for the supplied phone. If found, registration sends an id; otherwise it sends the new parent's name and phone. The lookup result is state-dependent and can become obsolete while the user is still typing or while another secretary registers the same parent.

An asynchronous race occurs when request A starts for one value, the user changes the input, request B starts for the new value, then A finishes later. A result must be associated with the input that produced it. Applying whichever response arrives last is not enough because arrival order is not user-intent order.

#### The problem it solves

Without lookup, siblings can create duplicate parent records or a form might overwrite an existing parent's details. Without response association, a result for one phone can be displayed under a different phone. A naive implementation would contain:

```ts
// Illustration only: this accepts a response without checking the current field.
setFound(await lookupParent({ data: { phone } }));
```

The project records both the latest field value and the value behind the accepted lookup. It deliberately uses a mutation, so the phone does not become a query-cache key.

#### How it works

`frontend/src/components/shared/useParentLookup.ts:20` owns the generated lookup mutation and two refs. `latestPhoneRef` follows current input. `lookedUpPhoneRef` identifies the exact input for the stored result. `onPhoneChange` clears visible result/error state when the new value differs. `ensureLookup` can reuse a result only when its associated phone equals the requested one, unless force is requested.

After awaiting the mutation, the hook compares `latestPhoneRef.current` with the phone used for that request. A mismatch prevents the old result from replacing the current field's panel. Matching success records the phone and result. Matching failure either places a phone field error or shows a common error toast. The hook returns a discriminated outcome so a caller can stop submission when lookup failed.

#### How THIS project implements it

`frontend/src/features/students/RegisterStudentForm.tsx:65` calls `ensureLookup` before constructing the registration request. A found parent becomes `{ id: outcome.parent.id }`. A missing parent requires a trimmed full name and becomes `{ fullName, phone }`. The form sends student name, optional phone, optional school year, relationship and that parent union. It does not send a client-supplied centre id.

The registration schema at `frontend/src/features/students/registerStudentSchema.ts:17` checks shape and lengths. Parent full name is not unconditionally required because an existing parent's id is enough. `ParentLookupField` renders the input and the existing/new branch; `AddParentDialog` reuses the same hook for an existing student. `PhoneInput` accepts raw text and imposes LTR display but leaves real phone normalization and validation to the server.

The submit catch handles `parent.phone_taken` specially: it forces another lookup rather than asserting that the earlier no-match result is still authoritative. That is the expected response to a check-then-create race. The backend's uniqueness rule remains the authority; the client updates its choices to the new state.

Verified source excerpt, `frontend/src/components/shared/useParentLookup.ts:56` (quoted directly from the current file):

```ts
      setFound(result.parent);
      return { ok: true, parent: result.parent };
    } catch (thrown) {
      if (latestPhoneRef.current !== phone) {
        return { ok: false };
      }

      const apiError = asApiError(thrown);
      const message = apiError.fieldErrors?.phone?.at(0);
      if (message !== undefined) {
```

#### Execution example

1. A secretary enters a student name and parent phone.
2. `onPhoneChange` records the current phone and clears any panel associated with an earlier one.
3. Lookup sends the phone in a generated mutation body. While pending, form controls reflect pending state.
4. The server returns an existing parent. The hook accepts it only if the field still contains that exact phone.
5. The form displays the existing parent and submits only its id with the relationship and student details.
6. On successful registration, the student list is invalidated, a localized toast appears and navigation opens the created student detail.
7. In a different execution, lookup returns no parent but another secretary registers that phone first. Registration gets `parent.phone_taken`.
8. The catch forces lookup again; the UI can now offer the existing parent instead of blindly retrying a duplicate create.

#### Why this project needs it

Siblings share a parent. Staff can work concurrently. Parent phone numbers are also sensitive contact data, so the lookup should not automatically become a bookmark, browser history item or reusable query key. The existing pattern addresses both workflow correctness and the placement of sensitive inputs in client state.

#### Alternatives

Always creating parents would rely on later deduplication. A mandatory separate parent-management page would add steps to student registration. A normal GET query with the phone in the URL is cache-friendly but exposes the input in places the current body-based protocol avoids. Debouncing alone would not solve stale response application. The repository describes its mutation choice in the hook comments. The following is an interpretation based on the implementation: keeping lookup state local lets both registration and Add Parent own independent, short-lived interaction state.

#### Important details

Refs preserve identity across renders without triggering renders; state triggers visual updates. Both are needed here. The hook compares exact raw values, so do not silently introduce a different normalization on only one side of that comparison. A result discarded because input moved on returns a distinct outcome; evaluate submit behavior if changing whether the form can be edited while a lookup is pending. The lookup's cache avoidance is not a promise that the browser never holds a phone in memory; the form necessarily does.

#### What I should understand before modifying this code

Keep the existing-parent id branch exclusive from the new-parent data branch. Preserve association between accepted results and their input. Handle server uniqueness conflicts even after a successful lookup. Do not duplicate phone-format rules in the shared input. Test changing the input while a request is pending, lookup failure followed by retry, siblings using the same parent, and registration racing another user's parent creation.

<a id="frontend-versioned-editing"></a>
## 6.48 Versioned edits, archive workflows and the teacher/account boundary

#### What it means

An edit based on an earlier read can overwrite someone else's change. Optimistic concurrency attaches a version to a resource. The client sends the version it actually saw with a mutation; the server decides whether that version still matches. The browser does not mint a new version itself. A conflict is a meaningful outcome: reload the resource and let the user decide again.

Archiving is a lifecycle transition, not deletion. The application retains records and relationships while ordinary lists default to active items. Restore is another transition. Teacher records are also separate from staff accounts: a named teacher can exist as a business record, and an optional link connects that record to an eligible sign-in identity.

#### The problem it solves

Without a version, two staff members can both read a group capacity of twenty, independently edit it, and overwrite one another without noticing. Without an archive lifecycle, removing a teacher could destroy references needed for group and schedule history. Without a separate teacher/account boundary, every teacher record would be forced to possess login credentials even when the office only needs to assign their name to a group.

```ts
// Illustration only: this request omits the read version.
await renameGroup({ id, data: { name: draftName } });
```

The actual edit dialogs carry the loaded DTO's version in request bodies. Subject archive and restore likewise use the version associated with the displayed row.

#### How it works

`frontend/src/features/subjects/SubjectsList.tsx:70` creates an invalidation function for all subject-list variants. Archive confirmation sends the target's id and version. Both success and failure invalidate the list; success reports completion, while failure reports its refusal and refreshes session permissions if required. Restore tracks which row is pending and clears that local state in `finally`.

Group detail uses the same principle at `frontend/src/routes/_authenticated/groups.$groupId.tsx:176`: archive/restore send the resource version and invalidate both detail and list reads. The separate rename, capacity and teacher-assignment dialogs avoid replacing an entire entity from an old form. They express one action with its appropriate fields.

#### How THIS project implements it

`frontend/src/features/teachers/TeacherAccountDialog.tsx:44` loads the staff list only while the account-linking dialog is mounted. `eligibleStaffForTeacher` filters active Teacher members not already linked elsewhere, while allowing the record's current account as appropriate. This role filtering is data eligibility for account linking, not the usual action authorization pattern; action visibility still uses permissions.

The dialog sends `userId` or null plus the teacher version to `useSetTeacherAccount`. `teacher.user_not_eligible` and `teacher.account_taken` become field messages under the account select; the staff choices are refreshed and selection clears. Other failures show a toast, refresh permissions if needed, invalidate teachers and close. The list is advisory: the backend must recheck eligibility because membership can change after loading.

`StaffFormDialog` implements another identity boundary. Its returned temporary password is placed in local `revealed` state for a one-time panel. Closing resets local fields and the mutation. Existing-account handling takes a different result branch. The comment's intention that a password is never retained in the query cache should be read carefully: the generated mutation has its own mutation state until reset; the code does call reset on close, but the underlying mutation still received the response. Avoid documenting stronger guarantees than the actual lifecycle provides.

Verified source excerpt, `frontend/src/features/subjects/SubjectsList.tsx:77` (quoted directly from the current file):

```tsx
      setArchiveTarget(null);
    } catch (thrown) {
      const apiError = asApiError(thrown);
      await invalidate();
      showApiError(apiError);
      refetchMeOnPermissionDenied(apiError, queryClient);
      setArchiveTarget(null);
    }
  };

  const handleRestore = async (subject: SubjectDto) => {
```

#### Execution example

1. An owner opens a group detail and receives a DTO with version V.
2. Another owner changes that group, advancing its version on the server.
3. The first owner submits an action with V. The server refuses stale work.
4. The feature maps the refusal, invalidates detail and list queries, and renders the new data.
5. In a teacher-linking flow, opening the dialog mounts the staff query and filters available accounts.
6. The owner selects a displayed eligible account and sends the teacher version with that user id.
7. If the account became linked elsewhere, the server refuses. The field-level error remains near the choice, and the refreshed options remove obsolete assumptions.
8. A later successful selection invalidates teachers and closes the dialog; group and schedule authorization derive from the server's resulting link, not from a client role toggle.

#### Why this project needs it

Staff share mutable office data. Capacity, teacher assignment and membership all affect later enrollment or schedule behavior. Explicit action requests and version-aware refusals are easier to recover from than a generic save of an entire stale entity. Separating record identity from sign-in identity also matches the practical fact that a centre can track a teacher before granting them account access.

#### Alternatives

Pessimistic locks could prevent editing while another user works, at greater coordination cost. Last-write-wins is simpler but silently loses changes. Hard deletion could be acceptable for ephemeral data but changes history semantics. Automatic retry with a newly fetched version would hide the conflict by overwriting work the user has not reviewed. The following is an interpretation based on the implementation: explicit stale refusals keep business decisions with the user while allowing independent reads without long-lived locks.

#### Important details

Never send a newly invented version to force acceptance. Archive visibility belongs to list search state and does not grant manage rights. A row's colored state also has a written badge. A teacher's eligible account list can contain emails, so mounting only on demand and guarding its use matter. Different mutations invalidate different resource sets; inspect their dependencies rather than invalidating only the screen currently visible.

#### What I should understand before modifying this code

Trace the loaded DTO's version into the generated request. Decide what stale refusal should leave open and what must refetch. Preserve separate teacher records and staff identities. Do not replace server eligibility with client filtering. Keep archive and restore as named actions where they already express business transitions. Review transient credential storage in both local component state and mutation state when changing staff creation.

<a id="frontend-enrollments"></a>
## 6.49 Enrollment actions, atomic transfer and cache convergence

#### What it means

Enrollment connects a student with a group. Viewing a roster, managing enrollments and opening a student's full details are separate capabilities. Capacity shown on a screen is advisory because another secretary can take the last seat before a submit reaches the server. An atomic transfer means ending one enrollment and opening its replacement as one server operation; the client must not implement that business action as two unrelated requests.

Cache convergence means every displayed view of changed data is refreshed after the operation. Enrollment affects more than a roster row: group counts, group detail, lists, dashboard seat totals and the student's enrollment history can all depend on it. One shared mutation layer enumerates those affected reads.

#### The problem it solves

A naive transfer can leave the student without either enrollment if its second request fails:

```ts
// Illustration only: not atomic and not used here.
await withdrawEnrollment(oldEnrollment);
await enrollStudent(newGroup); // target may now be full
```

Likewise, immediately adding an optimistic roster row would imply the client already won a capacity race. The actual workflow waits for the server and treats a full group, an ended enrollment and a stale version as ordinary outcomes that need a clear message and refreshed state.

#### How it works

`frontend/src/features/enrollments/useEnrollmentMutations.ts:23` owns enroll, withdraw and transfer. Its `run` helper awaits the request, returns `{ ok: true }` or `{ ok: false, error }`, and invalidates affected reads in `finally`. That means refresh happens after both success and refusal. A failed last-seat request is evidence that the displayed count may be stale, so failure still requires a reload.

Invalidation includes each touched group's roster and detail, all group-list variants, and the student's enrollment list. Transfer supplies both source and target ids. The transfer request contains enrollment id, target group id and the existing enrollment version. There is no separate withdraw call in that method.

#### How THIS project implements it

`frontend/src/features/enrollments/GroupRosterTab.tsx:27` calls `useGetGroupRoster`. `enrollments.view` allows viewing through its parent tab; `enrollments.manage` adds actions; `students.view` turns names into links. The teacher-facing roster therefore does not need a second component with hidden contact fields. It receives the server's scoped entries and renders available navigation and actions by permission.

`EnrollStudentDialog` searches students through the dedicated picker. It sends `{ studentId }` to the group enrollment action. A 404 closes the dialog with a message, while other business refusals remain inside it so the user can correct a choice. `TransferDialog` filters other active groups of the same subject. Full choices are displayed but disabled by `GroupChoiceList`; the server still rechecks them. If a target becomes full, selection clears while the dialog remains open.

`SeatMeter` at `frontend/src/features/enrollments/SeatMeter.tsx:17` displays server counts with written states: almost full from eighty percent and full at capacity. Its bar is decorative and hidden from assistive technology; count text is LTR-isolated. It never allocates a seat. `StudentEnrollmentsCard` reuses the same mutations and dialogs from the student's perspective, separates active and ended entries, and uses view permissions for links.

Verified source excerpt, `frontend/src/features/enrollments/useEnrollmentMutations.ts:81` (quoted directly from the current file):

```ts
    }) =>
      run(
        () => transferMutation.mutateAsync({ id: enrollmentId, data: { targetGroupId, version } }),
        [sourceGroupId, targetGroupId],
        studentId,
      ),

    isEnrolling: enrollMutation.isPending,
    isWithdrawing: withdrawMutation.isPending,
    isTransferring: transferMutation.isPending,
  };
}
```

#### Execution example

1. A secretary opens a group roster showing nineteen of twenty seats.
2. The enrollment dialog searches for a student and accepts a selection not already marked enrolled.
3. Another secretary takes the final seat before the first submit completes.
4. The first request receives `group.full`. The shared mutation helper refreshes roster, group detail, group lists and the student's enrollment list.
5. The dialog stays open and shows a specific localized refusal. The seat meter converges to twenty of twenty.
6. The user instead opens Transfer on another enrollment and selects an available group of the same subject.
7. One generated transfer request sends source enrollment id, target group id and version. Server success represents both lifecycle changes together.
8. Both rosters and group counts, plus the student's history, invalidate. The dialog reports success only after that outcome, rather than claiming completion after a preliminary withdrawal.

#### Why this project needs it

Capacity and enrollment are concurrent office work. UI counts cannot be treated as locks. The same operation is reachable from the roster and student page, so centralizing request execution and invalidation prevents those two entry points from drifting. Separate view/manage/link capabilities also avoid exposing broader student records merely because a teacher needs names on their own roster.

#### Alternatives

Optimistic updates can feel faster but need rollback and reconciliation for capacity races. Separate transfer requests require compensation and can violate atomic business expectations. A page reload after every operation would converge but lose local interaction state. Invalidating the whole client would be broad and could discard unrelated useful data. The following is an interpretation based on the implementation: the selected targeted invalidation balances correctness and reuse without pretending the client can predict transaction success.

#### Important details

Shared helpers convert thrown transport errors into typed outcomes; callers must inspect `ok`. `finally` invalidation can itself reject, so do not assume every possible infrastructure failure is already reduced to the outcome union. The full/archived disabled button is advice; backend checks remain necessary. Student and group links have their own permissions. Ended enrollment data is retained for history, so filtering it out of the table must not be mistaken for deleting it.

#### What I should understand before modifying this code

Keep transfer one request. Update the affected-query set whenever a new read depends on enrollment counts or status. Test the full target, duplicate enrollment, archived student/group, stale version and missing-resource paths. Ensure a teacher roster remains names-only and lacks management actions. Reuse the common mutation owner from new entry points instead of copying an enrollment request with a smaller invalidation set.

<a id="frontend-centre-time"></a>
## 6.50 Calendar dates, local times, instants and the centre's time zone

#### What it means

Three values that look like time are different domain concepts. A calendar date such as `2027-05-01` names a day and has no clock time or offset. A local time such as `18:00` names a wall-clock position and has no date. An instant such as a UTC timestamp names one point on the global timeline. A weekly schedule rule combines a weekday and local time; a materialized class session has actual start and end instants. Converting between them requires the centre's zone and the relevant date because daylight-saving rules can change the offset.

The browser's time zone describes the user's device. It does not necessarily describe the tutoring centre. A secretary traveling abroad must still see the centre's timetable, not a timetable silently shifted into the laptop's zone. Similarly, the centre's current date near midnight may differ from UTC's date and the device's date.

#### The problem it solves

Without explicit distinctions, a developer might use:

```ts
// Illustration only: these use the device's zone or conflate date-only data.
new Date(session.startsAt).toLocaleTimeString();
new Date('2027-05-01').getDate();
```

The first displays the device's wall clock. The second converts a date-shaped value into an instant-like Date and can shift the visible day under local accessors. Adding twenty-four hours to a local midnight can also behave unexpectedly across daylight-saving transitions. The project keeps pure date arithmetic separate from instant formatting and passes the centre zone explicitly when needed.

#### How it works

`frontend/src/features/schedule/centreTime.ts:15` formats an instant with `Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: timeZoneId })`. `formatTimeRange` formats two instants in that same zone. `todayIn` uses `formatToParts` with an explicit zone and an English numeric format to construct a stable `YYYY-MM-DD` calendar date; display locale is not allowed to change that protocol shape.

Local-time helpers validate `HH:mm`, convert it to minutes since midnight and add minutes with wraparound. `formatLocalTime` places a wall-clock value on a fixed UTC day and formats in UTC, deliberately preventing any real-world zone from moving it. `formatLocalTimeRange` moves the artificial end day forward if a wrapped end is earlier than its start, allowing a rule crossing midnight to display as a forward range.

`frontend/src/features/schedule/week.ts:12` sets Saturday as the week start. Its date arithmetic parses numeric parts and uses UTC day numbers with exactly 86,400,000 milliseconds per arithmetic day. UTC here is a computational device for date-only arithmetic, not an assertion that the centre's calendar operates in UTC. `isIsoDate` round-trips year, month and day to reject impossible dates such as February 30.

#### How THIS project implements it

`frontend/src/features/session/useCentreTimeZone.ts:10` finds the active centre's membership and reads its `timeZoneId`. It returns null before that information is available and never substitutes the browser zone. This lets any role use the centre calendar without reading owner-only settings.

`frontend/src/features/schedule/TimetablePage.tsx:53` calculates today with that zone, then chooses the URL's week or the current Saturday-start week. `TimetableGrid` groups returned sessions by the server's `localDate`, not by converting `startsAt` with device-local accessors. `SessionCard` formats start/end instants in the response's `timeZoneId`. These are complementary guarantees: the server chooses the centre day and the formatter chooses the centre wall clock.

`scheduleRules.ts` renders undated weekly start times through local-time helpers, not instant helpers. Its rule state compares ISO date strings: a past final date is ended; a non-null future or current final date is ending; otherwise a future first date is upcoming and the remaining case active. It sorts Saturday first and then by local start minute. The API's weekday ordering is not assumed to be the desired UI ordering.

Verified source excerpt, `frontend/src/features/schedule/centreTime.ts:15` (quoted directly from the current file):

```ts
  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: timeZoneId }).format(new Date(instant));
}

/** Two instants as a time range in `timeZoneId`, e.g. "6:00 – 7:30 PM". */
export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {
  return new Intl.DateTimeFormat(locale, { ...TimeOptions, timeZone: timeZoneId }).formatRange(new Date(startsAt), new Date(endsAt));
}

```

#### Execution example

1. A teacher opens `/timetable` from a device set to another time zone.
2. `useCentreTimeZone` reads the selected membership's IANA zone.
3. `todayIn` obtains the centre's date, then `startOfWeek` finds its Saturday without using device-local date accessors.
4. The generated timetable request sends that date as `weekStart`; filters remain ids and dates, not browser offsets.
5. The response contains session instants, local dates and the centre's zone.
6. `TimetableGrid` places each session in its `localDate` column. `SessionCard` formats its UTC start and end in the supplied zone.
7. Switching language changes text and number formatting but does not move the session or replace its instant.
8. Opening a group's weekly rule shows the undated local start time. A summer session and winter session can have different UTC starts while the rule still reads the same centre wall-clock time.

#### Why this project needs it

Tutoring schedules are usually promised in the centre's local wall clock: a class is at six in the evening, not at one fixed UTC hour all year. The frontend must preserve that model and must not independently generate sessions using its own device rules. The code comments and the centre-time tests explicitly teach this distinction, making it part of the current implementation rather than a hypothetical future concern.

#### Alternatives

A date library such as Luxon or Temporal-based utilities could model these values. Storing one fixed UTC recurrence would simplify conversion but change the business meaning across offset changes. Formatting everything in the device zone would be appropriate for a global video call application, not automatically for a local centre timetable. The repository states that these helpers use `Intl` and no date library. It does not provide a comprehensive library evaluation. The following is an interpretation based on the implementation: small pure helpers and explicit zone arguments make the limited required operations easy to test without a large abstraction layer.

#### Important details

An offset is not an IANA zone. ISO date string comparison is valid only for the validated fixed-width representation. Date parsing and display formatting have different goals: preserve protocol strings for requests and localize only when showing them. `Date.now()` compares instants and is suitable for a future-session button check, but does not determine a centre's calendar date. Some non-schedule timestamps elsewhere still use the device zone; the centre-time guarantee should be scoped to the code that actually supplies it.

#### What I should understand before modifying this code

Identify whether a field is a date, local time or instant. Use the corresponding helper instead of guessing from its string shape. Never generate the UTC schedule in a component. Test winter/summer offsets, midnight boundaries, cross-midnight durations, impossible dates and Saturday normalization. Keep `dir="ltr"` around ranges in Arabic UI. Do not let a missing zone silently fall back to the device's zone because that turns a visible missing-data state into a plausible wrong timetable.

<a id="frontend-schedule-actions"></a>
## 6.51 Weekly-rule forms, materialized sessions and schedule conflict recovery

#### What it means

A weekly rule describes repeated intent: a weekday, local start time, duration, optional room and first effective date. A class session is one generated occurrence with actual instants. Editing the rule lifecycle and cancelling one session are different actions. Ending a rule does not mean deleting its history; cancelling a session does not mean hiding the occurrence from the timetable.

The frontend treats scheduling conflicts as ordinary business outcomes. Two groups can compete for one teacher's time, or one group can be assigned overlapping sessions. The browser's current view cannot establish that a slot remains free, so it submits intent and displays the server's refusal if a conflict is detected.

#### The problem it solves

Without separate concepts a screen might send a teacher id, centre id and computed UTC offset with every rule, duplicating server authority and time-zone logic. Or it might delete a cancelled session from local state, making staff think it never existed. A naive manual recurrence loop would be particularly dangerous:

```ts
// Illustration only: not the implemented scheduling protocol.
for (let week = 0; week < 8; week++) {
  await createSession({ teacherId, startsAt: addFixedUtcWeek(start, week) });
}
```

That loop can partially succeed and applies the wrong abstraction across offset changes. The real form sends one weekly intent. The server generates sessions under its own rules and transaction boundary.

#### How it works

`frontend/src/features/schedule/scheduleSchemas.ts:37` validates weekday, twelve-hour input parts, five-minute minute steps, duration from thirty through 240 minutes in five-minute increments, room length at most forty and a real first date. The UI's duration choices offer quarter-hour increments; the schema accepts the broader five-minute contract. `toLocalTime` converts twelve-hour inputs to `HH:mm`, and `toAddScheduleRuleRequest` sends exactly weekday, start time, duration, room or null, and effective-from date.

`AddScheduleRuleDialog` maps server `startTime` field errors to the hour/time group. It defaults to four PM, ninety minutes, empty room and the centre's current date, but requires the user to choose a weekday. The effective-date input's upper bound is 366 days ahead, matching its declared server horizon mirror. Double-booking and archived-group refusals remain in the dialog. A successful response reports the number of materialized sessions.

#### How THIS project implements it

`frontend/src/features/schedule/useScheduleMutations.ts:25` owns four writes. `addRule` returns the server's session count as a number; `endRule` sends rule id, last date and version; `extend` sends a through date; `cancelSession` sends session id and version. Its `run` helper converts errors to a discriminated outcome and invalidates relevant rules plus all timetable-week queries in `finally`.

`ExtendScheduleDialog` limits the through date from today to ninety-two days ahead and suggests fifty-six days ahead. It validates the chosen date locally, then reports either newly created sessions or the translated up-to-date result. Teacher and group conflicts receive different explanatory text. The source comment mentions a future nightly job; the present frontend action is an explicit manual trigger and should not be described as a running browser background scheduler.

`CancelSessionDialog` confirms group, centre date and centre time, then sends the loaded session version. `TimetablePage` passes a cancel action only for a scheduled session whose start instant is still in the future and only with schedule-manage permission. `SessionCard` still renders cancelled entries with a written Cancelled badge and struck text. `GroupScheduleTab` shows rules to readers and adds add/end actions for managers.

Verified source excerpt, `frontend/src/features/schedule/useScheduleMutations.ts:47` (quoted directly from the current file):

```ts
  };

  return {
    addRule: (groupId: string, data: AddScheduleRuleRequest) =>
      run(async () => Number((await addMutation.mutateAsync({ groupId, data })).sessionsCreated), groupId),

    endRule: ({ ruleId, groupId, lastDate, version }: { ruleId: string; groupId: string; lastDate: string; version: number | string }) =>
      run(() => endMutation.mutateAsync({ id: ruleId, data: { lastDate, version } }), groupId),

    extend: (through: string) => run(async () => Number((await extendMutation.mutateAsync({ data: { through } })).sessionsCreated)),

    cancelSession: ({ sessionId, version }: { sessionId: string; version: number | string }) =>
      run(() => cancelMutation.mutateAsync({ id: sessionId, data: { version } })),
```

#### Execution example

1. An owner opens a group's Schedule tab. The parent group page already holds the group, and the tab queries its weekly rules.
2. The owner opens Add Weekly Session. The form receives group, current centre date and centre zone.
3. RHF and Zod validate the chosen weekday, time parts, duration, room and first date.
4. Mapping constructs a local-time request without teacher, centre or offset. The group relationship supplies the teacher on the server side.
5. The shared mutation sends one generated request. A teacher conflict returns a specific refusal; rules and timetable queries invalidate even on refusal.
6. The dialog remains open so the owner can choose another time rather than reconstructing all input.
7. After successful creation, a localized toast reports the returned count and the dialog closes. The timetable can now load materialized sessions.
8. Cancelling one future occurrence sends that occurrence's version. On success, refreshed timetable data retains it with cancelled status. The weekly rule is not silently erased by that single-session action.

#### Why this project needs it

The product now has recurring schedules and real session views. Time zones, overlapping work and concurrent staff make local guesses unsafe. Shared mutation invalidation also matters because a rule action from a group page can affect any currently cached week. Invalidating only the visible week's key would leave other navigable weeks stale.

#### Alternatives

A purely recurring calendar that computes occurrences on the client would avoid storing some sessions but move business logic into every reader and make single-session cancellation harder. Full optimistic insertion would require predicting server materialization, conflicts and time-zone resolution. Individual per-session creation would expose partial success unless wrapped by another server operation. The following is an interpretation based on the implementation: weekly intent plus materialized session DTOs keeps the browser a reader and action initiator while the server owns scheduling consistency.

#### Important details

Input constraints improve usability but do not reserve a slot. Five-minute schema acceptance differs from the fifteen-minute duration choices. The form's earliest and latest bounds are hints repeated by server checks, not an authorization boundary. Ending a rule, extending a horizon and cancelling one occurrence need separate invalidation analysis. A browser's current clock can make a cancellation button look available briefly even when the server has already crossed the start instant; the server refusal remains authoritative.

#### What I should understand before modifying this code

Keep local rule time and session instants separate. Do not add client-supplied tenant or teacher authority to a request that intentionally omits them. Preserve distinct group/teacher overlap messages. Retain cancelled sessions visually. Use the shared mutation owner from any new schedule entry point. Test horizon boundaries, stale versions, already-ended rules, already-cancelled sessions and a session becoming past while its confirmation is open.

<a id="frontend-shared-ui"></a>
## 6.52 Shared UI primitives, responsive tables and accessible state

#### What it means

A shared component can encapsulate a reusable interaction without owning business policy. The application has two layers of shared UI. `components/ui` contains headless-library wrappers and styled primitives such as buttons, dialogs, select, tabs, tooltip and radio controls. `components/shared` contains application-level conventions such as page headers, filter bars, empty states, status badges, phone text and responsive data tables.

Headless primitives supply interaction behavior and state while the application supplies styling. A dialog is more than an absolutely positioned div: it has open state, focus behavior, keyboard interaction, accessible naming and a portal. A shared business display such as `SeatMeter` can use primitives but remains in its feature because its eighty-percent threshold and capacity language are enrollment concepts.

#### The problem it solves

Without shared primitives every screen reinvents menu and dialog markup. Without application wrappers, every list invents different loading and empty states, physical spacing and mobile overflow behavior. A narrow phone screen can become unusable if a desktop table only scrolls horizontally and action buttons drift away from the relevant name.

```tsx
// Illustration only: an unstructured clickable div lacks button semantics.
<div onClick={onArchive}>Archive</div>
```

The actual code uses buttons, named menus, confirmation dialogs and written state labels. Colors and icons repeat state; they are not intended to be the only information.

#### How it works

`frontend/src/components/shared/DataTable.tsx:10` wraps a semantic table and exports row, header and cell helpers. It keeps explicit table roles while CSS changes mobile display. Cells carry `data-label` for their mobile label and `data-cell` for primary or action placement. The stylesheet's `.data-table` rules render stacked cards below the desktop breakpoint. On larger screens the wrapper allows horizontal overflow when a table genuinely needs width.

`StatusBadge` maps active/archived to translated words and variants. `PhoneInput` uses `type="tel"` and `dir="ltr"` without inventing phone-format rules. `PhoneText` chooses an isolated span or a `tel:` link. `PageHeader`, `FilterBar`, `StatTile` and `EmptyState` provide consistent composition points, leaving each feature to supply title, description, actions and content.

#### How THIS project implements it

`frontend/src/features/shell/AppShell.tsx:29` uses a Base UI dialog for mobile navigation and a menu for user actions. Desktop navigation is a persistent sidebar. The same translated links are reused in both, and navigation closes the mobile dialog. The shell reads route static data through `useMatches`; timetable sets `layout: "wide"` while ordinary pages use a narrower reading column. This is one shared shell policy rather than a timetable-specific CSS selector reaching into its parent.

Subject, student, staff, teacher and roster lists use the shared table helpers. `SeatMeter` supplies both count text and a badge. Loading states use skeletons with `role="status"` and an accessible label. Failure states use alerts and explicit Retry buttons. Group detail distinguishes a uniform not-found state from a transport failure that can be retried. A missing or foreign group should not be explained differently in the UI if the server intentionally returns the same 404.

Verified source excerpt, `frontend/src/components/shared/DataTable.tsx:10` (quoted directly from the current file):

```tsx
  return (
    <div className="rounded-xl border border-border bg-card sm:overflow-x-auto">
      <table role="table" className={cn("data-table w-full text-start text-sm", className)}>
        {children}
      </table>
    </div>
  );
}

```

#### Execution example

1. A staff member opens a student list on a phone.
2. The route supplies search state; the list query loads and a labeled skeleton appears.
3. The response renders `DataTable` rows. Each cell provides its column label for the mobile representation.
4. CSS presents each row as a card, with name and actions on its first line and labeled values beneath.
5. A phone is rendered LTR even in Arabic. Archived status includes a written badge, not only a muted color.
6. The user opens row actions. The headless menu manages its interaction while feature code supplies the actions allowed by permissions.
7. An archive action opens a named confirmation; the handler waits for the server, invalidates data and reports the outcome.
8. On desktop the same semantic row data becomes a conventional table without a second list implementation.

#### Why this project needs it

Office staff may use phones as well as wider screens. Repeated list-and-dialog patterns now span several features. Shared semantics and state presentation reduce inconsistencies, while keeping authorization and domain decisions in features avoids turning a generic table into an application-wide policy engine.

#### Alternatives

A large packaged design system could own more styling and behavior but reduce source control over the primitives. Feature-specific markup everywhere offers freedom at a maintenance cost. A generic CRUD framework could generate forms and tables but obscure domain-specific actions such as transfer and schedule ending. The repository chooses copied/styled component source and Base UI-backed controls. The following is an interpretation based on the implementation: this balances explicit ownership of visual code with reuse of difficult interaction mechanics.

#### Important details

Accessible labels and translation text are part of browser-test selectors, so changing them changes test expectations. A disabled control should explain why when the reason matters, as full/archived enrollment buttons do. Not all inline styles are absent: `SeatMeter` sets computed inline size, and the frontend README already discusses an inline-style allowance for runtime styling. A source comment promising no inline style anywhere would be stronger than the implementation. Responsive CSS still requires visual checks; semantic roles and unit tests cannot prove every layout fits.

#### What I should understand before modifying this code

Choose the correct layer: primitive, shared application display or feature component. Preserve button semantics, labels, focusable controls and written status. Check both mobile and desktop, both directions and long names. Add layout metadata through the existing static-data mechanism when a route needs width. Avoid copying shared table markup into a new feature unless its data presentation is actually different.

<a id="frontend-status-dashboard"></a>
## 6.53 Status, demo visibility, audit presentation and derived dashboard data

#### What it means

A dashboard can derive useful summaries from returned data without becoming a new source of truth. A status page answers a different question: whether the API can answer and whether its dependencies are healthy. An audit screen presents historical changes rather than editable current entities. Demo mode is deployment metadata displayed globally so a visitor understands the environment's purpose.

These screens demonstrate several distinct kinds of failure. A readiness response of 503 is a meaningful answer that the process is available but its dependency is not. A network error means the page did not obtain a trustworthy answer. An empty audit result is not an error. A missing demo flag is not proof that a request failed; the banner simply stays absent until the server says it is true.

#### The problem it solves

Without clear distinctions, a status page can say the API is unreachable when the API has explicitly reported a database outage. A dashboard might hard-code totals or sum a different client-maintained list. An audit display might expose raw enum values or mask already-masked phone text inconsistently. A demo warning placed in just one route can disappear when a visitor opens login directly.

```ts
// Illustration only: this discards a useful dependency-state answer.
if (!response.ok) throw new Error('API unreachable');
```

The actual readiness adapter handles 200 and 503 separately; the resulting discriminated data drives different UI branches.

#### How it works

`frontend/src/features/status/api.ts:12` requests `/health/ready` directly because this health contract uses status semantics rather than the generated JSON API protocol. A 200 becomes `{ state: "healthy", checkedAt }`; a 503 becomes unavailable data; other statuses throw. `useReadiness` caches under `['health', 'ready']` and polls every fifteen seconds. `StatusCard` renders pending, unreachable, database-unavailable and healthy branches. Retry explicitly calls `refetch`.

`SystemInfoCard` uses generated `useGetSystemInfo`, rendering application version, latest migration and whether the database is up to date. Technical strings remain LTR. `DemoBanner` uses that same generated query and renders only when `demoMode` is true. The root route mounts it before its outlet, so it is present for public and protected routes without repeating it in each page.

#### How THIS project implements it

`frontend/src/features/dashboard/DashboardOverview.tsx:105` filters returned groups to active ones. It sums returned enrollment counts and capacities, counts full groups and selects at most five groups at or above eighty-percent occupancy, sorted by occupancy then name. Quick links are independently gated by students-manage, students-view and groups-view permissions. `GroupsOverview` mounts only when its parent allows group viewing; a teacher sees a summary of whatever rows the server returns for that teacher's scope.

`frontend/src/features/audit/AuditLog.tsx:115` formats each audit entry with actor display name or System, translated action and entity type, timestamp, changed fields and optional correlation-reference copy. `auditFormatting.ts` contains the explicit vocabulary mapping. It does not infer a user's current role from historical text or turn every string into a translation key. The entity filter includes subject, membership, centre, student, parent, student_parent, teacher, group, enrollment, schedule_rule and class_session.

Verified source excerpt, `frontend/src/features/status/api.ts:12` (quoted directly from the current file):

```ts
  const response = await fetch("/health/ready");

  if (response.status === 200) {
    return { state: "healthy", checkedAt: new Date() };
  }

  if (response.status === 503) {
    return { state: "unavailable", checkedAt: new Date() };
  }

  throw new Error(`Unexpected readiness status ${String(response.status)}.`);
}
```

#### Execution example

1. A visitor opens login. The root route mounts `DemoBanner` and starts or shares the system-info query.
2. If the response says demo mode true, the non-dismissible translated warning appears above login.
3. A staff member later opens `/status`; readiness polling and the system-info card share their separate query contracts.
4. Database outage returns 503. The readiness adapter resolves unavailable data, so the UI can accurately distinguish it from no network response.
5. An owner opens the dashboard. The group-list response produces seat tiles and near-full groups using current server counts.
6. After an enrollment mutation invalidates group lists, dashboard subscriptions can refetch those counts.
7. The owner opens audit with an entity filter. Its infinite query loads matching change entries.
8. An entry's role or status values are translated through the audit formatter; its correlation id can be copied to locate the relevant server request.

#### Why this project needs it

The product now includes observable deployment state and operational data rather than only a placeholder dashboard. Derived counts avoid a separate manual dashboard model, while audit presentation connects business changes to server diagnostics. Global demo visibility is particularly useful for a portfolio/demo deployment where users must recognize that the environment is not an ordinary production centre.

#### Alternatives

A dedicated dashboard endpoint could aggregate on the server and avoid relying on list completeness. A monitoring service could own readiness display outside the product. A server-rendered banner could show demo state before JavaScript loads. Audit could display raw JSON, but would be less usable for ordinary staff. The following is an interpretation based on the implementation: current screens favor reuse of existing APIs and shared UI while keeping diagnostic details available through correlation references.

#### Important details

Dashboard totals cover the items returned by `useListGroups`; verify list completeness before calling them unrestricted global totals if pagination is added later. Readiness's `checkedAt` is browser observation time, not a database event timestamp. `StatusCard` uses `toLocaleTimeString` for that local observation, so it is intentionally different from centre-schedule formatting. Audit date formatting currently has no explicit centre zone. A banner failing to render because system info failed does not prove demo mode is false.

#### What I should understand before modifying this code

Keep 503 as meaningful readiness data. Preserve the global root placement for demo warning. State precisely which returned rows a dashboard metric summarizes. Translate code-defined historical vocabulary while rendering user strings as plain text. Do not treat audit history as an editable entity form. Preserve correlation-reference visibility and isolation when changing error or audit layouts.

<a id="frontend-testing"></a>
## 6.54 Frontend testing: network-level doubles, route integration and real browsers

#### What it means

A component test renders real UI code in a simulated browser environment and supplies controlled outside behavior. A route test also runs the real route tree and navigation. An end-to-end test runs a real browser against real servers. These tests answer different questions; a mock API can prove that a form handles a 409 but cannot prove that a real browser stores the API's encrypted cookie through the configured proxy.

MSW intercepts network requests so tests can exercise the real generated client and `apiFetch`. It is more representative than mocking a hook to return arbitrary flags because it leaves serialization, CSRF header handling and failure mapping in the path. jsdom supplies DOM APIs for unit/component tests but is not a complete browser layout engine. Playwright controls Chromium for the system journeys.

#### The problem it solves

Mocking a feature hook can make a screen test pass even when its real request URL or error conversion is wrong:

```ts
// Illustration only: a hook-level stub bypasses the protocol boundary.
mockUseStudents.mockReturnValue({ data: sample, isPending: false });
```

The repository normally overrides an MSW route instead. The component, generated function, fetch wrapper and request-building code still run. Browser journeys then cover the remaining seams: actual cookie rules, proxy forwarding, first-login changes, permissions and persisted data shared across screens.

#### How it works

`frontend/vite.config.ts:42` configures Vitest with jsdom and `src/test/setup.ts`, excluding `e2e/**` so Playwright specs are not mistaken for unit tests. `setup.ts` imports real i18n, stubs missing `matchMedia` behavior needed by the toaster and starts MSW with `onUnhandledRequest: "error"`. After each test it unmounts components and resets per-test handlers; after all tests it closes interception.

`frontend/src/test/render.tsx:6` creates a new `QueryClient` for each helper invocation and disables retries. This prevents one component test's cached data from satisfying another's request and makes expected failures appear promptly. `renderRouter.tsx:12` goes further: it creates the application's real error-wired client, memory history and actual generated route tree, and connects login redirects to that test's router. A global 401 test therefore executes the same cache-clearing/redirect function as the app, rather than a hand-written imitation.

#### How THIS project implements it

The current Vitest suite contains forty-three test files and 278 executed tests in the successful bounded-worker run. Tests cover API mapping, session and route gates, password change, language direction, shared phone/search/status controls, subject/staff/settings/audit pages, student/parent pages, teacher/group pages, enrollment actions, scheduling, calendar helpers, demo banner and dashboard. Pure helpers such as `week`, `centreTime`, schema validation and bidi isolation have focused tests alongside route-level UI tests.

`frontend/src/test/selectOption.ts:10` shows why test events must resemble the component's real interaction. Base UI selection needs pointerdown before click on an option; dispatching only a synthetic click can leave the selection unchanged. The helper opens the labeled combobox, finds its option, sends pointerdown and then click. This is an interaction adaptation, not bypassing the component by setting its internal state.

Playwright configuration at `frontend/playwright.config.ts:17` uses one Chromium project, isolated contexts, fully parallel tests, an HTML reporter and one retry in CI. Its web servers start the API's HTTPS profile and Vite. Readiness probes the HTTP sibling URL, while proxy traffic reaches the API over HTTPS because antiforgery checks the actual backend request's security state.

Nineteen browser test declarations exist across ten spec files. They cover authentication, subject lifecycle/centre switching, first login, restricted staff access, staff role audit, students and siblings, teachers/groups, enrollment, scheduling and a full Arabic centre setup. The first-login and full-centre-setup specs disable trace and video explicitly because they read a temporary password from the screen. That is an actual recording choice in those files, not an inferred property of every test artifact.

Verified source excerpt, `frontend/src/test/setup.ts:25` (quoted directly from the current file):

```ts

afterEach(() => {
  cleanup(); // unmount rendered components (Vitest globals are off, so Testing Library can't do it automatically)
  server.resetHandlers(); // drop per-test overrides
});

afterAll(() => {
  server.close();
});
```

#### Execution example

1. A roster component test installs an MSW handler for a particular group response.
2. `renderWithQueryClient` mounts the real component under a fresh cache.
3. The generated roster hook runs its real URL builder and mutator. MSW intercepts that network request and supplies the fixture.
4. The test asserts a loading state, then the visible roster and permissions-based controls.
5. A mutation override answers `group.full`. The test can assert the dialog stays open and refreshed counts appear, exercising the real error mapping.
6. A browser enrollment journey instead signs in through the real form, creates or selects groups and drives the actual roster actions against the API and database.
7. A teacher context opens the same group view and the spec asserts names-only read access with no management controls.
8. The browser and mocked tests together cover presentation and integration; neither alone proves every server concurrency guarantee, which belongs in backend tests too.

#### Why this project needs it

The expanded frontend now makes business changes rather than only displaying status. Correct forms are insufficient if cookie and permission wiring is broken. Conversely, expensive browser journeys are a poor place to exhaustively test every date helper or field error. The repository keeps those focused cases in Vitest and uses Playwright for cross-layer actions that need real integration.

#### Alternatives

Cypress is another browser tool. Testing hooks through mocks is fast but skips the transport seam. A full browser for every unit case is slower and harder to isolate. Snapshot-only tests can miss whether a button sent the right version or whether a forbidden page issued a request before hiding its content. The following is an interpretation based on the implementation: MSW plus the real route tree gives relatively broad integration coverage without needing the backend for each component case.

#### Important details

Actual check results from this session matter more than historical counts. Typecheck passed, production build passed, key parity passed for fifteen namespaces and ESLint exited successfully with zero errors and forty-two fast-refresh warnings. The first sandboxed default Vitest run hit temporary-file ENOENT errors. An unrestricted default-worker run completed with twenty-four failures, mostly timeouts, and 254 passes. A repeat using `npm run test:ci -- --maxWorkers=2` passed all forty-three files and 278 tests in 127.06 seconds. That difference is evidence of environment/concurrency sensitivity, not proof that the first twenty-four failures were product bugs. jsdom still printed unimplemented `scrollTo` warnings. Browser journeys were read here; their execution result must be reported separately by the process that actually runs the full system.

#### What I should understand before modifying this code

Choose the smallest test layer that can observe the behavior. Mock the network for component behavior, not the feature hook. Use the real route tree for guards and navigation. Keep each test cache fresh and restore MSW handlers. Do not interpret key parity as visual RTL verification or a passed build as runtime correctness. Keep secrets out of browser recordings, preserve isolated contexts for different users and distinguish a failed environmental startup from a failed business assertion when reporting results.

<a id="frontend-system-journey"></a>
## 6.55 A complete frontend workflow: establishing a centre and teaching a class

#### What it means

A feature is not fully understood by recognizing its filename. Its meaning appears when one operation's output becomes the next operation's input. The full-centre browser spec is useful because it connects subjects, identity, teacher records, groups, parents, students, enrollment and schedules into one office workflow. This section explains that chain as frontend orchestration. It does not claim that reading a test proves the entire journey passed in this environment.

The critical ownership rule is that the UI initiates named server operations and uses returned ids and versions. A subject name is not used as a database identity. A staff account is not automatically a teacher record. A weekly rule is not a set of timestamps calculated in the component. Each link is explicit.

#### The problem it solves

Without following the chain, onboarding can stop at labels such as CRUD, permissions and forms without showing why the files exist. A developer might create a teacher account and wonder why it has no groups, or create a weekly rule and assume the timetable reads that rule directly. The workflow reveals the actual relationships and the boundaries that keep these operations separate.

```ts
// Illustration only: these concepts are deliberately not one interchangeable record.
const teacherRecordId = staffUserId; // invalid assumption about identity ownership
```

The implemented flow creates staff identity, creates a teacher business record, then explicitly links them before expecting teacher-scoped access.

#### How it works

`frontend/e2e/full-centre-setup.spec.ts:67` names the journey and uses a 180-second per-test timeout. It signs in an owner of the seeded empty Giza centre, with Arabic browser locale and unique names per run. It computes a future Saturday in `Africa/Cairo`, not the test machine's local zone. This makes the timetable assertions about wholly future centre sessions.

The owner first creates a subject, then a staff member. The staff dialog reveals a temporary password. A separate browser context signs in that member, completes the forced password change and selects its centre. Meanwhile the owner creates a teacher record and links it to that account. This is a real separation between identity lifecycle and teaching identity, represented in UI actions rather than an undocumented setup script.

#### How THIS project implements it

The owner creates a two-seat group with selected subject and teacher. `CreateGroupDialog` sends four fields and navigates with the returned id. Register Student creates the first student with a new parent; registering the second with the same phone uses lookup and sends the existing parent id. Both students are enrolled through the group's roster dialog. Its returned counts produce Full state and a disabled enrollment affordance.

The owner adds Saturday and Tuesday weekly rules at eighteen hundred for ninety minutes. The forms submit local start time and the centre's first date, leaving materialization to the server. Navigating to the future week's timetable displays two sessions in centre time. The teacher's separate context then reads groups, a names-only roster and those sessions, while create/manage controls stay absent according to returned permissions.

The final audit check searches entries since the journey started for one teacher-link update, two enrollment creations and two weekly-rule creations. One stale source comment says the filter offers no entity type for those records, while `AuditEntityTypes` now explicitly contains teacher, enrollment, schedule_rule and class_session. The code's actual filter list is authoritative. This illustrates why even test comments need verification against current implementation.

Verified source excerpt, `frontend/e2e/full-centre-setup.spec.ts:220` (quoted directly from the current file):

```ts
    await expect(roster.getByText(firstStudent)).toBeVisible();
    await expect(roster.getByText(secondStudent)).toBeVisible();
    await expect(roster.getByRole("row")).toHaveCount(3);
    await expect(roster.getByRole("link")).toHaveCount(0);

    await teacherPage.getByRole("navigation").getByRole("link", { name: "جدول الحصص" }).click();
    await expect(teacherPage.getByRole("heading", { name: "جدول الحصص" })).toBeVisible();
    await teacherPage.goto(`/timetable?week=${nextWeek}`);
```

#### Execution example

1. Subject creation returns an id used in the group select; display text remains Arabic user data.
2. Staff creation returns a one-time credential or an existing-account outcome. The new identity uses the password-change route before ordinary work.
3. Teacher creation returns a separate record id. Account linking establishes which signed-in user represents it.
4. Group creation combines subject id, teacher id, name and capacity, then opens its overview.
5. Student registration creates a parent once and reuses that parent's id for the sibling branch.
6. Two enroll operations add the students; shared invalidation refreshes seats, rosters and student histories.
7. Two schedule-rule operations generate actual sessions. Timetable reading uses returned local dates and explicit zone formatting.
8. The teacher opens the same components with fewer server-returned rows and fewer permissions. The audit page shows historical operations with translated vocabulary and retained correlation information.

#### Why this project needs it

This chain demonstrates that current functionality is substantially beyond the v2 foundation. It also gives a new contributor a practical reading path: follow an actual record id from creation through its relationships and read model. The full setup journey intentionally uses an empty seeded centre so its workflow does not depend on the unrelated populated demo data of other centres.

#### Alternatives

A backend seed-only demonstration could prepopulate all relationships, but would not prove staff can establish them through the product. A single enormous setup form would reduce navigation but conflate independent permissions and lifecycle operations. Separate test-only APIs could create fixtures quickly, but would not exercise user-facing forms and their translations. The following is an interpretation based on the implementation: the full UI journey complements focused tests by checking that the product's separate modules form a usable office workflow.

#### Important details

The browser spec has unique data and a separate teacher context, but it still depends on seeded Giza ownership and a matching `SEED_PASSWORD`. Its temporary credentials require trace/video suppression. A test's expected visible text is evidence of intended behavior, while current component code establishes implementation and a run establishes execution. Do not merge those evidence levels. The journey does not cover attendance marking, payment recording or WhatsApp messaging merely because login's decorative marketing panel mentions them.

#### What I should understand before modifying this code

Read this journey alongside the real components before changing cross-feature shapes. Keep account and teacher ids distinct. Preserve sibling lookup, permission-scoped teacher reads, local-time rule submission and materialized timetable reading. Update integration selectors when intentionally changing accessible names. When adding a feature, connect its output to the next real operation rather than adding a disconnected screen that only looks complete.

### Frontend verification record for integration into the overview

| Command | Observed result |
| --- | --- |
| `npm run typecheck` | Exit 0. |
| `npm run build` | Exit 0; Vite 8.3.2 transformed 2,695 modules and emitted production assets. |
| `npm run i18n:check` | Exit 0; fifteen namespaces, matching Arabic/English keys. |
| `npm run lint` | Exit 0; zero errors, forty-two `react-refresh/only-export-components` warnings in route files. |
| `npm run test:ci` in the initial sandbox | Exit 1; temporary-file ENOENT errors prevented forty-two suites from loading and one executed suite had a timeout. |
| `npm run test:ci` outside the sandbox | Exit 1; 254 passed and twenty-four failed, predominantly timeout-related, across forty-three files. |
| `npm run test:ci -- --maxWorkers=2` outside the sandbox | Exit 0; forty-three files and 278 tests passed in 127.06 seconds. jsdom `scrollTo` warnings remained. |

The successful bounded-worker run supersedes a claim that those earlier timeout failures are established product regressions; it does not erase the fact that default parallel execution was unreliable in this environment. No frontend source file was changed for this investigation. All frontend project-owned files were opened by the ledger reader, which recorded hashes and structural extracts in `docs/onboarding-work/frontend-ledger.md`; substantive behavior above was checked against the named source files and tests.
