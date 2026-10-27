# `frontend/src/i18n/locales/en/settings.json`

## Purpose

JSON data with top-level keys `page`, `loading`, `error`, `form`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/settings.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 759 bytes. Source hash: `2cff58f187be02d1dc78b51f0c0cfa66c61f4c837b19afca18ed556bbef8d061`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "Centre settings",
    "subtitle": "Your centre's name and the language it opens in."
  },
  "loading": {
    "label": "Loading settings"
  },
  "error": {
    "title": "Settings could not be loaded",
    "reference": "Reference: {correlationId}"
  },
  "form": {
    "nameLabel": "Name",
    "defaultLocaleLabel": "Default language",
    "slugLabel": "Web address",
    "slugHint": "The web address cannot be changed here.",
    "timeZoneLabel": "Time zone",
    "timeZoneHint": "The time zone cannot be changed here.",
    "save": "Save"
  },
  "toasts": {
    "saved": "Settings saved."
  },
  "validation": {
    "nameRequired": "Centre name is required.",
    "nameTooLong": "Centre name must be at most 120 characters."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
