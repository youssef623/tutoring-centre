# Full inspection of generated migration models

All thirteen migration designers and `AppDbContextModelSnapshot.cs` were read completely. The source comprises 9,653 lines. Each file was partitioned into contiguous, non-overlapping full-source blocks. All fifty-one distinct blocks were inspected in seven bounded packets without truncation; exact duplicate blocks were verified by SHA-256 and mapped to every occurrence. The file/range/hash ledger is `generated-model-read-ledger.json`. This is full content inspection with duplicate identity verification, not inference from filenames or a grep-only summary.

## What the generated files mean

Each `*.Designer.cs` contains its migration identifier and `BuildTargetModel`: the entire EF model as it existed after that migration, not only that migration's new entity. The current snapshot contains `BuildModel`, which EF compares against the current runtime model when generating another migration. Historical files should retain their historical metadata; the presence of an older ProductVersion in an earlier designer is not model drift.

| Target model | EF ProductVersion | Distinct mapped entities/tables |
| --- | --- | ---: |
| InitialPlatform | 10.0.12 | 1 |
| AddIdentityAndMemberships | 10.0.12 | 6 |
| AddRuntimeRoleGrants | 10.0.12 | 6 |
| AddSubjects | 10.0.12 | 7 |
| AddMembershipRowVersion | 10.0.12 | 7 |
| AddAuditLog | 10.0.12 | 8 |
| AddCentreRowVersion | 10.0.12 | 8 |
| AddDataProtectionKeys | 10.0.12 | 9 |
| AddStudentsAndParents | 10.0.12 | 12 |
| AddTeachers | 10.0.13 | 13 |
| AddGroups | 10.0.13 | 14 |
| AddEnrollments | 10.0.13 | 15 |
| AddSchedule | 10.0.13 | 17 |
| Current snapshot | 10.0.13 | 17 |

The latest designer and current snapshot have byte-identical entity blocks, including relationship/navigation blocks and trailing model closure. Only their generated class/header/method scaffolding differs. The separately executed EF model check also reports no pending model changes.

## Current modeled storage

The final model maps two platform tables (`centres`, `data_protection_keys`), five identity tables (`users`, `memberships`, `user_claims`, `user_logins`, `user_tokens`), nine academics tables (`subjects`, `students`, `parents`, `student_parents`, `teachers`, `groups`, `enrollments`, `schedule_rules`, `class_sessions`) and one audit table (`audit_entries`). EF's migration history table is infrastructure bookkeeping, not another mapped application entity in this model.

`Version` maps to generated-on-add-or-update PostgreSQL `xmin`/`xid` concurrency tokens on Centre, Membership, Subject, Student, Parent, Teacher, Group, Enrollment, ScheduleRule and ClassSession. `ApplicationUser.ConcurrencyStamp` is a separate string concurrency token. AuditEntry, DataProtectionKey, StudentParentLink and the standard Identity child tables do not declare a Version concurrency property here. This is observed generated model content, not a guess based on entity type names.

The student-parent relationship has composite `(CentreId, StudentId)` and `(CentreId, ParentId)` Restrict foreign keys, a unique student-parent pair and a filtered unique primary-contact index. Student has a `LinksRevision` column and a `Parents` navigation. Parents have a required normalized phone string and per-centre unique phone index; student and teacher phone columns are nullable. The phone checks use the stored international-format regex.

Groups have capacity/status checks, a per-centre unique normalized name and composite subject/teacher relationships. Enrollments have the active student/group partial unique index, consistency checks for end fields, and same-centre student/group relationships. Schedule rules store local weekday, time and effective dates, with duration/day/date checks. Sessions store local date and UTC-capable timestamp columns, schedule/group/teacher relationships, status/range checks and a unique rule/date index.

Audit rows have JSONB changes, a sixty-four-character correlation reference, actor/action checks, descending chronology indexes and Restrict user/centre relationships. Data Protection keys use an integer identity key, friendly name and XML. The standard user claim/login/token relationships cascade from users; memberships restrict user/centre deletion.

## Important model-versus-migration distinctions

No unexpected discrepancy was found in the generated models. Several database guarantees deliberately live outside them:

- `fk_teachers_memberships` is added directly in `AddTeachers.cs`, referencing the membership centre/user unique index. `TeacherConfiguration` explicitly explains why that relationship is absent from EF metadata and the snapshot.
- Row-level policies and grants/revokes are raw migration operations. They do not appear as EF query filters or ordinary relationships in these generated targets. The absence of generated query-filter code is not evidence that runtime filters are missing.
- Scheduling's `btree_gist` extension and group/teacher exclusion constraints are migration SQL. The snapshot records ordinary session keys/checks/indexes but does not fully describe those raw SQL guarantees.
- Database privilege/grant migrations can have identical entity model bodies while materially changing database authority. `AddRuntimeRoleGrants` is an example; a model-only comparison cannot prove live grants.

Guide wording should therefore describe each designer as a historical complete target model, retain its actual ProductVersion, and direct database-behavior claims to migration bodies/configurations/tests. Never describe every designer as EF 10.0.13 merely because the current package file is 10.0.13, or treat snapshot parity as proof of all live security policy state.
