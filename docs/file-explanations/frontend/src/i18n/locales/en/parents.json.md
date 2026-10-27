# `frontend/src/i18n/locales/en/parents.json`

## Purpose

JSON data with top-level keys `loading`, `error`, `detailError`, `notFound`, `card`, `children`, `relationship`, `actions`, `dialogs`, `form`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/parents.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 57 lines and 1,399 bytes. Source hash: `20be492c1087545792b4c308487cd642c4efcaa19a247a92e00fffd336f1acee`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "loading": {
    "detailLabel": "Loading parent"
  },
  "error": {
    "reference": "Reference: {correlationId}"
  },
  "detailError": {
    "title": "This parent could not be loaded"
  },
  "notFound": {
    "title": "Parent not found",
    "description": "This parent does not exist, or you don't have access to it.",
    "backToStudents": "Back to students"
  },
  "card": {
    "phoneLabel": "Phone"
  },
  "children": {
    "title": "Children",
    "columns": {
      "name": "Name",
      "relationship": "Relationship",
      "status": "Status"
    },
    "primaryBadge": "Primary contact",
    "emptyState": "No students linked"
  },
  "relationship": {
    "father": "Father",
    "mother": "Mother",
    "guardian": "Guardian",
    "other": "Other"
  },
  "actions": {
    "edit": "Edit"
  },
  "dialogs": {
    "editTitle": "Edit parent"
  },
  "form": {
    "fullNameLabel": "Full name",
    "phoneLabel": "Phone",
    "phoneChangeNotice": "Changing the phone also changes how future registrations find this parent by number.",
    "save": "Save",
    "cancel": "Cancel"
  },
  "toasts": {
    "updated": "Parent updated."
  },
  "validation": {
    "fullNameRequired": "Parent name is required.",
    "fullNameTooLong": "Parent name must be at most 120 characters.",
    "phoneRequired": "Phone is required.",
    "phoneTooLong": "Phone number must be at most 30 characters."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
