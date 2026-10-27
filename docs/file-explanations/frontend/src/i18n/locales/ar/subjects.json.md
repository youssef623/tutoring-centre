# `frontend/src/i18n/locales/ar/subjects.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `status`, `actions`, `dialogs`, `form`, `loading`, `error`, `emptyState`, `confirmations`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/subjects.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 1,877 bytes. Source hash: `df1f6ee1861d0a71207cf5f97e3ebdd65a8e82d61bcd1f7be9ecc112a61088bc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "المواد",
    "showArchived": "إظهار المؤرشفة",
    "subtitle": "المواد التي يدرّسها مركزك؛ كل مجموعة تتبع مادة واحدة.",
    "filtersLabel": "تصفية المواد"
  },
  "columns": {
    "name": "الاسم",
    "status": "الحالة",
    "actions": "الإجراءات"
  },
  "status": {
    "active": "نشطة",
    "archived": "مؤرشفة"
  },
  "actions": {
    "create": "إضافة مادة",
    "rename": "إعادة التسمية",
    "archive": "أرشفة",
    "restore": "استعادة",
    "openMenu": "فتح قائمة الإجراءات"
  },
  "dialogs": {
    "createTitle": "إضافة مادة",
    "renameTitle": "إعادة تسمية المادة"
  },
  "form": {
    "nameLabel": "الاسم",
    "namePlaceholder": "مثال: الرياضيات",
    "save": "حفظ",
    "cancel": "إلغاء"
  },
  "loading": {
    "label": "جارٍ تحميل المواد"
  },
  "error": {
    "title": "تعذر تحميل المواد",
    "reference": "المرجع: {correlationId}"
  },
  "emptyState": {
    "title": "لا توجد مواد بعد",
    "description": "أضف أول مادة للبدء."
  },
  "confirmations": {
    "archiveTitle": "أرشفة \"{name}\"؟",
    "archiveBody": "لن تظهر في قوائم اختيار المواد حتى تتم استعادتها.",
    "archiveConfirm": "أرشفة المادة"
  },
  "toasts": {
    "created": "تمت إضافة المادة.",
    "renamed": "تمت إعادة تسمية المادة.",
    "archived": "تمت أرشفة المادة.",
    "restored": "تمت استعادة المادة."
  },
  "validation": {
    "nameRequired": "اسم المادة مطلوب.",
    "nameTooLong": "يجب ألا يزيد اسم المادة عن 80 حرفًا."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
