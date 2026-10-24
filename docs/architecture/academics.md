# Academics: students, parents, teachers, groups and enrollments (Days 37–47)

The Domain model behind registering a student and the parents a centre contacts about them, how that model is
stored, the use cases and endpoints built on it, and the two screens that use them. Teachers and groups
(Days 43–46) and enrollments (Day 47) follow in their own sections at the end. Written from the implementation; every name and rule
below is checked against the code, not the plan that produced it.

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

## Teachers and groups (Days 43–46)

| Type | Kind | Tenant-owned | Lives in |
| --- | --- | --- | --- |
| `Teacher` | aggregate root | yes | `TutoringCentre.Domain.Academics` |
| `Group` | aggregate root | yes | `TutoringCentre.Domain.Academics` |

**`Teacher`** (`academics.teachers`) is a person who teaches, not a login: a full name (2–120 characters), an
optional `PhoneNumber`, an optional `UserId`, and a status. Archived is read-only until restored, and
`ArchiveTeacherHandler` refuses to archive a teacher who still has an active group (`teacher.has_active_groups`)
— reassign or archive the groups first. Two teachers may share a name.

**`Group`** (`academics.groups`) is the class students enrol in: a name (1–80 characters, unique per centre by
its normalised form — `group.name_taken`), one `SubjectId` fixed at creation, one `TeacherId` that can change,
a capacity of 1–200, and a status (archived is read-only). It refers to its subject and teacher by identity
only, through composite foreign keys `(centre_id, subject_id)` and `(centre_id, teacher_id)`, so a group can
never point at another centre's subject or teacher even through privileged SQL. Whether that subject or teacher
is still active is the handlers' check (`group.subject_archived`, `group.teacher_archived`), on create, on
reassignment and again on restore; symmetrically, a subject (`subject.has_active_groups`) or a teacher with an
active group cannot be archived. The enrolled count is not the group's to own: it is 0 until enrolments exist
(Days 47–48).

**The account link.** A teacher record may be linked to one staff account, which is what lets a signed-in
Teacher-role user see their own groups (the ownership guard, `docs/architecture/authorization.md`). The link is
its own sub-resource (`PUT /api/teachers/{id}/account`, `SetTeacherAccountCommand`) — editing a teacher's
details never touches it, and no other request accepts a `userId`. Linking requires a user who is an active
Teacher-role member of the actor's centre; an unknown user, another centre's user, another role and an
inactive member all receive the same `teacher.user_not_eligible`, so the answer never discloses which case it
was. One account is linked to at most one teacher per centre (`teacher.account_taken`, checked by the handler
and backed by the unique index `ux_teachers_centre_user` for a race). `null` removes the link.

**The `Contracts` rule.** Eligibility is an Identity question, and Academics may not reach into Identity's
repositories or read services (`ModuleBoundaryRuleTests`). It asks instead through Identity's published
contract `TutoringCentre.Application.Identity.Contracts.IStaffDirectory`, whose one method answers with a bool —
never a membership or a DTO — so the caller learns exactly the one fact it asked for
(`docs/architecture/overview.md`, "Contracts").

### Endpoints (Days 43 and 45)

| Method | Route | Use case | Permission |
| --- | --- | --- | --- |
| `GET` | `/teachers` | `ListTeachersQuery` | `teachers.view` |
| `GET` | `/teachers/{id}` | `GetTeacherQuery` | `teachers.view` |
| `POST` | `/teachers` | `CreateTeacherCommand` | `teachers.manage` |
| `PUT` | `/teachers/{id}` | `UpdateTeacherCommand` | `teachers.manage` |
| `PUT` | `/teachers/{id}/account` | `SetTeacherAccountCommand` | `teachers.manage` |
| `POST` | `/teachers/{id}/archive` | `ArchiveTeacherCommand` | `teachers.manage` |
| `POST` | `/teachers/{id}/restore` | `RestoreTeacherCommand` | `teachers.manage` |
| `GET` | `/groups?subjectId=&teacherId=&includeArchived=` | `ListGroupsQuery` | `groups.view` (scoped) |
| `GET` | `/groups/{id}` | `GetGroupQuery` | `groups.view` (scoped) |
| `POST` | `/groups` | `CreateGroupCommand` | `groups.manage` |
| `PUT` | `/groups/{id}` | `RenameGroupCommand` | `groups.manage` |
| `PUT` | `/groups/{id}/capacity` | `ChangeGroupCapacityCommand` | `groups.manage` |
| `PUT` | `/groups/{id}/teacher` | `AssignGroupTeacherCommand` | `groups.manage` |
| `POST` | `/groups/{id}/archive` | `ArchiveGroupCommand` | `groups.manage` |
| `POST` | `/groups/{id}/restore` | `RestoreGroupCommand` | `groups.manage` |

Name, capacity and teacher are separate sub-resources with their own request models, so no route can change a
group's subject (a `subjectId` in any PUT body is an unknown field: `400 request.malformed`). "Scoped" means the
read is narrowed by the actor's teacher scope in SQL; there is no separate "my groups" route.

### Frontend (Day 46)

Three screens, under `frontend/src/features/teachers` and `frontend/src/features/groups`:

- `/teachers` (`teachers.view`, URL parameter `archived`): summary tiles and the directory — name, phone,
  linked account by display name, active-group count, status. Add, edit, account, archive and restore sit
  behind `Can` with `teachers.manage`, so a secretary sees the same list read-only. The account dialog reuses
  the Staff module's own `GET /api/staff` for its picker; both `staff.view` and `teachers.manage` are
  owner-only, so staff emails reach exactly the people who can use the dialog, and the list is requested only
  while the dialog is open. It offers active Teacher-role members not already linked — a convenience filter;
  the server's refusal (`teacher.user_not_eligible`, `teacher.account_taken`) is shown under the select.
- `/groups` (`groups.view`, URL parameters `subject`, `teacher`, `archived`): one page for owners, secretaries
  and teachers, rendering whatever the scoped API returns. The teacher filter needs `teachers.view` and is not
  rendered without it (so a teacher's session never calls `/api/teachers`); create needs `groups.manage`, and a
  session without it gets its own empty-state text ("No groups are assigned to you"). Seats show as
  "enrolled / capacity", isolated left-to-right, with a "Full" badge when no seat is left.
- `/groups/$groupId` (`groups.view`, URL parameter `tab` = `overview` | `roster` | `schedule`): a summary card
  and URL-addressable tabs; Roster is filled in on Day 50 (see "Frontend (Day 50)" below), Schedule is a
  placeholder until Day 53. Rename, capacity and
  teacher are three dialogs sending three requests, each with the loaded version — never one form posting to
  several endpoints, which could leave a half-applied edit. An unknown group, another centre's group and, for a
  teacher, someone else's group all render the same "Group not found" state.

Rendering follows permissions, never role names. Selects for subject and teacher (`SubjectSelect`,
`TeacherSelect`) offer active entries only; a refusal about either lands under that select and refetches its
list. Field errors carrying an error code are shown in the session's language (the `errors` namespace), with
the server's own field text only as a fallback; numbers stay in Western digits in both languages.
## Enrollments (Day 47)

| Type | Kind | Tenant-owned | Lives in |
| --- | --- | --- | --- |
| `Enrollment` | aggregate root | yes | `TutoringCentre.Domain.Academics` |

An `Enrollment` says "this student holds a seat in this group since this moment": a `StudentId` and a `GroupId`
(by ID only), a status (`active` or `ended`), the time it started, and — once ended — when and why
(`withdrawn` or `transferred`). It is its own aggregate rather than a child of `Student` or `Group` because it
changes on its own schedule and is shared ground between the two: a student's enrollments span many groups, a
group's span many students, and neither side should have to load the other's whole history to add one row.

History is kept by ending a row and starting a new one, never by editing the past. `End` happens once
(`enrollment.already_ended` keeps the first end) and never before the start (`enrollment.end_before_start`);
there is no `Reactivate` — a student who comes back gets a new enrollment, and a transfer is an end in one
group plus a start in another. Timestamps are passed in by the use case from the application's clock; the
entity never reads one.

**What the entity does not own.** Capacity is a rule about the group and a count of *other* rows; "not already
enrolled" is about other rows too (the handler checks it, a partial unique index backs it); whether the student
or the group is active belongs to those aggregates (the handler checks it). Day 48 adds those checks.

**What the table enforces** (`academics.enrollments`, migration `AddEnrollments`):

- at most one *active* enrollment per student and group — the partial unique index
  `ux_enrollments_active_student_group` (`WHERE status = 'active'`), partial so that ended enrollments stay as
  history and a student can re-enroll after a withdrawal; its violation maps to `enrollment.already_enrolled`;
- an enrollment joins a student and a group of the same centre — composite foreign keys through
  `(centre_id, id)` on both sides;
- an ended row always has its time and reason, an active row has neither, and nothing ends before it started —
  `ck_enrollments_end_consistency` and `ck_enrollments_end_after_start`;
- forced row-level security like every tenant table, and no `DELETE` grant: ended enrollments are history.

**What it cannot enforce: capacity.** "At most N active rows for this group" is a rule about a count of rows,
and unique and check constraints are evaluated per row. The decision lives in the Domain instead —
`Group.EnsureCanEnroll(activeEnrollmentCount)` refuses with `group.archived` for an archived group and with
`group.full` (a Conflict: the request was valid and may succeed later) once the count reaches the capacity, and
`Group.ChangeCapacity(capacity, activeEnrollmentCount)` refuses to shrink below it
(`group.capacity_below_enrolled`). The count is a parameter, not a field: `Group` stores no enrolled count, so
the enrollment rows remain the only source of truth, and the rule can be tested without a database.

What makes that count safe to use is a row lock on the group, taken before counting (Day 48;
`docs/adr/0011-pessimistic-lock-for-enrollment-capacity.md`). Without it, two requests for the last seat can
both count 19 of 20 and both insert. With it, the second waits:

```text
Tx A                                   Tx B
BEGIN + app.current_centre             BEGIN + app.current_centre
SELECT group ... FOR UPDATE  (granted)
                                       SELECT group ... FOR UPDATE  (waits)
COUNT active = 19
EnsureCanEnroll(19) ok -> INSERT
COMMIT  (lock released)
                                       (granted) COUNT active = 20
                                       EnsureCanEnroll(20) -> group.full -> ROLLBACK
```

PostgreSQL's default READ COMMITTED isolation is enough: Tx B's count is a new statement that runs only after
its lock is granted, which is only after Tx A has committed, so it sees Tx A's insert. Locking the enrollment
rows instead would not work — the row that would overbook the group is the one being inserted, and it does not
exist yet.

**Permissions.** `enrollments.view` (Owner, Secretary, and Teacher — narrowed to their own groups by the same
teacher scope as groups, applied by the read services) and `enrollments.manage` (Owner, Secretary). Teachers see
who is in their groups, which they need for attendance, but never enroll or withdraw anyone: enrollment drives
fees, so only office roles change it. Enrollment changes are audited (`studentId`, `groupId`, `status`,
`endReason`).

### Use cases and lock rules (Day 48)

| Use case | Takes the group lock | Refusals, in the order the handler checks them |
| --- | --- | --- |
| `EnrollStudentCommand` | the group | `group.not_found`, `student.not_found`, `enrollment.student_archived`, `enrollment.already_enrolled`, `group.archived`, `group.full` |
| `WithdrawEnrollmentCommand` | none | `enrollment.not_found`, `enrollment.already_ended`, `concurrency.stale` (on save) |
| `TransferEnrollmentCommand` | the target group only | `enrollment.not_found`, `enrollment.already_ended`, `enrollment.same_group`, `group.not_found`, `enrollment.subject_mismatch`, `enrollment.already_enrolled`, `group.archived`, `group.full`, `concurrency.stale` (on save) |
| `ChangeGroupCapacityCommand` | the group | `group.not_found`, `concurrency.stale`, `group.archived_read_only`, `group.capacity_invalid`, `group.capacity_below_enrolled` |
| `ArchiveGroupCommand` | the group | `group.not_found`, `concurrency.stale`, `group.has_active_enrollments`, `group.already_archived` |
| `GetGroupRosterQuery`, `ListStudentEnrollmentsQuery` | none (reads) | `group.not_found` / `student.not_found`, teacher-scoped like groups |

Two rules keep the capacity decision safe:

- **Every writer of capacity takes the lock.** Any command that decides from a group's active enrollment count —
  enroll, transfer in, change capacity, archive group — loads the group through
  `IGroupRepository.GetForEnrollmentAsync`, which takes `SELECT … FOR UPDATE` and returns the count taken under
  that lock. A new command that counts without it reopens the last-seat race.
- **One lock per command.** A transfer locks only its target: the source only loses a seat, which can never break
  its capacity, so it is read without a lock. With at most one group lock per transaction, two opposite transfers
  can never wait on each other in a cycle, and no lock-ordering rule is needed.

Withdrawing frees a seat and takes no lock; its only race is with another change to the same enrollment, which
the enrollment's own version (`xmin`) catches as `concurrency.stale`. Archiving a student who still has active
enrollments is refused with `student.has_active_enrollments`. The tests that prove these rules are named in
ADR 0011.

### Endpoints (Day 49)

| Method | Route | Use case | Permission |
| --- | --- | --- | --- |
| `POST` | `/groups/{groupId}/enrollments` | `EnrollStudentCommand` | `enrollments.manage` |
| `GET` | `/groups/{groupId}/roster` | `GetGroupRosterQuery` | `enrollments.view` (scoped) |
| `GET` | `/students/{studentId}/enrollments` | `ListStudentEnrollmentsQuery` | `enrollments.view` (scoped) |
| `POST` | `/enrollments/{id}/withdraw` | `WithdrawEnrollmentCommand` | `enrollments.manage` |
| `POST` | `/enrollments/{id}/transfer` | `TransferEnrollmentCommand` | `enrollments.manage` |

Enrolling is a nested resource of the group: the group comes from the route only (a `groupId` in the body is an
unknown field, `400 request.malformed`), and the `201`'s `Location` points at the roster, because a new
enrollment has no page of its own. The student's enrollment list sits under `/students` because that is where a
client looks, but it is an enrollment read with enrollment's permission; `GET /students/{id}` does not embed it.
Withdraw and transfer are version-guarded actions, never an HTTP `DELETE`: an ended enrollment stays as history.
Transfer is one endpoint dispatching one command, so ending the old enrollment and starting the new one commit
in one transaction (`200` with the new enrollment's ID). The 404 names the first missing or foreign thing in the
handler's lookup order — group, then student; enrollment, then target group.

### Frontend (Day 50)

Under `frontend/src/features/enrollments`, all text in the `enrollments` namespace in both languages:

- **Roster tab** (`GroupRosterTab`, `RosterTable`, on `/groups/$groupId?tab=roster`). One view, three permission
  levels: the tab exists for `enrollments.view` (so a group's own teacher sees it), the actions sit behind
  `enrollments.manage`, and names link to the student page only with `students.view`. A `SeatMeter` shows
  "enrolled / capacity" isolated as LTR, with a written "Full" badge. "Enroll student" is disabled with its reason
  at capacity or when archived — a courtesy; the server is the capacity control.
- **Enroll** (`EnrollStudentDialog`, `StudentPicker`). The picker is the existing students list query with a page of
  10 and the debounced search, so the server's normalized Arabic search works with no new endpoint; search state
  is local to the dialog. Students already on the roster are marked and cannot be picked. No contact details are
  shown.
- **Withdraw** (`WithdrawEnrollmentDialog`) asks for confirmation naming the student and the group, and sends the
  row's version. **Transfer** (`TransferDialog`) offers the other active groups of the same subject
  (`GET /groups?subjectId=`), full ones disabled, and sends one request — the client never withdraws and then
  enrolls.
- **Student page** (`StudentEnrollmentsCard`, rendered only with `enrollments.view`, so without it no enrollment
  request is sent): current groups first with their actions, past ones muted with end date and reason. "Enroll in
  a group" (`EnrollInGroupDialog`) picks a subject and then one of its groups, and calls the same mutation.
- **Groups list**: the compact `SeatMeter` on every row, and an optional "only groups with free seats (on this
  page)" filter applied in the browser to the loaded page; the counts themselves stay server-derived.

`useEnrollmentMutations` is the one definition of enroll, withdraw and transfer. It does no optimistic updates:
the seat count in the browser is advisory, and the server's 409 decides who gets the last seat. Every outcome —
success or refusal — invalidates every query that shows a touched resource: for enroll and withdraw, that roster,
that group, the groups list and that student's enrollments; for a transfer, both rosters, both groups, the list
and the student's enrollments. A 409 or 422 is an ordinary outcome shown in the dialog in words ("This group is
full", "Already in this group"), with the dialog left open; a 404 closes it with a toast.
