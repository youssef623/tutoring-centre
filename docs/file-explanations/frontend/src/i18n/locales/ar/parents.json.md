# `frontend/src/i18n/locales/ar/parents.json`

## Purpose

JSON data with top-level keys `loading`, `error`, `detailError`, `notFound`, `card`, `children`, `relationship`, `actions`, `dialogs`, `form`, `toasts`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/parents.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 57 lines and 1,882 bytes. Source hash: `dd5720e03f79bbd36f375b0e3483e2f649454c7cc45caf3f1ecc7127b0e134ea`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "detailLabel": "جاري تحميل بيانات ولي الأمر"
  },
  "error": {
    "reference": "المرجع: {correlationId}"
  },
  "detailError": {
    "title": "تعذر تحميل بيانات ولي الأمر هذا"
  },
  "notFound": {
    "title": "ولي الأمر غير موجود",
    "description": "ولي الأمر هذا غير موجود، أو ليس لديك صلاحية الوصول إليه.",
    "backToStudents": "العودة إلى الطلاب"
  },
  "card": {
    "phoneLabel": "الهاتف"
  },
  "children": {
    "title": "الأبناء",
    "columns": {
      "name": "الاسم",
      "relationship": "صلة القرابة",
      "status": "الحالة"
    },
    "primaryBadge": "جهة الاتصال الأساسية",
    "emptyState": "لا يوجد طلاب مرتبطون"
  },
  "relationship": {
    "father": "الأب",
    "mother": "الأم",
    "guardian": "ولي الأمر",
    "other": "آخر"
  },
  "actions": {
    "edit": "تعديل"
  },
  "dialogs": {
    "editTitle": "تعديل بيانات ولي الأمر"
  },
  "form": {
    "fullNameLabel": "الاسم الكامل",
    "phoneLabel": "الهاتف",
    "phoneChangeNotice": "تغيير الهاتف يغيّر أيضًا الرقم الذي تُستخدم به عمليات التسجيل المستقبلية للعثور على ولي الأمر هذا.",
    "save": "حفظ",
    "cancel": "إلغاء"
  },
  "toasts": {
    "updated": "تم تحديث بيانات ولي الأمر."
  },
  "validation": {
    "fullNameRequired": "اسم ولي الأمر مطلوب.",
    "fullNameTooLong": "يجب ألا يتجاوز اسم ولي الأمر 120 حرفًا.",
    "phoneRequired": "الهاتف مطلوب.",
    "phoneTooLong": "يجب ألا يتجاوز رقم الهاتف 30 حرفًا."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
