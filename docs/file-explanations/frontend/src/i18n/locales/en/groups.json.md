# `frontend/src/i18n/locales/en/groups.json`

## Purpose

JSON data with top-level keys `page`, `filters`, `stats`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `overview`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/groups.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 136 lines and 4,710 bytes. Source hash: `2c23181086349b6b35fb2f2c677f7ae009f4f7de1cf24247822baa9fa8b31492`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Groups",
    "subtitle": "Classes students enrol in: one subject, one teacher and a fixed number of seats."
  },
  "filters": {
    "label": "Filter groups",
    "subject": "Subject",
    "teacher": "Teacher",
    "allSubjects": "All subjects",
    "allTeachers": "All teachers",
    "showArchived": "Show archived"
  },
  "stats": {
    "activeGroups": "Active groups",
    "openSeats": "Open seats",
    "fullGroups": "Full groups"
  },
  "columns": {
    "name": "Name",
    "subject": "Subject",
    "teacher": "Teacher",
    "seats": "Seats",
    "status": "Status"
  },
  "actions": {
    "create": "Create group",
    "rename": "Rename",
    "changeCapacity": "Change capacity",
    "changeTeacher": "Change teacher",
    "archive": "Archive",
    "restore": "Restore"
  },
  "loading": {
    "label": "Loading groups",
    "detailLabel": "Loading group"
  },
  "error": {
    "title": "Groups could not be loaded",
    "reference": "Reference:"
  },
  "detailError": {
    "title": "This group could not be loaded"
  },
  "notFound": {
    "title": "Group not found",
    "description": "This group does not exist, or you don't have access to it.",
    "backToList": "Back to groups"
  },
  "emptyState": {
    "title": "No groups yet",
    "description": "Create a group for a subject and assign it a teacher to start enrolling students.",
    "assignedTitle": "No groups are assigned to you",
    "assignedDescription": "Groups you teach will appear here once the centre assigns them to you.",
    "noMatchTitle": "No groups match these filters",
    "noMatchDescription": "Try another subject or teacher, or clear the filters."
  },
  "detail": {
    "backToList": "All groups"
  },
  "overview": {
    "seats": "Seats",
    "subject": "Subject",
    "teacher": "Teacher",
    "capacity": "Capacity",
    "status": "Status"
  },
  "tabs": {
    "label": "Group sections",
    "overview": "Overview",
    "roster": "Roster",
    "schedule": "Schedule"
  },
  "archivedBanner": {
    "title": "This group is archived",
    "description": "An archived group is read-only. Restore it to rename it, change its capacity or change its teacher."
  },
  "dialogs": {
    "createTitle": "Create group",
    "createDescription": "Choose the subject and teacher. The subject cannot be changed later.",
    "renameTitle": "Rename group",
    "renameDescription": "Group names are unique within the centre.",
    "capacityTitle": "Change capacity",
    "capacityDescription": "Between 1 and 200 seats. {enrolled} currently enrolled.",
    "teacherTitle": "Change teacher",
    "teacherDescription": "The new teacher's account will see this group; the previous teacher's will not."
  },
  "form": {
    "nameLabel": "Name",
    "namePlaceholder": "e.g. Physics – Grade 11 A",
```

The excerpt stops at line 90; use the original file for the remaining 46 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
