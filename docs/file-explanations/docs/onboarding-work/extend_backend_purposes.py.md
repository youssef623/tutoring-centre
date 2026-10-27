# `docs/onboarding-work/extend_backend_purposes.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/extend_backend_purposes.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 43 lines and 10,773 bytes. Source hash: `858bbe44065d4cd321d7b226de48695a5a835db797923d86391b4d0c9ccd4f13`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import json`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```python
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
path = root / 'docs/onboarding-work/backend-purposes.json'
data = json.loads(path.read_text(encoding='utf-8-sig'))
entries = {
 'Infrastructure/ReadServices/CentreReadService.cs': 'Projects settings and the xmin-backed client version without tracking. Centre rows deliberately have neither tenant query filters nor RLS, so its supplied centre ID is the security boundary; callers must take that ID from the actor rather than untrusted request input.',
 'Infrastructure/ReadServices/EnrollmentReadService.cs': 'Builds active group rosters and a student enrollment history through tenant-filtered joins. TeacherScope filters the group before reading a roster; a nonexistent student returns null, whereas a valid student with no permitted groups yields an empty list. The roster cap equals the domain maximum capacity and assumes the write invariant remains intact.',
 'Infrastructure/ReadServices/GroupReadService.cs': 'Applies archive, subject, teacher, and teacher-ownership predicates before a capped SQL projection. Joins supply subject and teacher names; an active-enrollment correlated count supplies occupancy without a denormalized counter. Unknown or missing Own scope fails closed without querying groups.',
 'Infrastructure/ReadServices/MembershipReadService.cs': 'Reads profile, memberships, active-centre membership, password gate, and cookie session state. These identity rows are globally readable by the runtime role, so each operation explicitly filters the requested user. Session state includes the current role as well as security stamp and membership activity, allowing stale role claims to be rejected.',
 'Infrastructure/ReadServices/ParentReadService.cs': 'Reads parent details and exact canonical-phone matches without tracking. The children query joins filtered links to filtered students, orders by name, and caps children at fifty. Phone value objects are converted to output strings after materialization to avoid unsupported SQL member translation.',
 'Infrastructure/ReadServices/StaffReadService.cs': 'Lists centre memberships joined to user display name and email, including role, status, version, and the self marker. Membership has no implicit tenant protection, so the explicit centre predicate is mandatory. The application permission check protects this staff-specific exposure of email.',
 'Infrastructure/ReadServices/SubjectReadService.cs': 'Projects subject IDs, names, status, and xmin versions with no tracking. Entity filters run before projection; the list excludes archived subjects by default, sorts names, and caps results at five hundred. The cap is truncation rather than cursor pagination.',
 'Infrastructure/ReadServices/SystemInfoReadService.cs': 'Translates EF applied and pending migration metadata into the application SchemaStatus record. It returns the last applied migration and pending count, preventing EF migration types from leaking through the application port.',
 'Infrastructure/ReadServices/TeacherReadService.cs': 'Reads teachers with optional linked-account display names through a left join and computes active group counts. It deliberately exposes no linked-account email. List results are capped at five hundred, while user-to-teacher resolution requires an active teacher so archived records cannot obtain Own read scope.',
 'Infrastructure/Repositories/CentreRepository.cs': 'Tracks platform centres, checks slug uniqueness, and sets the caller version as EF OriginalValue for subsequent optimistic UPDATE checks. It contains no save call: dispatcher-owned UnitOfWork controls persistence and audit. Centre scoping remains the handler responsibility because this platform entity has no tenant filter.',
 'Infrastructure/Repositories/MembershipRepository.cs': 'Tracks explicitly centre-filtered memberships and installs expected xmin versions. Its active-owner locking query uses parameterized ADO.NET, joins the current EF transaction, reads FOR UPDATE rows, and counts them in memory because PostgreSQL cannot combine that locking clause with COUNT. This serialization supports last-owner protections.',
 'Infrastructure/Repositories/ParentRepository.cs': 'Tracks parent roots, sets expected xmin on mutation loads, supports exact PhoneNumber equality lookup, and excludes a chosen root during duplicate-phone checks. Add only enrolls a row in tracking; unique constraints and UnitOfWork determine the final write outcome.',
 'Infrastructure/Repositories/ScheduleRuleRepository.cs': 'Tracks rules and expected xmin versions. Active-rule enumeration includes only active groups and rules whose inclusive end has not passed the requested date, ordered by group, weekday, and start time. The group check asks whether any rule has not ended as of today rather than whether it has already begun.',
 'Infrastructure/Repositories/StudentRepository.cs': 'Always eagerly loads parent links for both mutation and ordinary tracked lookup, preserving aggregate collection invariants during unlink and primary-parent operations. Mutation lookup also sets the client xmin OriginalValue. Omitting Include would make valid links appear absent to the aggregate.',
 'Infrastructure/Repositories/SubjectRepository.cs': 'Provides tracked subject lookup, normalized-name duplicate checks excluding a supplied entity, and deferred insertion. Mutation lookup places the client version in EF OriginalValue; a stale write is detected by the eventual database UPDATE rather than an early in-memory comparison.',
 'Infrastructure/Repositories/TeacherRepository.cs': 'Provides tracked teacher loads, expected xmin assignment, account-link uniqueness checks excluding the current teacher, and deferred insertion. Its account check is tenant-filtered through the entity query filter; eligibility of the identity account belongs to the application staff-directory port.',
 'Infrastructure/Time/SystemClock.cs': 'Adapts TimeProvider UTC time and the host TimeZoneInfo database to IClock. General FromLocal obtains the zone offset and normalizes to UTC; recurrence generation has its own explicit ambiguous/invalid-time policy, so this utility should not be mistaken for the scheduling DST algorithm.',
 'Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs': 'Checks the actual PostgreSQL current_user for superuser, BYPASSRLS, or schema-owner identity and fails readiness with a generic description if any applies. It detects dangerous connection-string configuration independently of whether ordinary connectivity succeeds. Database exceptions flow to health-check handling.',
 'Infrastructure/Persistence/Migrations/RuntimeAccessGrants.cs': 'Emits schema USAGE and owner-specific default privileges for future tables, granting SELECT, INSERT, and UPDATE. Existing tables still need explicit grants and DELETE requires a deliberate exception. Reversal removes both default privileges and schema usage, so schema-sharing migration order matters.',
 'Infrastructure/Persistence/Migrations/AppendOnlyAccessGrants.cs': 'Provides a narrower schema/default/table grant protocol for append-only data: SELECT and INSERT only. Audit immutability depends on runtime privileges lacking UPDATE and DELETE in addition to application behavior. The reverse operation revokes table grants, future defaults, and schema usage.',
 'Infrastructure/Persistence/TenantViolationException.cs': 'Signals a tenant write-guard rejection and identifies the CLR entity type for internal diagnostics. It is intentionally not a business Result: uncaught guard violations reach the generic HTTP exception path, because they indicate a programming or isolation failure.',
 'Application/Academics/Students/Commands/ArchiveStudent/ArchiveStudentHandler.cs': 'Loads a versioned student, refuses archival while active enrollments exist, then asks the aggregate to archive. The enrollment existence check is a cross-aggregate business precondition; the repository and UnitOfWork still own tenant scoping and optimistic persistence.',
 'Application/Academics/Teachers/Commands/ArchiveTeacher/ArchiveTeacherHandler.cs': 'Loads a versioned teacher and refuses archival when active teaching groups remain. Archival changes business availability while retaining the optional account association and historical references. Staff membership lifecycle is separate.',
 'Application/Academics/Subjects/Commands/ArchiveSubject/ArchiveSubjectHandler.cs': 'Prevents subject archival while an active group references it, then delegates the state transition to Subject. This check preserves usable group dependencies without deleting either group or subject history.',
 'Application/Academics/Groups/Commands/RestoreGroup/RestoreGroupHandler.cs': 'Revalidates the referenced subject and teacher before restoring an archived group. Existing foreign keys establish existence and centre consistency, while these application checks require current active business state. Restoring a row therefore cannot silently revive a group with archived dependencies.',
 'Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs': 'Constructs the canonical phone value object and asks the parent read service for an exact match. Absence is a successful null response rather than NotFound, supporting optional reuse in student registration. The endpoint uses a POST body to keep phone values out of URL paths and query strings.',
 'Application/Academics/Students/Commands/UpdateStudent/UpdateStudentHandler.cs': 'Loads the student with the supplied version and applies the aggregate update to name and optional contact fields. This is a replacement-style operation: absent optional values clear them. Phone parsing and domain validation precede final UnitOfWork save.',
 'Application/Academics/Teachers/Commands/UpdateTeacher/UpdateTeacherHandler.cs': 'Loads a versioned teacher and replaces editable personal fields through domain validation. Optional phone removal is intentional, while account linkage is handled by a separate command with identity eligibility checks. Successful handler return remains provisional until persistence succeeds.',
 'Application/Audit/AuditCursor.cs': 'Encodes the descending audit keyset position as timestamp ticks plus event ID, transported as an opaque Base64 token. Decoding checks token structure and numeric/GUID parsing, but DateTimeOffset construction can throw for out-of-range ticks; callers should not assume every malformed token returns false.',
}
for suffix, paragraph in entries.items():
    key = 'src/TutoringCentre.' + suffix
    assert (root / key).exists(), key
    data[key] = paragraph
path.write_text(json.dumps(data, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
print(f'{len(data)} concrete purpose entries')
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
