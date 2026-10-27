# `docs/architecture/authorization.md`

## Purpose

Documentation covering Authorization; The matrix; The marker and the pipeline step; What the UI does and does not do; Three different checks. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../architecture/authorization.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 207 lines and 17,576 bytes. Source hash: `2033431833f5cf630dfc33354ac067b6dd66efd9ad71f5bde7582a5d5b837559`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 139](../../../architecture/authorization.md#L139): `All; a limited role gets Own of the active teacher record linked to its user in the acting centre`
- [Line 140](../../../architecture/authorization.md#L140): `(ITeacherReadService.FindIdByUserAsync), or None when there is no linked record or it is archived; any`
- [Line 163](../../../architecture/authorization.md#L163): `- **Tenancy and ownership compose.** The provider looks the teacher record up in the acting centre, so one`
- [Line 164](../../../architecture/authorization.md#L164): `account linked to a teacher record in each of two centres sees a different set of groups per selected`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Authorization

Who may do what, once tenancy (`docs/architecture/tenancy.md`) has already decided which centre a request
runs in. Written from the implementation; the matrix below is quoted from `RolePermissionsTests`
(`TutoringCentre.Domain.Tests`), not derived from the map it tests — if the two ever disagree, the test is
the source of truth and this file is wrong.

## The matrix

Three staff roles from Month 1 (Owner, Secretary, Teacher). The map is fixed in code
(`RolePermissions`, `TutoringCentre.Domain.Identity`) — no role or permission tables, no per-centre
overrides.

| Permission | Owner | Secretary | Teacher | Used by |
| --- | --- | --- | --- | --- |
| `subjects.view` | yes | yes | yes | list and get subjects |
| `subjects.manage` | yes | yes | no | create, rename, archive, restore subjects |
| `staff.view` | yes | no | no | `GET /api/staff` (Day 30) |
| `staff.manage` | yes | no | no | `POST /api/staff`, change role, deactivate, reactivate (Day 30) |
| `audit.view` | yes | no | no | read the audit log (Day 32) |
| `centre.settings.manage` | yes | no | no | read and update centre settings (Day 32) |
| `students.view` | yes | yes | no | list and get a student or parent, and the phone lookup (Days 39–41) |
| `students.manage` | yes | yes | no | register, update, archive, restore a student; create, update, link, unlink and set-primary a parent (Days 39–41) |
| `teachers.view` | yes | yes | no | list and get teachers (Day 43) |
```

The excerpt stops at line 24; use the original file for the remaining 183 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
