# `frontend/src/i18n/locales/ar/teachers.json`

## Purpose

JSON data with top-level keys `page`, `stats`, `columns`, `actions`, `loading`, `error`, `emptyState`, `dialogs`, `form`, `account`, `confirmations`, `toasts`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/teachers.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 87 lines and 3,898 bytes. Source hash: `5c4e49d6dbb767f230359e2780a247ff9699db1f2adc533163761faf4d5d123f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "المعلمون",
    "subtitle": "من يدرّسون مجموعاتك، وحسابات الموظفين التي يسجلون الدخول بها.",
    "showArchived": "إظهار المؤرشفين",
    "filtersLabel": "تصفية المعلمين"
  },
  "stats": {
    "activeTeachers": "المعلمون النشطون",
    "linkedAccounts": "الحسابات المرتبطة",
    "activeGroups": "المجموعات النشطة المسندة"
  },
  "columns": {
    "name": "الاسم",
    "phone": "الهاتف",
    "account": "الحساب",
    "activeGroups": "المجموعات النشطة",
    "activeGroupsCount": "{count, plural, zero {لا توجد مجموعات} one {مجموعة واحدة} two {مجموعتان} few {{countText} مجموعات} many {{countText} مجموعة} other {{countText} مجموعة}}",
    "status": "الحالة",
    "actions": "الإجراءات"
  },
  "actions": {
    "create": "إضافة معلم",
    "edit": "تعديل البيانات",
    "account": "ربط حساب",
    "archive": "أرشفة",
    "restore": "استعادة",
    "openMenu": "إجراءات {name}"
  },
  "loading": {
    "label": "جاري تحميل المعلمين"
  },
  "error": {
    "title": "تعذر تحميل المعلمين",
    "reference": "المرجع:"
  },
  "emptyState": {
    "title": "لا يوجد معلمون حتى الآن",
    "description": "أضف من يدرّسون في هذا المركز، ثم أسند إليهم المجموعات.",
    "readOnlyDescription": "سيظهر هنا المعلمون الذين يضيفهم مالك المركز."
  },
  "dialogs": {
    "createTitle": "إضافة معلم",
    "editTitle": "تعديل بيانات المعلم",
    "formDescription": "الاسم والهاتف فقط. يُربط حساب الدخول بشكل منفصل."
  },
  "form": {
    "fullNameLabel": "الاسم الكامل",
    "fullNamePlaceholder": "مثال: كريم فتحي",
    "phoneLabel": "الهاتف (اختياري)",
    "save": "حفظ",
    "cancel": "إلغاء"
  },
  "account": {
    "title": "حساب {name}",
    "effect": "سيرى هذا الحساب مجموعات هذا المعلم فقط.",
    "currentLabel": "الحساب المرتبط",
    "none": "لا يوجد حساب",
    "remove": "إزالة الربط",
    "selectLabel": "حساب الموظف",
    "selectPlaceholder": "اختر حساب معلم",
    "link": "ربط الحساب",
    "loading": "جاري تحميل حسابات الموظفين",
    "retry": "إعادة المحاولة",
    "noEligibleTitle": "لا يوجد حساب متاح للربط",
    "noEligibleDescription": "يمكن اختيار الموظفين النشطين بدور معلم وغير المرتبطين فقط. أضف موظفًا من صفحة الموظفين.",
    "goToStaff": "الذهاب إلى الموظفين"
  },
  "confirmations": {
    "archiveTitle": "أرشفة \"{name}\"؟",
    "archiveBody": "لا يمكن إسناد مجموعات إلى معلم مؤرشف. يجب إعادة إسناد المجموعات التي ما زال يدرّسها أو أرشفتها أولًا.",
    "archiveConfirm": "أرشفة المعلم"
  },
  "toasts": {
    "created": "تمت إضافة المعلم.",
    "updated": "تم تحديث بيانات المعلم.",
    "archived": "تمت أرشفة المعلم.",
    "restored": "تمت استعادة المعلم.",
    "accountLinked": "تم ربط الحساب.",
    "accountUnlinked": "تمت إزالة ربط الحساب."
  },
  "validation": {
    "fullNameRequired": "اسم المعلم مطلوب.",
    "fullNameTooLong": "يجب ألا يزيد اسم المعلم عن 120 حرفًا.",
    "phoneTooLong": "يجب ألا يزيد رقم الهاتف عن 30 حرفًا."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
