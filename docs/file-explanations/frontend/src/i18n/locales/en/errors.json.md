# `frontend/src/i18n/locales/en/errors.json`

## Purpose

Declares `was`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/errors.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 153 lines and 8,023 bytes. Source hash: `ad9b1db252abff2c8d64d7f12d1e1a1265349e20a5b1e7fcea29d4f2b1631048`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 137](../../../../../../../frontend/src/i18n/locales/en/errors.json#L137): `"stale": "This record was changed by someone else. Reload and try again."`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "byCode": {
    "validation": {
      "failed": "Some fields are invalid. Please check the form."
    },
    "centre": {
      "name_required": "Centre name is required.",
      "name_too_long": "Centre name must be at most 120 characters.",
      "name_invalid": "Centre name must be 1-120 characters.",
      "slug_invalid": "The web address may only contain lowercase letters, digits and single hyphens.",
      "time_zone_invalid": "The time zone is not recognised.",
      "slug_taken": "A centre with this web address already exists.",
      "create_forbidden": "Only the platform can create centres."
    },
    "tenant": {
      "not_selected": "Please select a centre to continue."
    },
    "auth": {
      "permission_denied": "You do not have permission to do this.",
      "password_change_required": "Please change your password before continuing.",
      "current_password_invalid": "The current password you entered is incorrect.",
      "password_too_weak": "This password does not meet the requirements.",
      "rate_limited": "Too many attempts. Please wait a minute and try again.",
      "invalid_credentials": "Incorrect email or password.",
      "not_authenticated": "Please sign in to continue."
    },
    "staff": {
      "already_member": "This person is already a member of this centre.",
      "last_owner": "This centre must keep at least one owner.",
      "cannot_change_self": "You cannot change your own role.",
      "not_found": "This staff member could not be found.",
      "email_invalid": "Enter a valid email address."
    },
    "membership": {
      "role_unchanged": "The role is already this value.",
      "inactive": "This membership is inactive.",
      "already_active": "This membership is already active.",
      "already_inactive": "This membership is already inactive.",
      "centre_required": "A centre is required.",
      "user_required": "A user is required."
    },
    "subject": {
      "not_found": "This subject could not be found.",
      "name_taken": "A subject with this name already exists.",
      "name_invalid": "Subject name must be 1-80 characters.",
      "already_archived": "This subject is already archived.",
      "not_archived": "This subject is not archived.",
      "archived_read_only": "An archived subject cannot be renamed until it is restored.",
      "has_active_groups": "This subject has active groups. Archive or move them first."
    },
    "phone": {
      "invalid": "Enter a valid phone number."
    },
    "parent": {
      "centre_required": "A centre is required.",
      "name_invalid": "Parent name must be 2-120 characters.",
      "not_found": "This parent could not be found.",
      "phone_taken": "A parent with this phone number already exists."
    },
    "student": {
      "centre_required": "A centre is required.",
      "name_invalid": "Student name must be 2-120 characters.",
      "school_year_invalid": "School year must be at most 40 characters.",
      "parent_required": "A parent is required.",
      "parent_already_linked": "This parent is already linked to this student.",
      "parent_not_linked": "This parent is not linked to this student.",
      "too_many_parents": "A student can have at most 4 parents.",
      "last_parent": "A student must keep at least one parent.",
      "primary_parent_required": "Make another parent primary before removing the primary parent.",
      "already_primary": "This parent is already the primary contact.",
      "already_archived": "This student is already archived.",
      "not_archived": "This student is not archived.",
      "archived_read_only": "An archived student is read-only until it is restored.",
      "not_found": "This student could not be found.",
      "has_active_enrollments": "This student is still enrolled in a group. Withdraw them first."
    },
    "teacher": {
      "centre_required": "A centre is required.",
      "name_invalid": "Teacher name must be 2-120 characters.",
      "user_required": "A user is required.",
      "already_archived": "This teacher is already archived.",
      "not_archived": "This teacher is not archived.",
      "archived_read_only": "An archived teacher is read-only until it is restored.",
      "not_found": "This teacher could not be found.",
      "user_not_eligible": "This person is not an eligible Teacher at this centre.",
      "account_taken": "This account is already linked to another teacher.",
      "has_active_groups": "This teacher has active groups. Reassign or archive them first."
    },
    "group": {
      "centre_required": "A centre is required.",
```

The excerpt stops at line 90; use the original file for the remaining 63 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
