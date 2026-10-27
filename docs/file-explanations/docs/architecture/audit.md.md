# `docs/architecture/audit.md`

## Purpose

Documentation covering Audit log; The flow; The table; What is audited; What is not in this log. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/audit.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 166 lines and 13,357 bytes. Source hash: `01d812ac375e5e0001fa6159ba2651f09a974eebceb78e8aad84a3fd14c4eb34`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../architecture/audit.md#L3): `Every change to an audited record made inside a centre — a subject, a staff membership, the centre's settings,`
- [Line 5](../../../architecture/audit.md#L5): `one class session — leaves one append-only row: who, when, what, and the before and after values, committed in`
- [Line 49](../../../architecture/audit.md#L49): `| entity_type | varchar(60) | One of the eleven names in "What is audited" below: 'subject', 'membership', 'centre', 'parent', 'student', 'student_parent', 'teacher', 'group', 'enrollment', 'schedule_rule' or 'class_session'. AuditEntityTypeNames (TutoringCentre.Infrastructure.Audit) is the one mapping between the enum and this string, shared by the EF value converter and the read side's API output, so the two can never drift apart — needed since StudentParent has no simple lower-case mapping from its CLR name alone. |`
- [Line 118](../../../architecture/audit.md#L118): `durable record with no redaction step that runs later: once a full number were written, it could never be`
- [Line 135](../../../architecture/audit.md#L135): `- **Reads.** The log is a record of writes; nothing here captures who viewed a subject, a staff list, a`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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
```

The excerpt stops at line 24; use the original file for the remaining 142 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
