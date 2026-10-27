# `frontend/src/i18n/locales/en/subjects.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `status`, `actions`, `dialogs`, `form`, `loading`, `error`, `emptyState`, `confirmations`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/subjects.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 1,509 bytes. Source hash: `0b634a395cf2a3d79ffae6b0a747f92f2a3b1e57bfcfcc69055e9e4bb3533277`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Subjects",
    "showArchived": "Show archived",
    "subtitle": "The subjects your centre teaches; every group belongs to one.",
    "filtersLabel": "Filter subjects"
  },
  "columns": {
    "name": "Name",
    "status": "Status",
    "actions": "Actions"
  },
  "status": {
    "active": "Active",
    "archived": "Archived"
  },
  "actions": {
    "create": "Add subject",
    "rename": "Rename",
    "archive": "Archive",
    "restore": "Restore",
    "openMenu": "Open actions menu"
  },
  "dialogs": {
    "createTitle": "Add subject",
    "renameTitle": "Rename subject"
  },
  "form": {
    "nameLabel": "Name",
    "namePlaceholder": "e.g. Mathematics",
    "save": "Save",
    "cancel": "Cancel"
  },
  "loading": {
    "label": "Loading subjects"
  },
  "error": {
    "title": "Subjects could not be loaded",
    "reference": "Reference: {correlationId}"
  },
  "emptyState": {
    "title": "No subjects yet",
    "description": "Add your first subject to get started."
  },
  "confirmations": {
    "archiveTitle": "Archive \"{name}\"?",
    "archiveBody": "It will no longer appear in subject pickers until it is restored.",
    "archiveConfirm": "Archive subject"
  },
  "toasts": {
    "created": "Subject added.",
    "renamed": "Subject renamed.",
    "archived": "Subject archived.",
    "restored": "Subject restored."
  },
  "validation": {
    "nameRequired": "Subject name is required.",
    "nameTooLong": "Subject name must be at most 80 characters."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
