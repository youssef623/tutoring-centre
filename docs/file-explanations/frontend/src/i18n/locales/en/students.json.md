# `frontend/src/i18n/locales/en/students.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `parentsCard`, `addParent`, `archivedBanner`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/students.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 119 lines and 3,880 bytes. Source hash: `0f6e8fcafed585f481d327a8eec4ff13a6f95ccac6eae6af10959fd2c6ad1714`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Students",
    "searchLabel": "Search students",
    "showArchived": "Show archived",
    "subtitle": "Every student registered at your centre, with their primary parent.",
    "filtersLabel": "Filter students"
  },
  "columns": {
    "name": "Name",
    "schoolYear": "School year",
    "primaryParent": "Primary parent",
    "parentPhone": "Parent phone",
    "status": "Status"
  },
  "actions": {
    "register": "Register student",
    "edit": "Edit",
    "archive": "Archive",
    "restore": "Restore",
    "addParent": "Add parent",
    "makePrimary": "Make primary",
    "unlink": "Unlink"
  },
  "loading": {
    "label": "Loading students",
    "detailLabel": "Loading student"
  },
  "error": {
    "title": "Students could not be loaded",
    "reference": "Reference: {correlationId}"
  },
  "detailError": {
    "title": "This student could not be loaded"
  },
  "notFound": {
    "title": "Student not found",
    "description": "This student does not exist, or you don't have access to it.",
    "backToList": "Back to students"
  },
  "emptyState": {
    "title": "No students yet",
    "description": "Register your first student to get started.",
    "noMatchTitle": "No students match \"{search}\"."
  },
  "detail": {
    "phoneLabel": "Phone"
  },
  "parentsCard": {
    "title": "Parents",
    "primaryBadge": "Primary contact",
    "openMenu": "Open actions menu"
  },
  "addParent": {
    "link": "Link",
    "retryLink": "Retry link",
    "linkFailedNotice": "The parent now exists but could not be linked yet. Try linking again."
  },
  "archivedBanner": {
    "title": "This student is archived",
    "description": "Restore the student to edit their details or manage their parents."
  },
  "form": {
    "studentSectionTitle": "Student",
    "parentSectionTitle": "Parent or guardian",
    "fullNameLabel": "Full name",
    "fullNamePlaceholder": "e.g. Ahmed Mahmoud",
    "phoneLabel": "Phone (optional)",
    "schoolYearLabel": "School year (optional)",
    "schoolYearPlaceholder": "e.g. Grade 5",
    "parentPhoneLabel": "Parent's phone",
    "parentFullNameLabel": "Parent's full name",
    "parentFullNamePlaceholder": "e.g. Mahmoud Hassan",
    "relationshipLabel": "Relationship",
    "save": "Save",
    "cancel": "Cancel"
  },
  "dialogs": {
    "editTitle": "Edit student",
    "addParentTitle": "Add parent"
  },
  "confirmations": {
    "archiveTitle": "Archive \"{name}\"?",
    "archiveBody": "They will become read-only until restored, and their parent actions will be hidden.",
    "archiveConfirm": "Archive student",
    "unlinkTitle": "Unlink {name}?",
    "unlinkBody": "They will no longer be linked to this student. This does not delete their record.",
    "unlinkConfirm": "Unlink"
  },
  "lookup": {
```

The excerpt stops at line 90; use the original file for the remaining 29 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
