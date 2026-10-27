# `frontend/src/i18n/locales/ar/settings.json`

## Purpose

JSON data with top-level keys `page`, `loading`, `error`, `form`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/settings.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 988 bytes. Source hash: `9aeab70ee3fe5160643da5b63566770f081cc7d3e657d0162b54d11f9a42155b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "إعدادات المركز",
    "subtitle": "اسم مركزك واللغة التي يفتح بها."
  },
  "loading": {
    "label": "جاري تحميل الإعدادات"
  },
  "error": {
    "title": "تعذر تحميل الإعدادات",
    "reference": "المرجع: {correlationId}"
  },
  "form": {
    "nameLabel": "الاسم",
    "defaultLocaleLabel": "اللغة الافتراضية",
    "slugLabel": "العنوان الإلكتروني",
    "slugHint": "لا يمكن تغيير العنوان الإلكتروني من هنا.",
    "timeZoneLabel": "المنطقة الزمنية",
    "timeZoneHint": "لا يمكن تغيير المنطقة الزمنية من هنا.",
    "save": "حفظ"
  },
  "toasts": {
    "saved": "تم حفظ الإعدادات."
  },
  "validation": {
    "nameRequired": "اسم المركز مطلوب.",
    "nameTooLong": "يجب ألا يزيد اسم المركز عن 120 حرفًا."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
