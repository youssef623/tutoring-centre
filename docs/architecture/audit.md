# Audit log

Every change to an audited record made inside a centre — a subject, a staff membership, the centre's settings,
a student, a parent and their link, a teacher, a group, an enrollment, a weekly schedule rule or a decision about
one class session — leaves one append-only row: who, when, what, and the before and after values, committed in
the same transaction as the change. Written from the implementation (Day 31, extended on Days 38, 43, 44, 47 and
52); the entity table below is quoted from `AuditPolicy`, and if the two ever disagree the code is the source of
truth.

## The flow

Handlers change entities and contain no audit code at all. During the one `SaveChanges` call a command's
unit of work makes, three save-time interceptors run in order:

1. `TimestampInterceptor` (Month 1) stamps `CreatedAt`/`UpdatedAt`.
2. `AuditInterceptor` (`TutoringCentre.Infrastructure.Persistence.Interceptors`) builds and adds one
   `AuditEntry` per audited change it finds on the change tracker.
3. `TenantWriteGuardInterceptor` (Day 20) checks every tracked write, including the `AuditEntry` rows the
   previous step just added — so an audit row is itself subject to the tenant write guard, and a Modified or
   Deleted `AuditEntry` is always refused outright (see "Who can still alter rows" below).

Because the audit row is added to the *same* change tracker, before the *same* save, it commits or rolls
back as one unit with the change it describes: a failed handler or a lost uniqueness race leaves neither the
data change nor its audit row; nothing partial ever lands. `AuditInterceptor` snapshots the change tracker's
entries into a list before adding anything, so adding audit rows never disturbs the enumeration it is reading
from.

`AuditInterceptor` resolves who is acting from `ICurrentActor` (Application) and when from `IClock`, and
tags the row with the request's correlation id from `ICorrelationContext`
(`TutoringCentre.Application.Common.Security`, Task 31.1) — set once per HTTP request by
`CorrelationIdMiddleware`, left `null` for non-HTTP callers (the seed CLI, tests). `AuditDiffBuilder`
(`TutoringCentre.Infrastructure.Audit`) turns the action and the tracked property values into the `changes`
document; it takes and returns plain values only, no EF type, so it is tested exhaustively without a
database (`AuditDiffBuilderTests`).

## The table

`audit.audit_entries` — append-only history of changes to audited records, one row per change, scoped to the
centre the change happened in:

| Column | Type | Notes |
| --- | --- | --- |
| `id` | `uuid` | UUIDv7, assigned by the application. |
| `centre_id` | `uuid` | The centre the change happened in — the entity's own centre, not necessarily the acting actor's (they must match; see "What is audited" below). |
| `occurred_at` | `timestamptz` | From `IClock` at save time. |
| `actor_type` | `varchar(10)` | `'staff'` or `'system'`. |
| `actor_user_id` | `uuid`, nullable | The staff user; `null` for a system actor. |
| `action` | `varchar(10)` | `'created'`, `'updated'` or `'deleted'`. |
| `entity_type` | `varchar(60)` | One of the eleven names in "What is audited" below: `'subject'`, `'membership'`, `'centre'`, `'parent'`, `'student'`, `'student_parent'`, `'teacher'`, `'group'`, `'enrollment'`, `'schedule_rule'` or `'class_session'`. `AuditEntityTypeNames` (`TutoringCentre.Infrastructure.Audit`) is the one mapping between the enum and this string, shared by the EF value converter and the read side's API output, so the two can never drift apart — needed since `StudentParent` has no simple lower-case mapping from its CLR name alone. |
| `entity_id` | `uuid` | The changed row's own id. No foreign key to it: the audited row can later be deleted (a subject is never hard-deleted today, but nothing here depends on that). |
| `changes` | `jsonb` | `{ "<field>": { "before": ..., "after": ... } }`, allowed fields only, camelCase keys. |
| `correlation_id` | `varchar(64)`, nullable | The request's correlation id, or `null` outside an HTTP request. |

Primary key `pk_audit_entries(id)`; unique `ux_audit_entries_centre_id_id(centre_id, id)` (Layer 4's tenant
composite-key convention, `ConfigureTenantOwned`); `fk_audit_entries_centres` (`RESTRICT`) and
`fk_audit_entries_users` (`RESTRICT`, nullable); indexes `ix_audit_entries_centre_occurred(centre_id,
occurred_at DESC, id DESC)` and `ix_audit_entries_centre_entity(centre_id, entity_type, entity_id, occurred_at
DESC)`, both for the read side Day 32 adds. Checks: `ck_audit_entries_actor_type`, `ck_audit_entries_action`,
and `ck_audit_entries_actor` (`actor_type = 'staff'` if and only if `actor_user_id` is set). No `created_at`,
`updated_at` or row version: a row never changes after it is written, so there is nothing to version.

Row-level security is enabled and forced, the same `tenant_isolation` policy every tenant-owned table carries
(Layer 3, `TenantRowLevelSecurity.EnableTenantRowLevelSecurity`). `tutoring_app` is granted `SELECT` and
`INSERT` only, through a dedicated migration helper, `AppendOnlyAccessGrants.GrantAppendOnly(schema, table)`
(`TutoringCentre.Infrastructure.Persistence.Migrations`) — never the general `GrantRuntimeAccess`, which
would also grant `UPDATE` for every future table in the schema. Default privileges for the `audit` schema are
limited to `SELECT`/`INSERT` the same way, so a later migration that adds a second audit table inherits the
same ceiling unless it explicitly asks for more.

## What is audited

An explicit allow-list, `AuditPolicy` (`TutoringCentre.Infrastructure.Audit`) — not a deny-list: a new
property on an audited entity is never recorded until someone adds it here on purpose.

Eleven entity types. Every one is found by its own `CentreId` except `Centre`, which is the tenant itself and is
found by its own `Id`.

| Entity | Audited as | Fields | Masked | Actions |
| --- | --- | --- | --- | --- |
| `Subject` | `subject` | `name`, `status` | — | all |
| `Membership` | `membership` | `userId` (only ever appears on creation — nothing changes it afterward), `role`, `status` | — | all |
| `Centre` | `centre` | `name`, `defaultLocale` | — | all |
| `Parent` (Day 38) | `parent` | `fullName`, `phone` | `phone` | all |
| `Student` (Day 38) | `student` | `fullName`, `schoolYear`, `status`, `phone` | `phone` | all |
| `StudentParentLink` (Day 38) | `student_parent` | `studentId`, `parentId`, `relationship`, `isPrimary` | — | all |
| `Teacher` (Day 43) | `teacher` | `fullName`, `userId` (the linked account), `status`, `phone` | `phone` | all |
| `Group` (Day 44) | `group` | `name`, `subjectId`, `teacherId`, `capacity`, `status` | — | all |
| `Enrollment` (Day 47) | `enrollment` | `studentId`, `groupId`, `status`, `endReason` | — | all |
| `ScheduleRule` (Day 52) | `schedule_rule` | `groupId`, `dayOfWeek`, `startTime`, `durationMinutes`, `room`, `effectiveFrom`, `effectiveTo` | — | all |
| `ClassSession` (Day 52) | `class_session` | `status`, `teacherId` | — | updated only |

**Action lists (Day 52).** `AuditPolicyEntry.Actions` limits which actions are written for an entity; `null`, the
default, means every action, which is how every entity before Day 52 behaves. `ClassSession` records `updated`
only: sessions are generated from a rule, which is audited itself, and four weeks of creation rows per rule would
bury the log, while a cancellation (`status`) or a teacher reassignment (`teacherId`) is a decision someone made.
The enrollment timestamps (`enrolledAt`, `endedAt`) are not fields either: the audit row carries its own
`occurred_at`.

Nothing else is audited. `AuditEntry` itself and Identity's user type (`ApplicationUser`) are never in the
policy — a test asserts this directly (`AuditPolicyTests.Entries_NeverIncludesAuditEntriesOrIdentityUsers`).
Normalized names, hashes, stamps, emails, passwords and tokens are never recorded, because none of them is in
any entity's field list above — not because of a separate redaction step that could be bypassed. `SearchName`
(`Parent`, `Student`, `Teacher`), `NormalizedName` (`Group`) and `LinksRevision` (`Student`) follow the same rule: neither is an independent fact —
one is derived from `FullName`, the other is persistence bookkeeping for optimistic concurrency — so neither
is in the field list either.

**Created** writes every allowed field, `before: null`, `after` the value. **Updated** writes only the
allowed fields that are both modified and whose value actually changed — a property touched but left equal
to what it already was writes nothing, and if no allowed field really changed, no row is written at all.
**Deleted** writes every allowed field, the value `before`, `after: null`. Enum values are written as the
same lower-case strings the API already uses (`"archived"`, `"teacher"`, `"en"`, ...); GUIDs as strings.

**Masked fields (Day 38).** `AuditPolicyEntry.MaskedFields` names the subset of `AllowedFields` whose values
are never written in full — today, every audited `phone` (parent, student and, since Day 43, teacher). `AuditDiffBuilder` decides whether a field changed
from the *real* tracked values first (so two different phone numbers that happen to share their last four
digits still produce an entry — masking could never have told them apart), then, only for what is actually
written, replaces the value with `PhoneNumber.Masked`. The audit table is append-only and this log is a
durable record with no redaction step that runs later: once a full number were written, it could never be
taken back, so masking happens before the row is ever inserted, not as a read-time transform. A masked field
holding anything other than a `PhoneNumber` (or `null`) is a policy mistake, not a business outcome, and
throws rather than writing silently wrong data.

**The two-centre match.** A change is audited only when the entity's own centre equals the *acting actor's*
centre. This is what keeps platform bootstrap — centre creation, the development identity seeder — out of
the log without a special case: both run with a `SystemActor` that has no centre (or, for the seeder, no
actor set at all, which defaults to anonymous), so the match never holds and nothing is written. The same
rule is why a centre's own settings change is audited (Day 32): the acting owner's centre equals the centre's own
id. It is also why the demo seed's per-centre steps (subjects, students, teachers, groups, enrollments, schedule
rules) are audited: each runs as `SystemActor(centreId)`, so the match holds and the row records
`actor_type = 'system'`, exactly like the same command sent over HTTP by a staff member (`SeedCommandTests`
asserts one `enrollment` and one `schedule_rule` entry per seeded row).

## What is not in this log

- **Reads.** The log is a record of writes; nothing here captures who viewed a subject, a staff list, a
  student or parent record, or the audit log itself. Since Month 3 the system holds personal data about minors,
  which raises the priority of read auditing (`later.md`).
- **Platform bootstrap.** Centre creation and the development identity seeder run with no actor centre, so
  nothing they do is audited (see "the two-centre match" above) — they are logged, not audited: visible in
  the application's own structured logs like any other request, with no before/after row in this table.
- **Password changes.** Changing a password is deliberately outside `AuditPolicy` (Identity's user type is
  never audited at all) — a password change is already refused from ever reaching a log or an audit row in
  readable form (Month 1's redaction policy), and the request itself is still visible in the standard
  structured request log (Serilog), by path and status, the same as any other request.
- **Hash-chaining or export to external, append-only storage**, and **auditing reads themselves** — both
  deferred; see `later.md`.

## Who can still alter rows

"Append-only" here means: the application's own runtime role, `tutoring_app`, can never update, delete or
truncate this table — enforced by what it is granted (`SELECT`/`INSERT` only) and by forced row-level
security, proven at the database-role level (`AuditImmutabilityTests`), not assumed from the application
code never trying to. It does not mean no one can:

- **`tutoring_owner`** (the migration role) owns the table and is not restricted by its grants; forced
  row-level security does bind it, but only while the policy exists — a migration run as that role could,
  deliberately, drop or alter the policy, or the table, before changing a row.
- **A PostgreSQL superuser** bypasses row-level security entirely, by design — the readiness check that
  fails the runtime connection closed if it is ever a superuser (Layer 0) says nothing about the database's
  actual superuser account, which every managed PostgreSQL offers to someone with infrastructure access.

Tamper-resistance here means: the one role the running application ever uses cannot alter history, and any
attempt through it is a transaction-level `42501`, not a silent partial write. It is not resistance against
someone with migration or infrastructure-level database access — that is a different, larger problem
(hash-chaining or shipping entries to storage that role cannot reach would be the next layer; see
`later.md`).
