# `frontend/src/i18n/locales/en/teachers.json`

## Purpose

JSON data with top-level keys `page`, `stats`, `columns`, `actions`, `loading`, `error`, `emptyState`, `dialogs`, `form`, `account`, `confirmations`, `toasts`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/teachers.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 87 lines and 2,893 bytes. Source hash: `7522ac58c21e129d4ae56f540f6dc9842591639e6b54cfc93d7e84fcc7bb821f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Teachers",
    "subtitle": "The people who teach your groups, and the staff accounts they sign in with.",
    "showArchived": "Show archived",
    "filtersLabel": "Filter teachers"
  },
  "stats": {
    "activeTeachers": "Active teachers",
    "linkedAccounts": "Linked accounts",
    "activeGroups": "Active groups taught"
  },
  "columns": {
    "name": "Name",
    "phone": "Phone",
    "account": "Account",
    "activeGroups": "Active groups",
    "activeGroupsCount": "{count, plural, =0 {No groups} one {{countText} group} other {{countText} groups}}",
    "status": "Status",
    "actions": "Actions"
  },
  "actions": {
    "create": "Add teacher",
    "edit": "Edit details",
    "account": "Link account",
    "archive": "Archive",
    "restore": "Restore",
    "openMenu": "Actions for {name}"
  },
  "loading": {
    "label": "Loading teachers"
  },
  "error": {
    "title": "Teachers could not be loaded",
    "reference": "Reference:"
  },
  "emptyState": {
    "title": "No teachers yet",
    "description": "Add the people who teach at this centre, then assign them to groups.",
    "readOnlyDescription": "Teachers added by the centre's owner will appear here."
  },
  "dialogs": {
    "createTitle": "Add teacher",
    "editTitle": "Edit teacher",
    "formDescription": "Name and phone only. A sign-in account is linked separately."
  },
  "form": {
    "fullNameLabel": "Full name",
    "fullNamePlaceholder": "e.g. Karim Fathy",
    "phoneLabel": "Phone (optional)",
    "save": "Save",
    "cancel": "Cancel"
  },
  "account": {
    "title": "Account for {name}",
    "effect": "This account will see only this teacher's groups.",
    "currentLabel": "Linked account",
    "none": "No account",
    "remove": "Remove link",
    "selectLabel": "Staff account",
    "selectPlaceholder": "Choose a teacher account",
    "link": "Link account",
    "loading": "Loading staff accounts",
    "retry": "Retry",
    "noEligibleTitle": "No account available to link",
    "noEligibleDescription": "Only active staff with the Teacher role who are not already linked can be chosen. Add one on the Staff page.",
    "goToStaff": "Go to Staff"
  },
  "confirmations": {
    "archiveTitle": "Archive \"{name}\"?",
    "archiveBody": "An archived teacher cannot be assigned to groups. Groups they still teach must be reassigned or archived first.",
    "archiveConfirm": "Archive teacher"
  },
  "toasts": {
    "created": "Teacher added.",
    "updated": "Teacher updated.",
    "archived": "Teacher archived.",
    "restored": "Teacher restored.",
    "accountLinked": "Account linked.",
    "accountUnlinked": "Account link removed."
  },
  "validation": {
    "fullNameRequired": "Teacher name is required.",
    "fullNameTooLong": "Teacher name must be at most 120 characters.",
    "phoneTooLong": "Phone number must be at most 30 characters."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
