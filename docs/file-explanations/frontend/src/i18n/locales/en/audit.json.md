# `frontend/src/i18n/locales/en/audit.json`

## Purpose

JSON data with top-level keys `page`, `filters`, `entityType`, `action`, `field`, `value`, `system`, `referenceLabel`, `copyReference`, `copied`, `loadMore`, `loading`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/audit.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 105 lines and 2,559 bytes. Source hash: `36c6e3ed0ff59cc3cb3e151f73a07a140e4adab6325bb5a6e7c56eb7467a43bc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "page": {
    "title": "Audit log",
    "subtitle": "Every change made in this centre, newest first.",
    "filtersLabel": "Filter the audit log"
  },
  "filters": {
    "entityTypeLabel": "Entity type",
    "entityTypeAll": "All",
    "fromLabel": "From",
    "toLabel": "To"
  },
  "entityType": {
    "subject": "Subject",
    "membership": "Staff membership",
    "centre": "Centre",
    "parent": "Parent",
    "student": "Student",
    "student_parent": "Student-parent link",
    "teacher": "Teacher",
    "group": "Group",
    "enrollment": "Enrollment",
    "schedule_rule": "Schedule rule",
    "class_session": "Class session"
  },
  "action": {
    "created": "Created",
    "updated": "Updated",
    "deleted": "Deleted"
  },
  "field": {
    "name": "Name",
    "status": "Status",
    "role": "Role",
    "userId": "User",
    "defaultLocale": "Default language",
    "fullName": "Full name",
    "phone": "Phone",
    "schoolYear": "School year",
    "studentId": "Student",
    "parentId": "Parent",
    "relationship": "Relationship",
    "isPrimary": "Primary contact",
    "subjectId": "Subject",
    "teacherId": "Teacher",
    "capacity": "Capacity",
    "groupId": "Group",
    "endReason": "End reason",
    "dayOfWeek": "Day",
    "startTime": "Start time",
    "durationMinutes": "Length (minutes)",
    "room": "Room",
    "effectiveFrom": "In effect from",
    "effectiveTo": "In effect until"
  },
  "value": {
    "status": {
      "active": "Active",
      "archived": "Archived",
      "inactive": "Inactive",
      "ended": "Ended",
      "scheduled": "Scheduled",
      "cancelled": "Cancelled"
    },
    "endReason": {
      "withdrawn": "Withdrawn",
      "transferred": "Transferred"
    },
    "relationship": {
      "father": "Father",
      "mother": "Mother",
      "guardian": "Guardian",
      "other": "Other"
    },
    "isPrimary": {
      "true": "Yes",
      "false": "No"
    },
    "dayOfWeek": {
      "saturday": "Saturday",
      "sunday": "Sunday",
      "monday": "Monday",
      "tuesday": "Tuesday",
      "wednesday": "Wednesday",
      "thursday": "Thursday",
      "friday": "Friday"
    }
  },
  "system": "System",
  "referenceLabel": "Reference:",
```

The excerpt stops at line 90; use the original file for the remaining 15 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
