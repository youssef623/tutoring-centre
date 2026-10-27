# `docs/onboarding-work/frontend.md`

## Purpose

Documentation covering 6.40 React, component ownership, server state and the browser entry point; 6.41 OpenAPI, Orval and the contract-derived API client; 6.42 The fetch boundary, typed failures and CSRF recovery; 6.43 Sessions, route guards, first-login gates and permission-aware rendering; 6.44 React Hook Form, Zod and field-level server errors. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/frontend.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 1269 lines and 120,242 bytes. Source hash: `4bd8da12031c774f0c2b446d7f6a255311841767ba20b25d727bac7835d2ffb3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../onboarding-work/frontend.md#L8): `A browser starts with an HTML document. React lets the application describe the changing part of that document as a tree of components. A component is a function that receives inputs, called props, and returns a description of the UI. JSX is the syntax used to write that description; it is transformed by the build tools into JavaScript. Returning JSX does not immediately issue a request or update an element. React renders the component tree, compares its next description with its previous description, then commits the necessary changes to the browser DOM.`
- [Line 120](../../../onboarding-work/frontend.md#L120): `frontend/src/features/students/StudentsList.tsx:5 imports listStudents, getListStudentsQueryKey and generated parameter/DTO types. The component needs infinite pagination rather than one page, so it uses the generated plain request function inside a hand-written useInfiniteQuery. Generation is therefore a reusable protocol layer, not a requirement to use one particular generated hook for every presentation.`
- [Line 163](../../../onboarding-work/frontend.md#L163): `The generated ProblemDetails interface is distinct from the hand-written runtime isProblemDetails guard. Neither the generated enum-like permissions mirror nor the hand-written ErrorKind is automatically synchronized just because generation exists. features/session/permissions.ts explicitly describes its keys as a hand-maintained backend mirror. Generated file size is not evidence that every operation has a UI. Conversely, a feature can use the plain generated function even when it does not use a generated hook.`
- [Line 167](../../../onboarding-work/frontend.md#L167): `Change the server contract first, rebuild its document, regenerate, then modify consumers. Never hand-edit generated request paths. Use generated key helpers for invalidation. Search both hook and plain-function usages before declaring an endpoint unused. When interpreting a test fixture, compare its actual object with the current generated DTO; older fixtures can omit newly added information even if a permissive mock still returns them.`
- [Line 209](../../../onboarding-work/frontend.md#L209): `export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {`
- [Line 281](../../../onboarding-work/frontend.md#L281): `frontend/src/features/session/meQueryOptions.ts:10 defines one session query. Its function calls generated getMe; a 401 resolves to null, while other failures throw. Signed out is data, not a transport error. retry: false avoids retrying a meaningful unauthenticated result. useSession exposes that data and its loading/error state.`
- [Line 366](../../../onboarding-work/frontend.md#L366): `The schema provides both runtime validation and an inferred TypeScript type. z.infer<typeof schema> prevents a separately handwritten form-values interface from drifting from validation. Translation keys are stored as schema error messages. Local errors are translated when rendered; errors marked type: "server" are already display text and are rendered as supplied by the feature's mapping.`
- [Line 374](../../../onboarding-work/frontend.md#L374): `frontend/src/features/groups/CreateGroupDialog.tsx:49 uses a typed schema and default values. Its submit function sends exactly name, subject id, teacher id and capacity. A subject-archived or subject-not-found code lands under subjectId and invalidates subject choices; corresponding teacher failures land under teacherId. Other field errors use the translation-aware mapper. A successful response invalidates groups, closes the dialog and navigates to the created group's overview.`
- [Line 381](../../../onboarding-work/frontend.md#L381): `export const changePasswordFormSchema = z`
- [Line 463](../../../onboarding-work/frontend.md#L463): `export function isolateLtr(value: string): string {`
- [Line 548](../../../onboarding-work/frontend.md#L548): `7. Clicking Load More calls fetchNextPage; its query function sends the returned cursor with unchanged filters.`
- [Line 553](../../../onboarding-work/frontend.md#L553): `Student and audit collections can grow beyond a convenient single response. Office staff also need to return to searches and share precise views. The separation keeps browsing behavior coherent while the backend owns filtering, ordering and continuation semantics. The same design is reused for a smaller student picker with a different page size rather than downloading every contact record into the enrollment dialog.`
- [Line 650](../../../onboarding-work/frontend.md#L650): `Archiving is a lifecycle transition, not deletion. The application retains records and relationships while ordinary lists default to active items. Restore is another transition. Teacher records are also separate from staff accounts: a named teacher can exist as a business record, and an optional link connects that record to an eligible sign-in identity.`
- [Line 654](../../../onboarding-work/frontend.md#L654): `Without a version, two staff members can both read a group capacity of twenty, independently edit it, and overwrite one another without noticing. Without an archive lifecycle, removing a teacher could destroy references needed for group and schedule history. Without a separate teacher/account boundary, every teacher record would be forced to possess login credentials even when the office only needs to assign their name to a group.`
- [Line 665](../../../onboarding-work/frontend.md#L665): `frontend/src/features/subjects/SubjectsList.tsx:70 creates an invalidation function for all subject-list variants. Archive confirmation sends the target's id and version. Both success and failure invalidate the list; success reports completion, while failure reports its refusal and refreshes session permissions if required. Restore tracks which row is pending and clears that local state in finally.`
- [Line 706](../../../onboarding-work/frontend.md#L706): `Staff share mutable office data. Capacity, teacher assignment and membership all affect later enrollment or schedule behavior. Explicit action requests and version-aware refusals are easier to recover from than a generic save of an entire stale entity. Separating record identity from sign-in identity also matches the practical fact that a centre can track a teacher before granting them account access.`
- [Line 804](../../../onboarding-work/frontend.md#L804): `Three values that look like time are different domain concepts. A calendar date such as 2027-05-01 names a day and has no clock time or offset. A local time such as 18:00 names a wall-clock position and has no date. An instant such as a UTC timestamp names one point on the global timeline. A weekly schedule rule combines a weekday and local time; a materialized class session has actual start and end instants. Converting between them requires the centre's zone and the relevant date because daylight-saving rules can change the offset.`
- [Line 843](../../../onboarding-work/frontend.md#L843): `export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {`
- [Line 862](../../../onboarding-work/frontend.md#L862): `Tutoring schedules are usually promised in the centre's local wall clock: a class is at six in the evening, not at one fixed UTC hour all year. The frontend must preserve that model and must not independently generate sessions using its own device rules. The code comments and the centre-time tests explicitly teach this distinction, making it part of the current implementation rather than a hypothetical future concern.`
- [Line 881](../../../onboarding-work/frontend.md#L881): `A weekly rule describes repeated intent: a weekday, local start time, duration, optional room and first effective date. A class session is one generated occurrence with actual instants. Editing the rule lifecycle and cancelling one session are different actions. Ending a rule does not mean deleting its history; cancelling a session does not mean hiding the occurrence from the timetable.`
- [Line 1041](../../../onboarding-work/frontend.md#L1041): `Without clear distinctions, a status page can say the API is unreachable when the API has explicitly reported a database outage. A dashboard might hard-code totals or sum a different client-maintained list. An audit display might expose raw enum values or mask already-masked phone text inconsistently. A demo warning placed in just one route can disappear when a visitor opens login directly.`
- [Line 1130](../../../onboarding-work/frontend.md#L1130): `frontend/src/test/render.tsx:6 creates a new QueryClient for each helper invocation and disables retries. This prevents one component test's cached data from satisfying another's request and makes expected failures appear promptly. renderRouter.tsx:12 goes further: it creates the application's real error-wired client, memory history and actual generated route tree, and connects login redirects to that test's router. A global 401 test therefore executes the same cache-clearing/redirect function as the app, rather than a hand-written imitation.`
- [Line 1207](../../../onboarding-work/frontend.md#L1207): `The owner first creates a subject, then a staff member. The staff dialog reveals a temporary password. A separate browser context signs in that member, completes the forced password change and selects its centre. Meanwhile the owner creates a teacher record and links it to that account. This is a real separation between identity lifecycle and teaching identity, represented in UI actions rather than an undocumented setup script.`
- [Line 1234](../../../onboarding-work/frontend.md#L1234): `3. Teacher creation returns a separate record id. Account linking establishes which signed-in user represents it.`
- [Line 1243](../../../onboarding-work/frontend.md#L1243): `This chain demonstrates that current functionality is substantially beyond the v2 foundation. It also gives a new contributor a practical reading path: follow an actual record id from creation through its relationships and read model. The full setup journey intentionally uses an empty seeded centre so its workflow does not depend on the unrelated populated demo data of other centres.`
- [Line 1257](../../../onboarding-work/frontend.md#L1257): `### Frontend verification record for integration into the overview`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
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
````

The excerpt stops at line 24; use the original file for the remaining 1245 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
