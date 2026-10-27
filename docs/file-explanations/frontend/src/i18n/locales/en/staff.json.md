# `frontend/src/i18n/locales/en/staff.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `status`, `you`, `loading`, `error`, `emptyState`, `actions`, `dialogs`, `form`, `confirmations`, `password`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/staff.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 2,325 bytes. Source hash: `3849f8be38863813077b4c762c8f3fd5d9083f8d7e69a888224a18e86eeea36e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Staff",
    "subtitle": "The people who can sign in to this centre, and their roles."
  },
  "columns": {
    "name": "Name",
    "email": "Email",
    "role": "Role",
    "status": "Status",
    "actions": "Actions"
  },
  "status": {
    "active": "Active",
    "inactive": "Inactive"
  },
  "you": "You",
  "loading": {
    "label": "Loading staff"
  },
  "error": {
    "title": "Staff could not be loaded",
    "reference": "Reference: {correlationId}"
  },
  "emptyState": {
    "title": "No staff yet",
    "description": "Add your first staff member to get started."
  },
  "actions": {
    "create": "Add staff",
    "changeRole": "Change role",
    "deactivate": "Deactivate",
    "reactivate": "Reactivate",
    "openMenu": "Open actions menu"
  },
  "dialogs": {
    "createTitle": "Add staff",
    "changeRoleTitle": "Change role for {name}"
  },
  "form": {
    "emailLabel": "Email",
    "emailPlaceholder": "e.g. teacher@example.com",
    "displayNameLabel": "Name",
    "displayNamePlaceholder": "e.g. Sara Ahmed",
    "roleLabel": "Role",
    "preferredLocaleLabel": "Preferred language",
    "save": "Save",
    "cancel": "Cancel",
    "changeRoleConsequence": "They will be signed out and will get the new role the next time they sign in."
  },
  "confirmations": {
    "deactivateTitle": "Deactivate {name}?",
    "deactivateBody": "{name} will lose access to this centre only. Their account is never deleted.",
    "deactivateConfirm": "Deactivate"
  },
  "password": {
    "title": "Temporary password",
    "description": "Share this password with {name}. It will not be shown again and must be changed at first sign-in.",
    "copy": "Copy",
    "copied": "Copied!",
    "done": "Done"
  },
  "existingAccount": {
    "title": "Account already exists",
    "description": "{name} already has an account and can now select this centre to sign in."
  },
  "toasts": {
    "created": "Staff member added.",
    "roleChanged": "Role changed.",
    "deactivated": "Staff member deactivated.",
    "reactivated": "Staff member reactivated."
  },
  "validation": {
    "emailRequired": "Email is required.",
    "emailInvalid": "Enter a valid email address.",
    "displayNameRequired": "Name is required.",
    "displayNameTooLong": "Name must be at most 120 characters."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
