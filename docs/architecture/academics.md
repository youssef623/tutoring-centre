# Academics: students and parents (Days 37–42)

The Domain model behind registering a student and the parents a centre contacts about them, how that model is
stored, the use cases and endpoints built on it, and the two screens that use them. Written from the
implementation; every name and rule below is checked against the code, not the plan that produced it.

## Entities

| Type | Kind | Tenant-owned | Lives in |
| --- | --- | --- | --- |
| `PhoneNumber` | value object | — | `TutoringCentre.Domain.Common` |
| `SearchText` | pure function | — | `TutoringCentre.Domain.Common` |
| `Parent` | aggregate root | yes | `TutoringCentre.Domain.Academics` |
| `Student` | aggregate root | yes | `TutoringCentre.Domain.Academics` |
| `StudentParentLink` | child entity of `Student` | yes | `TutoringCentre.Domain.Academics` |

`PhoneNumber` and `SearchText` sit in `Common` rather than `Academics` because both are used by more than one
future aggregate (`Teacher`, Day 43) — the same reasoning `docs/architecture/overview.md` gives for the
Domain/Application `Common` namespaces being the shared kernel, not a fifth module.

## Why `Parent` is a separate aggregate, not part of `Student`

A parent is edited on its own (a phone number changes; a name is corrected) and is shared across siblings —
two `Student` rows can name the same parent. Nesting `Parent` inside `Student` would mean one student's save
could touch a row another student's aggregate also claims to own, and editing a parent's phone would require
loading and re-saving every student that references them. `Student` therefore holds only a `ParentId`
(`Guid`), never a `Parent` reference or navigation property — the two aggregates communicate by identity, the
same pattern tenant isolation's Layer 4 relies on for cross-table references (`docs/architecture/tenancy.md`).
Nothing in `Student` or `Parent` knows whether a given `ParentId` actually exists in the centre; that check is
an Application-layer concern (Day 39), not a Domain rule — the Domain only knows the shape of a valid link,
not whether the other side of it is real.

## Why `StudentParentLink` is not a separate aggregate

Unlike `Parent`, a link has no identity or lifecycle of interest outside the one invariant below: it is
created, reassigned as primary, or removed only as a side effect of keeping that invariant true. Letting
something outside `Student` create or mutate a link would mean the invariant could be broken by code that
never consulted it. `StudentParentLink`'s constructor and its two primary-flag mutators
(`MarkPrimary`/`ClearPrimary`) are `internal` — reachable from anywhere in the Domain assembly, which is as
tight as C# access modifiers get without a second assembly, and tight enough that no Application or
Infrastructure code can construct or change one directly.

## The parent-link invariant

A `Student` always has:

- between 1 and 4 parent links (`Student.MinParents`, `Student.MaxParents`);
- never two links to the same `ParentId`;
- exactly one link with `IsPrimary == true`.

Every operation on `Student` — `Register`, `AddParent`, `RemoveParent`, `SetPrimaryParent`, `Archive`,
`Restore`, `UpdateDetails` — either leaves this true or fails and changes nothing; there is no path that
leaves a student with zero parents, two primaries, or a duplicate link. `StudentTests` asserts the invariant
itself, not just each operation's own effect, after every call including every failure.

`LinksRevision` increases by exactly one on every successful `AddParent`, `RemoveParent` and
`SetPrimaryParent`, and never on a failure or on `Archive`/`Restore`/`UpdateDetails`. Its purpose is Day 38's:
a parent link lives in a child table, and the optimistic-concurrency row version Month 2 put on the aggregate
root's own row (`xmin`) has nothing to say about a child row changing underneath it. Bumping a counter on the
root every time a child changes gives the root's row version a reason to change too, so a stale read of the
student (with its cached link count or primary parent) is caught the same way any other stale write already
is.

## The phone format

`PhoneNumber.Create(input)` accepts a wide range of spellings and reduces every one of them to canonical
E.164, or fails with `phone.invalid`. Accepted forms, exactly as pinned in `PhoneNumberTests`:

- a local Egyptian mobile number, any punctuation, Western or Arabic-Indic digits — `01012345678`,
  `010 1234 5678`, `٠١٠١٢٣٤٥٦٧٨` — all become `+201012345678`;
- the same number already in E.164, or with a leading `00` or a bare `20` in place of `+20`;
- an already-E.164 number for another country, e.g. `+966501234567`, unchanged; `00` in front of a country
  code becomes `+`.

Rejected: anything too short or too long, an Egyptian landline (prefix other than `010`/`011`/`012`/`015`),
a value with letters, or a value that does not start with `+` after the transformations above. The rule set
mirrors Egypt's actual mobile-prefix allocation (`010`, `011`, `012`, `015`); a new prefix added later is a
one-line change to the pattern, confirmed by the `015`-prefix mutation check in `PhoneNumberTests`.

`Masked` reveals a leading prefix and the last four digits, nothing more: `+20` for an Egyptian number (since
this type already knows that prefix), or just `+` and the first digit for anything else (since this type has
no database of other countries' country-code lengths to reveal more safely). Everything between becomes one
`•` per hidden digit.

## Search normalization

`SearchText.Normalize(value)` is the one function that turns a name — Arabic or Latin, however it was typed —
into a form used only for matching, never shown to a user. It lower-cases, collapses whitespace, strips
Arabic diacritics and tatweel, folds the four hamza-bearing letters and `ى`/`ة`/`ؤ`/`ئ` to their bare forms,
and maps Arabic-Indic digits to ASCII. `Parent` and `Student` call it from `SetName` whenever `FullName`
changes, storing the result as `SearchName`; Day 40's search query will call the same function on the search
term before comparing, so a search for "احمد" matches a student stored as "أحمد" without either side
special-casing the other. One function, called from both the write side and the read side, is what keeps that
true — two independent implementations could drift silently the first time either one changed.

## Storage (Day 38)

Three tables in the `academics` schema, added by one migration (`AddStudentsAndParents`) that touches nothing
else: `parents`, `students`, and `student_parents` (the `StudentParentLink` child entity). All three carry
the Day 38 contract's exact columns, constraints, grants and row-level security; `student_parents` is the
first table with foreign keys to two *other* tenant-owned tables, both composite on `(centre_id, …)` per
Layer 4 (`docs/architecture/tenancy.md`) — `fk_student_parents_students` and `fk_student_parents_parents`
each target `(centre_id, id)`, not a bare id, so a link can only ever name a student and a parent that both
belong to the same centre as the link itself.

`PhoneNumber` is stored as its E.164 string in one column (`Required`/`Optional` converters,
`PhoneNumberConverters`), never as an owned type with its own columns — the Domain type gains no EF
knowledge. A stored value that no longer passes `PhoneNumber.Create` surfaces as an exception on read, not a
`Result`: a value converter has no channel to report a failure through EF's materialisation path, and a
corrupt column is a bug, not a business outcome.

`Student.Parents` is mapped from `StudentConfiguration`, not from `StudentParentLinkConfiguration`: EF reads
and writes the collection through its private backing field (`Navigation(...).UsePropertyAccessMode(
PropertyAccessMode.Field)`), since the public property exposes only `IReadOnlyCollection<StudentParentLink>`
and EF cannot call `Add` through that. `StudentParentLink` carries no navigation back to `Student` or across
to `Parent` — only the scalar ids — so nothing reachable through the aggregate lets a handler reach into
`Parent`.

**Two persistence details found by running Task 38.5's tests, not predicted in advance:**

- **Removing a link needs an explicit delete.** `Student.RemoveParent` only takes the link out of its own
  in-memory collection. Because the foreign key to `Student` is `RESTRICT` — deliberately never `CASCADE` at
  the database level (Task 38.2's constraint: nothing here is removed by a database cascade) — EF does not
  infer "deleted" from a child disappearing out of a required collection; it raises
  `InvalidOperationException` instead, asking the caller to say what it meant. The caller (today, the Day
  38.5 test-only handler; Day 40's real one inherits the same shape) holds onto the removed link and calls
  `db.Set<StudentParentLink>().Remove(removedLink)` itself, right after a successful `RemoveParent`. This is
  an EF client-side concern, not a Domain one: `Student` still only ever says "this link is gone from my
  collection," and the persistence layer is what turns that into a row deletion.
- **The primary-contact switch needs no explicit statement ordering.** `SetPrimaryParent` calls
  `current?.ClearPrimary()` before `link.MarkPrimary()`, in that order, inside one Domain method; Task 38.5's
  test proves `ux_student_parents_one_primary` (a statement-level partial unique index) never sees two
  primary rows at once during the save — EF's default change-tracking order already clears the old primary's
  `UPDATE` before issuing the new one's, because that is the order the two calls touched the entities in.
  No explicit reordering was needed in the end; this is recorded here because the task called for proving it
  by running the test rather than assuming it, not because anything had to change.

## Use cases (Days 39–40)

Thirteen use cases, one per endpoint below, each an `IRequirePermission` command or query the same way every
other Month 2 use case is (`docs/architecture/authorization.md`):

| Use case | Kind | Permission |
| --- | --- | --- |
| `ListStudentsQuery` | query | `students.view` |
| `GetStudentQuery` | query | `students.view` |
| `RegisterStudentCommand` | command | `students.manage` |
| `UpdateStudentCommand` | command | `students.manage` |
| `ArchiveStudentCommand` | command | `students.manage` |
| `RestoreStudentCommand` | command | `students.manage` |
| `LinkParentCommand` | command | `students.manage` |
| `UnlinkParentCommand` | command | `students.manage` |
| `SetPrimaryParentCommand` | command | `students.manage` |
| `CreateParentCommand` | command | `students.manage` |
| `GetParentQuery` | query | `students.view` |
| `UpdateParentCommand` | command | `students.manage` |
| `LookupParentByPhoneQuery` | query | `students.view` |

`ListStudentsQuery` searches by calling `SearchText.Normalize` on the search term before comparing it against
`SearchName` — the same function `Student.SetName` already called to produce that column, so "احمد" and
"أحمد" compare equal without either side special-casing the other
(`StudentEndpointTests.List_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza` pins this end to end,
through the real endpoint and database). Paging is keyset, not offset: the
cursor encodes `(searchName, id)`, matching the query's own `ORDER BY search_name, id`, so a page boundary
never shifts under a concurrent insert the way an offset would. `LookupParentByPhoneQuery` is a query, not a
command, even though the frontend drives it from a mutation hook (`useLookupParent`) rather than a cached
`useQuery` — the Day 42 contract's own reason: the phone never belongs in a query-cache key, since it is
typed once per lookup and never reused for a later, different number. `CreateParentCommand` and
`GetParentQuery` require `students.manage`/`students.view` like every other row here, even though today's only
caller of `CreateParentCommand` is `RegisterStudentCommand` (see below) and of `GetParentQuery` is the parent
detail page — both are ordinary, independently reachable endpoints, not private helpers, so each carries its
own permission rather than inheriting one from a caller.

## The registration transaction

`RegisterStudentCommand` is the one use case here that spans two aggregates and, for a brand-new parent, one
of Day 38's uniqueness checks, all inside the one transaction its own `SaveChanges` call makes —
`RegisterStudentHandler`'s own words for why: "the dispatcher saves once: both aggregates and the link, or
neither." Given an existing parent id, it loads and links to that `Parent`; given a new parent's name and
phone, it checks `PhoneNumber.Create`, then `IParentRepository.PhoneExistsAsync` (failing `409
parent.phone_taken` if someone else claimed that number in the time between the frontend's own lookup and this
request), then `Parent.Create`, adds it, and links the student to the id it was just given — all before a
single row is written. The handler never dispatches a nested command to get there (`CreateParentCommand`
exists as its own endpoint for the frontend's create-then-link flow in the parents card, Day 42, which is
deliberately two separate requests, never dressed up as one transaction) — the dispatcher's nested-transaction
guard would refuse a handler calling back into itself, and doing so would split one transaction into two,
reopening exactly the partial-write window this design avoids. `RegisterStudentResult` carries both the new
`studentId` and the resolved `parentId` (new or existing) in the one `201` response, so the frontend never has
to ask which parent it ended up with.

## Endpoints (Day 41)

Thirteen endpoints under `/api`, each bind → dispatch → map its use case's result with no rule, repository or
tenant logic of its own (`docs/architecture/api-conventions.md`); none is anonymous.

| Method | Route | Use case |
| --- | --- | --- |
| `GET` | `/students` | `ListStudentsQuery` |
| `GET` | `/students/{id}` | `GetStudentQuery` |
| `POST` | `/students` | `RegisterStudentCommand` |
| `PUT` | `/students/{id}` | `UpdateStudentCommand` |
| `POST` | `/students/{id}/archive` | `ArchiveStudentCommand` |
| `POST` | `/students/{id}/restore` | `RestoreStudentCommand` |
| `POST` | `/students/{id}/parents` | `LinkParentCommand` |
| `POST` | `/students/{id}/parents/{parentId}/unlink` | `UnlinkParentCommand` |
| `POST` | `/students/{id}/parents/{parentId}/make-primary` | `SetPrimaryParentCommand` |
| `POST` | `/parents` | `CreateParentCommand` |
| `GET` | `/parents/{id}` | `GetParentQuery` |
| `PUT` | `/parents/{id}` | `UpdateParentCommand` |
| `POST` | `/parents/lookup` | `LookupParentByPhoneQuery` |

Unlink and make-primary are action sub-resources carrying the student's own `version` in the body, Month 2's
convention for a version-guarded state transition, rather than an HTTP `DELETE` with a body: the parent id
comes from the route, the version from the body, so the two can never disagree about which student is being
changed. Every mutation here follows the same optimistic-concurrency shape every Month 2 write already
does — a stale `version` fails `409 concurrency.stale`, never silently overwritten and never retried
automatically by the frontend (Day 42 rules, `docs/architecture/api-conventions.md`).

## Frontend (Day 42)

Four screens: the students list (search, show-archived, Day 41) gets a registration form at `/students/new`,
a student detail page at `/students/$studentId`, and a parent detail page at `/parents/$parentId`, all under
`frontend/src/features/students` and `frontend/src/features/parents`. The registration form never decides
that a phone is valid or already known — it calls `useLookupParent` on blur and renders whichever of the two
branches the server's answer picks (an existing-parent panel, or a new-parent name field), the same way the
parents card's own "Add parent" dialog reuses that lookup. The detail pages hide an action the aggregate would
refuse — archived hides edit and the parents card's manage controls; the parents card itself has no Unlink or
Make primary on the only or the primary parent, and no Add at four — but that is convenience, never
enforcement: every one of those rules is still checked, again, by the use case it would otherwise let through.
